namespace SilkyRing.Memory
{
    public class Pattern(
        byte[] bytes,
        string mask,
        int instructionOffset,
        AddressingMode addressingMode,
        int offsetLocation = 0,
        int instructionLength = 0)
    {
        public byte[] Bytes { get; } = bytes;
        public string Mask { get; } = mask;
        public int InstructionOffset { get; } = instructionOffset;
        public AddressingMode AddressingMode { get; } = addressingMode;
        public int OffsetLocation { get; } = offsetLocation;
        public int InstructionLength { get; } = instructionLength;

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

        public static readonly Pattern WorldHitMan = new Pattern(
            new byte[] { 0x48, 0x8B, 0x2D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x63, 0xD8 },
            "xxx????xxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern WorldChrManDbg = new Pattern(
            [0x83, 0xCF, 0x10, 0x89, 0x7C, 0x24, 0x20, 0x48, 0x83],
            "xxxxxxxxx",
            0x7,
            AddressingMode.Relative,
            3,
            8
        );

        public static readonly Pattern GameDataMan = new Pattern(
            [0x0F, 0x84, 0x27, 0x01, 0x00, 0x00, 0xF6, 0x41],
            "xxxxxxxx",
            -0x1B,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CsDlcImp = new Pattern(
            [0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0xC6, 0x40, 0x42],
            "xxx????xxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern MapItemManImpl = new Pattern(
            [0xC7, 0x44, 0x24, 0x3C, 0x01, 0x00, 0x00, 0x00, 0xC7, 0x44, 0x24, 0x40],
            "xxxxxxxxxxxx",
            0x10,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern InputManager = new Pattern(
            [0x80, 0xB8, 0xF9, 0x02, 0x00, 0x00, 0x00, 0x74],
            "xxxxxxxx",
            -0x3F,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CSEmkSystem = new Pattern(
            [0x0F, 0x11, 0x80, 0x8C],
            "xxxx",
            0x7,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern WorldAreaTimeImpl = new Pattern(
            [0x48, 0x8B, 0x2D, 0x00, 0x00, 0x00, 0x00, 0x48, 0xC1],
            "xxx????xx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern GroupMask = new Pattern(
            [0x80, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x10, 0x00],
            "xx????xxxx",
            0,
            AddressingMode.Relative,
            2,
            7
        );

        public static readonly Pattern CSFlipperImp = new Pattern(
            [0xF3, 0x0F, 0x10, 0xB7, 0x14, 0x01, 0x00, 0x00, 0x85],
            "xxxxxxxxx",
            0xC,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CSDbgEvent = new Pattern(
            new byte[] { 0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x43, 0x30, 0x89, 0x44, 0x24, 0x50 },
            "xxx????xxxxxxx",
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

        public static readonly Pattern ShouldUpdateAi = new Pattern(
            new byte[] { 0x84, 0xC0, 0x40, 0x0F, 0x94, 0xC6, 0x83 },
            "xxxxxxx",
            -0x1B,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetForceActIdx = new Pattern(
            new byte[] { 0x0F, 0xBE, 0x80, 0xC1 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern TargetNoStagger = new Pattern(
            new byte[] { 0x32, 0xDB, 0x0F, 0x57 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern AttackInfo = new Pattern(
            new byte[] { 0xF3, 0x44, 0x0F, 0x59, 0xC8, 0x44, 0x0F, 0x2F },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern WarpCoordWrite = new Pattern(
            new byte[] { 0x0F, 0x11, 0x80, 0xA0, 0x0A },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern WarpAngleWrite = new Pattern(
            new byte[] { 0x0F, 0x11, 0x80, 0xB0, 0x0A, 0x00, 0x00, 0xC3 },
            "xxxxxxxx",
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

        public static readonly Pattern GiveRunes = new Pattern(
            new byte[] { 0x74, 0x12, 0x8B, 0x53, 0x6C },
            "xxxxx",
            0xA,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern LookupByFieldInsHandle = new Pattern(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x5C, 0x24, 0x30, 0x48, 0x85, 0xC0, 0x74, 0x15],
            "x????xxxxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern WarpToBlock = new Pattern(
            new byte[] { 0x0F, 0xB6, 0x93, 0xAE },
            "xxxx",
            0x13,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern ExternalEventTempCtor = new Pattern(
            new byte[] { 0xC7, 0x41, 0x10, 0x02, 0x00, 0x00, 0x00, 0x89, 0x51 },
            "xxxxxxxxx",
            -0xD,
            AddressingMode.Absolute
        );

        public static readonly Pattern ExecuteTalkCommand = new Pattern(
            new byte[] { 0x89, 0x7D, 0x80, 0x48, 0x8B, 0x02, 0x48, 0x8B, 0xCA },
            "xxxxxxxxx",
            -0x4F,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetEvent = new Pattern(
            [0x48, 0xB9, 0xA9, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0x0A, 0x48, 0x8B, 0x83, 0xE8, 0x00, 0x00, 0x00],
            "xxxxxxxxxxxxxxxxx",
            -0x2E,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetPlayerItemQuantityById = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x3B, 0xC6, 0x7D, 0x07 },
            "x????xxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern ItemSpawn = new Pattern(
            new byte[] { 0x4C, 0x8D, 0x45, 0x34 },
            "xxxx",
            0xB,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern MatrixVectorProduct = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x28, 0x00, 0x0F, 0x29, 0x85, 0xD0, 0x08, 0x00, 0x00 },
            "x????xxxxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern ChrInsByHandle = new Pattern(
            new byte[] { 0x48, 0xC1, 0xE8, 0x1C, 0x24, 0x0F, 0x3C, 0x01, 0x0F, 0x85, 0xA7 },
            "xxxxxxxxxxx",
            -0x1B,
            AddressingMode.Absolute
        );

        public static readonly Pattern FindAndRemoveSpEffect = new Pattern(
            new byte[] { 0x0F, 0xB6, 0xD0, 0x42 },
            "xxxx",
            0xC,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern EmevdSwitch = new Pattern(
            new byte[] { 0x8B, 0x8B, 0xC0, 0x00, 0x00, 0x00, 0x89, 0x4B },
            "xxxxxxxx",
            -0x1E,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern EmkEventInsCtor = new Pattern(
            new byte[] { 0x75, 0x0C, 0x0F, 0xB7, 0x47 },
            "xxxxx",
            -0x2E,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetMovement = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x28, 0xF0, 0xFF },
            "x????xxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern CanDrawEvents = new Pattern(
            new byte[] { 0x48, 0x89, 0x77, 0x28, 0xE8 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        //Patches

        public static readonly Pattern DungeonWarp = new Pattern(
            new byte[] { 0x74, 0x3A, 0x8B, 0x81, 0xA0 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoRunesFromEnemies = new Pattern(
            new byte[] { 0x41, 0xFF, 0x91, 0xC8, 0x05 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoRuneArcLoss = new Pattern(
            new byte[] { 0x74, 0x09, 0x66, 0xC7, 0x81, 0xFF },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoRuneLossOnDeath = new Pattern(
            new byte[] { 0x48, 0x0F, 0x4C, 0xC1, 0x44 },
            "xxxxx",
            0xB,
            AddressingMode.Absolute
        );

        public static readonly Pattern OpenMap = new Pattern(
            new byte[] { 0x84, 0xC0, 0x74, 0x2E, 0xC7 },
            "xxxxx",
            2,
            AddressingMode.Absolute
        );

        public static readonly Pattern CloseMap = new Pattern(
            new byte[] { 0x75, 0x11, 0x38, 0x83, 0xC2 },
            "xxxxx",
            0x3C,
            AddressingMode.Absolute
        );

        public static readonly Pattern EnableFreeCam = new Pattern(
            new byte[] { 0xF3, 0x0F, 0x59, 0xC2, 0xF3, 0x0F, 0x58, 0xC1, 0xF3, 0x0F, 0x11, 0x43 },
            "xxxxxxxxxxxx",
            0xD,
            AddressingMode.Relative,
            1,
            5
        );
    }

    public enum AddressingMode
    {
        Absolute,
        Relative,
    }
}