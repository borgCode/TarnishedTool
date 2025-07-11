using System;

namespace SilkyRing.Memory
{
    public static class CodeCaveOffsets
    {
        public static IntPtr Base;

        public enum NoClip
        {
            InAirTimer = 0x0,
            ZDirection = 0x50,
            Kb = 0x60,
            Triggers = 0xB0,
            UpdateCoords = 0x100
        }

        public const int Rykard = 0x300;

        public enum TargetView
        {
            MaxDist = 0x350,
            BlueTargetView = 0x360
        }

        public enum LockedTarget
        {
            SavedPtr = 0x560,
            Code = 0x570,
        }

        public const int InfinitePoise = 0x590;
    }
}