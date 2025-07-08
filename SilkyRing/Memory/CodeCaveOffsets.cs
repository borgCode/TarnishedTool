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
    }
}