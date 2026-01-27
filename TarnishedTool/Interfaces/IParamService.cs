// 

using System;
using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IParamService
{
    IntPtr GetParamRow(int tableIndex, int slotIndex, uint rowId);
    void PrintAllParamTableNames();
    public byte[] ReadRow(IntPtr row, int size);
    public object ReadFieldFromBytes(byte[] data, ParamFieldDef field);
    void WriteField(IntPtr row, ParamFieldDef field, object value);
    void SetBit(IntPtr row, int offset, int mask, bool setValue);
    void WriteRow(IntPtr row, byte[] data);
    void WriteFieldToAllRows(int tableIndex, int slotIndex, int offset, byte[] value);
    List<byte[]> ReadFieldFromAllRows(int tableIndex, int slotIndex, int offset, int size);
    void RestoreFieldToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]>? values);
}