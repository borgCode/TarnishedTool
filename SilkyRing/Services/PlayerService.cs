using System;
using SilkyRing.Memory;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class PlayerService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;


        public PlayerService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }

        // public int GetHp() =>
        //     _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp));
        //
        // public int GetMaxHp() =>
        //     _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.MaxHp));
        //
        // public void SetHp(int hp) =>
        //     _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), hp);
        //
        //
        // public void SetFullHp()
        // {
        //     var full = _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));
        //     _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), full);
        // }
        //
        // public void SetRtsr()
        // {
        //     var full = _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));
        //     _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), (full * 30) / 100);
        // }
        //
        // public int GetSp() =>
        //     _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Stamina));
        //
        // public void SetSp(int sp) =>
        //     _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Stamina), sp);
        //
        // private IntPtr GetPlayerCtrlField(int fieldOffset) =>
        //     _memoryIo.FollowPointers(GameManagerImp.Base, new[] { GameManagerImp.Offsets.PlayerCtrl, fieldOffset },
        //         false);
        //
        //
        // public void ToggleNoDeath(bool isNoDeathEnabled) =>
        //     _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.MinHp),
        //         isNoDeathEnabled ? 1 : -99999);
        //
        // public void ToggleNoDamage(bool isNoDamageEnabled)
        // {
        //     var hookLoc = Offsets.Hooks.HpWrite;
        //     var code = CodeCaveOffsets.Base + CodeCaveOffsets.PlayerNoDamage;
        //
        //     if (isNoDamageEnabled)
        //     {
        //         if (GameVersion.Current.Edition == GameEdition.Scholar)
        //         {
        //             var codeBytes = AsmLoader.GetAsmBytes("PlayerNoDamage64");
        //             var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
        //             Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
        //             bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x2C);
        //             Array.Copy(bytes, 0, codeBytes, 0x27 + 1, 4);
        //             _memoryIo.WriteBytes(code, codeBytes);
        //
        //             _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[] { 0x89, 0x83, 0x68, 0x01, 0x00, 0x00 });
        //         }
        //         else
        //         {
        //             var codeBytes = AsmLoader.GetAsmBytes("PlayerNoDamage32");
        //             var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
        //             Array.Copy(bytes, 0, codeBytes, 0x1 + 1, 4);
        //             bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x1F);
        //             Array.Copy(bytes, 0, codeBytes, 0x1A + 1, 4);
        //             _memoryIo.WriteBytes(code, codeBytes);
        //             _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[] { 0x89, 0x8E, 0xFC, 0x00, 0x00, 0x00 });
        //         }
        //         
        //     }
        //     else
        //     {
        //         _hookManager.UninstallHook(code.ToInt64());
        //     }
        // }
        //
        //
        // public void ToggleInfiniteStamina(bool isInfiniteStaminaEnabled) =>
        //     _memoryIo.WriteByte(Patches.InfiniteStam + 1, isInfiniteStaminaEnabled ? 0x82 : 0x83);
        //
        // public int GetPlayerStat(int statOffset) => _memoryIo.ReadUInt8(GetStatPtr(statOffset));
        //
        // private IntPtr GetStatPtr(int statOffset)
        // {
        //     return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PlayerCtrl,
        //         GameManagerImp.ChrCtrlOffsets.StatsPtr,
        //         statOffset
        //     }, false);
        // }
        //
        // public void SetPlayerStat(int statOffset, byte val)
        // {
        //     var currentStatVal = GetPlayerStat(statOffset);
        //     _memoryIo.WriteUInt8(GetStatPtr(statOffset), val);
        //     var numOfLevels = val - currentStatVal;
        //
        //     var buffer = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.Buffer;
        //     var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.Code;
        //     var negativeFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.NegativeFlag;
        //     var numOfLevelsShortLoc = buffer + 0xE2;
        //     var numOfLevelsIntLoc = buffer + 0xE8;
        //     var currentLevelLoc = buffer + 0xEC;
        //     var newLevelLoc = buffer + 0xF0;
        //     var currentSoulsLoc = buffer + 0xF4;
        //     var requiredSouls = buffer + 0xFC;
        //     var soulsAfterLevelUp = buffer + 0xF8;
        //
        //     var giveSouls = Offsets.Funcs.GiveSouls;
        //     var levelLookUp = Offsets.Funcs.LevelLookup;
        //     var levelUp = Offsets.Funcs.LevelUp;
        //
        //     var statsEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PlayerCtrl,
        //         GameManagerImp.ChrCtrlOffsets.StatsPtr
        //     }, true);
        //
        //     var currentStatBytes = _memoryIo.ReadBytes(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.Vigor), 22);
        //     var currentLevel = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulLevel));
        //
        //     if (numOfLevels <= 0)
        //     {
        //         _memoryIo.WriteBytes(Patches.NegativeLevel + 1, new byte[] { 0x85 });
        //     }
        //
        //     _memoryIo.WriteByte(negativeFlag, numOfLevels <= 0 ? 1 : 0);
        //
        //     _memoryIo.WriteBytes(buffer, currentStatBytes);
        //     _memoryIo.WriteByte(numOfLevelsShortLoc, numOfLevels);
        //     _memoryIo.WriteInt32(numOfLevelsIntLoc, numOfLevels);
        //     _memoryIo.WriteInt32(currentLevelLoc, currentLevel);
        //     _memoryIo.WriteInt32(newLevelLoc, currentLevel + numOfLevels);
        //     var currentSouls = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.CurrentSouls));
        //     _memoryIo.WriteInt32(currentSoulsLoc, currentSouls);
        //
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         var bytes = AsmLoader.GetAsmBytes("LevelUp64");
        //
        //         AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
        //         {
        //             (levelLookUp, 0x18 + 2),
        //             (statsEntity.ToInt64(), 0x4D + 2),
        //             (giveSouls, 0x57 + 2),
        //             (statsEntity.ToInt64(), 0x6C + 2),
        //             (statsEntity.ToInt64(), 0x90 + 2),
        //             (levelUp, 0xA8 + 2)
        //         });
        //
        //         AsmHelper.WriteRelativeOffsets(bytes, new[]
        //         {
        //             (code.ToInt64(), currentLevelLoc.ToInt64(), 6, 0x0 + 2),
        //             (code.ToInt64() + 0xD, negativeFlag.ToInt64(), 7, 0xD + 2),
        //             (code.ToInt64() + 0x33, newLevelLoc.ToInt64(), 6, 0x33 + 2),
        //             (code.ToInt64() + 0x3B, requiredSouls.ToInt64(), 6, 0x3B + 2),
        //             (code.ToInt64() + 0x41, currentSoulsLoc.ToInt64(), 6, 0x41 + 2),
        //             (code.ToInt64() + 0x7C, currentSoulsLoc.ToInt64(), 6, 0x7C + 2),
        //             (code.ToInt64() + 0x82, requiredSouls.ToInt64(), 6, 0x82 + 2),
        //             (code.ToInt64() + 0x8A, soulsAfterLevelUp.ToInt64(), 6, 0x8A + 2),
        //             (code.ToInt64() + 0x9A, buffer.ToInt64(), 7, 0x9A + 3)
        //         });
        //
        //         _memoryIo.WriteBytes(code, bytes);
        //     }
        //     else
        //     {
        //         var bytes = AsmLoader.GetAsmBytes("LevelUp32");
        //         AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
        //         {
        //             (currentLevelLoc.ToInt64(), 2),
        //             (negativeFlag.ToInt64(), 0xC + 2),
        //             (levelLookUp, 0x15 + 1),
        //             (newLevelLoc.ToInt64(), 0x23 + 2),
        //             (requiredSouls.ToInt64(), 0x2B + 2),
        //             (currentSoulsLoc.ToInt64(), 0x31 + 2),
        //             (statsEntity.ToInt64(), 0x3E + 1),
        //             (giveSouls, 0x43 + 1),
        //             (statsEntity.ToInt64(), 0x4A + 1),
        //             (currentSoulsLoc.ToInt64(), 0x55 + 2),
        //             (requiredSouls.ToInt64(), 0x5B + 2),
        //             (soulsAfterLevelUp.ToInt64(), 0x63 + 2),
        //             (buffer.ToInt64(), 0x6F + 2),
        //             (statsEntity.ToInt64(), 0x76 + 1),
        //             (levelUp, 0x7B + 1)
        //         });
        //         _memoryIo.WriteBytes(code, bytes);
        //     }
        //
        //
        //     _memoryIo.RunThreadAndWaitForCompletion(code);
        //     if (numOfLevels <= 0) _memoryIo.WriteBytes(Patches.NegativeLevel + 1, new byte[] { 0x84 });
        //
        //     var newSouls = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.CurrentSouls));
        //     GiveSouls(currentSouls - newSouls);
        // }
        //
        // public int GetSoulLevel() => _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulLevel));
        //
        // public float GetPlayerSpeed() => _memoryIo.ReadFloat(GetSpeedPtr());
        //
        // public void SetPlayerSpeed(float speed) => _memoryIo.WriteFloat(GetSpeedPtr(), speed);
        //
        // private IntPtr GetSpeedPtr()
        // {
        //     return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PlayerCtrl,
        //         GameManagerImp.ChrCtrlOffsets.Speed
        //     }, false);
        // }
        //
        // public void ToggleNoGoodsConsume(bool isNoGoodsConsumeEnabled)
        // {
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         _memoryIo.WriteBytes(Patches.InfiniteGoods,
        //             isNoGoodsConsumeEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x66, 0x29, 0x73, 0x20 });
        //     }
        //     else
        //     {
        //         _memoryIo.WriteBytes(Patches.InfiniteGoods,
        //             isNoGoodsConsumeEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x66, 0x29, 0x5E, 0x18 }
        //         );
        //     }
        // }
        //
        // public void ToggleInfiniteCasts(bool isInfiniteCastsEnabled)
        // {
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         _memoryIo.WriteBytes(Patches.InfiniteCasts,
        //             isInfiniteCastsEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x88, 0x4D, 0x20 });
        //     }
        //     else
        //     {
        //         _memoryIo.WriteBytes(Patches.InfiniteCasts,
        //             isInfiniteCastsEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x88, 0x43, 0x18 });
        //     }
        // }
        //
        // public void ToggleInfiniteDurability(bool isInfiniteDuraEnabled)
        // {
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         _memoryIo.WriteBytes(Patches.InfiniteDurability,
        //             isInfiniteDuraEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0xF3, 0x0F, 0x11, 0xB4, 0xC3, 0x94, 0x00, 0x00, 0x00 });
        //     }
        //     else
        //     {
        //         _memoryIo.WriteBytes(Patches.InfiniteDurability,
        //             isInfiniteDuraEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, }
        //                 : new byte[] { 0xF3, 0x0F, 0x11, 0x47, 0x6C });
        //     }
        // }
        //
        // public void SavePos(int index)
        // {
        //     byte[] positionBytes = _memoryIo.ReadBytes(GetPositionPtr(), 0x40);
        //     if (index == 0)
        //         _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos1, positionBytes);
        //     else _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos2, positionBytes);
        // }
        //
        // public void RestorePos(int index)
        // {
        //     byte[] positionBytes;
        //     if (index == 0)
        //         positionBytes = _memoryIo.ReadBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos1, 0x40);
        //     else positionBytes = _memoryIo.ReadBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos2, 0x40);
        //     _memoryIo.WriteBytes(GetPositionPtr(), positionBytes);
        // }
        //
        // private IntPtr GetPositionPtr() =>
        //     _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PxWorldPtr,
        //         GameManagerImp.PxWorld.HkpWorld,
        //         GameManagerImp.PxWorld.HkpChrRigidBodyArray,
        //         GameManagerImp.PxWorld.HkpChrRigidBody,
        //         GameManagerImp.PxWorld.HkpRigidBodyPtr,
        //         GameManagerImp.PxWorld.PlayerCoords
        //     }, false);
        //
        // public (float x, float y, float z) GetCoords()
        // {
        //     var xyzPtr = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PlayerCtrl,
        //         GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrlPtr,
        //         GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrl.Xyz
        //     }, false);
        //
        //     var coordBytes = _memoryIo.ReadBytes(xyzPtr, 12);
        //     float x = BitConverter.ToSingle(coordBytes, 0);
        //     float z = BitConverter.ToSingle(coordBytes, 4);
        //     float y = BitConverter.ToSingle(coordBytes, 8);
        //     return (x, y, z);
        // }
        //
        // public void SetNewGame(int value) => _memoryIo.WriteByte(GetNewGamePtr(), value);
        //
        // public int GetNewGame() => _memoryIo.ReadUInt8(GetNewGamePtr());
        //
        // private IntPtr GetNewGamePtr() =>
        //     _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.GameDataManager,
        //         GameManagerImp.GameDataManagerOffsets.NewGamePtr,
        //         GameManagerImp.GameDataManagerOffsets.NewGame
        //     }, false);
        //
        // public void GiveSouls(int souls)
        // {
        //     var giveSoulsFunc = Offsets.Funcs.GiveSouls;
        //     var statsEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PlayerCtrl,
        //         GameManagerImp.ChrCtrlOffsets.StatsPtr
        //     }, true);
        //
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         var codeBytes = AsmLoader.GetAsmBytes("GiveSouls64");
        //         AsmHelper.WriteAbsoluteAddresses64(codeBytes, new[]
        //         {
        //             (statsEntity.ToInt64(), 0x0 + 2),
        //             (souls, 0x0A + 2),
        //             (giveSoulsFunc, 0x18 + 2)
        //         });
        //         _memoryIo.AllocateAndExecute(codeBytes);
        //     }
        //     else
        //     {
        //         var codeBytes = AsmLoader.GetAsmBytes("GiveSouls32");
        //         AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
        //         {
        //             (souls, 0x0 + 1),
        //             (statsEntity.ToInt64(), 0x06 + 1),
        //             (giveSoulsFunc, 0xb + 1)
        //         });
        //         _memoryIo.AllocateAndExecute(codeBytes);
        //     }
        // }
        //
        // public int GetSoulMemory() =>
        //     _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulMemory));
        //
        // public void RestoreSpellcasts()
        // {
        //     var func = Offsets.Funcs.RestoreSpellcasts;
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         var inventoryBag = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //         {
        //             GameManagerImp.Offsets.GameDataManager,
        //             GameManagerImp.GameDataManagerOffsets.InventoryPtr,
        //             GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListForSpells
        //         }, true);
        //         
        //         var codeBytes = AsmLoader.GetAsmBytes("RestoreSpellcasts64");
        //         AsmHelper.WriteAbsoluteAddresses64(codeBytes, new[]
        //         {
        //             (inventoryBag.ToInt64(), 0x0 + 2),
        //             (func, 0x20 + 2)
        //         });
        //
        //         _memoryIo.AllocateAndExecute(codeBytes);
        //     }
        //     else
        //     {
        //         var inventoryBag = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //         {
        //             GameManagerImp.Offsets.GameDataManager,
        //             GameManagerImp.GameDataManagerOffsets.InventoryPtr,
        //             GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists
        //         }, true);
        //         
        //         var codeBytes = AsmLoader.GetAsmBytes("RestoreSpellcasts32");
        //         AsmHelper.WriteAbsoluteAddresses32(codeBytes, new []
        //         {
        //             (inventoryBag.ToInt64(), 1),
        //             (func, 0x1E + 1)
        //         });
        //         _memoryIo.AllocateAndExecute(codeBytes);
        //     }
        //     
        // }
        //
        // public void ToggleSilent(bool isSilentEnabled)
        // {
        //     if (isSilentEnabled)
        //     {
        //         _nopManager.InstallNop(Patches.Silent.ToInt64(),
        //             GameVersion.Current.Edition == GameEdition.Scholar ? 5 : 16);
        //     }
        //     else _nopManager.RestoreNop(Patches.Silent.ToInt64());
        // }
        //
        // public void ToggleHidden(bool isHiddenEnabled) =>
        //     _memoryIo.WriteBytes(Patches.Hidden + 1, isHiddenEnabled ? new byte[] { 0x85 } : new byte[] { 0x84 });
        //
        public void ToggleInfinitePoise(bool isInfinitePoiseEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.InfinitePoise;

            if (isInfinitePoiseEnabled)
            {
                var origin = Hooks.InfinitePoise;
                var hook = Hooks.InfinitePoise;
                var codeBytes = AsmLoader.GetAsmBytes("InfinitePoise");
                var bytes = BitConverter.GetBytes(WorldChrMan.Base.ToInt64());
                Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                AsmHelper.WriteJumpOffsets(codeBytes, new[]
                {
                    (hook, 7, code + 0x1D, 0x1D + 1),
                    (hook, 7, code + 0x2A, 0x2A + 1),
                });
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x80, 0xBF, 0x5F, 0x02, 0x00, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        //
        // public void ToggleAutoSetNg7(bool isAutoSetNewGameSevenEnabled) =>
        //     _memoryIo.WriteByte(Patches.Ng7 + 3, isAutoSetNewGameSevenEnabled ? 9 : 1);
        //
        // public void SetSpEffect(GameIds.SpEffects.SpEffectData restoreHumanity)
        // {
        //     var spEffectParams = CodeCaveOffsets.Base + CodeCaveOffsets.SpEffectParams;
        //     var code = CodeCaveOffsets.Base + CodeCaveOffsets.SpEffectCode;
        //
        //     var chrSpEffectCtrl = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        //     {
        //         GameManagerImp.Offsets.PlayerCtrl,
        //         GameManagerImp.ChrCtrlOffsets.ChrSpEffectCtrl
        //     }, true);
        //
        //     var setEffectFunc = Offsets.Funcs.SetSpEffect;
        //
        //     _memoryIo.WriteInt32(spEffectParams, restoreHumanity.EffectId);
        //     _memoryIo.WriteInt32(spEffectParams + 0x4, restoreHumanity.Quantity);
        //     _memoryIo.WriteFloat(spEffectParams + 0x8, restoreHumanity.FloatValue);
        //     _memoryIo.WriteByte(spEffectParams + 0xC, restoreHumanity.EffectType);
        //     _memoryIo.WriteByte(spEffectParams + 0xD, restoreHumanity.Param1);
        //     _memoryIo.WriteByte(spEffectParams + 0xE, restoreHumanity.Param2);
        //     _memoryIo.WriteByte(spEffectParams + 0xF, restoreHumanity.Param3);
        //
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         var codeBytes = AsmLoader.GetAsmBytes("SetSpEffect64");
        //         var bytes = BitConverter.GetBytes(chrSpEffectCtrl.ToInt64());
        //         Array.Copy(bytes, 0, codeBytes, 0x7 + 2, 8);
        //         AsmHelper.WriteRelativeOffsets(codeBytes, new[]
        //         {
        //             (code.ToInt64(), spEffectParams.ToInt64(), 7, 0x0 + 3),
        //             (code.ToInt64() + 0x15, setEffectFunc, 5, 0x15 + 1)
        //         });
        //
        //         _memoryIo.WriteBytes(code, codeBytes);
        //     }
        //     else
        //     {
        //         var codeBytes = AsmLoader.GetAsmBytes("SetSpEffect32");
        //
        //         var bytes = BitConverter.GetBytes(spEffectParams.ToInt32());
        //         Array.Copy(bytes, 0, codeBytes, 0x3 + 2, 4);
        //         bytes = BitConverter.GetBytes(chrSpEffectCtrl.ToInt32());
        //         Array.Copy(bytes, 0, codeBytes, 0xA + 1, 4);
        //
        //         AsmHelper.WriteRelativeOffsets(codeBytes, new[]
        //         {
        //             (code.ToInt64() + 0xF, setEffectFunc, 5, 0xF + 1)
        //         });
        //
        //         _memoryIo.WriteBytes(code, codeBytes);
        //     }
        //
        //     _memoryIo.RunThread(code);
        // }
        //
        // public void ToggleNoSoulGain(bool isEnabled)
        // {
        //     if (isEnabled) _nopManager.InstallNop(Patches.NoSoulGain.ToInt64(), 5);
        //     else _nopManager.RestoreNop(Patches.NoSoulGain.ToInt64());
        // }
        //
        // public void ToggleNoHollowing(bool isEnabled)
        // {
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         _memoryIo.WriteBytes(Patches.NoHollowing,
        //             isEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x88, 0x81, 0xAC, 0x01, 0x00, 0x00 }
        //         );
        //     }
        //     else
        //     {
        //         _memoryIo.WriteBytes(Patches.NoHollowing,
        //             isEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x88, 0x91, 0xA8, 0x01, 0x00, 0x00 }
        //         );
        //     }
        // }
        //
        // public void ToggleNoSoulLoss(bool isEnabled)
        // {
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         _memoryIo.WriteBytes(Patches.NoSoulLoss,
        //             isEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0x89, 0x90, 0xEC, 0x00, 0x00, 0x00 }
        //         );
        //     }
        //     else
        //     {
        //         _memoryIo.WriteBytes(Patches.NoSoulLoss,
        //             isEnabled
        //                 ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
        //                 : new byte[] { 0xC7, 0x80, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
        //         );
        //     }
        // }
        //
        // public void ToggleSoulMemWrite(bool isEnabled)
        // {
        //     if (GameVersion.Current.Edition == GameEdition.Scholar)
        //     {
        //         if (isEnabled)
        //         {
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite1, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite2, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
        //         }
        //         else
        //         {
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite1, new byte[] { 0x89, 0x81, 0xF4, 0x00, 0x00, 0x00 });
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite2, new byte[] { 0x89, 0x81, 0xFC, 0x00, 0x00, 0x00 });
        //         }
        //     }
        //     else
        //     {
        //         if (isEnabled)
        //         {
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite1, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite2, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
        //         }
        //         else
        //         {
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite1, new byte[] { 0x89, 0x81, 0xF0, 0x00, 0x00, 0x00 });
        //             _memoryIo.WriteBytes(Patches.SoulMemWrite2, new byte[] { 0x89, 0x91, 0xF8, 0x00, 0x00, 0x00 });
        //         }
        //     }
        // }
        public void ApplySpEffect(long spEffectId)
        {
            var bytes = AsmLoader.GetAsmBytes("SetSpEffect");
            var playerIns =
                _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(WorldChrMan.Base) + WorldChrMan.Offsets.PlayerInsPtr);
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (playerIns, 0x0 + 2),
                (spEffectId, 0xA + 2),
                (Funcs.SetSpEffect, 0x18 + 2)
            });
            _memoryIo.AllocateAndExecute(bytes);
        }

        public void ToggleChrDataFlag(int offset, byte bitmask, bool isEnabled)
        {
            var chrData = _memoryIo.FollowPointers(WorldChrMan.Base, new[]
            {
                (WorldChrMan.Offsets.PlayerInsPtr),
                (WorldChrMan.Offsets.ChrIns.ModulesPtr),
                (WorldChrMan.Offsets.ChrIns.Modules.ChrDataPtr)
            }, true);

            _memoryIo.SetBitValue(chrData + offset, bitmask, isEnabled);
        }

        public void ToggleDebugFlag(int offset, bool isEnabled) =>
            _memoryIo.WriteByte(WorldChrManDbg.Base + offset, isEnabled ? 1 : 0);

        public void ToggleNoRuneGain(bool isNoRuneGainEnabled) =>
            _memoryIo.WriteBytes(Patches.NoRunesFromEnemies,
                isNoRuneGainEnabled
                    ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                    : new byte[] { 0x41, 0xFF, 0x91, 0xC8, 0x05, 0x00, 0x00 });

        public void ToggleNoRuneArcLoss(bool isNoRuneArcLossEnabled) =>
            _memoryIo.WriteByte(Patches.NoRuneArcLoss, isNoRuneArcLossEnabled ? 0xEB : 0x74);

        public void ToggleNoRuneLoss(bool isNoRuneLossEnabled) => 
        _memoryIo.WriteBytes(Patches.NoRuneLossOnDeath, 
            isNoRuneLossEnabled
                ? new byte[] { 0x90, 0x90, 0x90 }
                : new byte[] { 0x89, 0x45, 0x6C});
    }
}