using System;

namespace TarnishedTool.Memory
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
                CurrentMapCoords = 0x6C0,
                CurrentMapAngle = 0x6CC,
            }
        }

        public static class ChrIns
        {
            public const int ChrCtrl = 0x58;
            public const int SpecialEffect = 0x178;
            public const int EntityId = 0x1E8;

            public enum SpecialEffectOffsets
            {
                Head = 0x8,
            }

            public enum SpEffectEntry
            {
                ParamData = 0x0,
                Id = 0x8,
                Next = 0x30,
                TimeLeft = 0x40,
                Duration = 0x48,
            }

            public enum SpEffectParamData
            {
                StateInfo = 0x156,
            }

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
                AnimationId = 0xD0,
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
                IsHorseWhistleDisabled = 0x164,
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

            public enum TargetingSystemOffsets
            {
                DebugDrawFlags = 0xC8
            }
            
            public static readonly BitFlag BlueTargetView = new(0x1, 1 << 3);
            public static readonly BitFlag YellowTargetView = new(0xC8, 1 << 5);
            public static readonly BitFlag WhiteLineToPlayer = new(0xC8, 1 << 6);

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
            
        }

        public static class FieldArea
        {
            public static IntPtr Base;

            public const int GameRend = 0x20;
            public const int CamMode = 0xC8; // 1 for free cam
            public const int CSDebugCam = 0xD0;
            public const int CamCoords = 0x40;

            
            public const int DrawTiles1 = 0x61C;
            public const int DrawTiles2 = 0x61E;

            public const int WorldInfoOwner = 0x10;
            public const int ShouldDrawMiniMap = 0xB3368;
        }

        public static class FD4PadManager
        {
            public static IntPtr Base;
        }
        
        public static class DrawPathing
        {
            public static IntPtr Base;
        }


        public static class LuaEventMan
        {
            public static IntPtr Base;
        }

        public static class CSDbgEvent
        {
            public static IntPtr Base;

            public const int DrawEvent = 0x4;
            public const int DisableEvent = 0x28;
        }

        public static class VirtualMemFlag
        {
            public static IntPtr Base;
        }

        public static class CSEmkSystem
        {
            public static IntPtr Base;
        }

        public static class GroupMask
        {
            public static IntPtr Base;


            public enum GroupMasks
            {
                // MasterFlag = 0x0,
                ShouldShowGeom = 0x1,
                Unk02 = 0x2,
                Unk03 = 0x3,
                Unk04 = 0x4,
                Unk05 = 0x5,
                Unk06 = 0x6,
                Unk07 = 0x7,
                Unk08 = 0x8,
                ShouldShowMap = 0x9,
                HideSomeAssets = 0xA,
                ShouldShowMap2 = 0xB,
                Unk0C = 0xC,
                ShouldShowChrs = 0xD,
                Unk0E = 0xE,
                Unk0F = 0xF,
                Unk10 = 0x10,
                ShouldShowGrass = 0x11,
            }
       
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
            public const int Ragdoll = 0xF;
            public const int Mode = 0x14;
        }

        public static class MenuMan
        {
            public static IntPtr Base;

            public const int PopupMenu = 0x80;
            public const int FlagArray = 0x90;
            public const int IsLoaded = 0x94;
            public const int IsFading = 0x96;
            public const int IsPaused = 0xD1;

            public enum PopupMenuOffsets
            {
                DialogResult = 0x1A0,
            }

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
                RuneMemory = 0x70,
                Scadutree = 0xFC,
                SpiritAsh = 0xFD,
                TorrentHandle = 0x950,
            }

            public const int Options = 0x58;

            public enum OptionsOffsets
            {
                Music = 0x4
            }

            public const int Igt = 0xA0; //Uint
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
            public const int ShouldStartNewGame = 0xB7D;
        }

        public static class MapItemManImpl
        {
            public static IntPtr Base;
        }

        public static class WorldAreaTimeImpl
        {
            public static IntPtr Base;
        }
        
        public static class SoloParamRepositoryImp
        {
            public static IntPtr Base;
        }
        
        public static class MsgRepository
        {
            public static IntPtr Base;
        }

        public static class UserInputManager
        {
            public static IntPtr Base;

            public const int SteamInputEnum = 0x88B;
        }
        
        public static class CSTrophy
        {
            public static IntPtr Base;

            public const int CSTrophyPlatformImp_forSteam = 0x8;
            public const int IsAwardAchievementEnabled = 0x4C;
        }

        public static class CSFlipperImp
        {
            public static IntPtr Base;
            public const int GameSpeed = 0x2CC;
        }
        
        public static class MapDebugFlags
        {
            public static IntPtr Base;
            public const int ShowAllMaps = 0x0;
            public const int ShowAllGraces = 0x1;
            public const int ShowMapTiles = 0x6;
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
            public static IntPtr HookedDeathFunction;
            public static long LionCooldownHook;
            public static long SetActionRequested;
            public static long NoMapAcquiredPopup;
            public static long NoGrab;
            public static long LoadScreenMsgLookup;
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
            public static long FindAndRemoveSpEffect;
            public static long EmevdSwitch;
            public static long EmkEventInsCtor;
            public static long GetMovement;
            public static long GetChrInsByEntityId;
            public static long NpcEzStateTalkCtor;
            public static long EzStateEnvQueryImplCtor;
        }

        public static class Patches
        {
            public static IntPtr CanFastTravel;
            public static IntPtr NoRunesFromEnemies;
            public static IntPtr NoRuneArcLoss;
            public static IntPtr NoRuneLossOnDeath;
            public static IntPtr OpenMap;
            public static IntPtr CloseMap;
            public static IntPtr EnableFreeCam;
            public static IntPtr CanDrawEvents1;
            public static IntPtr CanDrawEvents2;
            public static IntPtr GetShopEvent;
            public static IntPtr NoLogo;
            public static IntPtr DebugFont;
            public static IntPtr PlayerSound;
            public static IntPtr IsTorrentDisabledInUnderworld;
            public static IntPtr IsWhistleDisabled;
            public static IntPtr IsWorldPaused;
            public static IntPtr GetItemChance;
        }
    }
}