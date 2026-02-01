// 

using System;
using System.Collections.Generic;
using System.Numerics;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class ChrInsService(MemoryService memoryService) : IChrInsService
{
    public const int ChrInsEntrySize = 0x8;

    #region Public Methods

    public List<ChrInsEntry> GetNearbyChrInsEntries()
    {
        var worldChrMan = memoryService.Read<IntPtr>(WorldChrMan.Base);
        nint begin = memoryService.Read<nint>(worldChrMan + WorldChrMan.ChrInsByUpdatePrioBegin);
        nint end = memoryService.Read<nint>(worldChrMan + WorldChrMan.ChrInsByUpdatePrioEnd);

        var count = (end - begin) / ChrInsEntrySize;

        byte[] buffer = memoryService.ReadBytes(begin, (int)(count * ChrInsEntrySize));
        var entries = new List<ChrInsEntry>();
        for (int i = 0; i < count; i++)
        {
            var entry = ParseEntry(buffer, i * ChrInsEntrySize);

            var blockId = memoryService.Read<uint>(entry.ChrIns + ChrIns.BlockId);
            if (blockId == 0xFFFFFFFF)
                continue;

            entries.Add(entry);
        }

        return entries;
    }

    public int GetChrIdByChrIns(IntPtr chrIns) =>
        memoryService.Read<int>(chrIns + ChrIns.ChrId);

    public uint GetNpcParamIdByChrIns(IntPtr chrIns) =>
        memoryService.Read<uint>(chrIns + ChrIns.NpcParamId);

    public long GetHandleByChrIns(IntPtr chrIns) =>
        memoryService.Read<long>(chrIns + ChrIns.Handle);

    public void SetSelected(nint chrIns, bool isSelected) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.SelectedEntity, isSelected);

    public Position GetChrInsMapCoords(IntPtr chrIns)
    {
        var blockId = memoryService.Read<uint>(chrIns + ChrIns.BlockId);

        Vector3 localPos = GetChrInsLocalPos(chrIns);

        Vector3 mapCoords = ConvertHavokCoordsToMapCoords(localPos, blockId);

        return new Position(blockId, mapCoords, 0);
    }

    public Vector3 GetChrInsLocalPos(IntPtr chrIns) =>
        memoryService.Read<Vector3>(GetChrPhysicsPtr(chrIns) + (int)ChrIns.ChrPhysicsOffsets.Coords);

    public void ToggleTargetAi(IntPtr chrIns, bool isDisableTargetAiEnabled) =>
        memoryService.SetBitValue(GetChrCtrlFlagsPtr(chrIns) + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit,
            isDisableTargetAiEnabled);

    public bool IsAiDisabled(IntPtr chrIns) =>
        memoryService.IsBitSet(GetChrCtrlFlagsPtr(chrIns) + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit);

    public void ToggleTargetView(IntPtr chrIns, bool isTargetViewEnabled)
    {
        var targetingSystem =
            memoryService.ReadInt64(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.TargetingSystem);
        var flags = targetingSystem + (int)ChrIns.TargetingSystemOffsets.DebugDrawFlags;
        memoryService.SetBitValue((IntPtr)flags + ChrIns.BlueTargetView.Offset, ChrIns.BlueTargetView.Bit,
            isTargetViewEnabled);
    }

    public bool IsTargetViewEnabled(IntPtr chrIns)
    {
        var targetingSystem =
            memoryService.ReadInt64(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.TargetingSystem);
        var flags = targetingSystem + (int)ChrIns.TargetingSystemOffsets.DebugDrawFlags;
        return memoryService.IsBitSet((IntPtr)flags + ChrIns.BlueTargetView.Offset,
            ChrIns.BlueTargetView.Bit);
    }

    public void ToggleNoAttack(nint chrIns, bool isEnabled) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoAttack, isEnabled);

    public bool IsNoAttackEnabled(IntPtr chrIns) =>
        memoryService.IsBitSet(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoAttack);

    public void ToggleNoMove(nint chrIns, bool isEnabled) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoMove, isEnabled);

    public bool IsNoMoveEnabled(IntPtr chrIns) =>
        memoryService.IsBitSet(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoMove);

    public void ToggleNoDamage(nint chrIns, bool isEnabled)
    {
        var bitFlags = GetChrDataPtr(chrIns) + ChrIns.ChrDataFlags;
        memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage, isEnabled);
    }

    public bool IsNoDamageEnabled(IntPtr chrIns)
    {
        var bitFlags = GetChrDataPtr(chrIns) + ChrIns.ChrDataFlags;
        return memoryService.IsBitSet(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage);
    }

    #endregion

    #region Private Methods

    private ChrInsEntry ParseEntry(byte[] buffer, int offset)
    {
        IntPtr chrIns = (IntPtr)BitConverter.ToInt64(buffer, offset);

        return new ChrInsEntry(chrIns);
    }

    private IntPtr GetChrInsFlagsPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [ChrIns.Flags], false, false);

    private IntPtr GetChrPhysicsPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrPhysicsModule], true, false);

    private IntPtr GetChrCtrlFlagsPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [ChrIns.ChrCtrl, ..ChrIns.ChrCtrlFlags], false, false);

    private IntPtr GetChrDataPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrDataModule], true, false);
    
    private IntPtr GetAiThinkPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.AiThink], true, false);

    private Vector3 ConvertHavokCoordsToMapCoords(Vector3 localPos, uint blockId)
    {
        var output = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.Output;
        var input = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.Input;
        var pBlockId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.BlockId;
        var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.Code;

        memoryService.Write(input, localPos);
        memoryService.Write(pBlockId, blockId);

        var bytes = AsmLoader.GetAsmBytes("LocalToMapCoords");
        AsmHelper.WriteRelativeOffsets(bytes, [
            (code.ToInt64(), output.ToInt64(), 0x7, 0x0 + 3),
            (code.ToInt64() + 0x7, input.ToInt64(), 0x7, 0x7 + 3),
            (code.ToInt64() + 0xE, pBlockId.ToInt64(), 0x7, 0xE + 3),
            (code.ToInt64() + 0x19, Functions.LocalToMapCoords, 0x5, 0x19 + 1)
        ]);

        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code);

        return memoryService.Read<Vector3>(output);
    }

    #endregion
}