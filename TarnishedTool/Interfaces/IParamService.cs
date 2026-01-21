// 

using System;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IParamService
{
    IntPtr GetParamRow(int tableIndex, int slotIndex, uint rowId);
    void PrintAllParamTableNames();
    object ReadField(IntPtr row, ParamFieldDef field);
    public byte[] ReadRow(IntPtr row, int size);
    public object ReadFieldFromBytes(byte[] data, ParamFieldDef field);
    void WriteField(IntPtr row, ParamFieldDef field, object value);
    void SetBit(IntPtr row, int offset, int mask, bool setValue);
}