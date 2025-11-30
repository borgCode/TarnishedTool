using System;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services;

public class EnemyService(MemoryService memoryService, HookManager hookManager) : IEnemyService
{
    public void ToggleRykardMega(bool isRykardNoMegaEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.Rykard;
        if (isRykardNoMegaEnabled)
        {
            var hook = Hooks.HasSpEffect;
            var codeBytes = AsmLoader.GetAsmBytes("RykardNoMega");
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x17);
            Array.Copy(bytes, 0, codeBytes, 0x12 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                { 0x48, 0x8B, 0x49, 0x08, 0x48, 0x85, 0xC9 });
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void ForceActSequence(int[] actSequence, int npcThinkParamId)
    {
        var actsArr = CodeCaveOffsets.Base + CodeCaveOffsets.ActArray;
        var currentIdx = CodeCaveOffsets.Base + CodeCaveOffsets.CurrentIdx;
        var shouldRunFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldRun;
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ForceActSequence;
        var hookLoc = Hooks.GetForceActIdx;

        for (int i = 0; i < actSequence.Length; i++)
        {
            memoryService.WriteInt32(actsArr + 0x4 * i, actSequence[i]);
        }

        memoryService.WriteInt32(currentIdx, 0);

        var bytes = AsmLoader.GetAsmBytes("ForceActSequence");
        AsmHelper.WriteRelativeOffsets(bytes, new[]
        {
            (code.ToInt64(), shouldRunFlag.ToInt64(), 7, 0x0 + 2),
            (code.ToInt64() + 0x15, currentIdx.ToInt64(), 6, 0x15 + 2),
            (code.ToInt64() + 0x1B, actsArr.ToInt64(), 7, 0x1B + 3),
            (code.ToInt64() + 0x28, currentIdx.ToInt64(), 6, 0x28 + 2),
            (code.ToInt64() + 0x33, shouldRunFlag.ToInt64(), 7, 0x33 + 2),
            (code.ToInt64() + 0x3D, hookLoc + 7, 5, 0x3D + 1),
            (code.ToInt64() + 0x49, hookLoc + 7, 5, 0x49 + 1)
        });

        memoryService.WriteBytes(code, bytes);
        memoryService.WriteInt32(code + 0x9 + 3, npcThinkParamId);

        memoryService.WriteUInt8(shouldRunFlag, 1);

        hookManager.InstallHook(code.ToInt64(), hookLoc,
            [0x0F, 0xBE, 0x80, 0xC1, 0xE9, 0x00, 0x00]);
    }

    public void UnhookForceAct()
    {
        var codeLoc = CodeCaveOffsets.Base + CodeCaveOffsets.ForceActSequence;
        hookManager.UninstallHook(codeLoc.ToInt64());
    }
}