using System;

namespace TarnishedTool.Memory
{
    public static class CodeCaveOffsets
    {
        public static IntPtr Base;

        public const int SaveCurrentTime = 0x0;

        public enum NoClip
        {
            ZDirection = 0x50,
            SpeedScale = 0x54,
            Kb = 0x60,
            Triggers = 0xB0,
            UpdateCoords = 0x100
        }

        
        public enum TargetView
        {
            MaxDist = 0x350,
            BlueTargetView = 0x360
        }

        public const int TargetPtr = 0x560;
        public const int SaveTargetPtrCode = 0x570;
        
        public const int DisableAllExceptTarget = 0x600;

        public const int ActArray = 0x650;
        public const int CurrentIdx = 0x6A0;
        public const int ShouldRun = 0x6A4;
        public const int ForceActSequence = 0x6B0;

        public const int TargetNoStagger = 0x750;

        public const int AttackInfoProcessedFlags = 0x7A0;
        public const int AttackInfoId = 0x7B0;
        public const int AttackInfoWriteIndex = 0x7B4;
        public const int AttackInfoStart = 0x7C0; // 16 structs
        public const int AttackInfoCode = 0xC40;

        public const int WarpCoords = 0xE40;
        public const int Angle = 0xE50;
        public const int WarpCode = 0xE60;
        public const int AngleCode = 0xE80;

        public const int EzStateTalkParams = 0x1000;
        public const int EzStateTalkCode = 0x1040;
        
        public const int GetEventResult = 0x1240;

        public const int ShouldCheckQuantity = 0x1244;
        public const int MaxQuantity = 0x1248;
        public const int ItemSpawnStruct = 0x1250;
        public const int ItemSpawnCode = 0x1350;

        public const int EmevdArgs = 0x1600;
        public const int Rykard = 0x1700;

        public const int InAirTimer = 0x1800;

        public const int LookedUpChrIns = 0x1900;
        public const int LionCooldownHook = 0x1A00;

        public const int NoClipJmpHook = 0x1B00;

        public const int InfinitePoise = 0x1b30; // 0x6F

        public const int SavedNpcTalk = 0x1C00;
        public const int EnvParams = 0x1C70;
        public const int EnvQueryResult = 0x1CF0;

        public const int NoAcquiredMapPopup = 0x1D50; // 0x35

        public const int NoGrab = 0x1D90; //0x28

        public const int LoadScreenForce = 0x1E00; //0xE

        public const int EventLogWriteIndex = 0x1E20;
        public const int EventLogCode = 0x1E40; //0x39
        public const int EventLogBuffer = 0x1E80; //0x1000
        
        public const int NoHeal = 0x2F00; //0x1B

        public enum LocalToMap
        {
            Output = 0x2F40,
            Input = 0x2F50,
            BlockId = 0x2F60,
            Code = 0x2F70, // 0x22
        }
        
        public const int PlayerLockHp = 0x3000;
        



    }
}