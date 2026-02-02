// 

using System;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Enums.GameVersion;

namespace TarnishedTool.Services;

public class ReminderService : IReminderService
{
    private const uint MessageCategory = 205;
    private const int ReminderEntryIndex = 3;
    private const string ReminderText = "Tarnished Tool Active";

    private bool _hasDoneReminder;
    private readonly IMemoryService _memoryService;
    private readonly HookManager _hookManager;

    public ReminderService(IMemoryService memoryService, HookManager hookManager, IStateService stateService)
    {
        _memoryService = memoryService;
        _hookManager = hookManager;
        stateService.Subscribe(State.Detached, OnDetached);
    }

    private void OnDetached()
    {
        _hasDoneReminder = false;
    }

    public void TrySetReminder()
    {
        if (_hasDoneReminder) return;

        var (fmg, stringTable, count) = GetFmgData(0, MessageCategory);
        if (fmg == 0 || ReminderEntryIndex >= count) return;

        var offset = _memoryService.ReadInt64(stringTable + ReminderEntryIndex * 8);
        if (offset == 0) return;

        InstallHook();

        _memoryService.WriteString((IntPtr)(fmg + offset), ReminderText, ReminderText.Length * 2);
        _hasDoneReminder = true;
    }

    private (long fmg, IntPtr stringTable, int count) GetFmgData(uint version, uint category)
    {
        var msgRepo = _memoryService.ReadInt64(Offsets.MsgRepository.Base);
        if (msgRepo == 0) return (0, IntPtr.Zero, 0);

        var versionCount = _memoryService.ReadUInt32((IntPtr)(msgRepo + 0x10));
        var categoryCount = _memoryService.ReadUInt32((IntPtr)(msgRepo + 0x14));

        if (version >= versionCount || category >= categoryCount) return (0, IntPtr.Zero, 0);

        var versionsArray = _memoryService.ReadInt64((IntPtr)(msgRepo + 0x8));
        var versionPtr = _memoryService.ReadInt64((IntPtr)(versionsArray + version * 8));
        if (versionPtr == 0) return (0, IntPtr.Zero, 0);

        var fmg = _memoryService.ReadInt64((IntPtr)(versionPtr + category * 8));
        if (fmg == 0) return (0, IntPtr.Zero, 0);

        var stringTable = _memoryService.ReadInt64((IntPtr)(fmg + 0x18));
        var rangeCount = _memoryService.ReadInt32((IntPtr)(fmg + 0x0C));

        if (rangeCount <= 0) return (fmg, (IntPtr)stringTable, 0);

        var lastDescBase = fmg + (rangeCount - 1) * 0x10;
        var lastBaseIndex = _memoryService.ReadUInt32((IntPtr)(lastDescBase + 0x28));
        var lastStart = _memoryService.ReadUInt32((IntPtr)(lastDescBase + 0x2C));
        var lastEnd = _memoryService.ReadUInt32((IntPtr)(lastDescBase + 0x30));
        var totalEntries = (int)(lastBaseIndex + (lastEnd - lastStart + 1));

        return (fmg, (IntPtr)stringTable, totalEntries);
    }

    private void InstallHook()
    {
        switch (Offsets.Version)
        {
            case Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1 or Version1_3_2 or Version1_4_0
                or Version1_4_1 or Version1_5_0 or Version1_6_0:
                DoEarlyPatchesHook();
                break;
            case Version1_7_0:
                DoMidPatchesHook();
                break;
            default:
                DoNormalHook();
                break;
        }
    }

    private void DoMidPatchesHook()
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.LoadScreenForce;
        var hook = Offsets.Hooks.LoadScreenMsgLookupMidPatches;
        var bytes = AsmLoader.GetAsmBytes("ForceLoadScreenMidPatches");
        var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 6, code + 0x12);
        Array.Copy(jmpBytes, 0, bytes, 0xD + 1, jmpBytes.Length);
        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code.ToInt64(), hook, [0x41, 0xB8, 0xCD, 0x00, 0x00, 0x00]);
    }

    private void DoNormalHook()
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.LoadScreenForce;
        var hook = Offsets.Hooks.LoadScreenMsgLookup;
        var bytes = AsmLoader.GetAsmBytes("ForceLoadScreenReminder");
        var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 5, code + 0xE);
        Array.Copy(jmpBytes, 0, bytes, 0x9 + 1, jmpBytes.Length);
        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code.ToInt64(), hook, [0x44, 0x8B, 0xCA, 0x33, 0xD2]);
    }

    private void DoEarlyPatchesHook()
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.LoadScreenForce;
        var hook = Offsets.Hooks.LoadScreenMsgLookupEarlyPatches;
        var bytes = AsmLoader.GetAsmBytes("ForceLoadScreenReminderEarlyPatches");
        var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 5, code + 0x11);
        Array.Copy(jmpBytes, 0, bytes, 0xC + 1, jmpBytes.Length);
        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code.ToInt64(), hook, [0xBA, 0xCD, 0x00, 0x00, 0x00]);
    }
}