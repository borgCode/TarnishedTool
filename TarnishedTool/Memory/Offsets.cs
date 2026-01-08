using System;
using TarnishedTool.Enums;

namespace TarnishedTool.Memory
{
    public static class Offsets
    {
        private static GameVersion? _version;

        public static GameVersion Version => _version
                                             ?? GameVersion.Version2_6_1;

        public static bool Initialize(string fileVersion, IntPtr moduleBase)
        {
            _version = fileVersion switch
            {
                var v when v.StartsWith("1.2.0.") => GameVersion.Version1_2_0,
                var v when v.StartsWith("1.9.1.") => GameVersion.Version1_9_1,
                var v when v.StartsWith("2.0.1.") => GameVersion.Version2_0_1,
                var v when v.StartsWith("2.2.0.") => GameVersion.Version2_2_0,
                var v when v.StartsWith("2.2.3.") => GameVersion.Version2_2_3,
                var v when v.StartsWith("2.3.0.") => GameVersion.Version2_3_0,
                var v when v.StartsWith("2.4.0.") => GameVersion.Version2_4_0,
                var v when v.StartsWith("2.5.0.") => GameVersion.Version2_5_0,
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
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x60C,
                _ => 0x61C,
            };

            public static int DrawTiles2 => Version switch
            {
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x60E,
                _ => 0x61E,
            };

            public const int WorldInfoOwner = 0x10;

            public static int ShouldDrawMiniMap => Version switch
            {
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0xB3918,
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
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x930,
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

            public static int StoredTime => Version switch
            {
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x18,
                _ => 0x20,
            };

            public static int ForceSave => Version switch
            {
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0xB42,
                _ => 0xb72,
            };

            public static int ShouldStartNewGame => Version switch
            {
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0xB4D,
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
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x2D4,
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
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x5,
                _ => 0x6,
            };
        }

        public static class WorldChrManDbg
        {
            public static IntPtr Base;
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
            public static IntPtr NoTimePassOnDeath;
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
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDCDD8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D65F88,
                _ => 0
            };

            FieldArea.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C53470,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDFFC0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D691D8,
                _ => 0
            };

            LuaEventMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C520F8,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDEC38,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D67E48,
                _ => 0
            };

