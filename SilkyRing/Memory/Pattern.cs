namespace SilkyRing.Memory
{
    public class Pattern
    {
        public byte[] Bytes { get; }
        public string Mask { get; }
        public int InstructionOffset { get; }
        public AddressingMode AddressingMode { get; }
        public int OffsetLocation { get; }
        public int InstructionLength { get; }

        public Pattern(byte[] bytes, string mask, int instructionOffset, AddressingMode addressingMode,
            int offsetLocation = 0, int instructionLength = 0)
        {
            Bytes = bytes;
            Mask = mask;
            InstructionOffset = instructionOffset;
            AddressingMode = addressingMode;
            OffsetLocation = offsetLocation;
            InstructionLength = instructionLength;
        }

        public static readonly Pattern WorldChrMan = new Pattern(
            new byte[] { 0x48, 0x39, 0x2D, 0x42 },
            "xxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern FieldArea = new Pattern(
            new byte[] { 0x48, 0x8B, 0x05, 0xEC, 0xEB },
            "xxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern LuaEventMan = new Pattern(
            new byte[] { 0x48, 0x83, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x1D, 0x01, 0x00, 0x00 },
            "xxx????xxxxxxx",
            0,
            AddressingMode.Relative,
            3,
            8
        );

        public static readonly Pattern VirtualMemFlag = new Pattern(
            new byte[] { 0x48, 0x8B, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xFF, 0x74, 0x53 },
            "xxx????xxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern DamageManager = new Pattern(
            new byte[] { 0x48, 0x3B, 0xDF, 0x0F, 0x84, 0x87, 0x01 },
            "xxxxxxx",
            0x27,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern MenuMan = new Pattern(
            new byte[] { 0x0F, 0x45, 0xF8, 0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC9, 0x75, 0x2E },
            "xxxxxx????xxxxx",
            3,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern TargetView = new Pattern(
            new byte[] { 0x75, 0x0D, 0x40, 0x38, 0x35, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x41 },
            "xxxxx????xxx",
            0x2,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern GameMan = new Pattern(
            new byte[] { 0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC0, 0x74, 0x07, 0x8B, 0x80, 0x84 },
            "xxx????xxxxxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );


        //Hooks
        public static readonly Pattern UpdateCoords = new Pattern(
            new byte[] { 0x0F, 0x11, 0x43, 0x70, 0xC7, 0x43 },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern InAirTimer = new Pattern(
            new byte[] { 0xF3, 0x0F, 0x11, 0x43, 0x18, 0xC6 },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoClipKb = new Pattern(
            new byte[] { 0xF6, 0x84, 0x08, 0xE8 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoClipTriggers = new Pattern(
            new byte[] { 0x0F, 0xB6, 0x44, 0x24, 0x36, 0x0F },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern CreateGoalObj = new Pattern(
            new byte[] { 0x48, 0x8B, 0xF1, 0x85, 0xD2, 0x0F, 0x88, 0x78 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern HasSpEffect = new Pattern(
            new byte[] { 0x39, 0x51, 0x08, 0x74, 0x0C, 0x48, 0x8B },
            "xxxxxxx",
            -0x10,
            AddressingMode.Absolute
        );

        public static readonly Pattern BlueTargetViewHook = new Pattern(
            new byte[] { 0x0F, 0x84, 0x41, 0x01, 0x00, 0x00, 0x48, 0x8D, 0x54 },
            "xxxxxxxxx",
            0x6,
            AddressingMode.Absolute
        );

        public static readonly Pattern LockedTargetPtr = new Pattern(
            new byte[] { 0x74, 0x17, 0x48, 0x8B, 0x8F, 0x88 },
            "xxxxxx",
            2,
            AddressingMode.Absolute
        );

        public static readonly Pattern InfinitePoise = new Pattern(
            new byte[] { 0x80, 0xBF, 0x5F, 0x02 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );



        //Funcs


        public static readonly Pattern GraceWarp = new Pattern(
            new byte[] { 0xC7, 0x44, 0x24, 0x30, 0x10, 0x27, 0x00, 0x00, 0x48, 0x8B, 0xCF },
            "xxxxxxxxxxx",
            -0x14,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetEvent = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x80, 0x7C, 0x24, 0x60, 0x00, 0x74, 0x5C },
            "x????xxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern SetSpEffect = new Pattern(
            new byte[] { 0x85, 0xD2, 0x78, 0x09, 0x48, 0x8B },
            "xxxxxx",
            0x8,
            AddressingMode.Relative,
            1,
            5
        );


        //Patches

        public static readonly Pattern DungeonWarp = new Pattern(
            new byte[] { 0x74, 0x3A, 0x8B, 0x81, 0xA0 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );
    }


    public enum AddressingMode
    {
        Absolute,
        Relative,
    }
}