// 

using System;
using System.Collections.Generic;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class SpEffectService(IMemoryService memoryService, IReminderService reminderService) : ISpEffectService
{
    private const int SpEffectEntrySize = 0x50;
    
    public void ApplySpEffect(nint chrIns, uint spEffectId)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect);
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
        var specialEffect = memoryService.Read<nint>(chrIns + ChrIns.SpecialEffect);
        var bytes = AsmLoader.GetAsmBytes(AsmScript.RemoveSpEffect);
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
        var specialEffect = memoryService.Read<nint>(chrIns + ChrIns.SpecialEffect);
        var current = memoryService.Read<nint>(specialEffect + (int) ChrIns.SpecialEffectOffsets.Head);
        
        while (current != IntPtr.Zero)
        {
            if (memoryService.Read<uint>(current + (int)ChrIns.SpEffectEntry.Id) == spEffectId) return true;
            current = memoryService.Read<nint>(current + (int)ChrIns.SpEffectEntry.Next);
        }
        return false;
    }

    public List<SpEffectEntry> GetActiveSpEffectList(nint chrIns)
    {
        reminderService.TrySetReminder();
        var spEffectList = new List<SpEffectEntry>();
        var specialEffect = memoryService.Read<nint>(chrIns + ChrIns.SpecialEffect);
        var current = (IntPtr) memoryService.Read<nint>((IntPtr)specialEffect + (int) ChrIns.SpecialEffectOffsets.Head);
        
        while (current != IntPtr.Zero)
        {
            var entry = new MemoryBlock(memoryService.ReadBytes(current, SpEffectEntrySize));
    
            int id = entry.Get<int>((int)ChrIns.SpEffectEntry.Id);
            float timeLeft = entry.Get<float>((int)ChrIns.SpEffectEntry.TimeLeft);
            float duration = entry.Get<float>((int)ChrIns.SpEffectEntry.Duration);
            nint paramData = entry.Get<nint>(0);
            nint next = entry.Get<nint>((int)ChrIns.SpEffectEntry.Next);
    
            ushort stateInfo = memoryService.Read<ushort>(paramData + (int)ChrIns.SpEffectParamData.StateInfo);
    
            spEffectList.Add(new SpEffectEntry(id, timeLeft, duration, stateInfo));
            current = next;
        }
        
        return spEffectList;
    }
}