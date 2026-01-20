// 

using System;

namespace TarnishedTool.Interfaces;

public interface IParamService
{
    IntPtr GetParamRow(int tableIndex, int slotIndex, uint rowId);
    void PrintAllParamTableNames();
    void WriteInt32(IntPtr row, int offset, int value);
    void WriteFloat(IntPtr row, int offset, float value);
    void WriteInt16(IntPtr row, int offset, short value);
    void WriteUInt8(IntPtr row, int offset, byte value);
    void SetBit(IntPtr row, int offset, int mask, bool setValue);
}