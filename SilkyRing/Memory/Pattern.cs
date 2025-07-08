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



    }
    
  
    
    

    
    
    

    public enum AddressingMode
    {
        Absolute,
        Relative,
    }
}