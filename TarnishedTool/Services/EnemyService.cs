using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class EnemyService(IMemoryService memoryService, HookManager hookManager, IReminderService reminderService) : IEnemyService
{
    private const int MaxNumOfActs = 10;

    
    public void ToggleNoDeath(bool isEnabled) =>
        memoryService.WriteUInt8(ChrDbgFlags.Base + ChrDbgFlags.AllNoDeath, isEnabled ? 1 : 0);

    public void ToggleNoDamage(bool isEnabled) =>
        memoryService.WriteUInt8(ChrDbgFlags.Base + ChrDbgFlags.AllNoDamage, isEnabled ? 1 : 0);

    public void ToggleNoHit(bool isEnabled) =>
        memoryService.WriteUInt8(ChrDbgFlags.Base + ChrDbgFlags.AllNoHit, isEnabled ? 1 : 0);

    public void ToggleNoAttack(bool isEnabled)
    {
        reminderService.TrySetReminder();
        memoryService.WriteUInt8(ChrDbgFlags.Base + ChrDbgFlags.AllNoAttack, isEnabled ? 1 : 0);
    }

    public void ToggleNoMove(bool isEnabled)
    {
        reminderService.TrySetReminder();
        memoryService.WriteUInt8(ChrDbgFlags.Base + ChrDbgFlags.AllNoMove, isEnabled ? 1 : 0);
    }

    public void ToggleDisableAi(bool isEnabled)
    {
        reminderService.TrySetReminder();
        memoryService.WriteUInt8(ChrDbgFlags.Base + ChrDbgFlags.AllDisableAi, isEnabled ? 1 : 0);
    }

    public void ToggleTargetingView(bool isTargetingViewEnabled) =>
        memoryService.WriteUInt8(TargetView.Base, isTargetingViewEnabled ? 1 : 0);

    public void ToggleReducedTargetingView(bool isTargetingViewEnabled)
    {
        var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.TargetView.BlueTargetView;
        if (isTargetingViewEnabled)
        {
            var maxDist = CodeCaveOffsets.Base + (int)CodeCaveOffsets.TargetView.MaxDist;
            memoryService.WriteFloat(maxDist, 100.0f * 100.0f);
            var codeBytes = AsmLoader.GetAsmBytes("ReduceTargetView");
            var bytes = BitConverter.GetBytes(WorldChrMan.Base.ToInt64());
            var hook = Hooks.BlueTargetView;
            
            AsmHelper.WriteImmediateDwords(codeBytes, new[] { (WorldChrMan.PlayerIns, 0x43 + 3) });
            
            Array.Copy(bytes, 0, codeBytes, 0x36 + 2, 8);
            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (code.ToInt64() + 0x86, maxDist.ToInt64(), 8, 0x86 + 4),
                (code.ToInt64() + 0xC4, hook + 0x5, 5, 0xC4 + 1),
                (code.ToInt64() + 0xCA, hook + 0x141, 5, 0xCA + 1),
            });
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                { 0x48, 0x8D, 0x54, 0x24, 0x40 });
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void SetTargetViewMaxDist(float reducedTargetViewDistance)
    {
        var maxDist = CodeCaveOffsets.Base + (int)CodeCaveOffsets.TargetView.MaxDist;
        memoryService.WriteFloat(maxDist, reducedTargetViewDistance * reducedTargetViewDistance);
    }

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
            hookManager.InstallHook(code.ToInt64(), hook, [0x48, 0x8B, 0x49, 0x08, 0x48, 0x85, 0xC9]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void ForceActSequence(int[] actSequence, int npcThinkParamId)
    {
        reminderService.TrySetReminder();
        var actsArr = CodeCaveOffsets.Base + CodeCaveOffsets.ActArray;
        var currentIdx = CodeCaveOffsets.Base + CodeCaveOffsets.CurrentIdx;
        var shouldRunFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldRun;
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ForceActSequence;
        var hookLoc = Hooks.GetForceActIdx;

        for (int i = 0; i < actSequence.Length; i++)
        {
            memoryService.WriteInt32(actsArr + 0x4 * i, actSequence[i]);
        }

        for (int i = actSequence.Length; i < MaxNumOfActs + 1; i++)
        {
            memoryService.WriteInt32(actsArr + 0x4 * i, 0);
        }

        memoryService.WriteInt32(currentIdx, 0);

        var bytes = AsmLoader.GetAsmBytes("ForceActSequence");

        var originalBytes = OriginalBytesByPatch.GetForceActIdx.GetOriginal();
        //Copy to usage in script
        Array.Copy(originalBytes, 0, bytes, 0x42, 7);
        
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

        hookManager.InstallHook(code.ToInt64(), hookLoc, originalBytes);
    }

    public void UnhookForceAct()
    {
        var codeLoc = CodeCaveOffsets.Base + CodeCaveOffsets.ForceActSequence;
        hookManager.UninstallHook(codeLoc.ToInt64());
    }

    public void ToggleLionCooldownHook(bool isEnabled, int lionEntityId)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.LionCooldownHook;
        if (isEnabled)
        {
            reminderService.TrySetReminder();
            var hook = Hooks.LionCooldownHook;
            var codeBytes = AsmLoader.GetAsmBytes("LionCooldownHook");
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 5, code + 0x36);
            Array.Copy(bytes, 0, codeBytes, 0x31 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            memoryService.WriteInt32(code + 0x4, lionEntityId);
            hookManager.InstallHook(code.ToInt64(), hook, [0xF3, 0x0F, 0x59, 0x71, 0x08]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void ToggleDrawNavigationRoute(bool isDrawNavigationRouteEnabled) => 
        memoryService.WriteUInt8(DrawPathing.Base, isDrawNavigationRouteEnabled ? 1 : 0);
}