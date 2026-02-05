using System;
using System.Collections.Generic;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class AttackInfoService(IMemoryService memoryService, HookManager hookManager) : IAttackInfoService
{
    private int _lastProcessedId;

    public const int StructSize = 0x48;
    public const int StructCount = 16;

    public void ToggleAttackInfoHook(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.AttackInfoCode;
        if (isEnabled)
        {
            var lockedTargetLoc = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
            var id = CodeCaveOffsets.Base + CodeCaveOffsets.AttackInfoId;
            var writeIndex = CodeCaveOffsets.Base + CodeCaveOffsets.AttackInfoWriteIndex;
            var attackInfoStart = CodeCaveOffsets.Base + CodeCaveOffsets.AttackInfoStart;
            var hookLoc = Hooks.AttackInfo;
            var bytes = AsmLoader.GetAsmBytes("SaveAttackInfo");
            Array.Copy(OriginalBytesByPatch.AttackInfo.GetOriginal(), 0, bytes, 0, 5);

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (code.ToInt64() + 0x6, lockedTargetLoc.ToInt64(), 7, 0x6 + 3),
                (code.ToInt64() + 0x1D, id.ToInt64(), 6, 0x1D + 2),
                (code.ToInt64() + 0x23, writeIndex.ToInt64(), 6, 0x23 + 2),
                (code.ToInt64() + 0x2C, attackInfoStart.ToInt64(), 7, 0x2C + 3),
                (code.ToInt64() + 0x3D, id.ToInt64(), 6, 0x3D + 2),
                (code.ToInt64() + 0xD6, writeIndex.ToInt64(), 6, 0xD6 + 2),
                (code.ToInt64() + 0xEB, hookLoc + 5, 5, 0xEB + 1),
            });
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, OriginalBytesByPatch.AttackInfo.GetOriginal());
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public List<AttackInfo> PollAttackInfo()
    {
        var results = new List<AttackInfo>();

        for (int slot = 0; slot < StructCount; slot++)
        {
            var baseAddr = CodeCaveOffsets.Base + CodeCaveOffsets.AttackInfoStart + (slot * StructSize);
            int writeComplete = memoryService.Read<int>(baseAddr + 0x4);
            if (writeComplete != 1) continue;

            int id = memoryService.Read<int>(baseAddr);
            if (id <= _lastProcessedId) continue;

            var info = new AttackInfo
            {
                MyId = id,
                RawPhysicalDamage = memoryService.Read<float>(baseAddr + 0x8),
                RawMagicDamage = memoryService.Read<float>(baseAddr + 0xC),
                RawFireDamage = memoryService.Read<float>(baseAddr + 0x10),
                RawLightningDamage = memoryService.Read<float>(baseAddr + 0x14),
                RawHolyDamage = memoryService.Read<float>(baseAddr + 0x18),
                PoiseDamage = memoryService.Read<float>(baseAddr + 0x20),
                PhysicalAttackType = (PhysicalAttackType)memoryService.Read<byte>(baseAddr + 0x24),
                TotalDamage = memoryService.Read<int>(baseAddr + 0x28),
                FireDamage = memoryService.Read<int>(baseAddr + 0x2C),
                MagicDamage = memoryService.Read<int>(baseAddr + 0x30),
                LightningDamage = memoryService.Read<int>(baseAddr + 0x34),
                HolyDamage = memoryService.Read<int>(baseAddr + 0x38),
                EnemyId = memoryService.Read<int>(baseAddr + 0x3C)
            };
            results.Add(info);
            if (id > _lastProcessedId)
                _lastProcessedId = id;

            memoryService.Write(baseAddr + 0x4, 0);
        }

        results.Sort((a, b) => a.MyId.CompareTo(b.MyId));
        return results;
    }
}