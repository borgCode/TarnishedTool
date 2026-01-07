using System;
using TarnishedTool.Enums;

namespace TarnishedTool.Memory
{
    public static class Offsets
    {
        private static GameVersion? _version;

        public static GameVersion Version => _version
                                             ?? throw new InvalidOperationException(
                                                 "Offsets not initialized. Call PatchManager.Initialize() first.");

        public static bool Initialize(string fileVersion, IntPtr moduleBase)
        {
            _version = fileVersion switch
            {
                var v when v.StartsWith("1.2.0.") => GameVersion.Version1_2_0,
                var v when v.StartsWith("2.0.1.") => GameVersion.Version2_0_1,
                var v when v.StartsWith("2.2.0.") => GameVersion.Version2_2_0,
                var v when v.StartsWith("2.2.3.") => GameVersion.Version2_2_3,
                var v when v.StartsWith("2.3.0.") => GameVersion.Version2_3_0,
                var v when v.StartsWith("2.4.0.") => GameVersion.Version2_4_0,
                var v when v.StartsWith("2.6.0.") => GameVersion.Version2_6_0,
                var v when v.StartsWith("2.6.1.") => GameVersion.Version2_6_1,
                _ => null
            };

            if (!_version.HasValue)
                return false;

            InitializeBaseAddresses(moduleBase);
            return true;
        }

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

            public static int DrawTiles1 => Version switch
            {
                GameVersion.Version2_0_1 => 0x60C,
                _ => 0x61C,
            };

            public static int DrawTiles2 => Version switch
            {
                GameVersion.Version2_0_1 => 0x60E,
                _ => 0x61E,
            };

            public const int WorldInfoOwner = 0x10;

            public static int ShouldDrawMiniMap => Version switch
            {
                GameVersion.Version2_0_1 => 0xB3918,
                _ => 0xB3368,
            };
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

            public static int TorrentHandle => Version switch
            {
                GameVersion.Version2_0_1 => 0x930,
                _ => 0x950,
            };

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

            public static int ForceSave => Version switch
            {
                GameVersion.Version2_0_1 => 0xB42,
                _ => 0xb72,
            };

            public static int ShouldStartNewGame => Version switch
            {
                GameVersion.Version2_0_1 => 0xB4D,
                _ => 0xB7D,
            };
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

            public static int GameSpeed => Version switch
            {
                GameVersion.Version2_0_1 => 0x2D4,
                _ => 0x2CC,
            };
        }

        public static class MapDebugFlags
        {
            public static IntPtr Base;
            public const int ShowAllMaps = 0x0;
            public const int ShowAllGraces = 0x1;

            public static int ShowMapTiles => Version switch
            {
                GameVersion.Version2_0_1 => 0x5,
                _ => 0x6,
            };
        }

        public static class WorldChrManDbg
        {
            public static IntPtr Base;

            // public const int PlayerNoDeath = 0x8;
            // public const int AllChrsSpheres = 0x9;
            // public const int OneShot = 0xA;
            // public const int InfiniteGoods = 0xB;
            // public const int InfiniteStam = 0xC;
            // public const int InfiniteFp = 0xD;
            // public const int InfiniteArrows = 0xE;
            // public const int Hidden = 0x10;
            // public const int Silent = 0x11;
            // public const int AllNoDeath = 0x12;
            // public const int AllNoDamage = 0x13;
            // public const int AllNoHit = 0x14;
            // public const int AllNoAttack = 0x15;
            // public const int AllNoMove = 0x16;
            // public const int AllDisableAi = 0x17;
            public const int PoiseBarsFlag = 0x69;
        }

        public static class ChrDbgFlags
        {
            public static IntPtr Base;
            public const int PlayerNoDeath = 0x0;
            public const int OneShot = 0x2;
            public const int InfiniteGoods = 0x3;
            public const int InfiniteStam = 0x4;
            public const int InfiniteFp = 0x5;
            public const int InfiniteArrows = 0x6;
            public const int Hidden = 0x8;
            public const int Silent = 0x9;
            public const int AllNoDeath = 0xA;
            public const int AllNoDamage = 0xB;
            public const int AllNoHit = 0xC;
            public const int AllNoAttack = 0xD;
            public const int AllNoMove = 0xE;
            public const int AllDisableAi = 0xF;
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

        private static void InitializeBaseAddresses(IntPtr moduleBase)
        {
            WorldChrMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50268,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x3D65F88,
                _ => 0
            };

