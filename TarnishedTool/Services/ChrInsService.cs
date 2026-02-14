// 

using System;
using System.Collections.Generic;
using System.Numerics;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class ChrInsService(IMemoryService memoryService) : IChrInsService
{
    public const int ChrInsEntrySize = 0x8;
    public const int ResistBlockSize = 0x44;

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

    public int GetChrId(nint chrIns) =>
        memoryService.Read<int>(chrIns + ChrIns.ChrId);

    public uint GetNpcParamId(nint chrIns) =>
        memoryService.Read<uint>(chrIns + ChrIns.NpcParamId);

    public long GetHandleByChrIns(nint chrIns) =>
        memoryService.Read<long>(chrIns + ChrIns.Handle);

    public int GetChrInstanceId(nint chrIns)
    {
        var instanceIdPtr = memoryService.FollowPointers(GetChrDataPtr(chrIns), ChrIns.InstanceId, false, false);
        return memoryService.Read<int>(instanceIdPtr);
    }

    public void SetSelected(nint chrIns, bool isSelected) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.SelectedEntity, isSelected);

    public Position GetChrInsMapCoords(nint chrIns)
    {
        var blockId = memoryService.Read<uint>(chrIns + ChrIns.BlockId);

        Vector3 localPos = GetLocalCoords(chrIns);

        Vector3 mapCoords = ConvertLocalCoordsToMapCoords(localPos, blockId);

        return new Position(blockId, mapCoords, 0);
    }

    public Vector3 GetLocalCoords(nint chrIns) =>
        memoryService.Read<Vector3>(GetChrPhysicsPtr(chrIns) + (int)ChrIns.ChrPhysicsOffsets.Coords);

    public void ToggleTargetAi(nint chrIns, bool isDisableTargetAiEnabled) =>
        memoryService.SetBitValue(GetChrCtrlFlagsPtr(chrIns) + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit,
            isDisableTargetAiEnabled);

    public bool IsAiDisabled(nint chrIns) =>
        memoryService.IsBitSet(GetChrCtrlFlagsPtr(chrIns) + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit);

    public void ToggleTargetView(nint chrIns, bool isTargetViewEnabled)
    {
        var targetingSystem =
            memoryService.Read<nint>(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.TargetingSystem);
        var flags = targetingSystem + (int)ChrIns.TargetingSystemOffsets.DebugDrawFlags;
        memoryService.SetBitValue((IntPtr)flags + ChrIns.BlueTargetView.Offset, ChrIns.BlueTargetView.Bit,
            isTargetViewEnabled);
    }

    public bool IsTargetViewEnabled(nint chrIns)
    {
        var targetingSystem =
            memoryService.Read<nint>(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.TargetingSystem);
        var flags = targetingSystem + (int)ChrIns.TargetingSystemOffsets.DebugDrawFlags;
        return memoryService.IsBitSet((IntPtr)flags + ChrIns.BlueTargetView.Offset,
            ChrIns.BlueTargetView.Bit);
    }

    public void ToggleNoAttack(nint chrIns, bool isEnabled) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoAttack, isEnabled);

    public bool IsNoAttackEnabled(nint chrIns) =>
        memoryService.IsBitSet(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoAttack);

    public void ToggleNoMove(nint chrIns, bool isEnabled) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoMove, isEnabled);

    public bool IsNoMoveEnabled(nint chrIns) =>
        memoryService.IsBitSet(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.NoMove);

    public void ToggleNoDamage(nint chrIns, bool isEnabled) => 
        memoryService.SetBitValue(GetChrDataFlags(chrIns), (int)ChrIns.ChrDataBitFlags.NoDamage, isEnabled);

    public bool IsNoDamageEnabled(nint chrIns) =>
        memoryService.IsBitSet(GetChrDataFlags(chrIns), (int)ChrIns.ChrDataBitFlags.NoDamage);

    public void SetHp(nint chrIns, int health) =>
        memoryService.Write(GetChrDataPtr(chrIns) + (int)ChrIns.ChrDataOffsets.Health, health);

    public int GetCurrentHp(nint chrIns) =>
        memoryService.Read<int>(GetChrDataPtr(chrIns) + (int)ChrIns.ChrDataOffsets.Health);

    public int GetMaxHp(nint chrIns) =>
        memoryService.Read<int>(GetChrDataPtr(chrIns) + (int)ChrIns.ChrDataOffsets.MaxHealth);

    public float GetCurrentPoise(nint chrIns) =>
        memoryService.Read<float>(GetChrSuperArmorPtr(chrIns) + (int)ChrIns.ChrSuperArmorOffsets.CurrentPoise);

    public float GetMaxPoise(nint chrIns) =>
        memoryService.Read<float>(GetChrSuperArmorPtr(chrIns) + (int)ChrIns.ChrSuperArmorOffsets.MaxPoise);

    public float GetPoiseTimer(nint chrIns) =>
        memoryService.Read<float>(GetChrSuperArmorPtr(chrIns) + (int)ChrIns.ChrSuperArmorOffsets.PoiseTimer);

    public float GetSpeed(nint chrIns) =>
        memoryService.Read<float>(GetChrBehaviorPtr(chrIns) + (int)ChrIns.ChrBehaviorOffsets.AnimSpeed);

    public void SetSpeed(nint chrIns, float speed) =>
        memoryService.Write(GetChrBehaviorPtr(chrIns) + (int)ChrIns.ChrBehaviorOffsets.AnimSpeed, speed);

    public float[] GetDefenses(nint chrIns)
    {
        var ptr = GetNpcParamPtr(chrIns);
        var defenses = new float[8];
        defenses[0] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.StandardAbsorption);
        defenses[1] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.SlashAbsorption);
        defenses[2] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.StrikeAbsorption);
        defenses[3] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.ThrustAbsorption);
        defenses[4] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.MagicAbsorption);
        defenses[5] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.FireAbsorption);
        defenses[6] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.LightningAbsorption);
        defenses[7] = memoryService.Read<float>(ptr + (int)ChrIns.NpcParamOffsets.HolyAbsorption);
        return defenses;
    }

    public bool[] GetImmunities(nint chrIns)
    {
        var ptr = GetNpcParamPtr(chrIns);
        var immunities = new bool[5];
        immunities[0] = memoryService.Read<int>(ptr + (int)ChrIns.NpcParamOffsets.SleepImmune) == 90300;
        immunities[1] = memoryService.Read<int>(ptr + (int)ChrIns.NpcParamOffsets.PoisonImmune) == 90000;
        immunities[2] = memoryService.Read<int>(ptr + (int)ChrIns.NpcParamOffsets.RotImmune) == 90010;
        immunities[4] = memoryService.Read<int>(ptr + (int)ChrIns.NpcParamOffsets.FrostImmune) == 90040;
        immunities[3] = memoryService.Read<int>(ptr + (int)ChrIns.NpcParamOffsets.BleedImmune) == 90020;
        return immunities;
    }

    public int GetResistance(nint chrIns, int offset) => memoryService.Read<int>(GetChrResistPtr(chrIns) + offset);
    public ResistanceData GetAllResistances(nint chrIns)
    {
        var ptr = GetChrResistPtr(chrIns);
        var block = new MemoryBlock(memoryService.ReadBytes(ptr, ResistBlockSize));
        return new ResistanceData(
            block.Get<int>((int)ChrIns.ChrResistOffsets.PoisonCurrent),
            block.Get<int>((int)ChrIns.ChrResistOffsets.PoisonMax),
            block.Get<int>((int)ChrIns.ChrResistOffsets.RotCurrent),
            block.Get<int>((int)ChrIns.ChrResistOffsets.RotMax),
            block.Get<int>((int)ChrIns.ChrResistOffsets.BleedCurrent),
            block.Get<int>((int)ChrIns.ChrResistOffsets.BleedMax),
            block.Get<int>((int)ChrIns.ChrResistOffsets.FrostCurrent),
            block.Get<int>((int)ChrIns.ChrResistOffsets.FrostMax),
            block.Get<int>((int)ChrIns.ChrResistOffsets.SleepCurrent),
            block.Get<int>((int)ChrIns.ChrResistOffsets.SleepMax)
        );
    }

    public uint GetEntityId(nint chrIns) => memoryService.Read<uint>(chrIns + ChrIns.EntityId);

    public int GetNpcThinkParamId(nint chrIns) =>
        memoryService.Read<int>(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.NpcThinkParamId);

    public float GetDistBetweenChrs(nint chrIns1, nint chrIns2)
    {
        PosWithHurtbox pos1 = GetPosWithHurtbox(chrIns1);
        PosWithHurtbox pos2 = GetPosWithHurtbox(chrIns2);
        float distance = Vector3.Distance(pos1.position, pos2.position);
        return distance - pos1.capsuleRadius - pos2.capsuleRadius;
    }

    public int GetCurrentAnimation(nint chrIns) =>
        memoryService.Read<int>(GetChrTimeActPtr(chrIns) + (int)ChrIns.ChrTimeActOffsets.AnimationId);

    public void SetLocalCoords(nint chrIns, Vector3 pos) =>
        memoryService.Write(GetChrPhysicsPtr(chrIns) + (int)ChrIns.ChrPhysicsOffsets.Coords, pos);

    public bool IsNoDeathEnabled(nint chrIns) =>
        memoryService.IsBitSet(GetChrDataFlags(chrIns), (int)ChrIns.ChrDataBitFlags.NoDeath);

    public IntPtr ChrInsByHandle(int handle)
    {
        int poolIndex = (handle >> 20) & 0xFF;
        int slotIndex = handle & 0xFFFFF;

        var worldChrMan = memoryService.Read<nint>(WorldChrMan.Base);
        var chrSet = memoryService.Read<nint>(worldChrMan + WorldChrMan.ChrSetPool + poolIndex * 8);
        var entriesBase = memoryService.Read<nint>(chrSet + (int)WorldChrMan.ChrSetOffsets.ChrSetEntries);
        var chrIns = memoryService.Read<nint>(entriesBase + slotIndex * 16);

#if DEBUG

        Console.WriteLine($@"ChrIns looked up by handle: 0x{(long)chrIns:X}");
#endif

        return chrIns;
    }

    public nint ChrInsByEntityId(uint entityId)
    {
        nint lookedUpChrIns = CodeCaveOffsets.Base + CodeCaveOffsets.LookedUpChrIns;
        var worldChrMan = memoryService.Read<nint>(WorldChrMan.Base);
        var bytes = AsmLoader.GetAsmBytes(AsmScript.GetChrIns);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (worldChrMan, 0x0 + 2),
            (Functions.GetChrInsByEntityId, 0x19 + 2),
            (lookedUpChrIns, 0x25 + 2)
        ]);
        Array.Copy(BitConverter.GetBytes(entityId), 0, bytes, 0x13 + 2, 4);
        memoryService.AllocateAndExecute(bytes);
        return memoryService.Read<nint>(lookedUpChrIns);
    }

    public void ToggleNoDeath(nint chrIns, bool isEnabled) =>
        memoryService.SetBitValue(GetChrDataFlags(chrIns), (int)ChrIns.ChrDataBitFlags.NoDeath, isEnabled);

    public uint GetBlockId(nint chrIns) =>
        memoryService.Read<uint>(chrIns + ChrIns.BlockId);

    public void ToggleNoGravity(IntPtr chrIns, bool isEnabled) =>
        memoryService.Write(GetChrPhysicsPtr(chrIns) + (int)ChrIns.ChrPhysicsOffsets.NoGravity, isEnabled);

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

    private nint GetChrSuperArmorPtr(nint chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrSuperArmorModule], true, false);

    private nint GetChrBehaviorPtr(nint chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrBehaviorModule], true, false);

    private nint GetNpcParamPtr(nint chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.NpcParam], true, false);

    private nint GetChrResistPtr(nint chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrResistModule], true, false);

    private nint GetChrTimeActPtr(nint chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrTimeActModule], true, false);

    private nint GetChrDataFlags(nint chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrDataModule, ChrIns.ChrDataFlags], false, false);

    private PosWithHurtbox GetPosWithHurtbox(nint chrIns)
    {
        var physPtr = GetChrPhysicsPtr(chrIns);
        var position = memoryService.Read<Vector3>(physPtr + (int)ChrIns.ChrPhysicsOffsets.Coords);
        var capsuleRadius = memoryService.Read<float>(physPtr + (int)ChrIns.ChrPhysicsOffsets.HurtCapsuleRadius);
        return new PosWithHurtbox(position, capsuleRadius);
    }

    private Vector3 ConvertLocalCoordsToMapCoords(Vector3 localPos, uint blockId)
    {
        var output = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.Output;
        var input = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.Input;
        var pBlockId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.BlockId;
        var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LocalToMap.Code;

        memoryService.Write(input, localPos);
        memoryService.Write(pBlockId, blockId);

        var bytes = AsmLoader.GetAsmBytes(AsmScript.LocalToMapCoords);
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