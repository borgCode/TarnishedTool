// 

using System;
using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IParamService
{
    IntPtr GetParamRow(int tableIndex, int slotIndex, uint rowId);
    IntPtr GetParamRowByMatchingBytes(int tableIndex, int slotIndex, byte[] bytes, int offset);
    void Write<T>(IntPtr row, int offset, T value) where T : unmanaged;
    void PrintAllParamTableNames();
    public byte[] ReadRow(IntPtr row, int size);
    public object ReadFieldFromBytes(byte[] data, ParamFieldDef field);
    void WriteField(IntPtr row, ParamFieldDef field, object value);
    void SetBit(IntPtr row, int offset, int mask, bool setValue);
    void WriteRow(IntPtr row, byte[] data);
    void WriteFieldToAllRows(int tableIndex, int slotIndex, int offset, byte[] value, int rowSize);
    List<byte[]> ReadFieldFromAllRows(int tableIndex, int slotIndex, int offset, int size);
    void RestoreFieldToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]>? values, int rowSize);
    void WriteFieldBitToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]> values, int rowSize);
    void RestoreFieldBitToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]>? values, int rowSize);
    int GetRowSize(int tableIndex, int slotIndex);
    void WriteFieldsToSpecificRows(int tableIndex, int slotIndex, IEnumerable<uint> rowIds, int offset, byte[] value,
        int rowSize);
    void RestoreFieldsToSpecificRows(int tableIndex, int slotIndex, Dictionary<uint, byte[]> rowValues,
        int offset, int rowSize);
}