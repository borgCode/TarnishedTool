// 

using System;
using System.Collections.Generic;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class ParamService(MemoryService memoryService) : IParamService
{
    public IntPtr GetParamRow(int tableIndex, int slotIndex, uint rowId)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return IntPtr.Zero;

        int low = 0, high = rowCount - 1;
        while (low <= high)
        {
            int mid = (low + high) >> 1;
            var id = memoryService.ReadUInt32((IntPtr)(descriptorBase + mid * 0x18));

            if (id == rowId)
            {
                var dataOffset = memoryService.ReadInt64((IntPtr)(descriptorBase + mid * 0x18 + 0x08));
                return paramData + (int)dataOffset;
            }

            if (id < rowId)
                low = mid + 1;
            else
                high = mid - 1;
        }

        return IntPtr.Zero;
    }

    public void PrintAllParamTableNames()
    {
        var soloParamRepo = memoryService.ReadInt64(SoloParamRepositoryImp.Base);

        for (int tableIndex = 0; tableIndex < 0xC2; tableIndex++)
        {
            var tableBase = soloParamRepo + tableIndex * 0x48;

            var capacity = memoryService.ReadInt32((IntPtr)(tableBase + 0x80));
            if (capacity <= 0) continue;
            
            var paramResCap = memoryService.ReadInt64((IntPtr)(tableBase + 0x88));
            if (paramResCap == 0) continue;

            var namePtr = memoryService.ReadInt64((IntPtr)(paramResCap + 0x18));
            if (namePtr == 0) continue;

            var name = memoryService.ReadString((IntPtr)namePtr);
        
            Console.WriteLine($"[{tableIndex}] {name}");
        }
    }

    
    public void WriteField(IntPtr row, ParamFieldDef field, object value)
    {
        IntPtr addr = row + field.Offset;

        if (field.BitWidth.HasValue)
        {
            byte current = memoryService.ReadUInt8(addr);
            int mask = (1 << field.BitWidth.Value) - 1;
            int shifted = mask << field.BitPos.Value;
            
            int newVal = value is bool b ? (b ? 1 : 0) : Convert.ToInt32(value);
            newVal &= mask;
        
            byte result = (byte)((current & ~shifted) | (newVal << field.BitPos.Value));
            memoryService.WriteUInt8(addr, result);
            return;
        }

        switch (field.DataType)
        {
            case "f32": memoryService.WriteFloat(addr, Convert.ToSingle(value)); break;
            case "s32": memoryService.WriteInt32(addr, Convert.ToInt32(value)); break;
            case "u32": memoryService.WriteUInt32(addr, Convert.ToUInt32(value)); break;
            case "s16": memoryService.WriteInt16(addr, Convert.ToInt16(value)); break;
            case "u16": memoryService.WriteUInt16(addr, Convert.ToUInt16(value)); break;
            case "s8" or "u8" or "dummy8": memoryService.WriteUInt8(addr, Convert.ToByte(value)); break;
        }
    }
    
    public byte[] ReadRow(IntPtr row, int size)
    {
        return memoryService.ReadBytes(row, size);
    }
    
    public object ReadFieldFromBytes(byte[] data, ParamFieldDef field)
    {
        if (field.BitWidth.HasValue)
        {
            byte raw = data[field.Offset];
            int mask = (1 << field.BitWidth.Value) - 1;
            int value =  (raw >> field.BitPos.Value) & mask;
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
    
    public void SetBit(IntPtr row, int offset, int mask, bool setValue) => 
        memoryService.SetBitValue(row + offset, mask, setValue);

    public void WriteRow(IntPtr row, byte[] data) => memoryService.WriteBytes(row, data);
    
    public void WriteFieldToAllRows(int tableIndex, int slotIndex, int offset, byte[] value)
    {
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        for (int i = 0; i < rowCount; i++)
        {
            var dataOffset = memoryService.ReadInt64((IntPtr)(descriptorBase + i * 0x18 + 0x08));
            memoryService.WriteBytes(paramData + (int)dataOffset + offset, value);
        }
    }
    
    public List<byte[]> ReadFieldFromAllRows(int tableIndex, int slotIndex, int offset, int size)
    {
        var result = new List<byte[]>();
    
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return result;

        for (int i = 0; i < rowCount; i++)
        {
            var dataOffset = memoryService.ReadInt64((IntPtr)(descriptorBase + i * 0x18 + 0x08));
            var bytes = memoryService.ReadBytes(paramData + (int)dataOffset + offset, size);
            result.Add(bytes);
        }

        return result;
    }
    
    public void RestoreFieldToAllRows(int tableIndex, int slotIndex, int offset, List<byte[]>? values)
    {
        if (values == null) return;
    
        var data = GetParamData(tableIndex, slotIndex);
        if (data is not var (paramData, rowCount, descriptorBase)) return;

        for (int i = 0; i < rowCount && i < values.Count; i++)
        {
            var dataOffset = memoryService.ReadInt64((IntPtr)(descriptorBase + i * 0x18 + 0x08));
            memoryService.WriteBytes(paramData + (int)dataOffset + offset, values[i]);
        }
    }
    
    private (IntPtr paramData, int rowCount, long descriptorBase)? GetParamData(int tableIndex, int slotIndex)
    {
        if (tableIndex < 0 || tableIndex >= 0xC2) return null;

        var soloParamRepo = memoryService.ReadInt64(SoloParamRepositoryImp.Base);
        if (soloParamRepo == 0) return null;

        var tableBase = soloParamRepo + tableIndex * 0x48;

        var capacity = memoryService.ReadInt32((IntPtr)(tableBase + 0x80));
        if (slotIndex < 0 || slotIndex >= capacity) return null;

        var paramResCap = memoryService.ReadInt64((IntPtr)(tableBase + 0x88 + slotIndex * 8));
        if (paramResCap == 0) return null;

        var ptr1 = memoryService.ReadInt64((IntPtr)(paramResCap + 0x80));
        if (ptr1 == 0) return null;

        var paramData = memoryService.ReadInt64((IntPtr)(ptr1 + 0x80));
        if (paramData == 0) return null;

        var rowCount = memoryService.ReadUInt16((IntPtr)(paramData + 0x0A));
        var descriptorBase = paramData + 0x40;

        return ((IntPtr)paramData, rowCount, descriptorBase);
    }
}