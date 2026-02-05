// 

using System;
using System.Collections.Generic;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class SpEffectService(IMemoryService memoryService, IReminderService reminderService) : ISpEffectService
{
    public void ApplySpEffect(nint chrIns, uint spEffectId)
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

    public void RemoveSpEffect(nint chrIns, uint spEffectId)
    {
        var specialEffect = memoryService.ReadInt64(chrIns + ChrIns.SpecialEffect);
        var bytes = AsmLoader.GetAsmBytes("RemoveSpEffect");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (specialEffect, 0x0 + 2),
            (spEffectId, 0xA + 2),
            (Functions.FindAndRemoveSpEffect, 0x14 + 2)
        });
        memoryService.AllocateAndExecute(bytes);
        
    }

    public bool HasSpEffect(nint chrIns, uint spEffectId)
    {
        var specialEffect = memoryService.ReadInt64(chrIns + ChrIns.SpecialEffect);
        var current = (IntPtr) memoryService.ReadInt64((IntPtr)specialEffect + (int) ChrIns.SpecialEffectOffsets.Head);
        
        while (current != IntPtr.Zero)
        {
            if (memoryService.ReadUInt32(current + (int)ChrIns.SpEffectEntry.Id) == spEffectId) return true;
            current = (IntPtr)memoryService.ReadInt64(current + (int)ChrIns.SpEffectEntry.Next);
        }
        return false;
    }

    public List<SpEffectEntry> GetActiveSpEffectList(nint chrIns)
    {
        reminderService.TrySetReminder();
        var spEffectList = new List<SpEffectEntry>();
        var specialEffect = memoryService.ReadInt64(chrIns + ChrIns.SpecialEffect);
        var current = (IntPtr) memoryService.ReadInt64((IntPtr)specialEffect + (int) ChrIns.SpecialEffectOffsets.Head);
        
        while (current != IntPtr.Zero)
        {
            int id = memoryService.ReadInt32(current + (int)ChrIns.SpEffectEntry.Id);
            float timeLeft = memoryService.ReadFloat(current + (int)ChrIns.SpEffectEntry.TimeLeft);
            float duration = memoryService.ReadFloat(current + (int)ChrIns.SpEffectEntry.Duration);
            var paramData = memoryService.ReadInt64(current);
            ushort stateInfo = memoryService.ReadUInt16((IntPtr) paramData + (int)ChrIns.SpEffectParamData.StateInfo);
            spEffectList.Add(new SpEffectEntry(id, timeLeft, duration, stateInfo));
            current = (IntPtr)memoryService.ReadInt64(current + (int)ChrIns.SpEffectEntry.Next);
        }
        
        return spEffectList;
    }
}