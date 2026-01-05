namespace TarnishedTool.Memory
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

        public static readonly Pattern WorldChrMan = new(
            [0x48, 0x39, 0x2D, 0x42],
            "xxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern FieldArea = new(
            [0x48, 0x8B, 0x05, 0xEC, 0xEB],
            "xxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern LuaEventMan = new(
            [0x48, 0x83, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x1D, 0x01, 0x00, 0x00],
            "xxx????xxxxxxx",
            0,
            AddressingMode.Relative,
            3,
            8
        );

        public static readonly Pattern VirtualMemFlag = new(
            [0x48, 0x8B, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xFF, 0x74, 0x53],
            "xxx????xxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern DamageManager = new(
            [0x48, 0x3B, 0xDF, 0x0F, 0x84, 0x87, 0x01],
            "xxxxxxx",
            0x27,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern MenuMan = new(
            [0x0F, 0x45, 0xF8, 0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC9, 0x75, 0x2E],
            "xxxxxx????xxxxx",
            3,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern TargetView = new(
            [0x75, 0x0D, 0x40, 0x38, 0x35, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x41],
            "xxxxx????xxx",
            0x2,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern GameMan = new(
            [0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC0, 0x74, 0x07, 0x8B, 0x80, 0x84],
            "xxx????xxxxxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern WorldHitMan = new(
            [0x48, 0x8B, 0x2D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x63, 0xD8],
            "xxx????xxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern WorldChrManDbg = new(
            [0x83, 0xCF, 0x10, 0x89, 0x7C, 0x24, 0x20, 0x48, 0x83],
            "xxxxxxxxx",
            0x7,
            AddressingMode.Relative,
            3,
            8
        );

        public static readonly Pattern GameDataMan = new(
            [0x0F, 0x84, 0x27, 0x01, 0x00, 0x00, 0xF6, 0x41],
            "xxxxxxxx",
            -0x1B,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CsDlcImp = new(
            [0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0xC6, 0x40, 0x42],
            "xxx????xxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern MapItemManImpl = new(
            [0xC7, 0x44, 0x24, 0x3C, 0x01, 0x00, 0x00, 0x00, 0xC7, 0x44, 0x24, 0x40],
            "xxxxxxxxxxxx",
            0x10,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern FD4PadManager = new(
            [0x80, 0xB8, 0xF9, 0x02, 0x00, 0x00, 0x00, 0x74],
            "xxxxxxxx",
            -0x3F,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CSEmkSystem = new(
            [0x0F, 0x11, 0x80, 0x8C],
            "xxxx",
            0x7,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern WorldAreaTimeImpl = new(
            [0x48, 0x8B, 0x2D, 0x00, 0x00, 0x00, 0x00, 0x48, 0xC1],
            "xxx????xx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern GroupMask = new(
            [0x80, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x10, 0x00],
            "xx????xxxx",
            0,
            AddressingMode.Relative,
            2,
            7
        );

        public static readonly Pattern CSFlipperImp = new(
            [0xF3, 0x0F, 0x10, 0xB7, 0x14, 0x01, 0x00, 0x00, 0x85],
            "xxxxxxxxx",
            0xC,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CSDbgEvent = new(
            [0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x43, 0x30, 0x89, 0x44, 0x24, 0x50],
            "xxx????xxxxxxx",
            0,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern UserInputManager = new(
            [0x0F, 0xB6, 0x80, 0x8E, 0x08, 0x00, 0x00, 0xB9],
            "xxxxxxxx",
            -0x11,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern CSTrophy = new(
            [0x85, 0xC0, 0x78, 0x18, 0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00],
            "xxxxxxx????",
            0x4,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern MapDebugFlags = new Pattern(
            [0x85, 0xD2, 0x78, 0x10, 0x0F],
            "xxxxx",
            0x4,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern SoloParamRepositoryImp = new Pattern(
            new byte[] { 0x45, 0x33, 0xC0, 0xBA, 0x8F },
            "xxxxx",
            -0x10,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern MsgRepository = new Pattern(
            new byte[] { 0x80, 0xF9, 0x03, 0x41 },
            "xxxx",
            0xC,
            AddressingMode.Relative,
            3,
            7
        );

        public static readonly Pattern DrawPathing = new Pattern(
            [0x38, 0x99, 0xD9, 0x00, 0x00, 0x00, 0x4C],
            "xxxxxxx",
            0x10,
            AddressingMode.Absolute
        );

        //Hooks

        public static readonly Pattern LoadScreenMsgLookup = new Pattern(
            [0x44, 0x8B, 0xCA, 0x33, 0xD2, 0x41, 0xB8, 0xCD],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoMapAcquiredPopup = new Pattern(
            [0x74, 0x0D, 0x8B, 0x54],
            "xxxx",
            0x2,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetActionRequested = new Pattern(
            [0x74, 0x05, 0x49, 0x09, 0x41, 0x10],
            "xxxxxx",
            2,
            AddressingMode.Absolute
        );

        public static readonly Pattern InAirTimer = new(
            [0xF3, 0x0F, 0x11, 0x43, 0x18, 0xC6],
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern UpdateCoords = new(
            [0x0F, 0x11, 0x43, 0x70, 0xC7, 0x43],
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoClipKb = new(
            [0xF6, 0x84, 0x08, 0xE8],
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoClipTriggers = new(
            [0x0F, 0xB6, 0x44, 0x24, 0x36, 0x0F],
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern HasSpEffect = new(
            [0x39, 0x51, 0x08, 0x74, 0x0C, 0x48, 0x8B],
            "xxxxxxx",
            -0x10,
            AddressingMode.Absolute
        );

        public static readonly Pattern BlueTargetViewHook = new(
            [0x0F, 0x84, 0x41, 0x01, 0x00, 0x00, 0x48, 0x8D, 0x54],
            "xxxxxxxxx",
            0x6,
            AddressingMode.Absolute
        );

        public static readonly Pattern LockedTargetPtr = new(
            [0x74, 0x17, 0x48, 0x8B, 0x8F, 0x88],
            "xxxxxx",
            2,
            AddressingMode.Absolute
        );

        public static readonly Pattern InfinitePoise = new(
            [0x80, 0xBF, 0x5F, 0x02],
            "xxxx",
            0x26,
            AddressingMode.Absolute
        );

        public static readonly Pattern ShouldUpdateAi = new(
            [0x84, 0xC0, 0x40, 0x0F, 0x94, 0xC6, 0x83],
            "xxxxxxx",
            -0x1B,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetForceActIdx = new(
            [0x0F, 0xBE, 0x80, 0xC1],
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoStagger = new Pattern(
            [0x48, 0x8B, 0x41, 0x08, 0x83, 0x48, 0x2C, 0x08, 0x0F],
            "xxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern AttackInfo = new(
            [0xF3, 0x44, 0x0F, 0x59, 0xC8, 0x44, 0x0F, 0x2F],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern WarpCoordWrite = new(
            [0x0F, 0x11, 0x80, 0xA0, 0x0A],
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern WarpAngleWrite = new(
            [0x0F, 0x11, 0x80, 0xB0, 0x0A, 0x00, 0x00, 0xC3],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern LionCooldownHook = new Pattern(
            [0xF3, 0x0F, 0x59, 0x71, 0x08],
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoGrab = new Pattern(
            new byte[] { 0x41, 0x8B, 0x56, 0x44, 0x48, 0x8D, 0x4C },
            "xxxxxxx",
            0,
            AddressingMode.Absolute
        );

        //Funcs

        public static readonly Pattern GetChrInsByEntityId = new Pattern(
            [0x48, 0x8D, 0x93, 0x34, 0x02],
            "xxxxx",
            0x7,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GraceWarp = new(
            [0xC7, 0x44, 0x24, 0x30, 0x10, 0x27, 0x00, 0x00, 0x48, 0x8B, 0xCF],
            "xxxxxxxxxxx",
            -0x14,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetEvent = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x80, 0x7C, 0x24, 0x60, 0x00, 0x74, 0x5C],
            "x????xxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern SetSpEffect = new(
            [0x85, 0xD2, 0x78, 0x09, 0x48, 0x8B],
            "xxxxxx",
            0x8,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GiveRunes = new(
            [0x74, 0x12, 0x8B, 0x53, 0x6C],
            "xxxxx",
            0xA,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern LookupByFieldInsHandle = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x5C, 0x24, 0x30, 0x48, 0x85, 0xC0, 0x74, 0x15],
            "x????xxxxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern WarpToBlock = new(
            [0x0F, 0xB6, 0x93, 0xAE],
            "xxxx",
            0x13,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern ExternalEventTempCtor = new(
            [0xC7, 0x41, 0x10, 0x02, 0x00, 0x00, 0x00, 0x89, 0x51],
            "xxxxxxxxx",
            -0xD,
            AddressingMode.Absolute
        );

        public static readonly Pattern ExecuteTalkCommand = new(
            [0x89, 0x7D, 0x80, 0x48, 0x8B, 0x02, 0x48, 0x8B, 0xCA],
            "xxxxxxxxx",
            -0x4F,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetEvent = new(
            [0x48, 0xB9, 0xA9, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0x0A, 0x48, 0x8B, 0x83, 0xE8, 0x00, 0x00, 0x00],
            "xxxxxxxxxxxxxxxxx",
            -0x2E,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetPlayerItemQuantityById = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x3B, 0xC6, 0x7D, 0x07],
            "x????xxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern ItemSpawn = new(
            [0x4C, 0x8D, 0x45, 0x34],
            "xxxx",
            0xB,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern MatrixVectorProduct = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x28, 0x00, 0x0F, 0x29, 0x85, 0xD0, 0x08, 0x00, 0x00],
            "x????xxxxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern ChrInsByHandle = new(
            [0x48, 0xC1, 0xE8, 0x1C, 0x24, 0x0F, 0x3C, 0x01, 0x0F, 0x85, 0xA7],
            "xxxxxxxxxxx",
            -0x1B,
            AddressingMode.Absolute
        );

        public static readonly Pattern FindAndRemoveSpEffect = new(
            [0x0F, 0xB6, 0xD0, 0x42],
            "xxxx",
            0xC,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern EmevdSwitch = new(
            [0x8B, 0x8B, 0xC0, 0x00, 0x00, 0x00, 0x89, 0x4B],
            "xxxxxxxx",
            -0x1E,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern EmkEventInsCtor = new(
            [0x75, 0x0C, 0x0F, 0xB7, 0x47],
            "xxxxx",
            -0x2E,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetMovement = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x28, 0xF0, 0xFF],
            "x????xxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern CanDrawEvents = new(
            [0x48, 0x89, 0x77, 0x28, 0xE8],
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NpcEzStateTalkCtor = new Pattern(
            [0x44, 0x89, 0x41, 0x20, 0x48, 0x89, 0x79],
            "xxxxxxx",
            -0x2D,
            AddressingMode.Absolute
        );

        public static readonly Pattern EzStateEnvQueryImplCtor = new Pattern(
            [0xC7, 0x44, 0x24, 0x50, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4D],
            "xxxxxxxxxxx",
            -0x3A,
            AddressingMode.Absolute
        );

        //Patches

        public static readonly Pattern GetItemChance = new Pattern(
            [0x41, 0x0F, 0xB7, 0xC0, 0xC3, 0x45, 0x0F, 0xB7, 0x41, 0x42],
            "xxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern IsWorldPaused = new Pattern(
            new byte[] { 0x0F, 0x84, 0x87, 0x00, 0x00, 0x00, 0xC6, 0x83 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern IsTorrentDisabledInUnderworld = new Pattern(
            [0x80, 0x78, 0x36, 0x00, 0x0F],
            "xxxxx",
            4,
            AddressingMode.Absolute
        );

        public static readonly Pattern IsWhistleDisabled = new Pattern(
            [0x80, 0x79, 0x36, 0x00, 0x0F],
            "xxxxx",
            4,
            AddressingMode.Absolute
        );

        public static readonly Pattern DebugFont = new(
            [0xF3, 0x0F, 0x11, 0x55, 0xE3, 0x66],
            "xxxxxx",
            0xA,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern CanFastTravel = new Pattern(
            [0x74, 0x14, 0xBA, 0x16],
            "xxxx",
            0xC,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoRunesFromEnemies = new(
            [0x41, 0xFF, 0x91, 0xC8, 0x05],
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoRuneArcLoss = new(
            [0x74, 0x09, 0x66, 0xC7, 0x81, 0xFF],
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoRuneLossOnDeath = new(
            [0x0F, 0x84, 0xE4, 0x01, 0x00, 0x00, 0x45, 0x84],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern OpenMap = new(
            [0x84, 0xC0, 0x74, 0x2E, 0xC7],
            "xxxxx",
            2,
            AddressingMode.Absolute
        );

        public static readonly Pattern PlayerSound = new(
            [0x40, 0x38, 0xB7, 0xCA],
            "xxxx",
            7,
            AddressingMode.Absolute
        );

        public static readonly Pattern CloseMap = new(
            [0x75, 0x11, 0x38, 0x83, 0xC2],
            "xxxxx",
            0x3C,
            AddressingMode.Absolute
        );

        public static readonly Pattern EnableFreeCam = new(
            [0xF3, 0x0F, 0x59, 0xC2, 0xF3, 0x0F, 0x58, 0xC1, 0xF3, 0x0F, 0x11, 0x43],
            "xxxxxxxxxxxx",
            0xD,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetShopEvent = new(
            [0x84, 0xC0, 0x74, 0x17, 0x48, 0x8D, 0x54],
            "xxxxxxx",
            -0x5,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern NoLogo = new(
            [0x48, 0x85, 0xD2, 0x74, 0x07, 0xC6, 0x82],
            "xxxxxxx",
            0x18,
            AddressingMode.Absolute
        );
    }

    public enum AddressingMode
    {
        Absolute,
        Relative,
    }
}