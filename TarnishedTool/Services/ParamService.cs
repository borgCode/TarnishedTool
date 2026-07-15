// 

using System;
using System.Collections.Generic;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class ParamService(IMemoryService memoryService) : IParamService
{
    public nint GetParamRow(int tableIndex, int slotIndex, uint rowId)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return 0;

        int low = 0, high = rowCount - 1;
        while (low <= high)
        {
            int mid = (low + high) >> 1;
            var id = memoryService.Read<uint>(descriptorBase + mid * 0x18);

            if (id == rowId)
            {
                var dataOffset = memoryService.Read<uint>(descriptorBase + mid * 0x18 + 0x08);
                return paramData + (int)dataOffset;
            }

            if (id < rowId)
                low = mid + 1;
            else
                high = mid - 1;
        }

        return 0;
    }

    public nint GetParamRowByMatchingBytes(int tableIndex, int slotIndex, byte[] bytes, int offset)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return 0;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);

        for (int i = 0; i < rowCount; i++)
        {
            var dataOffset = BitConverter.ToInt64(descriptors, i * 0x18 + 0x08);
            var row = paramData + (int)dataOffset;
            var rowBytes = memoryService.ReadBytes(row + offset, bytes.Length);

            if (rowBytes.AsSpan().SequenceEqual(bytes))
                return row;
        }

        return 0;
    }

    public void Write<T>(nint row, int offset, T value) where T : unmanaged
        => memoryService.Write<T>(row + offset, value);

    public void PrintAllParamTableNames()
    {
        var soloParamRepo = memoryService.Read<nint>(SoloParamRepositoryImp.Base);

        for (int tableIndex = 0; tableIndex < 0xC2; tableIndex++)
        {
            var tableBase = soloParamRepo + tableIndex * 0x48;

            var capacity = memoryService.Read<int>(tableBase + 0x80);
            if (capacity <= 0) continue;

            var paramResCap = memoryService.Read<nint>(tableBase + 0x88);
            if (paramResCap == 0) continue;

            var namePtr = memoryService.Read<nint>(paramResCap + 0x18);
            if (namePtr == 0) continue;

            var name = memoryService.ReadString(namePtr);

            Console.WriteLine($"[{tableIndex}] {name}");
        }
    }


    public void WriteField(nint row, ParamFieldDef field, object value)
    {
        nint addr = row + field.Offset;

        if (field.BitWidth.HasValue)
        {
            byte current = memoryService.Read<byte>(addr);
            int mask = (1 << field.BitWidth.Value) - 1;
            int shifted = mask << field.BitPos.Value;

            int newVal = value is bool b ? (b ? 1 : 0) : Convert.ToInt32(value);
            newVal &= mask;

            byte result = (byte)((current & ~shifted) | (newVal << field.BitPos.Value));
            memoryService.Write(addr, result);
            return;
        }

        switch (field.DataType)
        {
            case "f32": memoryService.Write(addr, Convert.ToSingle(value)); break;
            case "s32": memoryService.Write(addr, Convert.ToInt32(value)); break;
            case "u32": memoryService.Write(addr, Convert.ToUInt32(value)); break;
            case "s16": memoryService.Write(addr, Convert.ToInt16(value)); break;
            case "u16": memoryService.Write(addr, Convert.ToUInt16(value)); break;
            case "s8" or "u8" or "dummy8": memoryService.Write(addr, Convert.ToByte(value)); break;
        }
    }

    public byte[] ReadRow(nint row, int size)
    {
        return memoryService.ReadBytes(row, size);
    }

    public object ReadFieldFromBytes(byte[] data, ParamFieldDef field)
    {
        if (field.BitWidth.HasValue)
        {
            byte raw = data[field.Offset];
            int mask = (1 << field.BitWidth.Value) - 1;
            int value = (raw >> field.BitPos.Value) & mask;
            if (field.BitWidth.Value == 1)
                return value != 0;

            return value;
        }

        return field.DataType switch
        {
            "f32" => BitConverter.ToSingle(data, field.Offset),
            "s32" => BitConverter.ToInt32(data, field.Offset),
            "u32" => BitConverter.ToUInt32(data, field.Offset),
            "s16" => BitConverter.ToInt16(data, field.Offset),
            "u16" => BitConverter.ToUInt16(data, field.Offset),
            "s8" => (sbyte)data[field.Offset],
            "u8" or "dummy8" => data[field.Offset],
            _ => 0
        };
    }

    public void SetBit(nint row, int offset, int mask, bool setValue) =>
        memoryService.SetBitValue(row + offset, mask, setValue);

    public void WriteRow(nint row, byte[] data) => memoryService.WriteBytes(row, data);

    public void WriteFieldToAllRows(int tableIndex, int slotIndex, int offset, byte[] value, int rowSize)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        var firstOffset = (int)BitConverter.ToInt64(descriptors, 0x08);

        var block = memoryService.ReadBytes(paramData + firstOffset, rowSize * rowCount);

        for (int i = 0; i < rowCount; i++)
        {
            var dataOffset = (int)BitConverter.ToInt64(descriptors, i * 0x18 + 0x08);
            int rowStart = dataOffset - firstOffset;
            for (int b = 0; b < value.Length; b++)
                block[rowStart + offset + b] = value[b];
        }

        memoryService.WriteBytes(paramData + firstOffset, block);
    }

    public void WriteFieldBitToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]> values, int rowSize)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        var firstOffset = (int)BitConverter.ToInt64(descriptors, 0x08);

        var block = memoryService.ReadBytes(paramData + firstOffset, rowSize * rowCount);

        for (int i = 0; i < rowCount && i < values.Count; i++)
        {
            var dataOffset = (int)BitConverter.ToInt64(descriptors, i * 0x18 + 0x08);
            int rowStart = dataOffset - firstOffset;
            var val = values[i];
            for (int b = 0; b < val.Length; b++)
                block[rowStart + offset + b] = val[b];
        }

        memoryService.WriteBytes(paramData + firstOffset, block);
    }

    public void RestoreFieldBitToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]>? values,
        int rowSize)
    {
        if (values == null) return;

        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        var firstOffset = (int)BitConverter.ToInt64(descriptors, 0x08);

        var block = memoryService.ReadBytes(paramData + firstOffset, rowSize * rowCount);

        for (int i = 0; i < rowCount && i < values.Count; i++)
        {
            var dataOffset = (int)BitConverter.ToInt64(descriptors, i * 0x18 + 0x08);
            int rowStart = dataOffset - firstOffset;
            var val = values[i];
            for (int b = 0; b < val.Length; b++)
                block[rowStart + offset + b] = val[b];
        }

        memoryService.WriteBytes(paramData + firstOffset, block);
    }

    public int GetRowSize(int tableIndex, int slotIndex)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (_, rowCount, descriptorBase)) return 0;
        if (rowCount < 2) return 0;

        var descriptors = memoryService.ReadBytes(descriptorBase, 2 * 0x18);
        var first = BitConverter.ToInt64(descriptors, 0x08);
        var second = BitConverter.ToInt64(descriptors, 0x18 + 0x08);
        return (int)(second - first);
    }

    public List<byte[]> ReadFieldFromAllRows(int tableIndex, int slotIndex, int offset, int size)
    {
        var result = new List<byte[]>();

        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return result;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        for (int i = 0; i < rowCount; i++)
        {
            var dataOffset = BitConverter.ToInt64(descriptors, i * 0x18 + 0x08);
            var bytes = memoryService.ReadBytes(paramData + (int)dataOffset + offset, size);
            result.Add(bytes);
        }

        return result;
    }

    public void RestoreFieldToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]>? values, int rowSize)
    {
        if (values == null) return;

        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        var firstOffset = (int)BitConverter.ToInt64(descriptors, 0x08);

        var block = memoryService.ReadBytes(paramData + firstOffset, rowSize * rowCount);

        for (int i = 0; i < rowCount && i < values.Count; i++)
        {
            var dataOffset = (int)BitConverter.ToInt64(descriptors, i * 0x18 + 0x08);
            int rowStart = dataOffset - firstOffset;
            var val = values[i];
            for (int b = 0; b < val.Length; b++)
                block[rowStart + offset + b] = val[b];
        }

        memoryService.WriteBytes(paramData + firstOffset, block);
    }

    public void WriteFieldsToSpecificRows(int tableIndex, int slotIndex, IEnumerable<uint> rowIds, int offset,
        byte[] value,
        int rowSize)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        var firstOffset = (int)BitConverter.ToInt64(descriptors, 0x08);

        var block = memoryService.ReadBytes(paramData + firstOffset, rowSize * rowCount);
        
        var idToIndex = new Dictionary<uint, int>(rowCount);
        for (int i = 0; i < rowCount; i++)
        {
            var id = BitConverter.ToUInt32(descriptors, i * 0x18);
            idToIndex[id] = i;
        }

        foreach (var rowId in rowIds)
        {
            if (!idToIndex.TryGetValue(rowId, out int idx)) continue;
            var dataOffset = (int)BitConverter.ToInt64(descriptors, idx * 0x18 + 0x08);
            int rowStart = dataOffset - firstOffset;
            for (int b = 0; b < value.Length; b++)
                block[rowStart + offset + b] = value[b];
        }

        memoryService.WriteBytes(paramData + firstOffset, block);
    }

    public void RestoreFieldsToSpecificRows(int tableIndex, int slotIndex, Dictionary<uint, byte[]> rowValues,
        int offset, int rowSize)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        var descriptors = memoryService.ReadBytes(descriptorBase, rowCount * 0x18);
        var firstOffset = (int)BitConverter.ToInt64(descriptors, 0x08);

        var block = memoryService.ReadBytes(paramData + firstOffset, rowSize * rowCount);

        var idToIndex = new Dictionary<uint, int>(rowCount);
        for (int i = 0; i < rowCount; i++)
        {
            var id = BitConverter.ToUInt32(descriptors, i * 0x18);
            idToIndex[id] = i;
        }

        foreach (var kvp in rowValues)
        {
            if (!idToIndex.TryGetValue(kvp.Key, out int idx)) continue;
            var dataOffset = (int)BitConverter.ToInt64(descriptors, idx * 0x18 + 0x08);
            int rowStart = dataOffset - firstOffset;
            for (int b = 0; b < kvp.Value.Length; b++)
                block[rowStart + offset + b] = kvp.Value[b];
        }

        memoryService.WriteBytes(paramData + firstOffset, block);
    }

    private (nint paramData, int rowCount, nint descriptorBase)? GetParamData(int tableIndex, int slotIndex)
    {
        if (tableIndex < 0 || tableIndex >= 0xC2) return null;

        var soloParamRepo = memoryService.Read<nint>(SoloParamRepositoryImp.Base);
        if (soloParamRepo == 0) return null;

        var tableBase = soloParamRepo + tableIndex * 0x48;

        var capacity = memoryService.Read<int>(tableBase + 0x80);
        if (slotIndex < 0 || slotIndex >= capacity) return null;

        var paramResCap = memoryService.Read<nint>(tableBase + 0x88 + slotIndex * 8);
        if (paramResCap == 0) return null;

        var ptr1 = memoryService.Read<nint>(paramResCap + 0x80);
        if (ptr1 == 0) return null;

        var paramData = memoryService.Read<nint>(ptr1 + 0x80);
        if (paramData == 0) return null;

        var rowCount = memoryService.Read<ushort>(paramData + 0x0A);
        var descriptorBase = paramData + 0x40;

        return (paramData, rowCount, descriptorBase);
    }
}