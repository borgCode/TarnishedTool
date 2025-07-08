using System;

namespace SilkyRing.Memory
{
    public static class Offsets
    {
        public static void Initialize() // Patch
        {
            WorldChrMan.Offsets.Intialize();
            CsTargetingSystem.Initialize();
            // GameManagerImp.Offsets.Initialize(edition);
            // GameManagerImp.CharacterManagerOffsets.Initialize(edition);
            // GameManagerImp.EventManagerOffsets.Initialize(edition);
            // GameManagerImp.GameDataManagerOffsets.Initialize(edition);
            // GameManagerImp.GameDataManagerOffsets.Inventory.Initialize(edition);
            // GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList.Initialize(edition);
            // GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.Initialize(edition);
            // GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.Initialize(edition);
            // GameManagerImp.SaveLoadSystem.Initialize(edition);
            // GameManagerImp.ChrCtrlOffsets.Initialize(edition);
            // GameManagerImp.ChrCtrlOffsets.Operator.Initialize(edition);
            // GameManagerImp.ChrCtrlOffsets.ChrAiMan.Initialize(edition);
            // GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrl.Initialize(edition);
            // GameManagerImp.ChrCtrlOffsets.Stats.Initialize(edition);
            // GameManagerImp.PxWorld.Initialize(edition);
        }
        
        
        public static class WorldChrMan
        {
            public static IntPtr Base;
            
            public static class Offsets
            {
                public static void Intialize()
                {
                    PlayerInsPtr = 0x1E508;

                    PlayerIns.Initialize();
                }

                public static int PlayerInsPtr { get; private set; }
                
                public static class PlayerIns
                {
                    public static void Initialize()
                    {
                        PlayerCtrlPtr = 0x58;
                        CsSpecialEffectPtr = 0x178;
                        ModulesPtr = 0x190;

                        Modules.Initialize();
                    }

                    public static int PlayerCtrlPtr { get; private set; }
                    public static int CsSpecialEffectPtr { get; private set; }
                    public static int ModulesPtr { get; private set; }

                    public static class Modules
                    {
                        public static void Initialize()
                        {
                            ChrPhysicsPtr = 0x68;

                            ChrPhysics.Initialize();
                        }
                        
                        public static int ChrPhysicsPtr { get; private set; }

                        public static class ChrPhysics
                        {
                            public static void Initialize()
                            {
                                Coords = 0x70;
                            }
                            
                            public static int Coords { get; private set; }
                        }
                    }
                }
            }
        }

        public static class FieldArea
        {
            public static IntPtr Base;
        }

        public static class CsTargetingSystem
        {
            //Com manip +0xC0 --> Ptr1 to targeting
            // ptr1 + 0xC480 --> CSTargetingSystem
            public static void Initialize()
            {
                TargetingFlags = 0xC8;
                // 1 << 11 for blue target view
                // 1 << 12 for yellow 
                // 1 << 13 white line to entity from player
            }

            public static int TargetingFlags { get; set; }
        }
        
        public static class Hooks
        {
            public static long UpdateCoords;
            public static long InAirTimer;
            public static long NoClipKb;
            public static long NoClipTriggers;
        }
    }
}