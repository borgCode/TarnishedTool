using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class DamageService(IMemoryService memoryService, HookManager hookManager, IReminderService reminderService) : IDamageService
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
            
            reminderService.TrySetReminder();

            var outFactor = CodeCaveOffsets.Base + CodeCaveOffsets.DamageOutFactor;
            var inFactor = CodeCaveOffsets.Base + CodeCaveOffsets.DamageInFactor;

            memoryService.Write(outFactor, outgoing);
            memoryService.Write(inFactor, incoming);

            var bytes = AsmLoader.GetAsmBytes(AsmScript.DamageMultiplier);
            AsmHelper.WriteAbsoluteAddresses(bytes, [(WorldChrMan.Base, 0x2C + 2)]);
            AsmHelper.WriteImmediateDwords(bytes, [(WorldChrMan.PlayerIns, 0x3E + 3)]);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code + 0x72, outFactor, 8, 0x72 + 4),
                (code + 0x7C, inFactor, 8, 0x7C + 4),
                (code + 0xB5, hookLoc + 0x7, 5, 0xB5 + 1)
            ]);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, hookLoc, [0x41, 0x8B, 0x96, 0x28, 0x02, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code);
        }
    }
}
