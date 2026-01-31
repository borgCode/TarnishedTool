// 

using System;
using System.Collections.Generic;
using System.Numerics;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
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
    
    public Position GetChrInsPos(IntPtr chrIns)
    {
        var blockId = memoryService.Read<uint>(chrIns + ChrIns.BlockId);

        var worldBlockInfo = FindWorldBlockInfoByBlockId(blockId);

        Vector3 blockInfoPos = memoryService.Read<Vector3>(worldBlockInfo + 0x70);
        Vector3 chrInsLocalPos = GetChrInsLocalPos(chrIns);

        return new Position(blockId, Vector3.Subtract(chrInsLocalPos, blockInfoPos), 0);
    }

    public Vector3 GetChrInsLocalPos(IntPtr chrIns) =>
        memoryService.Read<Vector3>(GetChrPhysicsPtr(chrIns) + (int)ChrIns.ChrPhysicsOffsets.Coords);

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

    private IntPtr FindWorldBlockInfoByBlockId(uint blockId)
    {
        var worldInfoOwner =
            memoryService.Read<IntPtr>(memoryService.Read<IntPtr>(FieldArea.Base) + FieldArea.WorldInfoOwner);

        var areaCount = memoryService.Read<int>(worldInfoOwner + FieldArea.WorldInfoOwnerOffsets.AreaCount);
        var areaArrayBase = worldInfoOwner + FieldArea.WorldInfoOwnerOffsets.AreaArrayBase;

        var targetArea = (blockId >>> 24) & 0xFF;

        for (int i = 0; i < areaCount; i++)
        {
            var areaPtr = memoryService.Read<nint>(areaArrayBase + i * 8);

            var areaId = memoryService.Read<int>(areaPtr + 0xC);

            if (areaId == targetArea)
            {
                var blockCount = memoryService.Read<int>(areaPtr + 0x40);
                var blocksPtr = memoryService.Read<nint>(areaPtr + 0x48);

                for (int j = 0; j < blockCount; j++)
                {
                    var blockInfoPtr = blocksPtr + j * 0xE0;
                    var storedBlockId = memoryService.Read<uint>(blockInfoPtr + 0x8);

                    if (storedBlockId == blockId)
                    {
                        return blockInfoPtr;
                    }
                }

                break;
            }
        }

        return IntPtr.Zero;
    }

    #endregion
    
}