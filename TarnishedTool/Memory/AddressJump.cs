// 

namespace TarnishedTool.Memory;

public struct AddressJump
{
    public enum JumpType
    {
        Relative32,
        Absolute64
    }

    public int Offset { get; }
    public int ImmediatePosition { get; }
    public int InstructionLength { get; }
    public JumpType Type { get; }

    private AddressJump(int offset, int immediatePosition, int instructionLength, JumpType type)
    {
        Offset = offset;
        ImmediatePosition = immediatePosition;
        InstructionLength = instructionLength;
        Type = type;
    }

    public static AddressJump Relative(int offset, int immediatePosition, int instructionLength)
        => new(offset, immediatePosition, instructionLength, JumpType.Relative32);

    public static AddressJump Absolute(int offset, int immediatePosition)
        => new(offset, immediatePosition, 0, JumpType.Absolute64);
}