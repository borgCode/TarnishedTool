using System;
using System.Diagnostics.CodeAnalysis;

namespace SilkyRing.Memory
{
    public static class Offsets
    {
        public static class WorldChrMan
        {
            public static IntPtr Base;

            public const int PlayerIns = 0x1E508;
        }

        public static class ChrIns
        {
            public const int ChrCtrl = 0x58;
            public const int SpEffectMan = 0x178;
            public const int Modules = 0x190;
            public const int ChrManipulator = 0x580;


            public static readonly int[] ChrCtrlFlags = [0xC8, 0x24];
            public static readonly BitFlag DisableAi = new(0x0, 1 << 0);

            public static readonly int[] ChrDataModule = [Modules, 0x0];
            public static readonly int[] ChrBehaviorModule = [Modules, 0x28];
            public static readonly int[] ChrSuperArmorModule = [Modules, 0x40];
            public static readonly int[] ChrPhysicsModule = [Modules, 0x68];

            public enum ChrDataOffsets
            {
                Health = 0x138,
                MaxHealth = 0x13C,
                Flags = 0x19B,
            }

            [Flags]
            public enum ChrDataBitFlags
            {
                NoDeath = 1 << 0,
                NoDamage = 1 << 1,
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
                Coords = 0x70
            }

            public static readonly int[] AiThink = [ChrManipulator, 0xC0];

            public enum AiThinkOffsets
            {
                TargetingSystem = 0xC480,
                ForceAct = 0xE9C1,
                LastAct = 0xE9C2,
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
        }

        public static class GameDataMan
        {
            public static IntPtr Base;

            public const int PlayerGameData = 0x8;
            public const int NewGame = 0x120;


            public const int RuneLevel = 0x68;
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

            public const int ForceSave = 0xb72;
        }
        
        public static class WorldChrManDbg
        {
            public static IntPtr Base;

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
            public const int AllDisableAi = 0x17;
            public const int PoiseBarsFlag = 0x69;
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
        }

        public static class Funcs
        {
            public static long GraceWarp;
            public static long SetEvent;
            public static long SetSpEffect;
            public static long GiveRunes;
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

