// 

using System;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Utilities;

namespace SilkyRing.Services;

public class EnemyService(MemoryService memoryService, HookManager hookManager) : IEnemyService
{
    public void ToggleRykardMega(bool isRykardNoMegaEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.Rykard;
        if (isRykardNoMegaEnabled)
        {
            var hook = Offsets.Hooks.HasSpEffect;
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
}