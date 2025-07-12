using System;
using System.Diagnostics.CodeAnalysis;

namespace SilkyRing.Memory
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class Offsets
    {
        public static void Initialize() // Patch
        {
            WorldChrMan.Offsets.Intialize();
            MenuMan.Offsets.Initialize();
            TargetView.Offsets.Initialize();
            GameMan.Offsets.Initialize();
            DamageManager.Offsets.Initialize();
            WorldHitMan.Offsets.Initialize();
            WorldChrManDbg.Offsets.Initialize();
        }


        public static class WorldChrMan
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Intialize()
                {
                    PlayerInsPtr = 0x1E508;

                    ChrIns.Initialize();
                }

                public static int PlayerInsPtr { get; private set; }

                public static class ChrIns
                {
                    public static void Initialize()
                    {
                        ChrCtrlPtr = 0x58;
                        CsSpecialEffectPtr = 0x178;
                        ModulesPtr = 0x190;
                        ComManipulatorPtr = 0x580;

                        ChrCtrl.Initialize();
                        Modules.Initialize();
                        ComManipulator.Initialize();
                    }

                    public static int ChrCtrlPtr { get; private set; }
                    public static int CsSpecialEffectPtr { get; private set; }
                    public static int ModulesPtr { get; private set; }
                    public static int ComManipulatorPtr { get; private set; }

                    
                    public static class ChrCtrl
                    {
                        public static void Initialize()
                        {
                            UnkPtr = 0xC8;
                            
                            Unk.Initialize();
                        }

                        public static int UnkPtr { get; private set; }
                        
                        public static class Unk
                        {
                            public static void Initialize()
                            {
                                Flags = 0x24;
                            }
                            public enum Flag
                            {
                                DisableAi = 1 << 0,
                            }
                            

                            public static int Flags { get; private set; }
                        
                        
                        }
                    }

                    public static class Modules
                    {
                        public static void Initialize()
                        {
                            ChrDataPtr = 0x0;
                            ChrSuperArmorPtr = 0x40;
                            ChrPhysicsPtr = 0x68;

                            ChrData.Initialize();
                            ChrPhysics.Initialize();
                        }

                        public static int ChrDataPtr { get; private set; }
                        public static int ChrSuperArmorPtr { get; private set; }
                        public static int ChrPhysicsPtr { get; private set; }

                        public static class ChrData
                        {
                            public static void Initialize()
                            {
                                Health = 0x138;
                                MaxHealth = 0x13C;
                                Flags = 0x19B;
                            }

                            public enum Flag
                            {
                                NoDeath = 1 << 0,
                                NoDamage = 1 << 1,
                            }

                            public static int Health { get; private set; }
                            public static int MaxHealth { get; private set; }
                            public static int Flags { get; private set; }
                        }
                        
                        public static class ChrSuperArmor
                        {
                            public static void Initialize()
                            {
                                CurrentPoise = 0x10;
                                MaxPoise = 0x14;
                                PoiseTimer = 0x1C;
                            }

                            public static int CurrentPoise { get; private set; }
                            public static int MaxPoise { get; private set; }
                            public static int PoiseTimer { get; private set; }
                        }

                        public static class ChrPhysics
                        {
                            public static void Initialize()
                            {
                                Coords = 0x70;
                            }

                            public static int Coords { get; private set; }
                        }
                    }
                    
                    public static class ComManipulator
                    {
                        public static void Initialize()
                        {
                            AiPtr = 0xC0;
                            
                            Ai.Initialize();
                        }

                        public static int AiPtr { get; private set; }
                        
                        public static class Ai
                        {
                            public static void Initialize()
                            {
                                TargetingSystemPtr = 0xC480;
                                ForceAct = 0xE9C1;
                                LastAct = 0xE9C2;
                                
                                TargetingSystem.Initialize();
                            }

                            public static int TargetingSystemPtr { get; private set; }
                            public static int ForceAct { get; private set; }
                            public static int LastAct { get; private set; }
                            
                            
                            
                            public static class TargetingSystem
                            {
                                public static void Initialize()
                                {
                                    Flags = 0xC8;
                         
                                }

                                public enum Flag
                                {
                                    //Bit pos
                                    BlueTargetView = 11,
                                    YellowTargetView = 12,
                                    WhiteLineToPlayer = 13
                                }
                                public static int Flags { get; private set; }
                       
                            }
                        }
                    }
                }
            }
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

            public static class Offsets
            {
                public static void Initialize()
                {
                    HitboxView = 0xA0;
                    HitboxView2 = 0xA1;
                }

                public static int HitboxView { get; private set; }
                public static int HitboxView2 { get; private set; }
            }
        }
        
        public static class WorldHitMan
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Initialize()
                {
                    LowHit = 0xC;
                    HighHit = 0xD;
                    Ragdoll = 0xE;
                    Mode = 0x14;
                }

                public static int LowHit { get; private set; }
                public static int HighHit { get; private set; }
                public static int Ragdoll { get; private set; }
                public static int Mode { get; private set; }
            }
        }

        public static class MenuMan
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Initialize()
                {
                    IsLoaded = 0x94;
                }

                public static int IsLoaded { get; private set; }
            }
        }
        
        public static class TargetView
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Initialize()
                {
                    Blue = 0x0;
                    Yellow = 0x1;
                }

                public static int Blue { get; private set; }
                public static int Yellow { get; private set; }
            }
        }
        
        public static class GameMan
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Initialize()
                {
                    ForceSave = 0xb72;
                }

                public static int ForceSave { get; private set; }
            }
        }
        
        public static class WorldChrManDbg
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Initialize()
                {
                    AllChrsSpheres = 0x9;
                    OneShot = 0xA;
                    InfiniteGoods = 0xB;
                    InfiniteStam = 0xC;
                    InfiniteFp = 0xD;
                    InfiniteArrows = 0xE;
                    Hidden = 0x10;
                    Silent = 0x11;
                    AllNoDeath = 0x12;
                    AllNoDamage = 0x13;
                    AllDisableAi = 0x17;
                    PoiseBarsFlag = 0x69;
              
                }

                public static int OneShot { get; private set; }
                public static int InfiniteGoods { get; private set; }
                public static int InfiniteStam { get; private set; }
                public static int InfiniteFp { get; private set; }
                public static int InfiniteArrows { get; private set; }
                public static int Hidden { get; private set; }
                public static int Silent { get; private set; }
                public static int AllNoDeath { get; private set; }
                public static int AllNoDamage { get; private set; }
                public static int AllDisableAi { get; private set; }
                public static int AllChrsSpheres { get; private set; }
                public static int PoiseBarsFlag { get; private set; }
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
        }

        public static class Funcs
        {
            public static long GraceWarp;
            public static long SetEvent;
            public static long SetSpEffect;
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