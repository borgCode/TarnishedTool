using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class PlayerService(
        IMemoryService memoryService,
        HookManager hookManager,
        ITravelService travelService,
        IReminderService reminderService,
        IParamService paramService,
        IChrInsService chrInsService) : IPlayerService
    {
        private const float LongDistanceRestore = 500f;

        private const float InitialLevelUpCost = 0.1f;
        private const float InitialLevelUpOffset = 1f;
        private const float LevelUpCostIncrease = 0.02f;
        private const float LevelUpIncreaseInterval = 92f;
        private const int BaseLevelOffset = 80;
        
        private const int StatsBlockSize = 0x20;

        private readonly Position[] _positions =
        [
            new(0, Vector3.Zero, 0f),
            new(0, Vector3.Zero, 0f)
        ];

        public MapLocation GetMapLocation()
        {
            var playerIns = GetPlayerIns();

            var blockId = memoryService.Read<uint>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentBlockId);
            var mapCoords = memoryService.Read<Vector3>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapCoords);
            var angle = memoryService.Read<float>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapAngle);
            var localCoords = memoryService.Read<Vector3>(GetChrPhysicsPtr() + (int)ChrIns.ChrPhysicsOffsets.Coords);

            return new MapLocation(blockId, localCoords, mapCoords, angle);
        }

        public Vector3 GetPlayerPos() => chrInsService.GetLocalCoords(GetPlayerIns());
        public void SetPlayerPos(Vector3 pos) => chrInsService.SetLocalCoords(GetPlayerIns(), pos);
        public Vector3 GetTorrentPos() => chrInsService.GetLocalCoords(GetTorrentChrIns());
        public void SetTorrentPos(Vector3 pos) => chrInsService.SetLocalCoords(GetTorrentChrIns(), pos);

        public void SavePos(int index)
        {
            var posToSave = _positions[index];
            var playerIns = GetPlayerIns();
            posToSave.BlockId = memoryService.Read<uint>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentBlockId);
            posToSave.Coords =
                memoryService.Read<Vector3>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapCoords);
            posToSave.Angle =
                memoryService.Read<float>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapAngle);
        }

        public void RestorePos(int index)
        {
            var savedPos = _positions[index];
            var currentPos = GetPlayerPosition();

            uint currentArea = (currentPos.BlockId >> 24) & 0xFF;
            uint savedArea = (savedPos.BlockId >> 24) & 0xFF;

            if (currentArea == savedArea)
            {
                var currentAbsolute = PositionUtils.ToAbsolute(currentPos.Coords, currentPos.BlockId);
                var savedAbsolute = PositionUtils.ToAbsolute(savedPos.Coords, savedPos.BlockId);
                var delta = savedAbsolute - currentAbsolute;

                var chrRideModule = GetChrRidePtr();
                var isRiding = IsRidingInternal(chrRideModule);
                var physicsPtr = isRiding ? GetTorrentPhysicsPtr() : GetChrPhysicsPtr();
                var coordsPtr = physicsPtr + (int)ChrIns.ChrPhysicsOffsets.Coords;
                var isLongDistance = delta.Length() > LongDistanceRestore;

                if (isLongDistance)
                    memoryService.Write(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, true);

                memoryService.Write(coordsPtr, memoryService.Read<Vector3>(coordsPtr) + delta);
                memoryService.Write((IntPtr)GetPlayerIns() + WorldChrMan.PlayerInsOffsets.CurrentMapAngle,
                    savedPos.Angle);

                if (isLongDistance)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        memoryService.Write(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, false);
                    });
                }
            }

            else
            {
                _ = Task.Run(() => travelService.WarpToBlockId(savedPos));
            }
        }

        public void MoveToPosition(Position targetPosition)
        {
            var currentPos = GetPlayerPosition();

            uint currentArea = (currentPos.BlockId >> 24) & 0xFF;
            uint savedArea = (targetPosition.BlockId >> 24) & 0xFF;

            Vector3 targetAbsolute;
            if (currentArea == savedArea)
            {
                targetAbsolute = PositionUtils.ToAbsolute(targetPosition.Coords, targetPosition.BlockId);
            }
            else
            {
                targetAbsolute = LegacyConv(targetPosition);
            }

            var currentAbsolute = PositionUtils.ToAbsolute(currentPos.Coords, currentPos.BlockId);
            var delta = targetAbsolute - currentAbsolute;
            var chrRideModule = GetChrRidePtr();
            var isRiding = IsRidingInternal(chrRideModule);
            var physicsPtr = isRiding ? GetTorrentPhysicsPtr() : GetChrPhysicsPtr();
            var coordsPtr = physicsPtr + (int)ChrIns.ChrPhysicsOffsets.Coords;

            var playerIns = GetPlayerIns();
            var torrentIns = GetTorrentChrIns();

            bool wasPlayerNoDeathEnabled = IsChrDbgFlagEnabled(ChrDbgFlags.PlayerNoDeath);
            bool wasPlayerNoDamageEnabled = chrInsService.IsNoDamageEnabled(playerIns);
            bool wasTorrentNoDeathEnabled = chrInsService.IsNoDeathEnabled(torrentIns);
            bool wasTorrentNoDamageEnabled = chrInsService.IsNoDamageEnabled(torrentIns);

            ToggleDebugFlag(ChrDbgFlags.PlayerNoDeath, true);
            chrInsService.ToggleNoDamage(playerIns, true);
            chrInsService.ToggleNoDeath(torrentIns, true);
            chrInsService.ToggleNoDamage(torrentIns, true);

            memoryService.Write(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, true);

            memoryService.Write(coordsPtr, memoryService.Read<Vector3>(coordsPtr) + delta);
            memoryService.Write(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapAngle,
                targetPosition.Angle);

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                memoryService.Write(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, false);
            });

            ToggleDebugFlag(ChrDbgFlags.PlayerNoDeath, wasPlayerNoDeathEnabled);
            chrInsService.ToggleNoDamage(playerIns, wasPlayerNoDamageEnabled);
            chrInsService.ToggleNoDeath(torrentIns, wasTorrentNoDeathEnabled);
            chrInsService.ToggleNoDamage(torrentIns, wasTorrentNoDamageEnabled);
        }

        private bool IsChrDbgFlagEnabled(int offset) => memoryService.Read<byte>(ChrDbgFlags.Base + offset) == 1;

        private Vector3 LegacyConv(Position targetPosition)
        {
            var src = new byte[]
            {
                (byte)((targetPosition.BlockId >> 24) & 0xFF),
                (byte)((targetPosition.BlockId >> 16) & 0xFF),
                (byte)((targetPosition.BlockId >> 8) & 0xFF)
            };

            (int tableIndex, int slotIndex) = ParamIndices.All["WorldMapLegacyConvParam"];
            var row = paramService.GetParamRowByMatchingBytes(
                tableIndex, slotIndex, src, 0x4);


            var block = new MemoryBlock(memoryService.ReadBytes(row, 0x24));

            var srcPosX = block.Get<float>(0x08);
            var srcPosY = block.Get<float>(0x0C);
            var srcPosZ = block.Get<float>(0x10);
            var dstGridXNo = block.Get<byte>(0x15);
            var dstGridZNo = block.Get<byte>(0x16);
            var dstPosX = block.Get<float>(0x18);
            var dstPosY = block.Get<float>(0x1C);
            var dstPosZ = block.Get<float>(0x20);


            return new Vector3(
                targetPosition.Coords.X + (dstPosX - srcPosX) + (dstGridXNo * 256),
                targetPosition.Coords.Y + (dstPosY - srcPosY),
                targetPosition.Coords.Z + (dstPosZ - srcPosZ) + (dstGridZNo * 256)
            );
        }

        public bool IsRiding() => IsRidingInternal(GetChrRidePtr());

        private bool IsRidingInternal(IntPtr chrRideModule)
        {
            var rideNode = memoryService.Read<nint>(chrRideModule + (int)ChrIns.ChrRideOffsets.RideNode);
            return memoryService.Read<int>(rideNode + (int)ChrIns.RideNodeOffsets.IsRiding) != 0;
        }

        private IntPtr GetTorrentPhysicsPtr()
        {
            var torrentChrIns = GetTorrentChrIns();
            return memoryService.FollowPointers(torrentChrIns, [..ChrIns.ChrPhysicsModule], true, false);
        }

        private IntPtr GetTorrentChrIns()
        {
            var playerGameData =
                memoryService.Read<nint>(memoryService.Read<nint>(GameDataMan.Base) + GameDataMan.PlayerGameData);
            var handle =
                memoryService.Read<int>(playerGameData + GameDataMan.TorrentHandle);
            return chrInsService.ChrInsByHandle(handle);
        }

        public nint GetPlayerIns() =>
            memoryService.Read<nint>(memoryService.Read<nint>(WorldChrMan.Base) + WorldChrMan.PlayerIns);

        public uint GetBlockId() =>
            memoryService.Read<uint>(GetPlayerIns() + WorldChrMan.PlayerInsOffsets.CurrentBlockId);

        public void SetHp(int hp) => chrInsService.SetHp(GetPlayerIns(), hp);
        public int GetCurrentHp() => chrInsService.GetCurrentHp(GetPlayerIns());
        public int GetMaxHp() => chrInsService.GetMaxHp(GetPlayerIns());

        public void SetFullHp()
        {
            var playerIns = GetPlayerIns();
            var full = chrInsService.GetMaxHp(playerIns);
            chrInsService.SetHp(playerIns, full);
        }

        public void SetRfbs()
        {
            var playerIns = GetPlayerIns();
            var full = chrInsService.GetMaxHp(playerIns);
            chrInsService.SetHp(playerIns, (full * 20) / 100 - 1);
        }

        public void SetFp(int fp) =>
            memoryService.Write(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Fp, fp);

        public int GetCurrentFp() =>
            memoryService.Read<int>(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Fp);

        public void SetSp(int sp) =>
            memoryService.Write(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Sp, sp);

        public int GetCurrentSp() =>
            memoryService.Read<int>(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Sp);

        public float GetSpeed() => chrInsService.GetSpeed(GetPlayerIns());
        public void SetSpeed(float speed) => chrInsService.SetSpeed(GetPlayerIns(), speed);

        public void ToggleInfinitePoise(bool isInfinitePoiseEnabled)
        {
            var poiseCode = CodeCaveOffsets.Base + CodeCaveOffsets.InfinitePoise;
            var noGrabCode = CodeCaveOffsets.Base + CodeCaveOffsets.NoGrab;

            if (isInfinitePoiseEnabled)
            {
                HookPoiseDamage(poiseCode);
                HookGrab(noGrabCode);
            }
            else
            {
                hookManager.UninstallHook(poiseCode.ToInt64());
                hookManager.UninstallHook(noGrabCode.ToInt64());
            }
        }

        private void HookPoiseDamage(IntPtr code)
        {
            var hook = Hooks.InfinitePoise;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.InfinitePoise);

            var originalBytes = OriginalBytesByPatch.InfinitePoise.GetOriginal();
            Array.Copy(originalBytes, 0, bytes, 0, originalBytes.Length);


            var patchSpecificPlayerIns = WorldChrMan.PlayerIns;
            AsmHelper.WriteImmediateDwords(bytes, new[]
            {
                (patchSpecificPlayerIns, 0xF + 3),
                (patchSpecificPlayerIns, 0x18 + 3)
            });

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (code.ToInt64() + 0x8, WorldChrMan.Base.ToInt64(), 7, 0x8 + 3),
                (code.ToInt64() + 0x3D, WorldChrMan.Base.ToInt64(), 7, 0x3D + 3),
                (code.ToInt64() + 0x53, Functions.GetChrInsByEntityId, 5, 0x53 + 1),
                (code.ToInt64() + 0x6A, hook + 0x7, 5, 0x6A + 1)
            });

            memoryService.WriteBytes(code, bytes);

            hookManager.InstallHook(code.ToInt64(), hook, originalBytes);
        }

        private void HookGrab(IntPtr noGrabCode)
        {
            var hook = Hooks.NoGrab;
            var skipGrabJmpLoc = hook + 0x95;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoGrab);

            AsmHelper.WriteImmediateDwords(codeBytes, new[] { (WorldChrMan.PlayerIns, 0x8 + 3) });

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (noGrabCode.ToInt64() + 0x1, WorldChrMan.Base.ToInt64(), 7, 0x1 + 3),
                (noGrabCode.ToInt64() + 0x14, skipGrabJmpLoc, 6, 0x14 + 2),
                (noGrabCode.ToInt64() + 0x23, hook + 0x9, 5, 0x23 + 1)
            });
            memoryService.WriteBytes(noGrabCode, codeBytes);
            hookManager.InstallHook(noGrabCode.ToInt64(), hook, new byte[]
                { 0x41, 0x8B, 0x56, 0x44, 0x48, 0x8D, 0x4C, 0x24, 0x40 });
        }

        public void ToggleDebugFlag(int offset, bool isEnabled, bool needsReminder = false)
        {
            if (needsReminder) reminderService.TrySetReminder();
            memoryService.Write(ChrDbgFlags.Base + offset, isEnabled);
        }

        public void ToggleNoDamage(bool isNoDamageEnabled)
        {
            reminderService.TrySetReminder();
            chrInsService.ToggleNoDamage(GetPlayerIns(), isNoDamageEnabled);
        }

        public void ToggleNoHit(bool isNoHitEnabled)
        {
            reminderService.TrySetReminder();
            memoryService.SetBitValue(GetChrInsFlagsPtr(), (int)ChrIns.ChrInsFlags.NoHit, isNoHitEnabled);
        }

        public void ToggleNoRuneGain(bool isNoRuneGainEnabled) =>
            memoryService.WriteBytes(Patches.NoRunesFromEnemies,
                isNoRuneGainEnabled
                    ? [0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90]
                    : OriginalBytesByPatch.NoRunesFromEnemies.GetOriginal());

        public void ToggleNoRuneArcLoss(bool isNoRuneArcLossEnabled) =>
            memoryService.Write(Patches.NoRuneArcLoss, isNoRuneArcLossEnabled ? (byte)0xEB : (byte)0x74);

        private byte[] _originalRuneBytes;

        public void ToggleNoRuneLoss(bool isNoRuneLossEnabled)
        {
            if (isNoRuneLossEnabled)
            {
                _originalRuneBytes = memoryService.ReadBytes(Patches.NoRuneLossOnDeath, 6);
                var bytes = _originalRuneBytes.ToArray();
                bytes[0] = 0xE9;

                int offset = BitConverter.ToInt32(bytes, 2) + 1;
                Buffer.BlockCopy(BitConverter.GetBytes(offset), 0, bytes, 1, 4);

                bytes[5] = 0x90;
                memoryService.WriteBytes(Patches.NoRuneLossOnDeath, bytes);
            }
            else if (_originalRuneBytes != null)
            {
                memoryService.WriteBytes(Patches.NoRuneLossOnDeath, _originalRuneBytes);
            }
        }

        public void ToggleNoTimePassOnDeath(bool isNoTimePassOnDeathEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.SaveCurrentTime;
            if (isNoTimePassOnDeathEnabled)
            {
                var hook = Hooks.NoTimePassOnDeath.ToInt64();
                var bytes = AsmLoader.GetAsmBytes(AsmScript.NoTimePassOnDeath);
                AsmHelper.WriteRelativeOffsets(bytes, new[]
                {
                    (code.ToInt64() + 0x8, WorldAreaTimeImpl.Base.ToInt64(), 7, 0x8 + 3),
                    (code.ToInt64() + 0xF, GameMan.Base.ToInt64(), 7, 0xF + 3),
                    (code.ToInt64() + 0x28, hook + 5, 5, 0x28 + 1)
                });


                //Patch specific offsets within GameMan
                int savedTimeMovIndex1 = 0x19 + 3;
                int savedTimeMovIndex2 = 0x21 + 3;
                bytes[savedTimeMovIndex1] = (byte)GameMan.StoredTime;
                bytes[savedTimeMovIndex2] = (byte)(GameMan.StoredTime + 8);

                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hook, [0x4C, 0x8B, 0x74, 0x24, 0x70]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void SetNewGame(int value) =>
            memoryService.Write(memoryService.Read<nint>(GameDataMan.Base) + GameDataMan.NewGame, value);

        public int GetNewGame() =>
            memoryService.Read<int>(memoryService.Read<nint>(GameDataMan.Base) + GameDataMan.NewGame);

        public void GiveRunes(int runes)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.GiveRunes);
            var playerGameData =
                memoryService.Read<nint>(memoryService.Read<nint>(GameDataMan.Base) + GameDataMan.PlayerGameData);
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (playerGameData, 0x0 + 2),
                (runes, 0xA + 2),
                (Functions.GiveRunes, 0x14 + 2)
            });

            memoryService.AllocateAndExecute(bytes);
        }

        public int GetRuneLevel() =>
            memoryService.Read<int>(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.RuneLevel);

        public Stats GetStats()
        {
            const int statsStart = (int)GameDataMan.PlayerGameDataOffsets.Vigor;
            var ptr = GetGameDataPtr() + statsStart;
            var block = new MemoryBlock(memoryService.ReadBytes(ptr, StatsBlockSize));

            return new Stats
            {
                Vigor = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Vigor - statsStart),
                Mind = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Mind - statsStart),
                Endurance = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Endurance - statsStart),
                Strength = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Strength - statsStart),
                Dexterity = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Dexterity - statsStart),
                Intelligence = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Intelligence - statsStart),
                Faith = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Faith - statsStart),
                Arcane = block.Get<int>((int)GameDataMan.PlayerGameDataOffsets.Arcane - statsStart),
            };
        }

        public void SetStat(int offset, int newValue)
        {
            var gameData = GetGameDataPtr();
            var currentStatVal = memoryService.Read<int>(gameData + offset);

            if (currentStatVal == newValue) return;

            var diff = newValue - currentStatVal;
            var levelPtr = gameData + (int)GameDataMan.PlayerGameDataOffsets.RuneLevel;
            var currentLevel = memoryService.Read<int>(levelPtr);

            if (newValue > currentStatVal)
            {
                long runeCost = 0;
                for (int i = 1; i <= diff; i++)
                {
                    runeCost += CalculateLevelUpCost(currentLevel + i);
                }

                var runeMemPtr = gameData + (int)GameDataMan.PlayerGameDataOffsets.RuneMemory;
                var currentRuneMem = memoryService.Read<uint>(runeMemPtr);
                var newRuneMem = Math.Min(currentRuneMem + (ulong)runeCost, 0xFFFFFFFF);
                memoryService.Write(runeMemPtr, (uint)newRuneMem);
            }

            memoryService.Write(levelPtr, currentLevel + diff);
            memoryService.Write(gameData + offset, newValue);
        }

        public long GetHandle() =>
            memoryService.Read<long>(GetPlayerIns() + WorldChrMan.PlayerInsOffsets.Handle);

        public void ToggleNoGravity(bool isEnabled)
        {
            chrInsService.ToggleNoGravity(GetTorrentChrIns(), isEnabled);
            chrInsService.ToggleNoGravity(GetPlayerIns(), isEnabled);
        }

        public void ToggleTorrentNoDeath(bool isEnabled) => chrInsService.ToggleNoDeath(GetTorrentChrIns(), isEnabled);

        public void SetScadu(int value) =>
            memoryService.Write(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.Scadutree, (byte)value);

        public int GetScadu() =>
            memoryService.Read<byte>(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.Scadutree);

        public void SetSpiritAsh(int value) =>
            memoryService.Write(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.SpiritAsh, (byte)value);

        public int GetSpiritAsh() =>
            memoryService.Read<byte>(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.SpiritAsh);

        public int GetCurrentAnimation() => chrInsService.GetCurrentAnimation(GetPlayerIns());

        public void ToggleTorrentAnywhere(bool isEnabled)
        {
            if (isEnabled)
            {
                memoryService.WriteBytes(Patches.IsTorrentDisabledInUnderworld, [0x30, 0xC0, 0x90]);
                memoryService.WriteBytes(Patches.IsWhistleDisabled, [0x30, 0xC0, 0x90,]);
                memoryService.Write(GetChrRidePtr() + (int)ChrIns.ChrRideOffsets.IsHorseWhistleDisabled, (byte)0);
            }
            else
            {
                memoryService.WriteBytes(Patches.IsTorrentDisabledInUnderworld, [0x0F, 0x95, 0xC0]);
                memoryService.WriteBytes(Patches.IsWhistleDisabled, [0x0F, 0x95, 0xC0]);
            }
        }

        private int CalculateLevelUpCost(int nextLevel)
        {
            float baseLevel = nextLevel + BaseLevelOffset;
            float adjustedLevel = Math.Max(0f, baseLevel - LevelUpIncreaseInterval);
            float cost = baseLevel * baseLevel
                                   * (LevelUpCostIncrease * adjustedLevel + InitialLevelUpCost)
                         + InitialLevelUpOffset;

            return (int)cost;
        }

        private nint GetGameDataPtr() =>
            memoryService.FollowPointers(GameDataMan.Base, [GameDataMan.PlayerGameData], true);

        private IntPtr GetChrDataPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrDataModule], true);

        private IntPtr GetChrPhysicsPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);

        private IntPtr GetChrRidePtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrRideModule], true);

        private IntPtr GetChrInsFlagsPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ChrIns.Flags], false);

        private Position GetPlayerPosition()
        {
            var playerIns = GetPlayerIns();

            uint currentBlockId =
                memoryService.Read<uint>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentBlockId);

            var coords = memoryService.Read<Vector3>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapCoords);
            var angle = memoryService.Read<float>(playerIns + WorldChrMan.PlayerInsOffsets.CurrentMapAngle);

            return new Position(currentBlockId, coords, angle);
        }
    }
}