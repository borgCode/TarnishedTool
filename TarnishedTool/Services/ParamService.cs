// 

using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class ParamService(MemoryService memoryService) : IParamService
{
    public IntPtr GetParamRow(int tableIndex, int slotIndex, uint rowId)
    {
        if (tableIndex < 0 || tableIndex >= 0xC2) return IntPtr.Zero;
    
        var soloParamRepo = memoryService.ReadInt64(SoloParamRepositoryImp.Base);
        if (soloParamRepo == 0) return IntPtr.Zero;
    
        var tableBase = soloParamRepo + tableIndex * 0x48;
    
        var capacity = memoryService.ReadInt32((IntPtr)(tableBase + 0x80));
        if (slotIndex < 0 || slotIndex >= capacity) return IntPtr.Zero;
    
        var paramResCap = memoryService.ReadInt64((IntPtr)(tableBase + 0x88 + slotIndex * 8));
        if (paramResCap == 0) return IntPtr.Zero;
    
        var ptr1 = memoryService.ReadInt64((IntPtr)(paramResCap + 0x80));
        if (ptr1 == 0) return IntPtr.Zero;
    
        var paramData = memoryService.ReadInt64((IntPtr)(ptr1 + 0x80));
        if (paramData == 0) return IntPtr.Zero;
    
        var rowCount = memoryService.ReadUInt16((IntPtr)(paramData + 0x0A));
        var descriptorBase = paramData + 0x40;
        
        int low = 0, high = rowCount - 1;
        while (low <= high)
        {
            int mid = (low + high) >> 1;
            var id = memoryService.ReadUInt32((IntPtr)(descriptorBase + mid * 0x18));
        
            if (id == rowId)
            {
                var dataOffset = memoryService.ReadInt64((IntPtr)(descriptorBase + mid * 0x18 + 0x08));
                return (IntPtr)(paramData + dataOffset);
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

    public object ReadField(IntPtr row, ParamFieldDef field)
    {
        IntPtr addr = row + field.Offset;

        if (field.BitWidth.HasValue)
        {
            byte raw = memoryService.ReadUInt8(addr);
            int mask = (1 << field.BitWidth.Value) - 1;
            return (raw >> field.BitPos.Value) & mask;
        }

        return field.DataType switch
        {
            "f32" => memoryService.ReadFloat(addr),
            "s32" => memoryService.ReadInt32(addr),
            "u32" => memoryService.ReadUInt32(addr),
            "s16" => memoryService.ReadInt16(addr),
            "u16" => memoryService.ReadUInt16(addr),
            "s8" => (sbyte)memoryService.ReadUInt8(addr),
            "u8" or "dummy8" => memoryService.ReadUInt8(addr),
            _ => 0
        };
    }

    public void WriteField(IntPtr row, ParamFieldDef field, object value)
    {
        IntPtr addr = row + field.Offset;

        if (field.BitWidth.HasValue)
        {
            byte current = memoryService.ReadUInt8(addr);
            int mask = (1 << field.BitWidth.Value) - 1;
            int shifted = mask << field.BitPos.Value;
            int newVal = Convert.ToInt32(value) & mask;
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
            return (raw >> field.BitPos.Value) & mask;
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
}