            FieldArea.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C53470,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x3D691D8,
                _ => 0
            };

            LuaEventMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C520F8,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x3D67E48,
                _ => 0
            };

            VirtualMemFlag.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C526E8,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x3D68448,
                _ => 0
            };

            DamageManager.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50658,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0, // Fill from debug output
                _ => 0
            };

            MenuMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C55B30,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            TargetView.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C4C4EA,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            GameMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C53B88,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            WorldHitMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C54320,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            WorldChrManDbg.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50478,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            GameDataMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C481B8,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            CsDlcImp.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C705D8,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            MapItemManImpl.Base = moduleBase + Version switch
            {

                GameVersion.Version1_2_0 => 0x3C51CF0,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            FD4PadManager.Base = moduleBase + Version switch
            {
                
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            CSEmkSystem.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C51E78,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            WorldAreaTimeImpl.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C535A0,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            GroupMask.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3A367E0,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            SoloParamRepositoryImp.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x4473138,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            MsgRepository.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C66D48,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            CSFlipperImp.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x4473138,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            CSDbgEvent.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C522A0,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            ChrDbgFlags.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50480,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            UserInputManager.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x45255C8,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            CSTrophy.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x4472AD8,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            DrawPathing.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C4C030,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            MapDebugFlags.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C56BE0,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            // Functions
            Functions.GraceWarp = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5855B0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.SetEvent = moduleBase.ToInt64() + Version switch
            {

                GameVersion.Version1_2_0 => 0x5D9E40,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.SetSpEffect = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3E17E0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.GiveRunes = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x258270,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.LookupByFieldInsHandle = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4F7580,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.WarpToBlock = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5D7DA0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.ExternalEventTempCtor = moduleBase.ToInt64() + Version switch
            {
                
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.ExecuteTalkCommand = moduleBase.ToInt64() + Version switch
            {
                
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.GetEvent = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5D9650,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.GetPlayerItemQuantityById = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x75ACC0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.ItemSpawn = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x54E570,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.MatrixVectorProduct = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0xAB01E0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.ChrInsByHandle = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4F74A0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.FindAndRemoveSpEffect = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4E6970,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.EmevdSwitch = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x555D00,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.EmkEventInsCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x56ECA0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.GetMovement = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x25B6CD0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.GetChrInsByEntityId = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4F7630,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.NpcEzStateTalkCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0xE17E70,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Functions.EzStateEnvQueryImplCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x1FD0D40,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            // Hooks
            Hooks.UpdateCoords = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x45E3C9,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.InAirTimer = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x444CA8,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.NoClipKb = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x1EBF67F,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.NoClipTriggers = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x1EBE79E,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.HasSpEffect = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4E99C0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.BlueTargetView = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3382C3,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.LockedTargetPtr = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x6F0A16,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.InfinitePoise = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5E885CB0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.ShouldUpdateAi = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C09F0,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.GetForceActIdx = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x6402D8C4,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.TargetNoStagger = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5E8BE3C7,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.AttackInfo = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5E8BE22B,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.WarpCoordWrite = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x657AFA,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.WarpAngleWrite = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x657ADA,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.LionCooldownHook = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.SetActionRequested = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3FF362,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.NoMapAcquiredPopup = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.NoGrab = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4402FB,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            Hooks.LoadScreenMsgLookup = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5F15093C,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_6_0 => 0x0L,
                GameVersion.Version2_6_1 => 0x0L,
                _ => 0L
            };

            // Patches
            Patches.CanFastTravel = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x798300,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.NoRunesFromEnemies = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x49456E,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.NoRuneArcLoss = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x258347,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.NoRuneLossOnDeath = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x5EA3C215,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.OpenMap = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x7C4CDA,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.CloseMap = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x5EE0392E,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.EnableFreeCam = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0xDC0D60,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.CanDrawEvents1 = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0xDD0810,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.CanDrawEvents2 = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0xDD07F0,
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.GetShopEvent = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.NoLogo = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.DebugFont = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.PlayerSound = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.IsTorrentDisabledInUnderworld = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.IsWhistleDisabled = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.IsWorldPaused = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            Patches.GetItemChance = moduleBase + Version switch
            {
                GameVersion.Version2_0_1 => 0x0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_6_0 => 0x0,
                GameVersion.Version2_6_1 => 0x0,
                _ => 0
            };

            // Don't forget the derived one
            Hooks.HookedDeathFunction = Patches.NoRuneLossOnDeath - 7;
        }
    }
}
