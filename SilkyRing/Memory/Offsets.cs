using System;
using System.Diagnostics.CodeAnalysis;

namespace SilkyRing.Memory
{
    public static class Offsets
    {
        public static class WorldChrMan
        {
            public static IntPtr Base;

            public const int ChrSetPool = 0x1DED8;

            public enum ChrSetOffsets
            {
                ChrSetEntries = 0x18
            }
            
            public const int PlayerIns = 0x1E508;
            
            public enum PlayerInsOffsets
            {
                Handle = 0x8,
                CurrentBlockId = 0x6D0,
                CurrentGlobalCoords = 0x6C0,
                CurrentGlobalAngle = 0x6CC,
            }
        }

        public static class ChrIns
        {
            public const int ChrCtrl = 0x58;
            public const int SpEffectMan = 0x178;
            public const int Modules = 0x190;
            public const int Flags = 0x530;

            public enum ChrInsFlags
            {
                NoHit = 1 << 3,
                NoAttack = 1 << 4,
                NoMove = 1 << 5,
                // 1 << 6 is a red capsule towards the direction the boss is facing
                //1 << 7 same but white capsule
            }
            
            public const int ChrManipulator = 0x580;
            
            public static readonly int[] ChrCtrlFlags = [0xC8, 0x24];
            public static readonly BitFlag DisableAi = new(0x0, 1 << 0);

            public static readonly int[] ChrDataModule = [Modules, 0x0];
            public static readonly int[] ChrTimeActModule = [Modules, 0x18];
            public static readonly int[] ChrResistModule = [Modules, 0x20];
            public static readonly int[] ChrBehaviorModule = [Modules, 0x28];
            public static readonly int[] ChrSuperArmorModule = [Modules, 0x40];
            public static readonly int[] ChrPhysicsModule = [Modules, 0x68];
            public static readonly int[] ChrRideModule = [Modules, 0xE8];

            public enum ChrDataOffsets
            {
                Health = 0x138,
                MaxHealth = 0x13C,
                Fp = 0x148,
                MaxFp = 0x14C,
                Sp = 0x154,
                MaxSp = 0x158,
                Flags = 0x19B,
            }

            [Flags]
            public enum ChrDataBitFlags
            {
                NoDeath = 1 << 0,
                NoDamage = 1 << 1,
            }
            
            public enum ChrTimeActOffsets
            {
                AnimationId = 0x20,
            }
            
            public enum ChrResistOffsets
            {
                PoisonCurrent = 0x10,
                RotCurrent = 0x14,
                BleedCurrent = 0x18,
                FrostCurrent = 0x20,
                SleepCurrent = 0x24,
                PoisonMax = 0x2C,
                RotMax = 0x30,
                BleedMax = 0x34,
                FrostMax = 0x3C,
                SleepMax = 0x40,
            }

            public enum ChrBehaviorOffsets
            {
                AnimSpeed = 0x17C8,
            }

            public enum ChrSuperArmorOffsets
            {
                CurrentPoise = 0x10,
                MaxPoise = 0x14,
                PoiseTimer = 0x1C,
            }

            public enum ChrPhysicsOffsets
            {
                Coords = 0x70,
                NoGravity = 0x1D6,
                HurtCapsuleRadius = 0x344
            }

            public enum ChrRideOffsets
            {
                RideNode = 0x10,
            }

            public enum RideNodeOffsets
            {
                HorseHandle = 0x18,
                IsRiding = 0x50
            }

            public static readonly int[] AiThink = [ChrManipulator, 0xC0];

            public enum AiThinkOffsets
            {
                NpcThinkParamId = 0x28,
                AnimationRequest = 0xC428,
                TargetingSystem = 0xC480,
                ForceAct = 0xE9C1,
                LastAct = 0xE9C2,
            }
            
            public static readonly int[] NpcParam = [ChrManipulator, 0xC0, 0x18];
            public static readonly int[] NpcThinkParam = [ChrManipulator, 0xC0, 0x30];

            public enum NpcParamOffsets
            {
                PoisonImmune = 0x64,
                RotImmune = 0x68,
                BleedImmune = 0x178,
                FrostImmune = 0x180,
                SleepImmune = 0x184,
                StandardAbsorption = 0x1A4, 
                SlashAbsorption = 0x1A8, 
                StrikeAbsorption = 0x1AC, 
                ThrustAbsorption = 0x1B0, 
                MagicAbsorption = 0x1B4,
                FireAbsorption = 0x1B8, 
                LightningAbsorption = 0x1BC, 
                HolyAbsorption = 0x1C0,
            }
            
            
            public static readonly BitFlag BlueTargetView = new(0xC8, 1 << 4);
            public static readonly BitFlag YellowTargetView = new(0xC8, 1 << 5);
            public static readonly BitFlag WhiteLineToPlayer = new(0xC8, 1 << 6);
            
        }

