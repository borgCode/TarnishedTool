using System;
using TarnishedTool.Enums;
using TarnishedTool.Utilities;
using static TarnishedTool.Enums.GameVersion;

namespace TarnishedTool.Memory
{
    public static class Offsets
    {
        private static GameVersion? _version;

        public static GameVersion Version => _version
                                             ?? Version2_6_1;

        public static bool Initialize(string fileVersion, IntPtr moduleBase)
        {
            _version = fileVersion switch
            {
                var v when v.StartsWith("1.2.0.") => Version1_2_0,
                var v when v.StartsWith("1.2.1.") => Version1_2_1,
                var v when v.StartsWith("1.2.2.") => Version1_2_2,
                var v when v.StartsWith("1.2.3.") => Version1_2_3,
                var v when v.StartsWith("1.3.0.") => Version1_3_0,
                var v when v.StartsWith("1.3.1.") => Version1_3_1,
                var v when v.StartsWith("1.3.2.") => Version1_3_2,
                var v when v.StartsWith("1.4.0.") => Version1_4_0,
                var v when v.StartsWith("1.4.1.") => Version1_4_1,
                var v when v.StartsWith("1.5.0.") => Version1_5_0,
                var v when v.StartsWith("1.6.0.") => Version1_6_0,
                var v when v.StartsWith("1.7.0.") => Version1_7_0,
                var v when v.StartsWith("1.8.0.") => Version1_8_0,
                var v when v.StartsWith("1.8.1.") => Version1_8_1,
                var v when v.StartsWith("1.9.0.") => Version1_9_0,
                var v when v.StartsWith("1.9.1.") => Version1_9_1,
                var v when v.StartsWith("2.0.0.") => Version2_0_0,
                var v when v.StartsWith("2.0.1.") => Version2_0_1,
                var v when v.StartsWith("2.2.0.") => Version2_2_0,
                var v when v.StartsWith("2.2.3.") => Version2_2_3,
                var v when v.StartsWith("2.3.0.") => Version2_3_0,
                var v when v.StartsWith("2.4.0.") => Version2_4_0,
                var v when v.StartsWith("2.5.0.") => Version2_5_0,
                var v when v.StartsWith("2.6.0.") => Version2_6_0,
                var v when v.StartsWith("2.6.1.") => Version2_6_1,
                _ => null
            };

            if (!_version.HasValue)
            {
                MsgBox.Show(
                    $@"Unknown patch version: {_version}, please report it on GitHub. Scanning for addresses instead.",
                    "Unknown patch version");
                return false;
            }


            InitializeBaseAddresses(moduleBase);
            return true;
        }

        public static class WorldChrMan
        {
            public static IntPtr Base;

            public static int ChrSetPool => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0
                    or Version1_4_1 or Version1_5_0 or Version1_6_0 => 0x18038,
                _ => 0x1DED8,
            };

            public enum ChrSetOffsets
            {
                ChrSetEntries = 0x18
            }

            public static int PlayerIns => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 => 0x18468,
                _ => 0x1E508,
            };

