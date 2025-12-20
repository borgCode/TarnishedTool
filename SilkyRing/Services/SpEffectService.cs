// 

using System;
using System.Collections.Generic;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services;

public class SpEffectService(MemoryService memoryService) : ISpEffectService
{
    public void ApplySpEffect(long chrIns, uint spEffectId)
    {
        var bytes = AsmLoader.GetAsmBytes("SetSpEffect");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (chrIns, 0x0 + 2),
            (spEffectId, 0xA + 2),
            (Functions.SetSpEffect, 0x18 + 2)
        });
        memoryService.AllocateAndExecute(bytes);
    }

    public void RemoveSpEffect(long chrIns, uint spEffectId)
    {
        var specialEffect = memoryService.ReadInt64((IntPtr)chrIns + ChrIns.SpecialEffect);
        var bytes = AsmLoader.GetAsmBytes("RemoveSpEffect");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (specialEffect, 0x0 + 2),
            (spEffectId, 0xA + 2),
            (Functions.FindAndRemoveSpEffect, 0x14 + 2)
        });
        memoryService.AllocateAndExecute(bytes);
        
    }

    public bool HasSpEffect(long chrIns, uint spEffectId)
    {
        var specialEffect = memoryService.ReadInt64((IntPtr)chrIns + ChrIns.SpecialEffect);
        var current = (IntPtr) memoryService.ReadInt64((IntPtr)specialEffect + (int) ChrIns.SpecialEffectOffsets.Head);
        
        while (current != IntPtr.Zero)
        {
            if (memoryService.ReadUInt32(current + (int)ChrIns.SpEffectEntry.Id) == spEffectId) return true;
            current = (IntPtr)memoryService.ReadInt64(current + (int)ChrIns.SpEffectEntry.Next);
        }
        return false;
    }

    public List<SpEffectEntry> GetActiveSpEffectList(long chrIns)
    {
        var spEffectList = new List<SpEffectEntry>();
        var specialEffect = memoryService.ReadInt64((IntPtr)chrIns + ChrIns.SpecialEffect);
        var current = (IntPtr) memoryService.ReadInt64((IntPtr)specialEffect + (int) ChrIns.SpecialEffectOffsets.Head);
        
        while (current != IntPtr.Zero)
        {
            int id = memoryService.ReadInt32(current + (int)ChrIns.SpEffectEntry.Id);
            float timeLeft = memoryService.ReadFloat(current + (int)ChrIns.SpEffectEntry.TimeLeft);
            float duration = memoryService.ReadFloat(current + (int)ChrIns.SpEffectEntry.Duration);
            spEffectList.Add(new SpEffectEntry(id, timeLeft, duration));
            current = (IntPtr)memoryService.ReadInt64(current + (int)ChrIns.SpEffectEntry.Next);
        }
        
        return spEffectList;
    }
}