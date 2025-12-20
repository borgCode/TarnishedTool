using System;
using System.Numerics;
using System.Threading.Tasks;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Models;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class PlayerService(MemoryService memoryService, HookManager hookManager, ITravelService travelService)
        : IPlayerService
    {
        private const float LongDistanceRestore = 500f;

        private const float InitialLevelUpCost = 0.1f;
        private const float InitialLevelUpOffset = 1f;
        private const float LevelUpCostIncrease = 0.02f;
        private const float LevelUpIncreaseInterval = 92f;
        private const int BaseLevelOffset = 80;

        private readonly Position[] _positions =
        [
            new(0, Vector3.Zero, 0f),
            new(0, Vector3.Zero, 0f)
        ];

        public Vector3 GetPlayerPos() =>
            memoryService.ReadVector3(GetChrPhysicsPtr() + (int)ChrIns.ChrPhysicsOffsets.Coords);

        public void SavePos(int index)
        {
            var posToSave = _positions[index];
            var worldChrMan = memoryService.ReadInt64(WorldChrMan.Base);
            var playerIns = (IntPtr)memoryService.ReadInt64((IntPtr)worldChrMan + WorldChrMan.PlayerIns);
            posToSave.BlockId = memoryService.ReadUInt32(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentBlockId);
            posToSave.Coords =
                memoryService.ReadVector3(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalCoords);
            posToSave.Angle =
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
                var currentCoords =
                    memoryService.ReadVector3(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalCoords);
                var currentAbsolute = GetAbsoluteCoords(currentCoords, currentBlockId);
                var savedAbsolute = GetAbsoluteCoords(savedPos.Coords, savedPos.BlockId);
                var delta = savedAbsolute - currentAbsolute;

                var chrRideModule = GetChrRidePtr();
                var isRiding = IsRiding(chrRideModule);
                var physicsPtr = isRiding ? GetTorrentPhysicsPtr(chrRideModule) : GetChrPhysicsPtr();
                var coordsPtr = physicsPtr + (int)ChrIns.ChrPhysicsOffsets.Coords;
                var isLongDistance = delta.Length() > LongDistanceRestore;

                if (isLongDistance)
                    memoryService.WriteUInt8(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, 1);

                memoryService.WriteVector3(coordsPtr, memoryService.ReadVector3(coordsPtr) + delta);
                memoryService.WriteFloat(playerIns + (int)WorldChrMan.PlayerInsOffsets.CurrentGlobalAngle,
                    savedPos.Angle);

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
                _ = Task.Run(() => travelService.WarpToBlockId(savedPos));
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

        public PosWithHurtbox GetPosWithHurtbox()
        {
            var physPtr = GetChrPhysicsPtr();
            var position = memoryService.ReadVector3(physPtr + (int)ChrIns.ChrPhysicsOffsets.Coords);
            var capsuleRadius = memoryService.ReadFloat(physPtr + (int)ChrIns.ChrPhysicsOffsets.HurtCapsuleRadius);
            return new PosWithHurtbox(position, capsuleRadius);
        }

        public long GetPlayerIns() =>
            memoryService.ReadInt64((IntPtr)memoryService.ReadInt64(WorldChrMan.Base) + WorldChrMan.PlayerIns);

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
            memoryService.ReadInt32(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.RuneLevel);

        public Stats GetStats()
        {
            Stats stats = new Stats();
            var gameData = GetGameDataPtr();
            stats.Vigor = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Vigor);
            stats.Mind = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Mind);
            stats.Endurance = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Endurance);
            stats.Strength = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Strength);
            stats.Dexterity = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Dexterity);
            stats.Intelligence =
                memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Intelligence);
            stats.Faith = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Faith);
            stats.Arcane = memoryService.ReadInt32(gameData + (int)GameDataMan.PlayerGameDataOffsets.Arcane);
            return stats;
        }

        public void SetStat(int offset, int newValue)
        {
            var gameData = GetGameDataPtr();
            var currentStatVal = memoryService.ReadInt32(gameData + offset);

            if (currentStatVal == newValue) return;

            var diff = newValue - currentStatVal;
            var levelPtr = gameData + (int)GameDataMan.PlayerGameDataOffsets.RuneLevel;
            var currentLevel = memoryService.ReadInt32(levelPtr);

            if (newValue > currentStatVal)
            {
                long runeCost = 0;
                for (int i = 1; i <= diff; i++)
                {
                    runeCost += CalculateLevelUpCost(currentLevel + i);
                }

                var runeMemPtr = gameData + (int)GameDataMan.PlayerGameDataOffsets.RuneMemory;
                var currentRuneMem = memoryService.ReadUInt32(runeMemPtr);
                var newRuneMem = Math.Min(currentRuneMem + (ulong)runeCost, 0xFFFFFFFF);
                memoryService.WriteUInt32(runeMemPtr, (uint)newRuneMem);
            }

            memoryService.WriteInt32(levelPtr, currentLevel + diff);
            memoryService.WriteInt32(gameData + offset, newValue);
        }

        public long GetHandle()
        {
            var playerIns =
                memoryService.ReadInt64((IntPtr)memoryService.ReadInt64(WorldChrMan.Base) + WorldChrMan.PlayerIns);
            return memoryService.ReadInt64((IntPtr)playerIns + (int)WorldChrMan.PlayerInsOffsets.Handle);
        }

        public void EnableGravity()
        {
            var chrRideModule = GetChrRidePtr();
            var torrentPhysicsPtr = GetTorrentPhysicsPtr(chrRideModule);
            memoryService.WriteUInt8(torrentPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, 0);
            memoryService.WriteUInt8(GetChrPhysicsPtr() + (int)ChrIns.ChrPhysicsOffsets.NoGravity, 0);
        }

        public void ToggleTorrentNoDeath(bool isEnabled)
        {
            var chrRideModule = GetChrRidePtr();
            var rideNode = memoryService.ReadInt64(chrRideModule + (int)ChrIns.ChrRideOffsets.RideNode);
            var handle = memoryService.ReadInt32((IntPtr)rideNode + (int)ChrIns.RideNodeOffsets.HorseHandle);
            var torrentChrIns = ChrInsLookup(handle);
            var bitFlags = memoryService.FollowPointers(torrentChrIns,
                [..ChrIns.ChrDataModule, (int)ChrIns.ChrDataOffsets.Flags],
                false, false);
            memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDeath, isEnabled);
        }

        public void SetScadu(int value) =>
            memoryService.WriteUInt8(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.Scadutree, value);

        public int GetScadu() =>
            memoryService.ReadUInt8(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.Scadutree);

        public void SetSpiritAsh(int value) =>
            memoryService.WriteUInt8(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.SpiritAsh, value);

        public int GetSpiritAsh() =>
            memoryService.ReadUInt8(GetGameDataPtr() + (int)GameDataMan.PlayerGameDataOffsets.SpiritAsh);

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

        private IntPtr GetChrBehaviorPtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrBehaviorModule], true);

        private IntPtr GetChrRidePtr() =>
            memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrRideModule], true);

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