        public static class FieldArea
        {
            public static IntPtr Base;

            // +0xA0 = Current Dungeon / Boss flag
        }
        
        public static class InputManager
        {
            public static IntPtr Base;

            // +0xA0 = Current Dungeon / Boss flag
        }

        public static class LuaEventMan
        {
            public static IntPtr Base;
        }

        public static class VirtualMemFlag
        {
            public static IntPtr Base;
        }

        public static class DamageManager
        {
            public static IntPtr Base;

            public const int HitboxView = 0xA0;
            public const int HitboxView2 = 0xA1;
        }

        public static class WorldHitMan
        {
            public static IntPtr Base;

            public const int LowHit = 0xC;
            public const int HighHit = 0xD;
            public const int Ragdoll = 0xE;
            public const int Mode = 0x14;
        }

        public static class MenuMan
        {
            public static IntPtr Base;

            public const int IsLoaded = 0x94;
            public const int FadeFlags = 0x96;

            public enum FadeBitFlags
            {
                IsFadeScreen = 1 << 1,
            }
        }

        public static class GameDataMan
        {
            public static IntPtr Base;

            public const int PlayerGameData = 0x8;

            public enum PlayerGameDataOffsets
            {
                Vigor = 0x3C,
                Mind = 0x40,
                Endurance = 0x44,
                Strength = 0x48,
                Dexterity = 0x4C,
                Intelligence = 0x50,
                Faith = 0x54,
                Arcane = 0x58,
                RuneLevel = 0x68,
                Runes = 0x6C,
                RuneMemory = 0x70
            }
            public const int NewGame = 0x120;
        }

        public static class TargetView
        {
            public static IntPtr Base;

            public const int Blue = 0x0;
            public const int Yellow = 0x1;
        }

        public static class GameMan
        {
            public static IntPtr Base;

            public const int ShouldQuitout = 0x10;
            public const int ForceSave = 0xb72;
        }
        
        public static class MapItemManImpl
        {
            public static IntPtr Base;
        }
        
        public static class WorldChrManDbg
        {
            public static IntPtr Base;

            public const int PlayerNoDeath = 0x8;
            public const int AllChrsSpheres = 0x9;
            public const int OneShot = 0xA;
            public const int InfiniteGoods = 0xB;
            public const int InfiniteStam = 0xC;
            public const int InfiniteFp = 0xD;
            public const int InfiniteArrows = 0xE;
            public const int Hidden = 0x10;
            public const int Silent = 0x11;
            public const int AllNoDeath = 0x12;
            public const int AllNoDamage = 0x13;
            public const int AllNoHit = 0x14;
            public const int AllNoAttack = 0x15;
            public const int AllNoMove = 0x16;
            public const int AllDisableAi = 0x17;
            public const int PoiseBarsFlag = 0x69;
        }
        
        public static class CsDlcImp
        {
            public static IntPtr Base;

            public const int ByteFlags = 0x10;

            public enum Flags
            {
                DlcCheck = 0x1,
            }
        }

        public static class Hooks
        {
            public static long UpdateCoords;
            public static long InAirTimer;
            public static long NoClipKb;
            public static long NoClipTriggers;
            public static long CreateGoalObj;
            public static long HasSpEffect;
            public static long BlueTargetView;
            public static long LockedTargetPtr;
            public static long InfinitePoise;
            public static long ShouldUpdateAi;
            public static long GetForceActIdx;
            public static long TargetNoStagger;
            public static long AttackInfo;
            public static long WarpCoordWrite;
            public static long WarpAngleWrite;
            
        }

        public static class Functions
        {
            public static long GraceWarp;
            public static long SetEvent;
            public static long SetSpEffect;
            public static long GiveRunes;
            public static long LookupByFieldInsHandle;
            public static long WarpToBlock;
            public static long ExternalEventTempCtor;
            public static long ExecuteTalkCommand;
            public static long GetEvent;
            public static long GetPlayerItemQuantityById;
            public static long ItemSpawn;
            public static long MatrixVectorProduct;
            public static long ChrInsByHandle;
        }

        public static class Patches
        {
            public static IntPtr DungeonWarp;
            public static IntPtr NoRunesFromEnemies;
            public static IntPtr NoRuneArcLoss;
            public static IntPtr NoRuneLossOnDeath;
        }
    }
}

