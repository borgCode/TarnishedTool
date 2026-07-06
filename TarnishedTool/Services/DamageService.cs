using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class DamageService(IMemoryService memoryService, HookManager hookManager) : IDamageService
{
    public void SetOutgoingMultiplier(float value) =>
        memoryService.Write(CodeCaveOffsets.Base + CodeCaveOffsets.DamageOutFactor, value);

    public void SetIncomingMultiplier(float value) =>
        memoryService.Write(CodeCaveOffsets.Base + CodeCaveOffsets.DamageInFactor, value);

    public void ToggleDamageMultiplier(bool isEnabled, float outgoing, float incoming)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.DamageMultCode;
        if (isEnabled)
        {
            var hookLoc = Hooks.DamageApply;
            if (hookLoc == memoryService.BaseAddress) return;

            var outFactor = CodeCaveOffsets.Base + CodeCaveOffsets.DamageOutFactor;
            var inFactor = CodeCaveOffsets.Base + CodeCaveOffsets.DamageInFactor;

            memoryService.Write(outFactor, outgoing);
            memoryService.Write(inFactor, incoming);

            var bytes = AsmLoader.GetAsmBytes(AsmScript.DamageMultiplier);
            AsmHelper.WriteAbsoluteAddresses(bytes, [(WorldChrMan.Base.ToInt64(), 0x20 + 2)]);
            AsmHelper.WriteImmediateDwords(bytes, [(WorldChrMan.PlayerIns, 0x36 + 3)]);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64() + 0x49, outFactor.ToInt64(), 8, 0x49 + 4),
                (code.ToInt64() + 0x53, inFactor.ToInt64(), 8, 0x53 + 4),
                (code.ToInt64() + 0x8C, hookLoc + 0x7, 5, 0x8C + 1)
            ]);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x41, 0x8B, 0x96, 0x28, 0x02, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }
}