            public static int ChrInsByUpdatePrioBegin => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 => 0x18F10,
                Version1_7_0 or Version2_0_0 => 0x1F1B0,
                _ => 0x1F1B8
            };

            public static int ChrInsByUpdatePrioEnd => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 => 0x18F18,
                Version1_7_0 or Version2_0_0 => 0x1F1B8,
                _ => 0x1F1C0
            };

            public static class PlayerInsOffsets
            {
                public const int Handle = 0x8;

                public static int CurrentBlockId => Version switch
                {
                    Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                        or Version1_3_2 => 0x6C8,
                    Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0x6C0,
                    _ => 0x6D0,
                };

                public static int CurrentMapCoords => Version switch
                {
                    Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                        or Version1_3_2 => 0x6B8,
                    Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0x6B0,
                    _ => 0x6C0,
                };

                public static int CurrentMapAngle => Version switch
                {
                    Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                        or Version1_3_2 => 0x6C4,
                    Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0x6BC,
                    _ => 0x6CC,
                };
            }
        }

        public static class ChrIns
        {
            public const int ChrCtrl = 0x58;
            public const int ChrId = 0x64;
            public const int SpecialEffect = 0x178;

            public static int EntityId => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0x1E4,
                _ => 0x1E8,
            };

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

            public static int Flags => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0x520,
                _ => 0x530,
            };

            public enum ChrInsFlags
            {
                NoHit = 1 << 3,
                NoAttack = 1 << 4,
                NoMove = 1 << 5,
                // 1 << 6 is a red capsule towards the direction the boss is facing
                //1 << 7 same but white capsule
            }

            public static int ChrManipulator => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2
                    or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0x570,
                _ => 0x580,
            };

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
            }

            public static int ChrDataFlags => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 => 0x197,
                _ => 0x19B,
            };

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

            public static class AiThinkOffsets
            {
                public const int NpcThinkParamId = 0x28;
                public const int AnimationRequest = 0xC428;
                public const int TargetingSystem = 0xC480;

                public static int ForceAct => Version switch
                {
                    Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                        or Version1_3_2
                        or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0xE9B1,
                    _ => 0xE9C1,
                };

                public static int LastAct => Version switch
                {
                    Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                        or Version1_3_2
                        or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0 or Version1_7_0 => 0xE9B2,
                    _ => 0xE9C2,
                };
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
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0x60C,
                _ => 0x61C,
            };

            public static int DrawTiles2 => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0x60E,
                _ => 0x61E,
            };

            public const int WorldInfoOwner = 0x10;

            public static int ShouldDrawMiniMap => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0 => 0x7F098,
                Version1_7_0 or Version1_8_0 or Version1_8_1 or Version1_9_0 or Version1_9_1 or Version2_0_0
                    or Version2_0_1 => 0xB3918,
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

            public static int LowHit => Version switch
            {
                Version1_4_1 or Version1_5_0 => -0x14,
                _ => 0xC,
            };

            public static int HighHit => Version switch
            {
                Version1_4_1 or Version1_5_0 => -0x13,
                _ => 0xD,
            };

            public static int Ragdoll => Version switch
            {
                Version1_4_1 or Version1_5_0 => -0x11,
                _ => 0xF,
            };

            public static int Mode => Version switch
            {
                Version1_4_1 or Version1_5_0 => -0xC,
                _ => 0x14,
            };
        }

        public static class MenuMan
        {
            public static IntPtr Base;

            public const int FlagArray = 0x90;
            public const int IsLoaded = 0x94;

            public static int PopupMenu => Version switch
            {
                Version1_2_0 => 0x78,
                _ => 0x80
            };

            public static int IsFading => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 => 0x8E,
                _ => 0x96,
            };

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
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0x930,
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
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0x18,
                _ => 0x20,
            };

            public static int ForceSave => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0xB42,
                _ => 0xb72,
            };

            public static int ShouldStartNewGame => Version switch
            {
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0xB4D,
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
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0x2D4,
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
                Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1
                    or Version1_3_2 or Version1_4_0
                    or Version1_4_1 or Version1_5_0
                    or Version1_6_0
                    or Version1_7_0
                    or Version1_8_0
                    or Version1_8_1
                    or Version1_9_0 or Version1_9_1
                    or Version2_0_0 or Version2_0_1 => 0x5,
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
            public static long LoadScreenMsgLookupEarlyPatches;
            public static long LoadScreenMsgLookupMidPatches;
            public static long NoHeal;
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
                Version1_2_0 => 0x3C50268,
                Version1_2_1 => 0x3C50288,
                Version1_2_2 => 0x3C502A8,
                Version1_2_3 => 0x3C532C8,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C64E38,
                Version1_4_0 or Version1_4_1 => 0x3C080E8,
                Version1_5_0 => 0x3C1FE98,
                Version1_6_0 => 0x3C310B8,
                Version1_7_0 => 0x3C4BA78,
                Version1_8_0 or Version1_8_1 => 0x3CD9998,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDCDD8,
                Version2_2_0 => 0x3D65F88,
                Version2_2_3 or Version2_3_0 => 0x3D65FA8,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D65F88,
                _ => 0
            };

            FieldArea.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C53470,
                Version1_2_1 => 0x3C53490,
                Version1_2_2 => 0x3C534B0,
                Version1_2_3 => 0x3C564D0,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C68040,
                Version1_4_0 or Version1_4_1 => 0x3C0B2C0,
                Version1_5_0 => 0x3C23070,
                Version1_6_0 => 0x3C34298,
                Version1_7_0 => 0x3C4EC50,
                Version1_8_0 or Version1_8_1 => 0x3CDCB80,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDFFC0,
                Version2_2_0 => 0x3D691D8,
                Version2_2_3 or Version2_3_0 => 0x3D691F8,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D691D8,
                _ => 0
            };

            LuaEventMan.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C520F8,
                Version1_2_1 => 0x3C52118,
                Version1_2_2 => 0x3C52138,
                Version1_2_3 => 0x3C55158,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C66CB8,
                Version1_4_0 or Version1_4_1 => 0x3C09F48,
                Version1_5_0 => 0x3C21CF8,
                Version1_6_0 => 0x3C32F18,
                Version1_7_0 => 0x3C4D8D8,
                Version1_8_0 or Version1_8_1 => 0x3CDB7F8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDEC38,
                Version2_2_0 => 0x3D67E48,
                Version2_2_3 or Version2_3_0 => 0x3D67E68,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D67E48,
                _ => 0
            };

            VirtualMemFlag.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C526E8,
                Version1_2_1 => 0x3C52708,
                Version1_2_2 => 0x3C52728,
                Version1_2_3 => 0x3C55748,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C672A8,
                Version1_4_0 or Version1_4_1 => 0x3C0A538,
                Version1_5_0 => 0x3C222E8,
                Version1_6_0 => 0x3C33508,
                Version1_7_0 => 0x3C4DEC8,
                Version1_8_0 or Version1_8_1 => 0x3CDBDF8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDF238,
                Version2_2_0 => 0x3D68448,
                Version2_2_3 or Version2_3_0 => 0x3D68468,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D68448,
                _ => 0
            };

            DamageManager.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C50658,
                Version1_2_1 => 0x3C50678,
                Version1_2_2 => 0x3C50698,
                Version1_2_3 => 0x3C536B8,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C65228,
                Version1_4_0 or Version1_4_1 => 0x3C084B8,
                Version1_5_0 => 0x3C20268,
                Version1_6_0 => 0x3C31488,
                Version1_7_0 => 0x3C4BE48,
                Version1_8_0 or Version1_8_1 => 0x3CD9D68,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDD1A8,
                Version2_2_0 => 0x3D66378,
                Version2_2_3 or Version2_3_0 => 0x3D66398,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D66378,
                _ => 0
            };

            MenuMan.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C55B30,
                Version1_2_1 => 0x3C55B50,
                Version1_2_2 => 0x3C55B70,
                Version1_2_3 => 0x3C58B90,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C6A700,
                Version1_4_0 or Version1_4_1 => 0x3C0D9D0,
                Version1_5_0 => 0x3C25780,
                Version1_6_0 => 0x3C369A0,
                Version1_7_0 => 0x3C51360,
                Version1_8_0 or Version1_8_1 => 0x3CDF140,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CE2580,
                Version2_2_0 => 0x3D6B7B0,
                Version2_2_3 or Version2_3_0 => 0x3D6B7D0,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D6B7B0,
                _ => 0
            };

            TargetView.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C4C4EA,
                Version1_2_1 => 0x3C4C50A,
                Version1_2_2 => 0x3C4C52A,
                Version1_2_3 => 0x3C4F54A,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C610AA,
                Version1_4_0 or Version1_4_1 => 0x3C0435A,
                Version1_5_0 => 0x3C1C21A,
                Version1_6_0 => 0x3C2D43A,
                Version1_7_0 => 0x3C47DFA,
                Version1_8_0 or Version1_8_1 => 0x3CD5C7A,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CD90BA,
                Version2_2_0 => 0x3D6226B,
                Version2_2_3 or Version2_3_0 => 0x3D6228B,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D6226B,
                _ => 0
            };

            GameMan.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C53B88,
                Version1_2_1 => 0x3C53BA8,
                Version1_2_2 => 0x3C53BC8,
                Version1_2_3 => 0x3C56BE8,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C68758,
                Version1_4_0 or Version1_4_1 => 0x3C0BA08,
                Version1_5_0 => 0x3C237B8,
                Version1_6_0 => 0x3C349D8,
                Version1_7_0 => 0x3C4F398,
                Version1_8_0 or Version1_8_1 => 0x3CDD2C8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CE0708,
                Version2_2_0 => 0x3D69918,
                Version2_2_3 or Version2_3_0 => 0x3D69938,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D69918,
                _ => 0
            };

            WorldHitMan.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C54320,
                Version1_2_1 => 0x3C54340,
                Version1_2_2 => 0x3C54360,
                Version1_2_3 => 0x3C57380,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C68EF0,
                Version1_4_0 or Version1_4_1 => 0x3C0C1C8,
                Version1_5_0 => 0x3C23F78,
                Version1_6_0 => 0x3C35180,
                Version1_7_0 => 0x3C4FB40,
                Version1_8_0 or Version1_8_1 => 0x3CDDA70,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CE0EB0,
                Version2_2_0 => 0x3D6A0E0,
                Version2_2_3 or Version2_3_0 => 0x3D6A100,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D6A0E0,
                _ => 0
            };

            WorldChrManDbg.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C50478,
                Version1_2_1 => 0x3C50498,
                Version1_2_2 => 0x3C504B8,
                Version1_2_3 => 0x3C53520,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C65048,
                Version1_4_0 or Version1_4_1 => 0x3C082D8,
                Version1_5_0 => 0x3C200D0,
                Version1_6_0 => 0x3C312A8,
                Version1_7_0 => 0x3C4BC68,
                Version1_8_0 or Version1_8_1 => 0x3CD9BD0,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDD010,
                Version2_2_0 => 0x3D66198,
                Version2_2_3 or Version2_3_0 => 0x3D661B8,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D66198,
                _ => 0
            };

            GameDataMan.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C481B8,
                Version1_2_1 => 0x3C481D8,
                Version1_2_2 => 0x3C481F8,
                Version1_2_3 => 0x3C4B218,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C5CD78,
                Version1_4_0 or Version1_4_1 => 0x3C00028,
                Version1_5_0 => 0x3C17EE8,
                Version1_6_0 => 0x3C29108,
                Version1_7_0 => 0x3C43AC8,
                Version1_8_0 or Version1_8_1 => 0x3CD1948,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CD4D88,
                Version2_2_0 => 0x3D5DF38,
                Version2_2_3 or Version2_3_0 => 0x3D5DF58,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D5DF38,
                _ => 0
            };

            CsDlcImp.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C705D8,
                Version1_2_1 => 0x3C701F8,
                Version1_2_2 => 0x3C70218,
                Version1_2_3 => 0x3C73238,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C84DB8,
                Version1_4_0 or Version1_4_1 => 0x3C27F58,
                Version1_5_0 => 0x3C3FD58,
                Version1_6_0 => 0x3C50FB8,
                Version1_7_0 => 0x3C6B988,
                Version1_8_0 or Version1_8_1 => 0x3CFA3F8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CFD838,
                Version2_2_0 => 0x3D86BD8,
                Version2_2_3 or Version2_3_0 => 0x3D86BF8,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D86BD8,
                _ => 0
            };

            MapItemManImpl.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C51CF0,
                Version1_2_1 => 0x3C51D10,
                Version1_2_2 => 0x3C51D30,
                Version1_2_3 => 0x3C54D50,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C668C0,
                Version1_4_0 or Version1_4_1 => 0x3C09B50,
                Version1_5_0 => 0x3C21900,
                Version1_6_0 => 0x3C32B20,
                Version1_7_0 => 0x3C4D4E0,
                Version1_8_0 or Version1_8_1 => 0x3CDB400,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDE840,
                Version2_2_0 => 0x3D67A50,
                Version2_2_3 or Version2_3_0 => 0x3D67A70,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D67A50,
                _ => 0
            };

            FD4PadManager.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x45255D0,
                Version1_2_1 => 0x45251F0,
                Version1_2_2 => 0x4525210,
                Version1_2_3 => 0x4528230,
                Version1_3_0 => 0x4539DB0,
                Version1_3_1 or Version1_3_2 => 0x4539DA0,
                Version1_4_0 or Version1_4_1 => 0x44DD6F0,
                Version1_5_0 => 0x44F5830,
                Version1_6_0 => 0x45075D0,
                Version1_7_0 => 0x4521F90,
                Version1_8_0 or Version1_8_1 => 0x45B1920,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x45B4D50,
                Version2_2_0 => 0x485DB70,
                Version2_2_3 or Version2_3_0 => 0x485DB90,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x485DC20,
                _ => 0
            };

            CSEmkSystem.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C51E78,
                Version1_2_1 => 0x3C51E98,
                Version1_2_2 => 0x3C51EB8,
                Version1_2_3 => 0x3C54ED8,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C66A40,
                Version1_4_0 or Version1_4_1 => 0x3C09CD0,
                Version1_5_0 => 0x3C21A80,
                Version1_6_0 => 0x3C32CA0,
                Version1_7_0 => 0x3C4D660,
                Version1_8_0 or Version1_8_1 => 0x3CDB580,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDE9C0,
                Version2_2_0 => 0x3D67BD0,
                Version2_2_3 or Version2_3_0 => 0x3D67BF0,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D67BD0,
                _ => 0
            };

            WorldAreaTimeImpl.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C535A0,
                Version1_2_1 => 0x3C535C0,
                Version1_2_2 => 0x3C535E0,
                Version1_2_3 => 0x3C56600,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C68170,
                Version1_4_0 or Version1_4_1 => 0x3C0B3F0,
                Version1_5_0 => 0x3C231A0,
                Version1_6_0 => 0x3C343B8,
                Version1_7_0 => 0x3C4ED80,
                Version1_8_0 or Version1_8_1 => 0x3CDCCB0,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CE00F0,
                Version2_2_0 => 0x3D692F8,
                Version2_2_3 or Version2_3_0 => 0x3D69318,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D692F8,
                _ => 0
            };

            GroupMask.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3A367E0,
                Version1_2_1 or Version1_2_2 => 0x3A36800,
                Version1_2_3 => 0x3A39808,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3A4A808,
                Version1_4_0 or Version1_4_1 => 0x39F6820,
                Version1_5_0 => 0x3A0D839,
                Version1_6_0 => 0x3A1E830,
                Version1_7_0 => 0x3A37A20,
                Version1_8_0 or Version1_8_1 => 0x3ABFAE8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3AC2AE8,
                Version2_2_0 or Version2_2_3 or Version2_3_0 or Version2_4_0
                    or Version2_5_0
                    or Version2_6_0
                    or Version2_6_1 => 0x3B33D00,
                _ => 0
            };


            CSFlipperImp.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x4473138,
                Version1_2_1 => 0x4472D58,
                Version1_2_2 => 0x4472D78,
                Version1_2_3 => 0x4475D98,
                Version1_3_0 => 0x4487918,
                Version1_3_1 or Version1_3_2 => 0x4487908,
                Version1_4_0 or Version1_4_1 => 0x442AB08,
                Version1_5_0 => 0x4442C18,
                Version1_6_0 => 0x4453E98,
                Version1_7_0 => 0x446E858,
                Version1_8_0 or Version1_8_1 => 0x44FD2C8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x4500708,
                Version2_2_0 => 0x4589AD8,
                Version2_2_3 or Version2_3_0 => 0x4589AF8,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x4589AD8,
                _ => 0
            };

            CSDbgEvent.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C522A0,
                Version1_2_1 => 0x3C522C0,
                Version1_2_2 => 0x3C522E0,
                Version1_2_3 => 0x3C55300,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C66E60,
                Version1_4_0 or Version1_4_1 => 0x3C0A0F0,
                Version1_5_0 => 0x3C21EA0,
                Version1_6_0 => 0x3C330C0,
                Version1_7_0 => 0x3C4DA80,
                Version1_8_0 or Version1_8_1 => 0x3CDB9A8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDEDE8,
                Version2_2_0 => 0x3D67FF8,
                Version2_2_3 or Version2_3_0 => 0x3D68018,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D67FF8,
                _ => 0
            };


            UserInputManager.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x45255C8,
                Version1_2_1 => 0x45251E8,
                Version1_2_2 => 0x4525208,
                Version1_2_3 => 0x4528228,
                Version1_3_0 => 0x4539DA8,
                Version1_3_1 or Version1_3_2 => 0x4539D98,
                Version1_4_0 or Version1_4_1 => 0x44DD6E8,
                Version1_5_0 => 0x44F5828,
                Version1_6_0 => 0x45075C8,
                Version1_7_0 => 0x4521F88,
                Version1_8_0 or Version1_8_1 => 0x45B1918,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x45B4D48,
                Version2_2_0 => 0x485DB68,
                Version2_2_3 or Version2_3_0 => 0x485DB88,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x485DC18,
                _ => 0
            };

            CSTrophy.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x4472AD8,
                Version1_2_1 => 0x44726F8,
                Version1_2_2 => 0x4472718,
                Version1_2_3 => 0x4475738,
                Version1_3_0 => 0x44872B8,
                Version1_3_1 or Version1_3_2 => 0x44872A8,
                Version1_4_0 or Version1_4_1 => 0x442A4A8,
                Version1_5_0 => 0x44425B8,
                Version1_6_0 => 0x4453838,
                Version1_7_0 => 0x446E1F8,
                Version1_8_0 or Version1_8_1 => 0x44FCC68,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x45000A8,
                Version2_2_0 => 0x4589478,
                Version2_2_3 or Version2_3_0 => 0x4589498,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x4589478,
                _ => 0
            };


            MapDebugFlags.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C56BE0,
                Version1_2_1 => 0x3C56C00,
                Version1_2_2 => 0x3C56C20,
                Version1_2_3 => 0x3C59C40,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C6B7B0,
                Version1_4_0 or Version1_4_1 => 0x3C0E8B8,
                Version1_5_0 => 0x3C26660,
                Version1_6_0 => 0x3C37880,
                Version1_7_0 => 0x3C52240,
                Version1_8_0 or Version1_8_1 => 0x3CE0940,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CE3D80,
                Version2_2_0 => 0x3D6CFC0,
                Version2_2_3 or Version2_3_0 => 0x3D6CFE0,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D6CFC0,
                _ => 0
            };

            SoloParamRepositoryImp.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C6B578,
                Version1_2_1 => 0x3C6B598,
                Version1_2_2 => 0x3C6B5B8,
                Version1_2_3 => 0x3C6E5C8,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C80158,
                Version1_4_0 or Version1_4_1 => 0x3C232E8,
                Version1_5_0 => 0x3C3B0D8,
                Version1_6_0 => 0x3C4C348,
                Version1_7_0 => 0x3C66D18,
                Version1_8_0 or Version1_8_1 => 0x3CF5788,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CF8BC8,
                Version2_2_0 => 0x3D81EE8,
                Version2_2_3 or Version2_3_0 => 0x3D81F08,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D81EE8,
                _ => 0
            };

            MsgRepository.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C66D48,
                Version1_2_1 => 0x3C66D68,
                Version1_2_2 => 0x3C66D88,
                Version1_2_3 => 0x3C69D98,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C7B928,
                Version1_4_0 or Version1_4_1 => 0x3C1EAB8,
                Version1_5_0 => 0x3C36878,
                Version1_6_0 => 0x3C47AE8,
                Version1_7_0 => 0x3C624B8,
                Version1_8_0 or Version1_8_1 => 0x3CF0DD8,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CF4218,
                Version2_2_0 => 0x3D7D4F8,
                Version2_2_3 or Version2_3_0 => 0x3D7D518,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D7D4F8,
                _ => 0
            };
            
            DrawPathing.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C4C030,
                Version1_2_1 => 0x3C4C050,
                Version1_2_2 => 0x3C4C070,
                Version1_2_3 => 0x3C4F090,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C60BF0,
                Version1_4_0 or Version1_4_1 => 0x3C03EA0,
                Version1_5_0 => 0x3C1BD60,
                Version1_6_0 => 0x3C2CF80,
                Version1_7_0 => 0x3C47940,
                Version1_8_0 or Version1_8_1 => 0x3CD57C0,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CD8C00,
                Version2_2_0 => 0x3D61DB0,
                Version2_2_3 or Version2_3_0 => 0x3D61DD0,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D61DB0,
                _ => 0
            };
            
            ChrDbgFlags.Base = moduleBase + Version switch
            {
                Version1_2_0 => 0x3C50480,
                Version1_2_1 => 0x3C504A0,
                Version1_2_2 => 0x3C504C0,
                Version1_2_3 => 0x3C534DE,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C65050,
                Version1_4_0 or Version1_4_1 => 0x3C082E0,
                Version1_5_0 => 0x3C2008A,
                Version1_6_0 => 0x3C312B0,
                Version1_7_0 => 0x3C4BC70,
                Version1_8_0 or Version1_8_1 => 0x3CD9B97,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3CDCFE8,
                Version2_2_0 => 0x3D661A0,
                Version2_2_3 or Version2_3_0 => 0x3D661C0,
                Version2_4_0 or Version2_5_0 or Version2_6_0
                    or Version2_6_1 => 0x3D661A0,
                _ => 0
            };



            // Functions
            Functions.GraceWarp = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x5855B0,
                Version1_2_1 or Version1_2_2 => 0x585620,
                Version1_2_3 => 0x585740,
                Version1_3_0 or Version1_3_1 => 0x586720,
                Version1_3_2 => 0x586710,
                Version1_4_0 => 0x589410,
                Version1_4_1 => 0x589320,
                Version1_5_0 => 0x589930,
                Version1_6_0 => 0x58AC00,
                Version1_7_0 => 0x58BA70,
                Version1_8_0 or Version1_8_1 => 0x5951E0,
                Version1_9_0 => 0x595560,
                Version1_9_1 => 0x5955C0,
                Version2_0_0 or Version2_0_1 => 0x595800,
                Version2_2_0 or Version2_2_3 => 0x599B20,
                Version2_3_0 => 0x599CA0,
                Version2_4_0 or Version2_5_0 => 0x599D00,
                Version2_6_0 or Version2_6_1 => 0x599CD0,
                _ => 0L
            };

            Functions.SetEvent = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x5D9E40,
                Version1_2_1 or Version1_2_2 => 0x5D9EB0,
                Version1_2_3 => 0x5D9FD0,
                Version1_3_0 or Version1_3_1 => 0x5DB060,
                Version1_3_2 => 0x5DB040,
                Version1_4_0 => 0x5DDD40,
                Version1_4_1 => 0x5DDC50,
                Version1_5_0 => 0x5DE730,
                Version1_6_0 => 0x5DFED0,
                Version1_7_0 => 0x5E0D50,
                Version1_8_0 or Version1_8_1 => 0x5ED450,
                Version1_9_0 => 0x5EE170,
                Version1_9_1 => 0x5EE1D0,
                Version2_0_0 or Version2_0_1 => 0x5EE410,
                Version2_2_0 or Version2_2_3 => 0x5F9970,
                Version2_3_0 => 0x5F9AF0,
                Version2_4_0 or Version2_5_0 => 0x5F9B50,
                Version2_6_0 or Version2_6_1 => 0x5F9CD0,
                _ => 0L
            };

            Functions.SetSpEffect = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x3E17E0,
                Version1_2_1 or Version1_2_2 => 0x3E1850,
                Version1_2_3 => 0x3E1970,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3E2090,
                Version1_4_0 or Version1_4_1 => 0x3E4550,
                Version1_5_0 => 0x3E4920,
                Version1_6_0 => 0x3E5700,
                Version1_7_0 => 0x3E5730,
                Version1_8_0 or Version1_8_1 => 0x3E69C0,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3E6AF0,
                Version2_2_0 or Version2_2_3 or Version2_3_0 => 0x3E9100,
                Version2_4_0 or Version2_5_0 => 0x3E9120,
                Version2_6_0 or Version2_6_1 => 0x3E90F0,
                _ => 0L
            };

            Functions.GiveRunes = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x258270,
                Version1_2_1 or Version1_2_2 => 0x2582E0,
                Version1_2_3 => 0x258400,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x258B70,
                Version1_4_0 or Version1_4_1 => 0x25AB40,
                Version1_5_0 => 0x25ACB0,
                Version1_6_0 => 0x25BB30,
                Version1_7_0 => 0x25BCC0,
                Version1_8_0 or Version1_8_1 => 0x25C670,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x25C7A0,
                Version2_2_0 or Version2_2_3 or Version2_3_0 or Version2_4_0
                    or Version2_5_0
                    or Version2_6_0
                    or Version2_6_1 => 0x25E1B0,
                _ => 0L
            };

            Functions.LookupByFieldInsHandle = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x4F7580,
                Version1_2_1 or Version1_2_2 => 0x4F75F0,
                Version1_2_3 => 0x4F7710,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x4F8620,
                Version1_4_0 => 0x4FB430,
                Version1_4_1 => 0x4FB340,
                Version1_5_0 => 0x4FB6D0,
                Version1_6_0 => 0x4FC840,
                Version1_7_0 => 0x4FC7F0,
                Version1_8_0 or Version1_8_1 => 0x503B80,
                Version1_9_0 => 0x503EA0,
                Version1_9_1 => 0x503F00,
                Version2_0_0 or Version2_0_1 => 0x504140,
                Version2_2_0 or Version2_2_3 => 0x507BC0,
                Version2_3_0 => 0x507D40,
                Version2_4_0 or Version2_5_0 => 0x507D80,
                Version2_6_0 or Version2_6_1 => 0x507D50,
                _ => 0L
            };

            Functions.WarpToBlock = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x5D7DA0,
                Version1_2_1 or Version1_2_2 => 0x5D7E10,
                Version1_2_3 => 0x5D7F30,
                Version1_3_0 or Version1_3_1 => 0x5D8FC0,
                Version1_3_2 => 0x5D8FA0,
                Version1_4_0 => 0x5DBCA0,
                Version1_4_1 => 0x5DBBB0,
                Version1_5_0 => 0x5DC690,
                Version1_6_0 => 0x5DDE30,
                Version1_7_0 => 0x5DECB0,
                Version1_8_0 or Version1_8_1 => 0x5EB330,
                Version1_9_0 => 0x5EC050,
                Version1_9_1 => 0x5EC0B0,
                Version2_0_0 or Version2_0_1 => 0x5EC2F0,
                Version2_2_0 or Version2_2_3 => 0x5F7850,
                Version2_3_0 => 0x5F79D0,
                Version2_4_0 or Version2_5_0 => 0x5F7A30,
                Version2_6_0 or Version2_6_1 => 0x5F7BB0,
                _ => 0L
            };


            Functions.GetEvent = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x5D9650,
                Version1_2_1 or Version1_2_2 => 0x5D96C0,
                Version1_2_3 => 0x5D97E0,
                Version1_3_0 or Version1_3_1 => 0x5DA870,
                Version1_3_2 => 0x5DA850,
                Version1_4_0 => 0x5DD550,
                Version1_4_1 => 0x5DD460,
                Version1_5_0 => 0x5DDF40,
                Version1_6_0 => 0x5DF6E0,
                Version1_7_0 => 0x5E0560,
                Version1_8_0 or Version1_8_1 => 0x5ECC60,
                Version1_9_0 => 0x5ED980,
                Version1_9_1 => 0x5ED9E0,
                Version2_0_0 or Version2_0_1 => 0x5EDC20,
                Version2_2_0 or Version2_2_3 => 0x5F9180,
                Version2_3_0 => 0x5F9300,
                Version2_4_0 or Version2_5_0 => 0x5F9360,
                Version2_6_0 or Version2_6_1 => 0x5F94E0,
                _ => 0L
            };

            Functions.GetPlayerItemQuantityById = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x75ACC0,
                Version1_2_1 or Version1_2_2 => 0x75AD30,
                Version1_2_3 => 0x75AE00,
                Version1_3_0 or Version1_3_1 => 0x75C1A0,
                Version1_3_2 => 0x75C180,
                Version1_4_0 => 0x7603C0,
                Version1_4_1 => 0x7602D0,
                Version1_5_0 => 0x761810,
                Version1_6_0 => 0x7636A0,
                Version1_7_0 => 0x764D80,
                Version1_8_0 or Version1_8_1 => 0x773500,
                Version1_9_0 => 0x7745A0,
                Version1_9_1 => 0x774600,
                Version2_0_0 or Version2_0_1 => 0x774890,
                Version2_2_0 or Version2_2_3 => 0x784CF0,
                Version2_3_0 => 0x784EE0,
                Version2_4_0 or Version2_5_0 => 0x784F40,
                Version2_6_0 or Version2_6_1 => 0x7850C0,
                _ => 0L
            };

            Functions.ItemSpawn = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x54E570,
                Version1_2_1 or Version1_2_2 => 0x54E5E0,
                Version1_2_3 => 0x54E700,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x54F640,
                Version1_4_0 => 0x552330,
                Version1_4_1 => 0x552240,
                Version1_5_0 => 0x552840,
                Version1_6_0 => 0x5539E0,
                Version1_7_0 => 0x554850,
                Version1_8_0 or Version1_8_1 => 0x55C1A0,
                Version1_9_0 => 0x55C4C0,
                Version1_9_1 => 0x55C520,
                Version2_0_0 or Version2_0_1 => 0x55C760,
                Version2_2_0 or Version2_2_3 => 0x5604E0,
                Version2_3_0 => 0x560660,
                Version2_4_0 or Version2_5_0 => 0x5606A0,
                Version2_6_0 or Version2_6_1 => 0x560670,
                _ => 0L
            };

            Functions.MatrixVectorProduct = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0xAB01E0,
                Version1_2_1 => 0xAB0260,
                Version1_2_2 => 0xAB02D0,
                Version1_2_3 => 0xAB03B0,
                Version1_3_0 => 0xAB56C0,
                Version1_3_1 => 0xAB56D0,
                Version1_3_2 => 0xAB56B0,
                Version1_4_0 => 0xA95010,
                Version1_4_1 => 0xA94F20,
                Version1_5_0 => 0xA99620,
                Version1_6_0 => 0xA9D520,
                Version1_7_0 => 0xA9EBD0,
                Version1_8_0 or Version1_8_1 => 0xAE06E0,
                Version1_9_0 => 0xAE3270,
                Version1_9_1 => 0xAE32D0,
                Version2_0_0 or Version2_0_1 => 0xAE3560,
                Version2_2_0 or Version2_2_3 => 0xB11C40,
                Version2_3_0 => 0xB11FB0,
                Version2_4_0 or Version2_5_0 => 0xB12130,
                Version2_6_0 => 0xB122B0,
                Version2_6_1 => 0xB12310,
                _ => 0L
            };

            Functions.ChrInsByHandle = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x4F74A0,
                Version1_2_1 or Version1_2_2 => 0x4F7510,
                Version1_2_3 => 0x4F7630,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x4F8540,
                Version1_4_0 => 0x4FB350,
                Version1_4_1 => 0x4FB260,
                Version1_5_0 => 0x4FB5F0,
                Version1_6_0 => 0x4FC760,
                Version1_7_0 => 0x4FC710,
                Version1_8_0 or Version1_8_1 => 0x503AA0,
                Version1_9_0 => 0x503DC0,
                Version1_9_1 => 0x503E20,
                Version2_0_0 or Version2_0_1 => 0x504060,
                Version2_2_0 or Version2_2_3 => 0x507AE0,
                Version2_3_0 => 0x507C60,
                Version2_4_0 or Version2_5_0 => 0x507CA0,
                Version2_6_0 or Version2_6_1 => 0x507C70,
                _ => 0L
            };

            Functions.FindAndRemoveSpEffect = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x4E6970,
                Version1_2_1 or Version1_2_2 => 0x4E69E0,
                Version1_2_3 => 0x4E6B00,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x4E79C0,
                Version1_4_0 => 0x4EA730,
                Version1_4_1 => 0x4EA640,
                Version1_5_0 => 0x4EA9D0,
                Version1_6_0 => 0x4EBB40,
                Version1_7_0 => 0x4EBB50,
                Version1_8_0 or Version1_8_1 => 0x4F2E00,
                Version1_9_0 => 0x4F3060,
                Version1_9_1 => 0x4F3070,
                Version2_0_0 or Version2_0_1 => 0x4F32B0,
                Version2_2_0 or Version2_2_3 => 0x4F67F0,
                Version2_3_0 => 0x4F6970,
                Version2_4_0 or Version2_5_0 => 0x4F69B0,
                Version2_6_0 or Version2_6_1 => 0x4F6980,
                _ => 0L
            };

            Functions.EmevdSwitch = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x555D00,
                Version1_2_1 or Version1_2_2 => 0x555D70,
                Version1_2_3 => 0x555E90,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x556DA0,
                Version1_4_0 => 0x559AC0,
                Version1_4_1 => 0x5599D0,
                Version1_5_0 => 0x559FD0,
                Version1_6_0 => 0x55B170,
                Version1_7_0 => 0x55BFE0,
                Version1_8_0 or Version1_8_1 => 0x563930,
                Version1_9_0 => 0x563C50,
                Version1_9_1 => 0x563CB0,
                Version2_0_0 or Version2_0_1 => 0x563EF0,
                Version2_2_0 or Version2_2_3 => 0x567C70,
                Version2_3_0 => 0x567DF0,
                Version2_4_0 or Version2_5_0 => 0x567E30,
                Version2_6_0 or Version2_6_1 => 0x567E00,
                _ => 0L
            };

            Functions.EmkEventInsCtor = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x56ECA0,
                Version1_2_1 or Version1_2_2 => 0x56ED10,
                Version1_2_3 => 0x56EE30,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x56FDA0,
                Version1_4_0 => 0x572AC0,
                Version1_4_1 => 0x5729D0,
                Version1_5_0 => 0x572FE0,
                Version1_6_0 => 0x574180,
                Version1_7_0 => 0x574FF0,
                Version1_8_0 or Version1_8_1 => 0x57DDC0,
                Version1_9_0 => 0x57E0E0,
                Version1_9_1 => 0x57E140,
                Version2_0_0 or Version2_0_1 => 0x57E380,
                Version2_2_0 or Version2_2_3 => 0x582570,
                Version2_3_0 => 0x5826F0,
                Version2_4_0 or Version2_5_0 => 0x582730,
                Version2_6_0 or Version2_6_1 => 0x582700,
                _ => 0L
            };

            Functions.GetMovement = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x25B6CD0,
                Version1_2_1 => 0x25B6D20,
                Version1_2_2 => 0x25B71C0,
                Version1_2_3 => 0x25B8640,
                Version1_3_0 => 0x25C1EB0,
                Version1_3_1 => 0x25C1C50,
                Version1_3_2 => 0x25C1C30,
                Version1_4_0 => 0x25A98F0,
                Version1_4_1 => 0x25A9800,
                Version1_5_0 => 0x25B7CA0,
                Version1_6_0 => 0x25C1E00,
                Version1_7_0 => 0x25CF3D0,
                Version1_8_0 => 0x261BD10,
                Version1_8_1 => 0x261BCF0,
                Version1_9_0 => 0x261E9D0,
                Version1_9_1 => 0x261EAE0,
                Version2_0_0 => 0x261ED90,
                Version2_0_1 => 0x261EE70,
                Version2_2_0 => 0x2663950,
                Version2_2_3 => 0x2663970,
                Version2_3_0 => 0x2664210,
                Version2_4_0 or Version2_5_0 => 0x2664240,
                Version2_6_0 => 0x2664210,
                Version2_6_1 => 0x2664270,
                _ => 0L
            };

            Functions.GetChrInsByEntityId = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x4F7630,
                Version1_2_1 or Version1_2_2 => 0x4F76A0,
                Version1_2_3 => 0x4F77C0,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x4F86D0,
                Version1_4_0 => 0x4FB4E0,
                Version1_4_1 => 0x4FB3F0,
                Version1_5_0 => 0x4FB780,
                Version1_6_0 => 0x4FC8F0,
                Version1_7_0 => 0x4FC8A0,
                Version1_8_0 or Version1_8_1 => 0x503C30,
                Version1_9_0 => 0x503F50,
                Version1_9_1 => 0x503FB0,
                Version2_0_0 or Version2_0_1 => 0x5041F0,
                Version2_2_0 or Version2_2_3 => 0x507C70,
                Version2_3_0 => 0x507DF0,
                Version2_4_0 or Version2_5_0 => 0x507E30,
                Version2_6_0 or Version2_6_1 => 0x507E00,
                _ => 0L
            };

            Functions.NpcEzStateTalkCtor = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0xE17E70,
                Version1_2_1 => 0xE17EC0,
                Version1_2_2 => 0xE18360,
                Version1_2_3 => 0xE18E40,
                Version1_3_0 => 0xE211A0,
                Version1_3_1 => 0xE20F40,
                Version1_3_2 => 0xE20F20,
                Version1_4_0 => 0xE01D30,
                Version1_4_1 => 0xE01C40,
                Version1_5_0 => 0xE09B40,
                Version1_6_0 => 0xE11D90,
                Version1_7_0 => 0xE154D0,
                Version1_8_0 => 0xE5C320,
                Version1_8_1 => 0xE5C300,
                Version1_9_0 => 0xE5EDC0,
                Version1_9_1 => 0xE5EED0,
                Version2_0_0 => 0xE5F180,
                Version2_0_1 => 0xE5F260,
                Version2_2_0 => 0xE9E830,
                Version2_2_3 => 0xE9E850,
                Version2_3_0 => 0xE9ED40,
                Version2_4_0 or Version2_5_0 => 0xE9ECA0,
                Version2_6_0 => 0xE9EC70,
                Version2_6_1 => 0xE9ECD0,
                _ => 0L
            };

            Functions.EzStateEnvQueryImplCtor = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x1FD0D40,
                Version1_2_1 => 0x1FD0D90,
                Version1_2_2 => 0x1FD1230,
                Version1_2_3 => 0x1FD2570,
                Version1_3_0 => 0x1F9E470,
                Version1_3_1 => 0x1FDBA70,
                Version1_3_2 => 0x1FDBA50,
                Version1_4_0 => 0x1FC36C0,
                Version1_4_1 => 0x1FC35D0,
                Version1_5_0 => 0x1FD1830,
                Version1_6_0 => 0x1FDB990,
                Version1_7_0 => 0x1FE8F60,
                Version1_8_0 => 0x2035560,
                Version1_8_1 => 0x2035540,
                Version1_9_0 => 0x2038230,
                Version1_9_1 => 0x2038340,
                Version2_0_0 => 0x20385F0,
                Version2_0_1 => 0x20386D0,
                Version2_2_0 => 0x207DF50,
                Version2_2_3 => 0x207DF70,
                Version2_3_0 => 0x207E810,
                Version2_4_0 or Version2_5_0 => 0x207E850,
                Version2_6_0 => 0x207E820,
                Version2_6_1 => 0x207E880,
                _ => 0L
            };

            Functions.ExternalEventTempCtor = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x1F934E0,
                Version1_2_1 => 0x1F93530,
                Version1_2_2 => 0x1F939D0,
                Version1_2_3 => 0x1F94D10,
                Version1_3_0 => 0x1F9E470,
                Version1_3_1 => 0x1F9E210,
                Version1_3_2 => 0x1F9E1F0,
                Version1_4_0 => 0x1F85E60,
                Version1_4_1 => 0x1F85D70,
                Version1_5_0 => 0x1F93FD0,
                Version1_6_0 => 0x1F9E130,
                Version1_7_0 => 0x1FAB700,
                Version1_8_0 => 0x1FF7D00,
                Version1_8_1 => 0x1FF7CE0,
                Version1_9_0 => 0x1FFA9D0,
                Version1_9_1 => 0x1FFAAE0,
                Version2_0_0 => 0x1FFAD90,
                Version2_0_1 => 0x1FFAE70,
                Version2_2_0 => 0x20406F0,
                Version2_2_3 => 0x2040710,
                Version2_3_0 => 0x2040FB0,
                Version2_4_0 or Version2_5_0 => 0x2040FF0,
                Version2_6_0 => 0x2040FC0,
                Version2_6_1 => 0x2041020,
                _ => 0L
            };

            Functions.ExecuteTalkCommand = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0xE1E320,
                Version1_2_1 => 0xE1E370,
                Version1_2_2 => 0xE1E810,
                Version1_2_3 => 0xE1F2F0,
                Version1_3_0 => 0xE27650,
                Version1_3_1 => 0xE273F0,
                Version1_3_2 => 0xE273D0,
                Version1_4_0 => 0xE08220,
                Version1_4_1 => 0xE08130,
                Version1_5_0 => 0xE10150,
                Version1_6_0 => 0xE183A0,
                Version1_7_0 => 0xE1BB00,
                Version1_8_0 => 0xE628C0,
                Version1_8_1 => 0xE628A0,
                Version1_9_0 => 0xE65360,
                Version1_9_1 => 0xE65470,
                Version2_0_0 => 0xE65720,
                Version2_0_1 => 0xE65800,
                Version2_2_0 => 0xEA4E10,
                Version2_2_3 => 0xEA4E30,
                Version2_3_0 => 0xEA5320,
                Version2_4_0 or Version2_5_0 => 0xEA5280,
                Version2_6_0 => 0xEA5250,
                Version2_6_1 => 0xEA52B0,
                _ => 0L
            };

            // Hooks
            Hooks.UpdateCoords = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x45E3C9,
                Version1_2_1 or Version1_2_2 => 0x45E439,
                Version1_2_3 => 0x45E559,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x45F1B9,
                Version1_4_0 => 0x4619D9,
                Version1_4_1 => 0x4618E9,
                Version1_5_0 => 0x461C69,
                Version1_6_0 => 0x462CC9,
                Version1_7_0 => 0x462E19,
                Version1_8_0 or Version1_8_1 => 0x4647A9,
                Version1_9_0 or Version1_9_1 => 0x4648E9,
                Version2_0_0 or Version2_0_1 => 0x464A89,
                Version2_2_0 or Version2_2_3 => 0x467879,
                Version2_3_0 => 0x467989,
                Version2_4_0 or Version2_5_0 => 0x4679C9,
                Version2_6_0 or Version2_6_1 => 0x467999,
                _ => 0L
            };

            Hooks.InAirTimer = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x444CA8,
                Version1_2_1 or Version1_2_2 => 0x444D18,
                Version1_2_3 => 0x444E38,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x445A98,
                Version1_4_0 => 0x4482B8,
                Version1_4_1 => 0x4481C8,
                Version1_5_0 => 0x448548,
                Version1_6_0 => 0x4495A8,
                Version1_7_0 => 0x4496F8,
                Version1_8_0 or Version1_8_1 => 0x44B088,
                Version1_9_0 or Version1_9_1 => 0x44B1C8,
                Version2_0_0 or Version2_0_1 => 0x44B368,
                Version2_2_0 or Version2_2_3 => 0x44E158,
                Version2_3_0 => 0x44E268,
                Version2_4_0 or Version2_5_0 => 0x44E2A8,
                Version2_6_0 or Version2_6_1 => 0x44E278,
                _ => 0L
            };

            Hooks.NoClipKb = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x1EBF67F,
                Version1_2_1 => 0x1EBF6CF,
                Version1_2_2 => 0x1EBFB6F,
                Version1_2_3 => 0x1EC0DDF,
                Version1_3_0 => 0x1ECA53F,
                Version1_3_1 => 0x1ECA2DF,
                Version1_3_2 => 0x1ECA2BF,
                Version1_4_0 => 0x1EB1F2F,
                Version1_4_1 => 0x1EB1E3F,
                Version1_5_0 => 0x1EC009F,
                Version1_6_0 => 0x1ECA1FF,
                Version1_7_0 => 0x1ED77CF,
                Version1_8_0 => 0x1F23DCF,
                Version1_8_1 => 0x1F23DAF,
                Version1_9_0 => 0x1F26A9F,
                Version1_9_1 => 0x1F26BAF,
                Version2_0_0 => 0x1F26E5F,
                Version2_0_1 => 0x1F26F3F,
                Version2_2_0 => 0x1F6C7CF,
                Version2_2_3 => 0x1F6C7EF,
                Version2_3_0 => 0x1F6D08F,
                Version2_4_0 => 0x1F6D0CF,
                Version2_5_0 => 0x5D1954,
                Version2_6_0 => 0x1F6D09F,
                Version2_6_1 => 0x1F6D0FF,
                _ => 0L
            };

            Hooks.NoClipTriggers = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x1EBE79E,
                Version1_2_1 => 0x1EBE7EE,
                Version1_2_2 => 0x1EBEC8E,
                Version1_2_3 => 0x1EBFE9F,
                Version1_3_0 => 0x1EC95FF,
                Version1_3_1 => 0x1EC939F,
                Version1_3_2 => 0x1EC937F,
                Version1_4_0 => 0x1EB0FEF,
                Version1_4_1 => 0x1EB0EFF,
                Version1_5_0 => 0x1EBF15F,
                Version1_6_0 => 0x1EC92BF,
                Version1_7_0 => 0x1ED688F,
                Version1_8_0 => 0x1F22E8F,
                Version1_8_1 => 0x1F22E6F,
                Version1_9_0 => 0x1F25B5F,
                Version1_9_1 => 0x1F25C6F,
                Version2_0_0 => 0x1F25F1F,
                Version2_0_1 => 0x1F25FFF,
                Version2_2_0 => 0x1F6B88F,
                Version2_2_3 => 0x1F6B8AF,
                Version2_3_0 or Version2_4_0 or Version2_5_0 => 0x1F6C14F,
                Version2_6_0 => 0x1F6C11F,
                Version2_6_1 => 0x1F6C17F,
                _ => 0L
            };

            Hooks.HasSpEffect = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x4E99C0,
                Version1_2_1 or Version1_2_2 => 0x4E9A30,
                Version1_2_3 => 0x4E9B50,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x4EAA20,
                Version1_4_0 => 0x4ED780,
                Version1_4_1 => 0x4ED690,
                Version1_5_0 => 0x4EDA20,
                Version1_6_0 => 0x4EEB90,
                Version1_7_0 => 0x4EEB40,
                Version1_8_0 or Version1_8_1 => 0x4F5E10,
                Version1_9_0 => 0x4F6070,
                Version1_9_1 => 0x4F60A0,
                Version2_0_0 or Version2_0_1 => 0x4F62E0,
                Version2_2_0 or Version2_2_3 => 0x4F9880,
                Version2_3_0 => 0x4F9A00,
                Version2_4_0 or Version2_5_0 => 0x4F9A40,
                Version2_6_0 or Version2_6_1 => 0x4F9A10,
                _ => 0L
            };

            Hooks.BlueTargetView = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x3382C3,
                Version1_2_1 or Version1_2_2 => 0x338333,
                Version1_2_3 => 0x338453,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x338B53,
                Version1_4_0 or Version1_4_1 => 0x33AC43,
                Version1_5_0 => 0x33AE13,
                Version1_6_0 => 0x33BCA3,
                Version1_7_0 => 0x33BCD3,
                Version1_8_0 or Version1_8_1 => 0x33C913,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x33CA43,
                Version2_2_0 or Version2_2_3 or Version2_3_0 or Version2_4_0 or Version2_5_0 => 0x33E293,
                Version2_6_0 or Version2_6_1 => 0x33E263,
                _ => 0L
            };

            Hooks.LockedTargetPtr = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x6F0A16,
                Version1_2_1 or Version1_2_2 => 0x6F0A86,
                Version1_2_3 => 0x6F0BA6,
                Version1_3_0 or Version1_3_1 => 0x6F1ED6,
                Version1_3_2 => 0x6F1EB6,
                Version1_4_0 => 0x6F5D86,
                Version1_4_1 => 0x6F5C96,
                Version1_5_0 => 0x6F6B46,
                Version1_6_0 => 0x6F8996,
                Version1_7_0 => 0x6FA0E6,
                Version1_8_0 or Version1_8_1 => 0x7078C6,
                Version1_9_0 => 0x708966,
                Version1_9_1 => 0x7089C6,
                Version2_0_0 or Version2_0_1 => 0x708C56,
                Version2_2_0 or Version2_2_3 => 0x716FA2,
                Version2_3_0 => 0x717192,
                Version2_4_0 or Version2_5_0 => 0x7171F2,
                Version2_6_0 or Version2_6_1 => 0x717372,
                _ => 0L
            };

            Hooks.InfinitePoise = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x43CA6A,
                Version1_2_1 or Version1_2_2 => 0x43CADA,
                Version1_2_3 => 0x43CBFA,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x43D820,
                Version1_4_0 => 0x43FF10,
                Version1_4_1 => 0x43FF50,
                Version1_5_0 => 0x440390,
                Version1_6_0 => 0x4411D0,
                Version1_7_0 => 0x441250,
                Version1_8_0 or Version1_8_1 => 0x442BB0,
                Version1_9_0 or Version1_9_1 => 0x442CF0,
                Version2_0_0 or Version2_0_1 => 0x442DC0,
                Version2_2_0 or Version2_2_3 => 0x445B90,
                Version2_3_0 => 0x445CA0,
                Version2_4_0 or Version2_5_0 => 0x445CE0,
                Version2_6_0 or Version2_6_1 => 0x445CB0,
                _ => 0L
            };

            Hooks.ShouldUpdateAi = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x3C09F0,
                Version1_2_1 or Version1_2_2 => 0x3C0A60,
                Version1_2_3 => 0x3C0B80,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3C1250,
                Version1_4_0 or Version1_4_1 => 0x3C3700,
                Version1_5_0 => 0x3C3940,
                Version1_6_0 => 0x3C46B0,
                Version1_7_0 => 0x3C46E0,
                Version1_8_0 or Version1_8_1 => 0x3C5900,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x3C5A30,
                Version2_2_0 or Version2_2_3 or Version2_3_0 => 0x3C7920,
                Version2_4_0 or Version2_5_0 => 0x3C7940,
                Version2_6_0 or Version2_6_1 => 0x3C7910,
                _ => 0L
            };

            Hooks.GetForceActIdx = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x527214E,
                Version1_2_1 => 0x53F7581,
                Version1_2_2 => 0x4F45CB4,
                Version1_2_3 => 0x4EDD1F3,
                Version1_3_0 => 0x1E42A60,
                Version1_3_1 => 0x523C681,
                Version1_3_2 => 0x54E7D7A,
                Version1_4_0 => 0x4FF4209,
                Version1_4_1 => 0x55AC955,
                Version1_5_0 => 0x3683DE,
                Version1_6_0 => 0x4F22453,
                Version1_7_0 => 0xB7F343,
                Version1_8_0 => 0x5757732,
                Version1_8_1 => 0x56BA77E,
                Version1_9_0 => 0x5679CFE,
                Version1_9_1 => 0x17B5923,
                Version2_0_0 => 0x1D9C6F4,
                Version2_0_1 => 0x55611FB,
                Version2_2_0 => 0x5AA1B87,
                Version2_2_3 => 0x5A454DB,
                Version2_3_0 => 0x57D2875,
                Version2_4_0 => 0x1F7F940,
                Version2_5_0 => 0x123846F,
                Version2_6_0 => 0x53C8D9,
                Version2_6_1 => 0x5BED8C4,
                _ => 0L
            };


            Hooks.AttackInfo = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x474B3A,
                Version1_2_1 or Version1_2_2 => 0x474BAA,
                Version1_2_3 => 0x474CCA,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x47595A,
                Version1_4_0 => 0x47818A,
                Version1_4_1 => 0x47809A,
                Version1_5_0 => 0x47841A,
                Version1_6_0 => 0x47947A,
                Version1_7_0 => 0x4795CA,
                Version1_8_0 or Version1_8_1 => 0x47AF5A,
                Version1_9_0 or Version1_9_1 => 0x47B09A,
                Version2_0_0 or Version2_0_1 => 0x47B23A,
                Version2_2_0 or Version2_2_3 => 0x47E10B,
                Version2_3_0 => 0x47E21B,
                Version2_4_0 or Version2_5_0 => 0x47E25B,
                Version2_6_0 or Version2_6_1 => 0x47E22B,
                _ => 0L
            };

            Hooks.WarpCoordWrite = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x657AFA,
                Version1_2_1 or Version1_2_2 => 0x657B6A,
                Version1_2_3 => 0x657C8A,
                Version1_3_0 or Version1_3_1 => 0x658F8A,
                Version1_3_2 => 0x658F6A,
                Version1_4_0 => 0x65BC3A,
                Version1_4_1 => 0x65BB4A,
                Version1_5_0 => 0x65C92A,
                Version1_6_0 => 0x65E24A,
                Version1_7_0 => 0x65F11A,
                Version1_8_0 or Version1_8_1 => 0x66C2AA,
                Version1_9_0 => 0x66D20A,
                Version1_9_1 => 0x66D26A,
                Version2_0_0 or Version2_0_1 => 0x66D4DA,
                Version2_2_0 or Version2_2_3 => 0x67A75A,
                Version2_3_0 => 0x67A8DA,
                Version2_4_0 or Version2_5_0 => 0x67A93A,
                Version2_6_0 or Version2_6_1 => 0x67AABA,
                _ => 0L
            };

            Hooks.WarpAngleWrite = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x657ADA,
                Version1_2_1 or Version1_2_2 => 0x657B4A,
                Version1_2_3 => 0x657C6A,
                Version1_3_0 or Version1_3_1 => 0x658F6A,
                Version1_3_2 => 0x658F4A,
                Version1_4_0 => 0x65BC1A,
                Version1_4_1 => 0x65BB2A,
                Version1_5_0 => 0x65C90A,
                Version1_6_0 => 0x65E22A,
                Version1_7_0 => 0x65F0FA,
                Version1_8_0 or Version1_8_1 => 0x66C28A,
                Version1_9_0 => 0x66D1EA,
                Version1_9_1 => 0x66D24A,
                Version2_0_0 or Version2_0_1 => 0x66D4BA,
                Version2_2_0 or Version2_2_3 => 0x67A73A,
                Version2_3_0 => 0x67A8BA,
                Version2_4_0 or Version2_5_0 => 0x67A91A,
                Version2_6_0 or Version2_6_1 => 0x67AA9A,
                _ => 0L
            };

            Hooks.NoTimePassOnDeath = moduleBase + Version switch
            {
                Version1_2_0 => 0x5DC567,
                Version1_2_1 or Version1_2_2 => 0x5DC5D7,
                Version1_2_3 => 0x5DC6F7,
                Version1_3_0 or Version1_3_1 => 0x5DD787,
                Version1_3_2 => 0x5DD767,
                Version1_4_0 => 0x5E0467,
                Version1_4_1 => 0x5E0377,
                Version1_5_0 => 0x5E0E57,
                Version1_6_0 => 0x5E25F7,
                Version1_7_0 => 0x5E3477,
                Version1_8_0 or Version1_8_1 => 0x5EFB77,
                Version1_9_0 => 0x5F0897,
                Version1_9_1 => 0x5F08F9,
                Version2_0_0 => 0x5F0B37,
                Version2_0_1 => 0x5F0B39,
                Version2_2_0 or Version2_2_3 => 0x5FC09F,
                Version2_3_0 => 0x5FC21F,
                Version2_4_0 or Version2_5_0 => 0x5FC27F,
                Version2_6_0 or Version2_6_1 => 0x5FC3FF,
                _ => 0
            };

            Hooks.LionCooldownHook = moduleBase.ToInt64() + Version switch
            {
                Version2_2_0 or Version2_2_3 => 0x4FEF7A,
                Version2_3_0 => 0x4FF0FA,
                Version2_4_0 or Version2_5_0 => 0x4FF13A,
                Version2_6_0 or Version2_6_1 => 0x4FF10A,
                _ => 0L
            };

            Hooks.SetActionRequested = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x3FF362,
                Version1_2_1 or Version1_2_2 => 0x3FF3D2,
                Version1_2_3 => 0x3FF4F2,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x3FFFE2,
                Version1_4_0 => 0x402532,
                Version1_4_1 => 0x402542,
                Version1_5_0 => 0x402912,
                Version1_6_0 => 0x403752,
                Version1_7_0 => 0x4037D2,
                Version1_8_0 or Version1_8_1 => 0x404F92,
                Version1_9_0 or Version1_9_1 => 0x4050D2,
                Version2_0_0 or Version2_0_1 => 0x4050C2,
                Version2_2_0 or Version2_2_3 => 0x407B82,
                Version2_3_0 => 0x407BA2,
                Version2_4_0 or Version2_5_0 => 0x407BE2,
                Version2_6_0 or Version2_6_1 => 0x407BB2,
                _ => 0L
            };

            Hooks.NoGrab = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x4402FB,
                Version1_2_1 or Version1_2_2 => 0x44036B,
                Version1_2_3 => 0x44048B,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x4410EB,
                Version1_4_0 => 0x44390B,
                Version1_4_1 => 0x44381B,
                Version1_5_0 => 0x443C5B,
                Version1_6_0 => 0x444CBB,
                Version1_7_0 => 0x444E0B,
                Version1_8_0 or Version1_8_1 => 0x44679B,
                Version1_9_0 or Version1_9_1 => 0x4468DB,
                Version2_0_0 or Version2_0_1 => 0x446A7B,
                Version2_2_0 or Version2_2_3 => 0x44986B,
                Version2_3_0 => 0x44997B,
                Version2_4_0 or Version2_5_0 => 0x4499BB,
                Version2_6_0 or Version2_6_1 => 0x44998B,
                _ => 0L
            };

            Hooks.LoadScreenMsgLookup = moduleBase.ToInt64() + Version switch
            {
                Version1_8_0 => 0xCDACFC,
                Version1_8_1 => 0xCDACDC,
                Version1_9_0 => 0xCDD91C,
                Version1_9_1 => 0xCDD97C,
                Version2_0_0 or Version2_0_1 => 0x446A7B,
                Version2_2_0 or Version2_2_3 => 0xD0FE9C,
                Version2_3_0 => 0xD1038C,
                Version2_4_0 or Version2_5_0 => 0xD1075C,
                Version2_6_0 => 0xD108DC,
                Version2_6_1 => 0xD1093C,
                _ => 0L
            };

            Hooks.LoadScreenMsgLookupEarlyPatches = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x737994,
                Version1_2_1 or Version1_2_2 => 0x737A04,
                Version1_2_3 => 0x737B24,
                Version1_3_0 or Version1_3_1 => 0x738E54,
                Version1_3_2 => 0x738E34,
                Version1_4_0 => 0x73CF04,
                Version1_4_1 => 0x73CE14,
                Version1_5_0 => 0x73E324,
                Version1_6_0 => 0x740174,
                _ => 0L
            };

            Hooks.LoadScreenMsgLookupMidPatches = moduleBase.ToInt64() + Version switch
            {
                Version1_7_0 => 0xC95393,
                _ => 0L
            };

            Hooks.TargetNoStagger = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x474CC5,
                Version1_2_1 or Version1_2_2 => 0x474D35,
                Version1_2_3 => 0x474E55,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x475AE5,
                Version1_4_0 => 0x478315,
                Version1_4_1 => 0x478225,
                Version1_5_0 => 0x4785A5,
                Version1_6_0 => 0x479605,
                Version1_7_0 => 0x479755,
                Version1_8_0 or Version1_8_1 => 0x47B0E5,
                Version1_9_0 or Version1_9_1 => 0x47B225,
                Version2_0_0 or Version2_0_1 => 0x47B3C5,
                Version2_2_0 or Version2_2_3 => 0x47E2A7,
                Version2_3_0 => 0x47E3B7,
                Version2_4_0 or Version2_5_0 => 0x47E3F7,
                Version2_6_0 or Version2_6_1 => 0x47E3C7,
                _ => 0L
            };

            Hooks.NoMapAcquiredPopup = moduleBase.ToInt64() + Version switch
            {
                Version1_2_0 => 0x9929D1,
                Version1_2_1 => 0x992A51,
                Version1_2_2 => 0x992AC1,
                Version1_2_3 => 0x992B41,
                Version1_3_0 => 0x997771,
                Version1_3_1 => 0x997781,
                Version1_3_2 => 0x997761,
                Version1_4_0 => 0x975CB1,
                Version1_4_1 => 0x975BC1,
                Version1_5_0 => 0x979D81,
                Version1_6_0 => 0x97BDD1,
                Version1_7_0 => 0x97D511,
                Version1_8_0 or Version1_8_1 => 0x9A37A1,
                Version1_9_0 => 0x9A57F1,
                Version1_9_1 => 0x9A5851,
                Version2_0_0 or Version2_0_1 => 0x9A5AE1,
                Version2_2_0 or Version2_2_3 => 0x9C6071,
                Version2_3_0 => 0x9C61F1,
                Version2_4_0 or Version2_5_0 => 0x9C6251,
                Version2_6_0 or Version2_6_1 => 0x9C63D1,
                _ => 0L
            };
            
            Hooks.NoHeal = moduleBase.ToInt64() + Version switch
{
            Version1_2_0 => 0x42E062,
            Version1_2_1 or Version1_2_2 => 0x42E0D2,
            Version1_2_3 => 0x42E1F2,
            Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x42EE02,
            Version1_4_0 => 0x431482,
            Version1_4_1 => 0x431492,
            Version1_5_0 => 0x4318C2,
            Version1_6_0 => 0x432702,
            Version1_7_0 => 0x432782,
            Version1_8_0 or Version1_8_1 => 0x4340E2,
            Version1_9_0 or Version1_9_1 => 0x434222,
            Version2_0_0 or Version2_0_1 => 0x4342C2,
            Version2_2_0 or Version2_2_3 => 0x437022,
            Version2_3_0 => 0x437042,
            Version2_4_0 or Version2_5_0 => 0x437082,
            Version2_6_0 or Version2_6_1 => 0x437052,
            _ => 0
            };

            // Patches

            Patches.NoLogo = moduleBase + Version switch
            {
                Version1_2_0 => 0xAAAD4A,
                Version1_2_1 => 0xAAADCA,
                Version1_2_2 => 0xAAAE3A,
                Version1_2_3 => 0xAAAF1A,
                Version1_3_0 => 0xAB021D,
                Version1_3_1 => 0xAB022D,
                Version1_3_2 => 0xAB020D,
                Version1_4_0 => 0xA8FB6D,
                Version1_4_1 => 0xA8FA7D,
                Version1_5_0 => 0xA9417D,
                Version1_6_0 => 0xA9807D,
                Version1_7_0 => 0xA9972D,
                Version1_8_0 or Version1_8_1 => 0xADB0FD,
                Version1_9_0 => 0xADDC8D,
                Version1_9_1 => 0xADDCED,
                Version2_0_0 or Version2_0_1 => 0xADDF7D,
                Version2_2_0 or Version2_2_3 => 0xB0BD7D,
                Version2_3_0 => 0xB0C0ED,
                Version2_4_0 or Version2_5_0 => 0xB0C26D,
                Version2_6_0 => 0xB0C3ED,
                Version2_6_1 => 0xB0C44D,
                _ => 0
            };


            Patches.NoRunesFromEnemies = moduleBase + Version switch
            {
                Version1_2_0 => 0x630CAF,
                Version1_2_1 or Version1_2_2 => 0x630D1F,
                Version1_2_3 => 0x630E3F,
                Version1_3_0 or Version1_3_1 => 0x631C9F,
                Version1_3_2 => 0x631C7F,
                Version1_4_0 => 0x634A2F,
                Version1_4_1 => 0x63493F,
                Version1_5_0 => 0x63547F,
                Version1_6_0 => 0x636C5F,
                Version1_7_0 => 0x637ADF,
                Version1_8_0 or Version1_8_1 => 0x64463F,
                Version1_9_0 => 0x6453BF,
                Version1_9_1 => 0x64541F,
                Version2_0_0 or Version2_0_1 => 0x64568F,
                Version2_2_0 or Version2_2_3 => 0x65178F,
                Version2_3_0 => 0x65190F,
                Version2_4_0 or Version2_5_0 => 0x65196F,
                Version2_6_0 or Version2_6_1 => 0x651AEF,
                _ => 0
            };

            Patches.NoRuneArcLoss = moduleBase + Version switch
            {
                Version1_2_0 => 0x258347,
                Version1_2_1 or Version1_2_2 => 0x2583B7,
                Version1_2_3 => 0x2584D7,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x258C47,
                Version1_4_0 or Version1_4_1 => 0x25AC27,
                Version1_5_0 => 0x25AD97,
                Version1_6_0 => 0x25BC17,
                Version1_7_0 => 0x25BDA7,
                Version1_8_0 or Version1_8_1 => 0x25C757,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x25C887,
                Version2_2_0 or Version2_2_3 or Version2_3_0 or Version2_4_0 or Version2_5_0
                    or Version2_6_0 or Version2_6_1 => 0x25E297,
                _ => 0
            };

            Patches.NoRuneLossOnDeath = moduleBase + Version switch
            {
                Version1_2_0 => 0x5DC385,
                Version1_2_1 or Version1_2_2 => 0x5DC3F5,
                Version1_2_3 => 0x5DC515,
                Version1_3_0 or Version1_3_1 => 0x5DD5A5,
                Version1_3_2 => 0x5DD585,
                Version1_4_0 => 0x5E0285,
                Version1_4_1 => 0x5E0195,
                Version1_5_0 => 0x5E0C75,
                Version1_6_0 => 0x5E2415,
                Version1_7_0 => 0x5E3295,
                Version1_8_0 or Version1_8_1 => 0x5EF995,
                Version1_9_0 => 0x5F06B5,
                Version1_9_1 => 0x5F0715,
                Version2_0_0 or Version2_0_1 => 0x5F0955,
                Version2_2_0 or Version2_2_3 => 0x5FBEB5,
                Version2_3_0 => 0x5FC035,
                Version2_4_0 or Version2_5_0 => 0x5FC095,
                Version2_6_0 or Version2_6_1 => 0x5FC215,
                _ => 0
            };

            Patches.CanFastTravel = moduleBase + Version switch
            {
                Version1_2_0 => 0x798300,
                Version1_2_1 or Version1_2_2 => 0x798370,
                Version1_2_3 => 0x798440,
                Version1_3_0 or Version1_3_1 => 0x79A320,
                Version1_3_2 => 0x79A300,
                Version1_4_0 => 0x79DD60,
                Version1_4_1 => 0x79DC70,
                Version1_5_0 => 0x79FAA0,
                Version1_6_0 => 0x7A1AA0,
                Version1_7_0 => 0x7A31E0,
                Version1_8_0 or Version1_8_1 => 0x7B2110,
                Version1_9_0 => 0x7B31B0,
                Version1_9_1 => 0x7B3210,
                Version2_0_0 or Version2_0_1 => 0x7B34A0,
                Version2_2_0 or Version2_2_3 => 0x7C4A90,
                Version2_3_0 => 0x7C4C80,
                Version2_4_0 or Version2_5_0 => 0x7C4CE0,
                Version2_6_0 or Version2_6_1 => 0x7C4E60,
                _ => 0
            };


            Patches.OpenMap = moduleBase + Version switch
            {
                Version1_2_0 => 0x7C4CDA,
                Version1_2_1 or Version1_2_2 => 0x7C4D4A,
                Version1_2_3 => 0x7C4E1A,
                Version1_3_0 or Version1_3_1 => 0x7C6E1A,
                Version1_3_2 => 0x7C6DFA,
                Version1_4_0 => 0x7C777A,
                Version1_4_1 => 0x7C768A,
                Version1_5_0 => 0x7C94DA,
                Version1_6_0 => 0x7CB4DA,
                Version1_7_0 => 0x7CCC1A,
                Version1_8_0 or Version1_8_1 => 0x7DBEBA,
                Version1_9_0 => 0x7DCFBA,
                Version1_9_1 => 0x7DD01A,
                Version2_0_0 or Version2_0_1 => 0x7DD2AA,
                Version2_2_0 or Version2_2_3 => 0x7EE97A,
                Version2_3_0 => 0x7EEB6A,
                Version2_4_0 or Version2_5_0 => 0x7EEBCA,
                Version2_6_0 or Version2_6_1 => 0x7EED4A,
                _ => 0
            };

            Patches.CloseMap = moduleBase + Version switch
            {
                Version1_2_0 => 0x990BA5,
                Version1_2_1 or Version1_2_2 => 0x990C25,
                Version1_2_3 => 0x990D15,
                Version1_3_0 => 0x99593A,
                Version1_3_1 => 0x99594A,
                Version1_3_2 => 0x99592A,
                Version1_4_0 => 0x973A0A,
                Version1_4_1 => 0x97391A,
                Version1_5_0 => 0x977ADA,
                Version1_6_0 => 0x979B2A,
                Version1_7_0 => 0x97B26A,
                Version1_8_0 or Version1_8_1 => 0x9A14FA,
                Version1_9_0 => 0x9A354A,
                Version1_9_1 => 0x9A35AA,
                Version2_0_0 or Version2_0_1 => 0x9A383A,
                Version2_2_0 or Version2_2_3 => 0x9C35AE,
                Version2_3_0 => 0x9C374E,
                Version2_4_0 or Version2_5_0 => 0x9C37AE,
                Version2_6_0 or Version2_6_1 => 0x9C392E,
                _ => 0
            };

            Patches.EnableFreeCam = moduleBase + Version switch
            {
                Version1_2_0 => 0xDC0D60,
                Version1_2_1 => 0xDC0DB0,
                Version1_2_2 => 0xDC11C0,
                Version1_2_3 => 0xDC1B90,
                Version1_3_0 => 0xDC9F10,
                Version1_3_1 => 0xDC9E10,
                Version1_3_2 => 0xDC9DF0,
                Version1_4_0 => 0xDAA990,
                Version1_4_1 => 0xDAA8A0,
                Version1_5_0 => 0xDB2680,
                Version1_6_0 => 0xDB8A00,
                Version1_7_0 => 0xDBBFF0,
                Version1_8_0 => 0xE028C0,
                Version1_8_1 => 0xE028A0,
                Version1_9_0 => 0xE055D0,
                Version1_9_1 => 0xE056E0,
                Version2_0_0 => 0xE05990,
                Version2_0_1 => 0xE05A70,
                Version2_2_0 => 0xE42EC0,
                Version2_2_3 => 0xE42EE0,
                Version2_3_0 => 0xE433D0,
                Version2_4_0 or Version2_5_0 => 0xE43330,
                Version2_6_0 => 0xE43300,
                Version2_6_1 => 0xE43360,
                _ => 0
            };

            Patches.CanDrawEvents1 = moduleBase + Version switch
            {
                Version1_2_0 => 0xDD0810,
                Version1_2_1 => 0xDD0860,
                Version1_2_2 => 0xDD0C70,
                Version1_2_3 => 0xDD1640,
                Version1_3_0 => 0xDD99C0,
                Version1_3_1 => 0xDD98C0,
                Version1_3_2 => 0xDD98A0,
                Version1_4_0 => 0xDBA4B0,
                Version1_4_1 => 0xDBA3C0,
                Version1_5_0 => 0xDC21A0,
                Version1_6_0 => 0xDC8670,
                Version1_7_0 => 0xDCBDB0,
                Version1_8_0 => 0xE127E0,
                Version1_8_1 => 0xE127C0,
                Version1_9_0 => 0xE154F0,
                Version1_9_1 => 0xE15600,
                Version2_0_0 => 0xE158B0,
                Version2_0_1 => 0xE15990,
                Version2_2_0 => 0xE53190,
                Version2_2_3 => 0xE531B0,
                Version2_3_0 => 0xE536A0,
                Version2_4_0 or Version2_5_0 => 0xE53600,
                Version2_6_0 => 0xE535D0,
                Version2_6_1 => 0xE53630,
                _ => 0
            };

            Patches.CanDrawEvents2 = moduleBase + Version switch
            {
                Version1_2_0 => 0xDD07F0,
                Version1_2_1 => 0xDD0840,
                Version1_2_2 => 0xDD0C50,
                Version1_2_3 => 0xDD1620,
                Version1_3_0 => 0xDD99A0,
                Version1_3_1 => 0xDD98A0,
                Version1_3_2 => 0xDD9880,
                Version1_4_0 => 0xDBA490,
                Version1_4_1 => 0xDBA3A0,
                Version1_5_0 => 0xDC2180,
                Version1_6_0 => 0xDC8650,
                Version1_7_0 => 0xDCBD90,
                Version1_8_0 => 0xE127C0,
                Version1_8_1 => 0xE127A0,
                Version1_9_0 => 0xE154D0,
                Version1_9_1 => 0xE155E0,
                Version2_0_0 => 0xE15890,
                Version2_0_1 => 0xE15970,
                Version2_2_0 => 0xE53170,
                Version2_2_3 => 0xE53190,
                Version2_3_0 => 0xE53680,
                Version2_4_0 or Version2_5_0 => 0xE535E0,
                Version2_6_0 => 0xE535B0,
                Version2_6_1 => 0xE53610,
                _ => 0
            };

            Patches.DebugFont = moduleBase + Version switch
            {
                Version1_2_0 => 0x25DF460,
                Version1_2_1 => 0x25DF4B0,
                Version1_2_2 => 0x25DF950,
                Version1_2_3 => 0x25E1760,
                Version1_3_0 => 0x25EAFD0,
                Version1_3_1 => 0x25EAD70,
                Version1_3_2 => 0x25EAD50,
                Version1_4_0 => 0x25D2A10,
                Version1_4_1 => 0x25D2920,
                Version1_5_0 => 0x25E0DC0,
                Version1_6_0 => 0x25EAF20,
                Version1_7_0 => 0x25F84F0,
                Version1_8_0 => 0x2644E30,
                Version1_8_1 => 0x2644E10,
                Version1_9_0 => 0x2647AF0,
                Version1_9_1 => 0x2647C00,
                Version2_0_0 => 0x2647EB0,
                Version2_0_1 => 0x2647F90,
                Version2_2_0 => 0x268CA70,
                Version2_2_3 => 0x268CA90,
                Version2_3_0 => 0x268D330,
                Version2_4_0 or Version2_5_0 => 0x268D360,
                Version2_6_0 => 0x268D330,
                Version2_6_1 => 0x268D390,
                _ => 0
            };


            Patches.PlayerSound = moduleBase + Version switch
            {
                Version1_2_0 => 0x3385F6,
                Version1_2_1 or Version1_2_2 => 0x338666,
                Version1_2_3 => 0x338786,
                Version1_3_0 or Version1_3_1 or Version1_3_2 => 0x338E86,
                Version1_4_0 or Version1_4_1 => 0x33AF76,
                Version1_5_0 => 0x33B146,
                Version1_6_0 => 0x33BFD6,
                Version1_7_0 => 0x33C006,
                Version1_8_0 or Version1_8_1 => 0x33CC46,
                Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => 0x33CD76,
                Version2_2_0 or Version2_2_3 or Version2_3_0 or Version2_4_0 or Version2_5_0 => 0x33E5C6,
                Version2_6_0 or Version2_6_1 => 0x33E596,
                _ => 0
            };

            Patches.IsTorrentDisabledInUnderworld = moduleBase + Version switch
            {
                Version1_2_0 => 0xC81F8A,
                Version1_2_1 => 0xC8200A,
                Version1_2_2 => 0xC8216A,
                Version1_2_3 => 0xC8225A,
                Version1_3_0 => 0xC891FA,
                Version1_3_1 => 0xC8920A,
                Version1_3_2 => 0xC891EA,
                Version1_4_0 => 0xC69A7A,
                Version1_4_1 => 0xC6998A,
                Version1_5_0 => 0xC6E77A,
                Version1_6_0 => 0xC730EA,
                Version1_7_0 => 0xC74DFA,
                Version1_8_0 => 0xCBA2FA,
                Version1_8_1 => 0xCBA2DA,
                Version1_9_0 => 0xCBCF1A,
                Version1_9_1 => 0xCBCF7A,
                Version2_0_0 or Version2_0_1 => 0xCBD20A,
                Version2_2_0 or Version2_2_3 => 0xCEF0EA,
                Version2_3_0 => 0xCEF5DA,
                Version2_4_0 or Version2_5_0 => 0xCEF9AA,
                Version2_6_0 => 0xCEFB2A,
                Version2_6_1 => 0xCEFB8A,
                _ => 0
            };

            Patches.IsWhistleDisabled = moduleBase + Version switch
            {
                Version1_2_0 => 0x6DFF7F,
                Version1_2_1 or Version1_2_2 => 0x6DFFEF,
                Version1_2_3 => 0x6E010F,
                Version1_3_0 or Version1_3_1 => 0x6E150F,
                Version1_3_2 => 0x6E14EF,
                Version1_4_0 => 0x6E51AF,
                Version1_4_1 => 0x6E50BF,
                Version1_5_0 => 0x6E5F6F,
                Version1_6_0 => 0xC730EA,
                Version1_7_0 => 0x6E92DF,
                Version1_8_0 or Version1_8_1 => 0x6F6ABF,
                Version1_9_0 => 0x6F7B5F,
                Version1_9_1 => 0x6F7BBF,
                Version2_0_0 or Version2_0_1 => 0x6F7E4F,
                Version2_2_0 or Version2_2_3 => 0x705E9F,
                Version2_3_0 => 0x70608F,
                Version2_4_0 or Version2_5_0 => 0x7060EF,
                Version2_6_0 or Version2_6_1 => 0x70626F,
                _ => 0
            };

            Patches.IsWorldPaused = moduleBase + Version switch
            {
                Version1_2_0 => 0xA98175,
                Version1_2_1 => 0xA981F5,
                Version1_2_2 => 0xA98265,
                Version1_2_3 => 0xA98345,
                Version1_3_0 => 0xA9D555,
                Version1_3_1 => 0xA9D565,
                Version1_3_2 => 0xA9D545,
                Version1_4_0 => 0xA7CDC5,
                Version1_4_1 => 0xA7CCD5,
                Version1_5_0 => 0xA81335,
                Version1_6_0 => 0xA851E5,
                Version1_7_0 => 0xA868F5,
                Version1_8_0 or Version1_8_1 => 0xAC78E5,
                Version1_9_0 => 0xACA475,
                Version1_9_1 => 0xACA4D5,
                Version2_0_0 or Version2_0_1 => 0xACA765,
                Version2_2_0 or Version2_2_3 => 0xAF7E05,
                Version2_3_0 => 0xAF8175,
                Version2_4_0 or Version2_5_0 => 0xAF8335,
                Version2_6_0 => 0xAF84B5,
                Version2_6_1 => 0xAF8515,
                _ => 0
            };

            Patches.GetItemChance = moduleBase + Version switch
            {
                Version1_2_0 => 0xCC9E0E,
                Version1_2_1 => 0xCC9E8E,
                Version1_2_2 => 0xCC9FEE,
                Version1_2_3 => 0xCCA0DE,
                Version1_3_0 => 0xCD183E,
                Version1_3_1 => 0xCD184E,
                Version1_3_2 => 0xCD182E,
                Version1_4_0 => 0xCB18EE,
                Version1_4_1 => 0xCB17FE,
                Version1_5_0 => 0xCB6ABE,
                Version1_6_0 => 0xCBC91E,
                Version1_7_0 => 0xCBEABE,
                Version1_8_0 => 0xD04AEE,
                Version1_8_1 => 0xD04ACE,
                Version1_9_0 => 0xD0770E,
                Version1_9_1 => 0xD0776E,
                Version2_0_0 or Version2_0_1 => 0xD079DE,
                Version2_2_0 => 0xD3B48E,
                Version2_2_3 => 0xD3B4AE,
                Version2_3_0 => 0xD3B99E,
                Version2_4_0 or Version2_5_0 => 0xD3BD6E,
                Version2_6_0 => 0xD3BEEE,
                Version2_6_1 => 0xD3BF4E,
                _ => 0
            };

            Patches.GetShopEvent = moduleBase + Version switch
            {
                Version1_2_0 => 0xCD7730,
                Version1_2_1 => 0xCD77B0,
                Version1_2_2 => 0xCD7910,
                Version1_2_3 => 0xCD7A00,
                Version1_3_0 => 0xCDF130,
                Version1_3_1 => 0xCDF140,
                Version1_3_2 => 0xCDF120,
                Version1_4_0 => 0xCBF1E0,
                Version1_4_1 => 0xCBF0F0,
                Version1_5_0 => 0xCC43B0,
                Version1_6_0 => 0xCCA3A0,
                Version1_7_0 => 0xCCC550,
                Version1_8_0 => 0xD12840,
                Version1_8_1 => 0xD12820,
                Version1_9_0 => 0xD15460,
                Version1_9_1 => 0xD15540,
                Version2_0_0 or Version2_0_1 => 0xD157B0,
                Version2_2_0 => 0xD49230,
                Version2_2_3 => 0xD49250,
                Version2_3_0 => 0xD49740,
                Version2_4_0 or Version2_5_0 => 0xD49B10,
                Version2_6_0 => 0xD49C90,
                Version2_6_1 => 0xD49CF0,
                _ => 0
            };


#if DEBUG
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
            Console.WriteLine($@"Hooks.LoadScreenMsgLookupEarlyPatches: 0x{Hooks.LoadScreenMsgLookupEarlyPatches:X}");
            Console.WriteLine($@"Hooks.TargetNoStagger: 0x{Hooks.TargetNoStagger:X}");
            Console.WriteLine($@"Hooks.NoMapAcquiredPopup: 0x{Hooks.NoMapAcquiredPopup:X}");
            Console.WriteLine($@"Hooks.NoHeal: 0x{Hooks.NoHeal:X}");

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
#endif
        }
    }
}