            VirtualMemFlag.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C526E8,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDF238,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D68448,
                _ => 0
            };

            DamageManager.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50658,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDD1A8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D66378,
                _ => 0
            };

            MenuMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C55B30,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CE2580,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D6B7B0,
                _ => 0
            };

            TargetView.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C4C4EA,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CD90BA,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D6226B,
                _ => 0
            };

            GameMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C53B88,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CE0708,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D69918,
                _ => 0
            };

            WorldHitMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C54320,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CE0EB0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D6A0E0,
                _ => 0
            };

            WorldChrManDbg.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50478,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDD010,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D66198,
                _ => 0
            };

            GameDataMan.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C481B8,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CD4D88,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D5DF38,
                _ => 0
            };

            CsDlcImp.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C705D8,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CFD838,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D86BD8,
                _ => 0
            };

            MapItemManImpl.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C51CF0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDE840,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D67A50,
                _ => 0
            };

            FD4PadManager.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x45B4D50,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x485DC20,
                _ => 0
            };

            CSEmkSystem.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C51E78,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDE9C0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D67BD0,
                _ => 0
            };

            WorldAreaTimeImpl.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C535A0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CE00F0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D692F8,
                _ => 0
            };

            GroupMask.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3A367E0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3AC2AE8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3B33D00,
                _ => 0
            };

            SoloParamRepositoryImp.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x4473138,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CF8BC8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D81EE8,
                _ => 0
            };

            MsgRepository.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C66D48,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CF4218,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D7D4F8,
                _ => 0
            };

            CSFlipperImp.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x4473138,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x4500708,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x4589AD8,
                _ => 0
            };

            CSDbgEvent.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C522A0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDEDE8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D67FF8,
                _ => 0
            };

            ChrDbgFlags.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C50480,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CDCFE8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D661A0,
                _ => 0
            };

            UserInputManager.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x45255C8,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x45B4D48,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x485DC18,
                _ => 0
            };

            CSTrophy.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x4472AD8,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x45000A8,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x4589478,
                _ => 0
            };

            DrawPathing.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C4C030,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CD8C00,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D61DB0,
                _ => 0
            };

            MapDebugFlags.Base = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C56BE0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3CE3D80,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3D6CFC0,
                _ => 0
            };

            // Functions
            Functions.GraceWarp = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5855B0,
                GameVersion.Version1_9_1 => 0x5955C0,
                GameVersion.Version2_0_1 => 0x595800,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x599D00,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x599CD0,
                _ => 0L
            };

            Functions.SetEvent = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5D9E40,
                GameVersion.Version1_9_1 => 0x5EE1D0,
                GameVersion.Version2_0_1 => 0x5EE410,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x5F9B50,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x5F9CD0,
                _ => 0L
            };

            Functions.SetSpEffect = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3E17E0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3E6AF0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x3E9120,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3E90F0,
                _ => 0L
            };

            Functions.GiveRunes = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x258270,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x25C7A0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x25E1B0,
                _ => 0L
            };

            Functions.LookupByFieldInsHandle = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4F7580,
                GameVersion.Version1_9_1 => 0x503F00,
                GameVersion.Version2_0_1 => 0x504140,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x507D80,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x507D50,
                _ => 0L
            };

            Functions.WarpToBlock = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5D7DA0,
                GameVersion.Version1_9_1 => 0x5EC0B0,
                GameVersion.Version2_0_1 => 0x5EC2F0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x5F7A30,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x5F7BB0,
                _ => 0L
            };

            
            Functions.GetEvent = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5D9650,
                GameVersion.Version1_9_1 => 0x5ED9E0,
                GameVersion.Version2_0_1 => 0x5EDC20,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x5F9360,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x5F94E0,
                _ => 0L
            };

            Functions.GetPlayerItemQuantityById = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x75ACC0,
                GameVersion.Version1_9_1 => 0x774600,
                GameVersion.Version2_0_1 => 0x774890,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x784F40,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x7850C0,
                _ => 0L
            };

            Functions.ItemSpawn = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x54E570,
                GameVersion.Version1_9_1 => 0x55C520,
                GameVersion.Version2_0_1 => 0x55C760,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x5606A0,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x560670,
                _ => 0L
            };

            Functions.MatrixVectorProduct = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0xAB01E0,
                GameVersion.Version1_9_1 => 0xAE32D0,
                GameVersion.Version2_0_1 => 0xAE3560,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0xB12130,
                GameVersion.Version2_6_0 => 0xB122B0,
                GameVersion.Version2_6_1 => 0xB12310,
                _ => 0L
            };

            Functions.ChrInsByHandle = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4F74A0,
                GameVersion.Version1_9_1 => 0x503E20,
                GameVersion.Version2_0_1 => 0x504060,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x507CA0,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x507C70,
                _ => 0L
            };

            Functions.FindAndRemoveSpEffect = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4E6970,
                GameVersion.Version1_9_1 => 0x4F3070,
                GameVersion.Version2_0_1 => 0x4F32B0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x4F69B0,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x4F6980,
                _ => 0L
            };

            Functions.EmevdSwitch = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x555D00,
                GameVersion.Version1_9_1 => 0x563CB0,
                GameVersion.Version2_0_1 => 0x563EF0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x567E30,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x567E00,
                _ => 0L
            };

            Functions.EmkEventInsCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x56ECA0,
                GameVersion.Version1_9_1 => 0x57E140,
                GameVersion.Version2_0_1 => 0x57E380,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x582730,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x582700,
                _ => 0L
            };

            Functions.GetMovement = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x25B6CD0,
                GameVersion.Version1_9_1 => 0x261EAE0,
                GameVersion.Version2_0_1 => 0x261EE70,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x2664240,
                GameVersion.Version2_6_0 => 0x2664210,
                GameVersion.Version2_6_1 => 0x2664270,
                _ => 0L
            };

            Functions.GetChrInsByEntityId = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4F7630,
                GameVersion.Version1_9_1 => 0x503FB0,
                GameVersion.Version2_0_1 => 0x5041F0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x507E30,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x507E00,
                _ => 0L
            };

            Functions.NpcEzStateTalkCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0xE17E70,
                GameVersion.Version1_9_1 => 0xE5EED0,
                GameVersion.Version2_0_1 => 0xE5F260,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0xE9ECA0,
                GameVersion.Version2_6_0 => 0xE9EC70,
                GameVersion.Version2_6_1 => 0xE9ECD0,
                _ => 0L
            };

            Functions.EzStateEnvQueryImplCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x1FD0D40,
                GameVersion.Version1_9_1 => 0x2038340,
                GameVersion.Version2_0_1 => 0x20386D0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x207E850,
                GameVersion.Version2_6_0 => 0x207E820,
                GameVersion.Version2_6_1 => 0x207E880,
                _ => 0L
            };

            Functions.ExternalEventTempCtor = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0x1FFAAE0,
                GameVersion.Version2_0_1 => 0x1FFAE70,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x2040FF0,
                GameVersion.Version2_6_0 => 0x2040FC0,
                GameVersion.Version2_6_1 => 0x2041020,
                _ => 0L
            };

            Functions.ExecuteTalkCommand = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0xE65470,
                GameVersion.Version2_0_1 => 0xE65800,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xEA5280,
                GameVersion.Version2_6_0 => 0xEA5250,
                GameVersion.Version2_6_1 => 0xEA52B0,
                _ => 0L
            };
            
            // Hooks
            Hooks.UpdateCoords = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x45E3C9,
                GameVersion.Version1_9_1 => 0x4648E9,
                GameVersion.Version2_0_1 => 0x464A89,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x4679C9,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x467999,
                _ => 0L
            };

            Hooks.InAirTimer = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x444CA8,
                GameVersion.Version1_9_1 => 0x44B1C8,
                GameVersion.Version2_0_1 => 0x44B368,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x44E2A8,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x44E278,
                _ => 0L
            };

            Hooks.NoClipKb = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x1EBF67F,
                GameVersion.Version1_9_1 => 0x1F26BAF,
                GameVersion.Version2_0_1 => 0x1F26F3F,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x5D1954,
                GameVersion.Version2_6_0 => 0x1F6D09F,
                GameVersion.Version2_6_1 => 0x1F6D0FF,
                _ => 0L
            };

            Hooks.NoClipTriggers = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x1EBE79E,
                GameVersion.Version1_9_1 => 0x1F25C6F,
                GameVersion.Version2_0_1 => 0x1F25FFF,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x1F6C14F,
                GameVersion.Version2_6_0 => 0x1F6C11F,
                GameVersion.Version2_6_1 => 0x1F6C17F,
                _ => 0L
            };

            Hooks.HasSpEffect = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4E99C0,
                GameVersion.Version1_9_1 => 0x4F60A0,
                GameVersion.Version2_0_1 => 0x4F62E0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x4F9A40,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x4F9A10,
                _ => 0L
            };

            Hooks.BlueTargetView = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3382C3,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x33CA43,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x33E293,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x33E263,
                _ => 0L
            };

            Hooks.LockedTargetPtr = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x6F0A16,
                GameVersion.Version1_9_1 => 0x7089C6,
                GameVersion.Version2_0_1 => 0x708C56,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x7171F2,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x717372,
                _ => 0L
            };

            Hooks.InfinitePoise = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5E885CB0,
                GameVersion.Version1_9_1 => 0x442CF0,
                GameVersion.Version2_0_1 => 0x442DC0,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x445CE0,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x445CB0,
                _ => 0L
            };

            Hooks.ShouldUpdateAi = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3C09F0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x3C5A30,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x3C7940,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x3C7910,
                _ => 0L
            };

            Hooks.GetForceActIdx = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x6402D8C4,
                GameVersion.Version1_9_1 => 0x17B5923,
                GameVersion.Version2_0_1 => 0x55611FB,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x123846F,
                GameVersion.Version2_6_0 => 0x53C8D9,
                GameVersion.Version2_6_1 => 0x5BED8C4,
                _ => 0L
            };

            Hooks.TargetNoStagger = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5E8BE3C7,
                GameVersion.Version1_9_1 => 0x47B225,
                GameVersion.Version2_0_1 => 0x47B3C5,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x47E3F7,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x47E3C7,
                _ => 0L
            };

            Hooks.AttackInfo = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5E8BE22B,
                GameVersion.Version1_9_1 => 0x47B09A,
                GameVersion.Version2_0_1 => 0x47B23A,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x47E25B,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x47E22B,
                _ => 0L
            };

            Hooks.WarpCoordWrite = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x657AFA,
                GameVersion.Version1_9_1 => 0x66D26A,
                GameVersion.Version2_0_1 => 0x66D4DA,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x67A93A,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x67AABA,
                _ => 0L
            };

            Hooks.WarpAngleWrite = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x657ADA,
                GameVersion.Version1_9_1 => 0x66D24A,
                GameVersion.Version2_0_1 => 0x66D4BA,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x67A91A,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x67AA9A,
                _ => 0L
            };

            Hooks.LionCooldownHook = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x0L,
                GameVersion.Version2_0_1 => 0x0L,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x4FF13A,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x4FF10A,
                _ => 0L
            };

            Hooks.SetActionRequested = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x3FF362,
                GameVersion.Version1_9_1 => 0x4050D2,
                GameVersion.Version2_0_1 => 0x4050C2,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x407BE2,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x407BB2,
                _ => 0L
            };

            Hooks.NoMapAcquiredPopup = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x0L,
                GameVersion.Version1_9_1 => 0x9A5851,
                GameVersion.Version2_0_1 => 0x9A5AE1,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x9C6251,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x9C63D1,
                _ => 0L
            };

            Hooks.NoGrab = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x4402FB,
                GameVersion.Version1_9_1 => 0x4468DB,
                GameVersion.Version2_0_1 => 0x446A7B,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0x4499BB,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x44998B,
                _ => 0L
            };

            Hooks.LoadScreenMsgLookup = moduleBase.ToInt64() + Version switch
            {
                GameVersion.Version1_2_0 => 0x5F15093C,
                GameVersion.Version1_9_1 => 0xCDD97C,
                GameVersion.Version2_0_1 => 0x446A7B,
                GameVersion.Version2_2_0 => 0x0L,
                GameVersion.Version2_2_3 => 0x0L,
                GameVersion.Version2_3_0 => 0x0L,
                GameVersion.Version2_4_0 => 0x0L,
                GameVersion.Version2_5_0 => 0xD1075C,
                GameVersion.Version2_6_0 => 0xD108DC,
                GameVersion.Version2_6_1 => 0xD1093C,
                _ => 0L
            };
            
            // Patches
            
            Patches.NoLogo = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0xADDCED,
                GameVersion.Version2_0_1 => 0xADDF7D,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xB0C26D,
                GameVersion.Version2_6_0 => 0xB0C3ED,
                GameVersion.Version2_6_1 => 0xB0C44D,
                _ => 0
            };

            
            Patches.NoRunesFromEnemies = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x49456E,
                GameVersion.Version1_9_1 => 0x64541F,
                GameVersion.Version2_0_1 => 0x64568F,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x65196F,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x651AEF,
                _ => 0
            };

            Patches.NoRuneArcLoss = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x258347,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x25C887,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 or GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x25E297,
                _ => 0
            };

            Patches.NoRuneLossOnDeath = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x5EA3C215,
                GameVersion.Version1_9_1 => 0x5F0715,
                GameVersion.Version2_0_1 => 0x5F0955,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x5FC095,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x5FC215,
                _ => 0
            };
            
            Patches.CanFastTravel = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x798300,
                GameVersion.Version1_9_1 => 0x7B3210,
                GameVersion.Version2_0_1 => 0x7B34A0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x7C4CE0,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x7C4E60,
                _ => 0
            };


            Patches.OpenMap = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x7C4CDA,
                GameVersion.Version1_9_1 => 0x7DD01A,
                GameVersion.Version2_0_1 => 0x7DD2AA,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x7EEBCA,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x7EED4A,
                _ => 0
            };

            Patches.CloseMap = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x5EE0392E,
                GameVersion.Version1_9_1 => 0x9A35AA,
                GameVersion.Version2_0_1 => 0x9A383A,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x9C37AE,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x9C392E,
                _ => 0
            };

            Patches.EnableFreeCam = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0xDC0D60,
                GameVersion.Version1_9_1 => 0xE056E0,
                GameVersion.Version2_0_1 => 0xE05A70,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xE43330,
                GameVersion.Version2_6_0 => 0xE43300,
                GameVersion.Version2_6_1 => 0xE43360,
                _ => 0
            };

            Patches.CanDrawEvents1 = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0xDD0810,
                GameVersion.Version1_9_1 => 0xE15600,
                GameVersion.Version2_0_1 => 0xE15990,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xE53600,
                GameVersion.Version2_6_0 => 0xE535D0,
                GameVersion.Version2_6_1 => 0xE53630,
                _ => 0
            };

            Patches.CanDrawEvents2 = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0xDD07F0,
                GameVersion.Version1_9_1 => 0xE155E0,
                GameVersion.Version2_0_1 => 0xE15970,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xE535E0,
                GameVersion.Version2_6_0 => 0xE535B0,
                GameVersion.Version2_6_1 => 0xE53610,
                _ => 0
            };

            Patches.DebugFont = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0x2647C00,
                GameVersion.Version2_0_1 => 0x2647F90,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x268D360,
                GameVersion.Version2_6_0 => 0x268D330,
                GameVersion.Version2_6_1 => 0x268D390,
                _ => 0
            };


            Patches.PlayerSound = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 or GameVersion.Version2_0_1 => 0x33CD76,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x33E5C6,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x33E596,
                _ => 0
            };

            Patches.IsTorrentDisabledInUnderworld = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0xCBCF7A,
                GameVersion.Version2_0_1 => 0xCBD20A,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xCEF9AA,
                GameVersion.Version2_6_0 => 0xCEFB2A,
                GameVersion.Version2_6_1 => 0xCEFB8A,
                _ => 0
            };

            Patches.IsWhistleDisabled = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0x6F7BBF,
                GameVersion.Version2_0_1 => 0x6F7E4F,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x7060EF,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x70626F,
                _ => 0
            };

            Patches.IsWorldPaused = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0xACA4D5,
                GameVersion.Version2_0_1 => 0xACA765,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xAF8335,
                GameVersion.Version2_6_0 => 0xAF84B5,
                GameVersion.Version2_6_1 => 0xAF8515,
                _ => 0
            };

            Patches.GetItemChance = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0xD0776E,
                GameVersion.Version2_0_1 => 0xD079DE,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xD3BD6E,
                GameVersion.Version2_6_0 => 0xD3BEEE,
                GameVersion.Version2_6_1 => 0xD3BF4E,
                _ => 0
            };

            Patches.GetShopEvent = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0xD15540,
                GameVersion.Version2_0_1 => 0xD157B0,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0xD49B10,
                GameVersion.Version2_6_0 => 0xD49C90,
                GameVersion.Version2_6_1 => 0xD49CF0,
                _ => 0
            };


            Hooks.NoTimePassOnDeath = moduleBase + Version switch
            {
                GameVersion.Version1_2_0 => 0x0,
                GameVersion.Version1_9_1 => 0x5F08F9,
                GameVersion.Version2_0_1 => 0x5F0B39,
                GameVersion.Version2_2_0 => 0x0,
                GameVersion.Version2_2_3 => 0x0,
                GameVersion.Version2_3_0 => 0x0,
                GameVersion.Version2_4_0 => 0x0,
                GameVersion.Version2_5_0 => 0x5FC27F,
                GameVersion.Version2_6_0 or GameVersion.Version2_6_1 => 0x5FC3FF,
                _ => 0
            };


            Console.WriteLine($@"WorldChrMan.Base: 0x{WorldChrMan.Base.ToInt64():X}");
            Console.WriteLine($@"FieldArea.Base: 0x{FieldArea.Base.ToInt64():X}");
            Console.WriteLine($@"LuaEventMan.Base: 0x{LuaEventMan.Base.ToInt64():X}");
            Console.WriteLine($@"VirtualMemFlag.Base: 0x{VirtualMemFlag.Base.ToInt64():X}");
            Console.WriteLine($@"DamageManager.Base: 0x{DamageManager.Base.ToInt64():X}");
            Console.WriteLine($@"MenuMan.Base: 0x{MenuMan.Base.ToInt64():X}");
            Console.WriteLine($@"TargetView.Base: 0x{TargetView.Base.ToInt64():X}");
            Console.WriteLine($@"GameMan.Base: 0x{GameMan.Base.ToInt64():X}");
            Console.WriteLine($@"WorldHitMan.Base: 0x{WorldHitMan.Base.ToInt64():X}");
            Console.WriteLine($@"WorldChrManDbg.Base: 0x{WorldChrManDbg.Base.ToInt64():X}");
            Console.WriteLine($@"GameDataMan.Base: 0x{GameDataMan.Base.ToInt64():X}");
            Console.WriteLine($@"CsDlcImp.Base: 0x{CsDlcImp.Base.ToInt64():X}");
            Console.WriteLine($@"MapItemManImpl.Base: 0x{MapItemManImpl.Base.ToInt64():X}");
            Console.WriteLine($@"FD4PadManager.Base: 0x{FD4PadManager.Base.ToInt64():X}");
            Console.WriteLine($@"CSEmkSystem.Base: 0x{CSEmkSystem.Base.ToInt64():X}");
            Console.WriteLine($@"WorldAreaTimeImpl.Base: 0x{WorldAreaTimeImpl.Base.ToInt64():X}");
            Console.WriteLine($@"GroupMask.Base: 0x{GroupMask.Base.ToInt64():X}");
            Console.WriteLine($@"CSFlipperImp.Base: 0x{CSFlipperImp.Base.ToInt64():X}");
            Console.WriteLine($@"CSDbgEvent.Base: 0x{CSDbgEvent.Base.ToInt64():X}");
            Console.WriteLine($@"UserInputManager.Base: 0x{UserInputManager.Base.ToInt64():X}");
            Console.WriteLine($@"CSTrophy.Base: 0x{CSTrophy.Base.ToInt64():X}");
            Console.WriteLine($@"MapDebugFlags.Base: 0x{MapDebugFlags.Base.ToInt64():X}");
            Console.WriteLine($@"SoloParamRepositoryImp.Base: 0x{SoloParamRepositoryImp.Base.ToInt64():X}");
            Console.WriteLine($@"MsgRepository.Base: 0x{MsgRepository.Base.ToInt64():X}");
            Console.WriteLine($@"DrawPathing.Base: 0x{DrawPathing.Base.ToInt64():X}");
            Console.WriteLine($@"ChrDbgFlags.Base: 0x{ChrDbgFlags.Base.ToInt64():X}");

            Console.WriteLine($@"Patches.NoLogo: 0x{Patches.NoLogo.ToInt64():X}");
            Console.WriteLine($@"Patches.NoRunesFromEnemies: 0x{Patches.NoRunesFromEnemies.ToInt64():X}");
            Console.WriteLine($@"Patches.NoRuneArcLoss: 0x{Patches.NoRuneArcLoss.ToInt64():X}");
            Console.WriteLine($@"Patches.NoRuneLossOnDeath: 0x{Patches.NoRuneLossOnDeath.ToInt64():X}");
            Console.WriteLine($@"Patches.CanFastTravel: 0x{Patches.CanFastTravel.ToInt64():X}");
            Console.WriteLine($@"Patches.OpenMap: 0x{Patches.OpenMap.ToInt64():X}");
            Console.WriteLine($@"Patches.CloseMap: 0x{Patches.CloseMap.ToInt64():X}");
            Console.WriteLine($@"Patches.EnableFreeCam: 0x{Patches.EnableFreeCam.ToInt64():X}");
            Console.WriteLine($@"Patches.CanDrawEvents1: 0x{Patches.CanDrawEvents1.ToInt64():X}");
            Console.WriteLine($@"Patches.CanDrawEvents2: 0x{Patches.CanDrawEvents2.ToInt64():X}");
            Console.WriteLine($@"Patches.DebugFont: 0x{Patches.DebugFont.ToInt64():X}");
            Console.WriteLine($@"Patches.PlayerSound: 0x{Patches.PlayerSound.ToInt64():X}");
            Console.WriteLine(
                $@"Patches.IsTorrentDisabledInUnderworld: 0x{Patches.IsTorrentDisabledInUnderworld.ToInt64():X}");
            Console.WriteLine($@"Patches.IsWhistleDisabled: 0x{Patches.IsWhistleDisabled.ToInt64():X}");
            Console.WriteLine($@"Patches.IsWorldPaused: 0x{Patches.IsWorldPaused.ToInt64():X}");
            Console.WriteLine($@"Patches.GetItemChance: 0x{Patches.GetItemChance.ToInt64():X}");
            Console.WriteLine($@"Patches.GetShopEvent: 0x{Patches.GetShopEvent.ToInt64():X}");

            Console.WriteLine($@"Hooks.UpdateCoords: 0x{Hooks.UpdateCoords:X}");
            Console.WriteLine($@"Hooks.InAirTimer: 0x{Hooks.InAirTimer:X}");
            Console.WriteLine($@"Hooks.NoClipKb: 0x{Hooks.NoClipKb:X}");
            Console.WriteLine($@"Hooks.NoClipTriggers: 0x{Hooks.NoClipTriggers:X}");
            Console.WriteLine($@"Hooks.HasSpEffect: 0x{Hooks.HasSpEffect:X}");
            Console.WriteLine($@"Hooks.BlueTargetView: 0x{Hooks.BlueTargetView:X}");
            Console.WriteLine($@"Hooks.LockedTargetPtr: 0x{Hooks.LockedTargetPtr:X}");
            Console.WriteLine($@"Hooks.InfinitePoise: 0x{Hooks.InfinitePoise:X}");
            Console.WriteLine($@"Hooks.ShouldUpdateAi: 0x{Hooks.ShouldUpdateAi:X}");
            Console.WriteLine($@"Hooks.GetForceActIdx: 0x{Hooks.GetForceActIdx:X}");
            Console.WriteLine($@"Hooks.AttackInfo: 0x{Hooks.AttackInfo:X}");
            Console.WriteLine($@"Hooks.WarpCoordWrite: 0x{Hooks.WarpCoordWrite:X}");
            Console.WriteLine($@"Hooks.WarpAngleWrite: 0x{Hooks.WarpAngleWrite:X}");
            Console.WriteLine($@"Hooks.NoTimePassOnDeath: 0x{Hooks.NoTimePassOnDeath.ToInt64():X}");
            Console.WriteLine($@"Hooks.LionCooldownHook: 0x{Hooks.LionCooldownHook:X}");
            Console.WriteLine($@"Hooks.SetActionRequested: 0x{Hooks.SetActionRequested:X}");
            Console.WriteLine($@"Hooks.NoGrab: 0x{Hooks.NoGrab:X}");
            Console.WriteLine($@"Hooks.LoadScreenMsgLookup: 0x{Hooks.LoadScreenMsgLookup:X}");
            Console.WriteLine($@"Hooks.TargetNoStagger: 0x{Hooks.TargetNoStagger:X}");
            Console.WriteLine($@"Hooks.NoMapAcquiredPopup: 0x{Hooks.NoMapAcquiredPopup:X}");

            Console.WriteLine($@"Funcs.GraceWarp: 0x{Functions.GraceWarp:X}");
            Console.WriteLine($@"Funcs.SetEvent: 0x{Functions.SetEvent:X}");
            Console.WriteLine($@"Funcs.SetSpEffect: 0x{Functions.SetSpEffect:X}");
            Console.WriteLine($@"Funcs.GiveRunes: 0x{Functions.GiveRunes:X}");
            Console.WriteLine($@"Funcs.LookupByFieldInsHandle: 0x{Functions.LookupByFieldInsHandle:X}");
            Console.WriteLine($@"Funcs.WarpToBlock: 0x{Functions.WarpToBlock:X}");
            Console.WriteLine($@"Funcs.GetEvent: 0x{Functions.GetEvent:X}");
            Console.WriteLine($@"Funcs.GetPlayerItemQuantityById: 0x{Functions.GetPlayerItemQuantityById:X}");
            Console.WriteLine($@"Funcs.ItemSpawn: 0x{Functions.ItemSpawn:X}");
            Console.WriteLine($@"Funcs.MatrixVectorProduct: 0x{Functions.MatrixVectorProduct:X}");
            Console.WriteLine($@"Funcs.ChrInsByHandle: 0x{Functions.ChrInsByHandle:X}");
            Console.WriteLine($@"Funcs.FindAndRemoveSpEffect: 0x{Functions.FindAndRemoveSpEffect:X}");
            Console.WriteLine($@"Funcs.EmevdSwitch: 0x{Functions.EmevdSwitch:X}");
            Console.WriteLine($@"Funcs.EmkEventInsCtor: 0x{Functions.EmkEventInsCtor:X}");
            Console.WriteLine($@"Funcs.GetMovement: 0x{Functions.GetMovement:X}");
            Console.WriteLine($@"Funcs.GetChrInsByEntityId: 0x{Functions.GetChrInsByEntityId:X}");
            Console.WriteLine($@"Funcs.NpcEzStateTalkCtor: 0x{Functions.NpcEzStateTalkCtor:X}");
            Console.WriteLine($@"Funcs.EzStateEnvQueryImplCtor: 0x{Functions.EzStateEnvQueryImplCtor:X}");
            Console.WriteLine($@"Funcs.ExternalEventTempCtor: 0x{Functions.ExternalEventTempCtor:X}");
            Console.WriteLine($@"Funcs.ExecuteTalkCommand: 0x{Functions.ExecuteTalkCommand:X}");
        }
    }
}