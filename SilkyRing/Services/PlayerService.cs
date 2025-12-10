using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Models;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class PlayerService(MemoryService memoryService, HookManager hookManager) : IPlayerService
    {
        private const float LongDistanceRestore = 500f;

        private readonly Position[] _positions =
        [
            new(0, Vector3.Zero, 0f),
            new(0, Vector3.Zero, 0f)
        ];

        public Vector3 GetPlayerPos() =>
            ReadVector3(GetChrPhysicsPtr() + (int)ChrIns.ChrPhysicsOffsets.Coords);

        public void SavePos(int index)
        {
            var posToSave = _positions[index];
            var worldChrMan = memoryService.ReadInt64(WorldChrMan.Base);
            var playerIns = (IntPtr)memoryService.ReadInt64((IntPtr)worldChrMan + WorldChrMan.PlayerIns);
            posToSave.BlockId = memoryService.ReadUInt32(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentBlockId);
            posToSave.GlobalCoords = ReadVector3(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalCoords);
            posToSave.GlobalAngle =
                memoryService.ReadFloat(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalAngle);
        }

        public void RestorePos(int index)
        {
            var savedPos = _positions[index];

            var worldChrMan = memoryService.ReadInt64(WorldChrMan.Base);
            var playerIns = (IntPtr)memoryService.ReadInt64((IntPtr)worldChrMan + WorldChrMan.PlayerIns);

            uint currentBlockId =
                memoryService.ReadUInt32(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentBlockId);
            uint currentArea = (currentBlockId >> 24) & 0xFF;
            uint savedArea = (savedPos.BlockId >> 24) & 0xFF;

            if (currentArea == savedArea)
            {
                var currentCoords = ReadVector3(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalCoords);
                var currentAbsolute = GetAbsoluteCoords(currentCoords, currentBlockId);
                var savedAbsolute = GetAbsoluteCoords(savedPos.GlobalCoords, savedPos.BlockId);
                var delta = savedAbsolute - currentAbsolute;

                var chrRideModule = GetChrRidePtr();
                var isRiding = IsRiding(chrRideModule);
                var physicsPtr = isRiding ? GetTorrentPhysicsPtr(chrRideModule) : GetChrPhysicsPtr();
                var coordsPtr = physicsPtr + (int)ChrIns.ChrPhysicsOffsets.Coords;
                var isLongDistance = delta.Length() > LongDistanceRestore;

                if (isLongDistance)
                    memoryService.WriteUInt8(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, 1);

                WriteVector3(coordsPtr, ReadVector3(coordsPtr) + delta);
                memoryService.WriteFloat(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalAngle,
                    savedPos.GlobalAngle);

                if (isLongDistance)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        memoryService.WriteUInt8(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, 0);
                    });
                }
            }

            else
            {
                _ = Task.Run(() => WarpToBlockId(savedPos));
            }
        }

        private bool IsRiding(IntPtr chrRideModule)
        {
            var rideNode = memoryService.ReadInt64(chrRideModule + (int)ChrIns.ChrRideOffsets.RideNode);
            return memoryService.ReadInt32((IntPtr)rideNode + (int)ChrIns.RideNodeOffsets.IsRiding) != 0;
        }

        private IntPtr GetTorrentPhysicsPtr(IntPtr chrRideModule)
        {
            var rideNode = memoryService.ReadInt64(chrRideModule + (int)ChrIns.ChrRideOffsets.RideNode);
            var handle = memoryService.ReadInt32((IntPtr)rideNode + (int)ChrIns.RideNodeOffsets.HorseHandle);
            var torrentChrIns = ChrInsLookup(handle);
            return memoryService.FollowPointers(torrentChrIns, [..ChrIns.ChrPhysicsModule], true, false);
        }

        Vector3 GetAbsoluteCoords(Vector3 globalCoords, uint blockId)
        {
            byte area = (byte)((blockId >> 24) & 0xFF);

            if (area == 0x3C)
            {
                byte gridX = (byte)((blockId >> 16) & 0xFF);
                byte gridZ = (byte)((blockId >> 8) & 0xFF);

                return new Vector3(
                    globalCoords.X + 256 * gridX,
                    globalCoords.Y,
                    globalCoords.Z + 256 * gridZ
                );
            }

            return globalCoords;
        }

        private void WarpToBlockId(Position savedPos)
        {
            int area = (int)(savedPos.BlockId >> 24) & 0xFF;
            int block = (int)(savedPos.BlockId >> 16) & 0xFF;
            int map = (int)(savedPos.BlockId >> 8) & 0xFF;
            int altNo = (int)savedPos.BlockId & 0xFF;

            var bytes = AsmLoader.GetAsmBytes("WarpToBlock");
            AsmHelper.WriteAbsoluteAddress(bytes, Functions.WarpToBlock, 0x16 + 2);
            AsmHelper.WriteImmediateDwords(bytes, new[]
            {
                (area, 0x0 + 1),
                (block, 0x5 + 1),
                (map, 0xA + 2),
                (altNo, 0x10 + 2),
            });

            memoryService.AllocateAndExecute(bytes);

            HookWarpCoordWrites(savedPos);
        }

        private void HookWarpCoordWrites(Position savedPos)
        {
            int angleOffset = 0xAB0;

            var coordHook = Hooks.WarpCoordWrite;
            var angleHook = Hooks.WarpAngleWrite;

            var targetCoords = CodeCaveOffsets.Base + CodeCaveOffsets.WarpCoords;
            var targetAngle = CodeCaveOffsets.Base + CodeCaveOffsets.Angle;
            var warpCode = CodeCaveOffsets.Base + CodeCaveOffsets.WarpCode;
            var angleCode = CodeCaveOffsets.Base + CodeCaveOffsets.AngleCode;
            WriteVector3(targetCoords, savedPos.GlobalCoords);
            memoryService.WriteFloat(targetCoords + 0xC, 1f);
            memoryService.WriteFloat(targetAngle + 0x4, savedPos.GlobalAngle);

            var bytes = AsmLoader.GetAsmBytes("WarpCoordWrite");
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (warpCode.ToInt64(), targetCoords.ToInt64(), 7, 0x0 + 3),
                (warpCode.ToInt64() + 0xE, coordHook + 7, 5, 0xE + 1)
            });
            memoryService.WriteBytes(warpCode, bytes);

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (angleCode.ToInt64(), targetAngle.ToInt64(), 7, 0x0 + 3),
                (angleCode.ToInt64() + 0xE, angleHook + 7, 5, 0xE + 1)
            });
            memoryService.WriteBytes(angleCode, bytes);
            memoryService.WriteInt32(angleCode + 0x7 + 3, angleOffset);

            hookManager.InstallHook(warpCode.ToInt64(), coordHook, [0x0F, 0x11, 0x80, 0xA0, 0x0A, 0x00, 0x00]);
            hookManager.InstallHook(angleCode.ToInt64(), angleHook, [0x0F, 0x11, 0x80, 0xB0, 0x0A, 0x00, 0x00]);

            var isFadedPtr = (IntPtr)memoryService.ReadInt64(MenuMan.Base) + MenuMan.FadeFlags;
            var fadeBit = (byte)MenuMan.FadeBitFlags.IsFadeScreen;

            WaitForCondition(() => memoryService.IsBitSet(isFadedPtr, fadeBit));
            WaitForCondition(() => !memoryService.IsBitSet(isFadedPtr, fadeBit));

            hookManager.UninstallHook(warpCode.ToInt64());
            hookManager.UninstallHook(angleCode.ToInt64());
        }

        private void WaitForCondition(Func<bool> condition, int timeoutMs = 10000, int pollMs = 50)
        {
            int start = Environment.TickCount;
            while (!condition() && Environment.TickCount < start + timeoutMs)
            {
                Thread.Sleep(pollMs);
            }
        }

        public PosWithHurtbox GetPosWithHurtbox()
        {
            var physPtr = GetChrPhysicsPtr();
            var position = ReadVector3(physPtr + (int)ChrIns.ChrPhysicsOffsets.Coords);
            var capsuleRadius = memoryService.ReadFloat(physPtr + (int)ChrIns.ChrPhysicsOffsets.HurtCapsuleRadius);
            return new PosWithHurtbox(position, capsuleRadius);
        }

        public void SetHp(int hp) =>
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health, hp);

        public int GetCurrentHp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health);

        public int GetMaxHp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHealth);

        public void SetFullHp()
        {
            var full = memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHealth);
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health, full);
        }

        public void SetRfbs()
        {
            var full = memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHealth);
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health, (full * 20) / 100 - 1);
        }

        public void SetFp(int fp) =>
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Fp, fp);

        public int GetCurrentFp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Fp);

        public void SetSp(int sp) =>
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Sp, sp);

        public int GetCurrentSp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Sp);

        public float GetSpeed() =>
            memoryService.ReadFloat(GetChrBehaviorPtr() + (int)ChrIns.ChrBehaviorOffsets.AnimSpeed);

        public void SetSpeed(float speed) =>
            memoryService.WriteFloat(GetChrBehaviorPtr() + (int)ChrIns.ChrBehaviorOffsets.AnimSpeed, speed);

        public void ToggleInfinitePoise(bool isInfinitePoiseEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.InfinitePoise;

            if (isInfinitePoiseEnabled)
            {
                var hook = Hooks.InfinitePoise;
                var codeBytes = AsmLoader.GetAsmBytes("InfinitePoise");
                var bytes = BitConverter.GetBytes(WorldChrMan.Base.ToInt64());
                Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                AsmHelper.WriteJumpOffsets(codeBytes, new[]
                {
                    (hook, 7, code + 0x1D, 0x1D + 1),
                    (hook, 7, code + 0x2A, 0x2A + 1),
                });
                memoryService.WriteBytes(code, codeBytes);
                hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x80, 0xBF, 0x5F, 0x02, 0x00, 0x00, 0x00 });
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ApplySpEffect(long spEffectId)
        {
            var bytes = AsmLoader.GetAsmBytes("SetSpEffect");
            var playerIns =
                memoryService.ReadInt64((IntPtr)memoryService.ReadInt64(WorldChrMan.Base) + WorldChrMan.PlayerIns);
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (playerIns, 0x0 + 2),
                (spEffectId, 0xA + 2),
                (Functions.SetSpEffect, 0x18 + 2)
            });
            memoryService.AllocateAndExecute(bytes);
        }
        
        public void ToggleDebugFlag(int offset, bool isEnabled) =>
            memoryService.WriteUInt8(WorldChrManDbg.Base + offset, isEnabled ? 1 : 0);

        public void ToggleNoDamage(bool isFreezeHealthEnabled)
        {
            var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Flags;
            memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage, isFreezeHealthEnabled);
        }

        public void ToggleNoRuneGain(bool isNoRuneGainEnabled) =>
            memoryService.WriteBytes(Patches.NoRunesFromEnemies,
                isNoRuneGainEnabled
                    ? [0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90]
                    : [0x41, 0xFF, 0x91, 0xC8, 0x05, 0x00, 0x00]);

        public void ToggleNoRuneArcLoss(bool isNoRuneArcLossEnabled) =>
            memoryService.WriteUInt8(Patches.NoRuneArcLoss, isNoRuneArcLossEnabled ? 0xEB : 0x74);

        public void ToggleNoRuneLoss(bool isNoRuneLossEnabled) =>
            memoryService.WriteBytes(Patches.NoRuneLossOnDeath,
                isNoRuneLossEnabled
                    ? [0x90, 0x90, 0x90]
                    : [0x89, 0x45, 0x6C]);

        public void SetNewGame(int value) =>
            memoryService.WriteInt32((IntPtr)memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.NewGame, value);

        public int GetNewGame() =>
            memoryService.ReadInt32((IntPtr)memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.NewGame);

        public void GiveRunes(int runes)
        {
            var bytes = AsmLoader.GetAsmBytes("GiveRunes");
            var playerGameData =
                memoryService.ReadInt64((IntPtr)memoryService.ReadInt64(GameDataMan.Base) +
                                        GameDataMan.PlayerGameData);
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (playerGameData, 0x0 + 2),
                (runes, 0xA + 2),
                (Functions.GiveRunes, 0x14 + 2)
            });

            memoryService.AllocateAndExecute(bytes);
        }

        public int GetRuneLevel() =>
            memoryService.ReadInt32((IntPtr)memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.RuneLevel);

        private IntPtr GetChrDataPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrDataModule], true);

        private IntPtr GetChrPhysicsPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);

        private IntPtr GetChrBehaviorPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrBehaviorModule], true);

        private IntPtr GetChrRidePtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrRideModule], true);

        private Vector3 ReadVector3(IntPtr address)
        {
            byte[] coordBytes = memoryService.ReadBytes(address, 12);
            return new Vector3(
                BitConverter.ToSingle(coordBytes, 0),
                BitConverter.ToSingle(coordBytes, 4),
                BitConverter.ToSingle(coordBytes, 8)
            );
        }

        private void WriteVector3(IntPtr address, Vector3 value)
        {
            byte[] coordBytes = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(value.X), 0, coordBytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(value.Y), 0, coordBytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(value.Z), 0, coordBytes, 8, 4);
            memoryService.WriteBytes(address, coordBytes);
        }

        private IntPtr ChrInsLookup(int handle)
        {
            int poolIndex = (handle >> 20) & 0xFF;
            int slotIndex = handle & 0xFFFFF;

            var worldChrMan = (IntPtr)memoryService.ReadInt64(WorldChrMan.Base);
            var chrSet = (IntPtr)memoryService.ReadInt64(worldChrMan + WorldChrMan.ChrSetPool + poolIndex * 8);
            var entriesBase = (IntPtr)memoryService.ReadInt64(chrSet + (int)WorldChrMan.ChrSetOffsets.ChrSetEntries);
            var chrIns = (IntPtr)memoryService.ReadInt64(entriesBase + slotIndex * 16);

            return chrIns;
        }
    }
}