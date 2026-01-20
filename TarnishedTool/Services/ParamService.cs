// 

using System;
using TarnishedTool.Interfaces;
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

    public void WriteInt32(IntPtr row, int offset, int value) => memoryService.WriteInt32(row + offset, value);
    
    public void WriteFloat(IntPtr row, int offset, float value) => memoryService.WriteFloat(row + offset, value);
    
    public void WriteInt16(IntPtr row, int offset, short value) => 
        memoryService.WriteBytes(row + offset, BitConverter.GetBytes(value));
    
    public void WriteUInt8(IntPtr row, int offset, byte value) => memoryService.WriteUInt8(row + offset, value);
    
    public void SetBit(IntPtr row, int offset, int mask, bool setValue) => 
        memoryService.SetBitValue(row + offset, mask, setValue);
}