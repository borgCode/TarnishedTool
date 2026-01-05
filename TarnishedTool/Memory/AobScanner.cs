using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TarnishedTool.Services;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Memory
{
    public class AoBScanner(MemoryService memoryService)
    {
        

        public void Scan()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TarnishedTool");
            Directory.CreateDirectory(appData);
            string savePath = Path.Combine(appData, "backup_addresses.txt");

            ConcurrentDictionary<string, long> saved = new ConcurrentDictionary<string, long>();
            if (File.Exists(savePath))
            {
                foreach (string line in File.ReadAllLines(savePath))
                {
                    string[] parts = line.Split('=');
                    saved[parts[0]] = Convert.ToInt64(parts[1], 16);
                }
            }


            Parallel.Invoke(
                () => WorldChrMan.Base = FindAddressByPattern(Pattern.WorldChrMan),
                () => FieldArea.Base = FindAddressByPattern(Pattern.FieldArea),
                () => LuaEventMan.Base = FindAddressByPattern(Pattern.LuaEventMan),
                () => VirtualMemFlag.Base = FindAddressByPattern(Pattern.VirtualMemFlag),
                () => DamageManager.Base = FindAddressByPattern(Pattern.DamageManager),
                () => MenuMan.Base = FindAddressByPattern(Pattern.MenuMan),
                () => TargetView.Base = FindAddressByPattern(Pattern.TargetView),
                () => GameMan.Base = FindAddressByPattern(Pattern.GameMan),
                () => WorldHitMan.Base = FindAddressByPattern(Pattern.WorldHitMan),
                () => WorldChrManDbg.Base = FindAddressByPattern(Pattern.WorldChrManDbg),
                () => GameDataMan.Base = FindAddressByPattern(Pattern.GameDataMan),
                () => CsDlcImp.Base = FindAddressByPattern(Pattern.CsDlcImp),
                () => MapItemManImpl.Base = FindAddressByPattern(Pattern.MapItemManImpl),
                () => FD4PadManager.Base = FindAddressByPattern(Pattern.FD4PadManager),
                () => CSEmkSystem.Base = FindAddressByPattern(Pattern.CSEmkSystem),
                () => WorldAreaTimeImpl.Base = FindAddressByPattern(Pattern.WorldAreaTimeImpl),
                () => GroupMask.Base = FindAddressByPattern(Pattern.GroupMask),
                () => SoloParamRepositoryImp.Base = FindAddressByPattern(Pattern.SoloParamRepositoryImp),
                () => MsgRepository.Base = FindAddressByPattern(Pattern.MsgRepository),
                () => CSFlipperImp.Base = FindAddressByPattern(Pattern.CSFlipperImp),
                () => CSDbgEvent.Base = FindAddressByPattern(Pattern.CSDbgEvent),
                () => UserInputManager.Base = FindAddressByPattern(Pattern.UserInputManager),
                () => CSTrophy.Base = FindAddressByPattern(Pattern.CSTrophy),
                () => DrawPathing.Base = FindAddressByChain(Pattern.DrawPathing,
                    AddressJump.Relative(0, 3, 7),
                    AddressJump.Absolute(0xA, 2),
                    AddressJump.Relative(0, 2, 6)
                    ),
                () => MapDebugFlags.Base = FindAddressByPattern(Pattern.MapDebugFlags),
                () => Functions.GraceWarp = FindAddressByPattern(Pattern.GraceWarp).ToInt64(),
                () => Functions.SetEvent = FindAddressByPattern(Pattern.SetEvent).ToInt64(),
                () => Functions.SetSpEffect = FindAddressByPattern(Pattern.SetSpEffect).ToInt64(),
                () => Functions.GiveRunes = FindAddressByPattern(Pattern.GiveRunes).ToInt64(),
                () => Functions.LookupByFieldInsHandle = FindAddressByPattern(Pattern.LookupByFieldInsHandle).ToInt64(),
                () => Functions.WarpToBlock = FindAddressByPattern(Pattern.WarpToBlock).ToInt64(),
                () => Functions.ExternalEventTempCtor = FindAddressByPattern(Pattern.ExternalEventTempCtor).ToInt64(),
                () => Functions.ExecuteTalkCommand = FindAddressByPattern(Pattern.ExecuteTalkCommand).ToInt64(),
                () => Functions.GetEvent = FindAddressByPattern(Pattern.GetEvent).ToInt64(),
                () => Functions.GetPlayerItemQuantityById =
                    FindAddressByPattern(Pattern.GetPlayerItemQuantityById).ToInt64(),
                () => Functions.ItemSpawn = FindAddressByPattern(Pattern.ItemSpawn).ToInt64(),
                () => Functions.MatrixVectorProduct = FindAddressByPattern(Pattern.MatrixVectorProduct).ToInt64(),
                () => Functions.ChrInsByHandle = FindAddressByPattern(Pattern.ChrInsByHandle).ToInt64(),
                () => Functions.FindAndRemoveSpEffect = FindAddressByPattern(Pattern.FindAndRemoveSpEffect).ToInt64(),
                () => Functions.EmevdSwitch = FindAddressByPattern(Pattern.EmevdSwitch).ToInt64(),
                () => Functions.EmkEventInsCtor = FindAddressByPattern(Pattern.EmkEventInsCtor).ToInt64(),
                () => Functions.GetMovement = FindAddressByPattern(Pattern.GetMovement).ToInt64(),
                () => Functions.GetChrInsByEntityId = FindAddressByPattern(Pattern.GetChrInsByEntityId).ToInt64(),
                () => Functions.NpcEzStateTalkCtor = FindAddressByPattern(Pattern.NpcEzStateTalkCtor).ToInt64(),
                () => Functions.EzStateEnvQueryImplCtor =
                    FindAddressByPattern(Pattern.EzStateEnvQueryImplCtor).ToInt64()
            );


            Parallel.Invoke(
                () => TryPatternWithFallback("CanFastTravel", Pattern.CanFastTravel,
                    addr => Patches.CanFastTravel = addr, saved),
                () => TryPatternWithFallback("NoRunesFromEnemies", Pattern.NoRunesFromEnemies,
                    addr => Patches.NoRunesFromEnemies = addr, saved),
                () => TryPatternWithFallback("NoRuneArcLoss", Pattern.NoRuneArcLoss,
                    addr => Patches.NoRuneArcLoss = addr, saved),
                () => TryPatternWithFallback("NoRuneLossOnDeath", Pattern.NoRuneLossOnDeath,
                    addr => Patches.NoRuneLossOnDeath = addr, saved),
                () => TryPatternWithFallback("IsWorldPaused", Pattern.IsWorldPaused,
                    addr => Patches.IsWorldPaused = addr, saved),
                () => TryPatternWithFallback("GetItemChance", Pattern.GetItemChance,
                    addr => Patches.GetItemChance = addr, saved),
                () => TryPatternWithFallback("OpenMap", Pattern.OpenMap, addr => Patches.OpenMap = addr, saved),
                () => TryPatternWithFallback("CloseMap", Pattern.CloseMap, addr => Patches.CloseMap = addr, saved),
                () => TryPatternWithFallback("NoLogo", Pattern.NoLogo, addr => Patches.NoLogo = addr, saved),
                () => TryPatternWithFallback("PlayerSound", Pattern.PlayerSound, addr => Patches.PlayerSound = addr,
                    saved),
                () => TryPatternWithFallback("IsTorrentDisabledInUnderworld", Pattern.IsTorrentDisabledInUnderworld, addr => Patches.IsTorrentDisabledInUnderworld = addr,
                    saved),
                () => TryPatternWithFallback("IsWhistleDisabled", Pattern.IsWhistleDisabled, addr => Patches.IsWhistleDisabled = addr,
                    saved),
                () => TryPatternWithFallback("UpdateCoords", Pattern.UpdateCoords,
                    addr => Hooks.UpdateCoords = addr.ToInt64(), saved),
                () => TryPatternWithFallback("InAirTimer", Pattern.InAirTimer,
                    addr => Hooks.InAirTimer = addr.ToInt64(), saved),
                () => TryPatternWithFallback("NoClipKb", Pattern.NoClipKb, addr => Hooks.NoClipKb = addr.ToInt64(),
                    saved),
                () => TryPatternWithFallback("NoClipTriggers", Pattern.NoClipTriggers,
                    addr => Hooks.NoClipTriggers = addr.ToInt64(), saved),
                () => TryPatternWithFallback("HasSpEffect", Pattern.HasSpEffect,
                    addr => Hooks.HasSpEffect = addr.ToInt64(), saved),
                () => TryPatternWithFallback("BlueTargetView", Pattern.BlueTargetViewHook,
                    addr => Hooks.BlueTargetView = addr.ToInt64(), saved),
                () => TryPatternWithFallback("LockedTargetPtr", Pattern.LockedTargetPtr,
                    addr => Hooks.LockedTargetPtr = addr.ToInt64(), saved),
                () => TryPatternWithFallback("InfinitePoise", Pattern.InfinitePoise,
                    addr => Hooks.InfinitePoise = addr.ToInt64(), saved),
                () => TryPatternWithFallback("ShouldUpdateAi", Pattern.ShouldUpdateAi,
                    addr => Hooks.ShouldUpdateAi = addr.ToInt64(), saved),
                () => TryPatternWithFallback("GetForceActIdx", Pattern.GetForceActIdx,
                    addr => Hooks.GetForceActIdx = addr.ToInt64(), saved),
                () => TryPatternWithFallback("NoStagger", Pattern.NoStagger,
                    addr => Hooks.TargetNoStagger = addr.ToInt64(), saved),
                () => TryPatternWithFallback("AttackInfo", Pattern.AttackInfo,
                    addr => Hooks.AttackInfo = addr.ToInt64(), saved),
                () => TryPatternWithFallback("WarpCoordWrite", Pattern.WarpCoordWrite,
                    addr => Hooks.WarpCoordWrite = addr.ToInt64(), saved),
                () => TryPatternWithFallback("WarpAngleWrite", Pattern.WarpAngleWrite,
                    addr => Hooks.WarpAngleWrite = addr.ToInt64(), saved),
                () => TryPatternWithFallback("LionCooldownHook", Pattern.LionCooldownHook,
                    addr => Hooks.LionCooldownHook = addr.ToInt64(), saved),
                () => TryPatternWithFallback("SetActionRequested", Pattern.SetActionRequested,
                    addr => Hooks.SetActionRequested = addr.ToInt64(), saved),
                () => TryPatternWithFallback("NoMapAcquiredPopup", Pattern.NoMapAcquiredPopup,
                    addr => Hooks.NoMapAcquiredPopup = addr.ToInt64(), saved),
                () => TryPatternWithFallback("LoadScreenMsgLookup", Pattern.LoadScreenMsgLookup,
                    addr => Hooks.LoadScreenMsgLookup = addr.ToInt64(), saved),
                () => TryPatternWithFallback("NoGrab", Pattern.NoGrab,
                    addr => Hooks.NoGrab = addr.ToInt64(), saved)
            );

            Patches.EnableFreeCam = FindAddressByPattern(Pattern.EnableFreeCam);
            Patches.GetShopEvent = FindAddressByPattern(Pattern.GetShopEvent);
            Patches.DebugFont = FindAddressByPattern(Pattern.DebugFont);
            FindMultipleCallsInFunction(Pattern.CanDrawEvents, new Dictionary<Action<long>, int>
            {
                { addr => Patches.CanDrawEvents1 = (IntPtr)addr, 0x4 },
                { addr => Patches.CanDrawEvents2 = (IntPtr)addr, 0xD },
            });


            using (var writer = new StreamWriter(savePath))
            {
                foreach (var pair in saved)
                    writer.WriteLine($"{pair.Key}={pair.Value:X}");
            }

            Hooks.HookedDeathFunction = Patches.NoRuneLossOnDeath - 7;


#if DEBUG
            Console.WriteLine($@"WorldChrMan.Base: 0x{WorldChrMan.Base.ToInt64():X}");
            Console.WriteLine($@"FieldArea.Base: 0x{FieldArea.Base.ToInt64():X}");
            Console.WriteLine($@"LuaEventMan.Base: 0x{LuaEventMan.Base.ToInt64():X}");
            Console.WriteLine($@"VirtualMemFlag.Base: 0x{VirtualMemFlag.Base.ToInt64():X}");
            Console.WriteLine($@"DamageManager.Base: 0x{DamageManager.Base.ToInt64():X}");
            Console.WriteLine($@"MenuMan.Base: 0x{MenuMan.Base.ToInt64():X}");
            Console.WriteLine($@"TargetView.Base: 0x{TargetView.Base.ToInt64():X}");
            Console.WriteLine($@"GameMan.Base: 0x{GameMan.Base.ToInt64():X}");
            Console.WriteLine($@"WorldHitMan.Base: 0x{WorldHitMan.Base.ToInt64():X}");
            Console.WriteLine($@"WorldChrManDbg.Base: 0x{WorldChrManDbg.Base.ToInt64():X}");
            Console.WriteLine($@"GameDataMan.Base: 0x{GameDataMan.Base.ToInt64():X}");
            Console.WriteLine($@"CsDlcImp.Base: 0x{CsDlcImp.Base.ToInt64():X}");
            Console.WriteLine($@"MapItemManImpl.Base: 0x{MapItemManImpl.Base.ToInt64():X}");
            Console.WriteLine($@"InputManager.Base: 0x{FD4PadManager.Base.ToInt64():X}");
            Console.WriteLine($@"CSEmkSystem.Base: 0x{CSEmkSystem.Base.ToInt64():X}");
            Console.WriteLine($@"WorldAreaTimeImpl.Base: 0x{WorldAreaTimeImpl.Base.ToInt64():X}");
            Console.WriteLine($@"GroupMask.Base: 0x{GroupMask.Base.ToInt64():X}");
            Console.WriteLine($@"CSFlipperImp.Base: 0x{CSFlipperImp.Base.ToInt64():X}");
            Console.WriteLine($@"CSDbgEvent.Base: 0x{CSDbgEvent.Base.ToInt64():X}");
            Console.WriteLine($@"UserInputManager.Base: 0x{UserInputManager.Base.ToInt64():X}");
            Console.WriteLine($@"CSTrophy.Base: 0x{CSTrophy.Base.ToInt64():X}");
            Console.WriteLine($@"MapDebugFlags.Base: 0x{MapDebugFlags.Base.ToInt64():X}");
            Console.WriteLine($@"SoloParamRepositoryImp.Base: 0x{SoloParamRepositoryImp.Base.ToInt64():X}");
            Console.WriteLine($@"MsgRepository.Base: 0x{MsgRepository.Base.ToInt64():X}");
            Console.WriteLine($@"DrawPathing.Base: 0x{DrawPathing.Base.ToInt64():X}");

            Console.WriteLine($@"Patches.NoLogo: 0x{Patches.NoLogo.ToInt64():X}");
            Console.WriteLine($@"Patches.NoRunesFromEnemies: 0x{Patches.NoRunesFromEnemies.ToInt64():X}");
            Console.WriteLine($@"Patches.NoRuneArcLoss: 0x{Patches.NoRuneArcLoss.ToInt64():X}");
            Console.WriteLine($@"Patches.NoRuneLossOnDeath: 0x{Patches.NoRuneLossOnDeath.ToInt64():X}");
            Console.WriteLine($@"Patches.CanFastTravel: 0x{Patches.CanFastTravel.ToInt64():X}");
            Console.WriteLine($@"Patches.OpenMap: 0x{Patches.OpenMap.ToInt64():X}");
            Console.WriteLine($@"Patches.CloseMap: 0x{Patches.CloseMap.ToInt64():X}");
            Console.WriteLine($@"Patches.EnableFreeCam: 0x{Patches.EnableFreeCam.ToInt64():X}");
            Console.WriteLine($@"Patches.CanDrawEvents1: 0x{Patches.CanDrawEvents1.ToInt64():X}");
            Console.WriteLine($@"Patches.CanDrawEvents2: 0x{Patches.CanDrawEvents2.ToInt64():X}");
            Console.WriteLine($@"Patches.NoLogo: 0x{Patches.NoLogo.ToInt64():X}");
            Console.WriteLine($@"Patches.DebugFont: 0x{Patches.DebugFont.ToInt64():X}");
            Console.WriteLine($@"Patches.PlayerSound: 0x{Patches.PlayerSound.ToInt64():X}");
            Console.WriteLine($@"Patches.IsTorrentDisabledInUnderworld: 0x{Patches.IsTorrentDisabledInUnderworld.ToInt64():X}");
            Console.WriteLine($@"Patches.IsWhistleDisabled: 0x{Patches.IsWhistleDisabled.ToInt64():X}");
            Console.WriteLine($@"Patches.IsWorldPaused: 0x{Patches.IsWorldPaused.ToInt64():X}");
            Console.WriteLine($@"Patches.GetItemChance: 0x{Patches.GetItemChance.ToInt64():X}");

            Console.WriteLine($@"Hooks.UpdateCoords: 0x{Hooks.UpdateCoords:X}");
            Console.WriteLine($@"Hooks.InAirTimer: 0x{Hooks.InAirTimer:X}");
            Console.WriteLine($@"Hooks.NoClipKb: 0x{Hooks.NoClipKb:X}");
            Console.WriteLine($@"Hooks.NoClipTriggers: 0x{Hooks.NoClipTriggers:X}");
            Console.WriteLine($@"Hooks.HasSpEffect: 0x{Hooks.HasSpEffect:X}");
            Console.WriteLine($@"Hooks.BlueTargetView: 0x{Hooks.BlueTargetView:X}");
            Console.WriteLine($@"Hooks.LockedTargetPtr: 0x{Hooks.LockedTargetPtr:X}");
            Console.WriteLine($@"Hooks.InfinitePoise: 0x{Hooks.InfinitePoise:X}");
            Console.WriteLine($@"Hooks.ShouldUpdateAi: 0x{Hooks.ShouldUpdateAi:X}");
            Console.WriteLine($@"Hooks.GetForceActIdx: 0x{Hooks.GetForceActIdx:X}");
            Console.WriteLine($@"Hooks.NoStagger: 0x{Hooks.TargetNoStagger:X}");
            Console.WriteLine($@"Hooks.AttackInfo: 0x{Hooks.AttackInfo:X}");
            Console.WriteLine($@"Hooks.WarpCoordWrite: 0x{Hooks.WarpCoordWrite:X}");
            Console.WriteLine($@"Hooks.WarpAngleWrite: 0x{Hooks.WarpAngleWrite:X}");
            Console.WriteLine($@"Hooks.HookedDeathFunction: 0x{Hooks.HookedDeathFunction.ToInt64():X}");
            Console.WriteLine($@"Hooks.LionCooldownHook: 0x{Hooks.LionCooldownHook:X}");
            Console.WriteLine($@"Hooks.SetActionRequested: 0x{Hooks.SetActionRequested:X}");
            Console.WriteLine($@"Hooks.NoGrab: 0x{Hooks.NoGrab:X}");
            Console.WriteLine($@"Hooks.LoadScreenMsgLookup: 0x{Hooks.LoadScreenMsgLookup:X}");

            Console.WriteLine($@"Funcs.GraceWarp: 0x{Functions.GraceWarp:X}");
            Console.WriteLine($@"Funcs.SetEvent: 0x{Functions.SetEvent:X}");
            Console.WriteLine($@"Funcs.SetSpEffect: 0x{Functions.SetSpEffect:X}");
            Console.WriteLine($@"Funcs.GiveRunes: 0x{Functions.GiveRunes:X}");
            Console.WriteLine($@"Funcs.LookupByFieldInsHandle: 0x{Functions.LookupByFieldInsHandle:X}");
            Console.WriteLine($@"Funcs.WarpToBlock: 0x{Functions.WarpToBlock:X}");
            Console.WriteLine($@"Funcs.GetEvent: 0x{Functions.GetEvent:X}");
            Console.WriteLine($@"Funcs.GetPlayerItemQuantityById: 0x{Functions.GetPlayerItemQuantityById:X}");
            Console.WriteLine($@"Funcs.ItemSpawn: 0x{Functions.ItemSpawn:X}");
            Console.WriteLine($@"Funcs.MatrixVectorProduct: 0x{Functions.MatrixVectorProduct:X}");
            Console.WriteLine($@"Funcs.ChrInsByHandle: 0x{Functions.ChrInsByHandle:X}");
            Console.WriteLine($@"Funcs.FindAndRemoveSpEffect: 0x{Functions.FindAndRemoveSpEffect:X}");
            Console.WriteLine($@"Funcs.EmevdSwitch: 0x{Functions.EmevdSwitch:X}");
            Console.WriteLine($@"Funcs.EmkEventInsCtor: 0x{Functions.EmkEventInsCtor:X}");
            Console.WriteLine($@"Funcs.GetMovement: 0x{Functions.GetMovement:X}");
            Console.WriteLine($@"Funcs.GetChrInsByEntityId: 0x{Functions.GetChrInsByEntityId:X}");
#endif
        }

        private void TryPatternWithFallback(string name, Pattern pattern, Action<IntPtr> setter,
            ConcurrentDictionary<string, long> saved)
        {
            var addr = FindAddressByPattern(pattern);

            if (addr == IntPtr.Zero && saved.TryGetValue(name, out var value))
                addr = new IntPtr(value);
            else if (addr != IntPtr.Zero)
                saved[name] = addr.ToInt64();

            setter(addr);
        }

        public IntPtr FindAddressByPattern(Pattern pattern)
        {
            var results = FindAddressesByPattern(pattern, 1);
            return results.Count > 0 ? results[0] : IntPtr.Zero;
        }

        public List<IntPtr> FindAddressesByPattern(Pattern pattern, int size)
        {
            List<IntPtr> addresses = PatternScanMultiple(pattern.Bytes, pattern.Mask, size);

            for (int i = 0; i < addresses.Count; i++)
            {
                IntPtr instructionAddress = IntPtr.Add(addresses[i], pattern.InstructionOffset);

                if (pattern.AddressingMode == AddressingMode.Absolute)
                {
                    addresses[i] = instructionAddress;
                }
                else
                {
                    int offset = memoryService.ReadInt32(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                    addresses[i] = IntPtr.Add(instructionAddress, offset + pattern.InstructionLength);
                }
            }

            return addresses;
        }

        private List<IntPtr> PatternScanMultiple(byte[] pattern, string mask, int size)
        {
            const int chunkSize = 4096 * 16;
            byte[] buffer = new byte[chunkSize];

            IntPtr currentAddress = memoryService.BaseAddress;
            int memSize = memoryService.ModuleMemorySize;
            IntPtr endAddress = IntPtr.Add(currentAddress, memSize);

            List<IntPtr> addresses = new List<IntPtr>();

            while (currentAddress.ToInt64() < endAddress.ToInt64())
            {
                int bytesRemaining = (int)(endAddress.ToInt64() - currentAddress.ToInt64());
                int bytesToRead = Math.Min(bytesRemaining, buffer.Length);

                if (bytesToRead < pattern.Length)
                    break;

                buffer = memoryService.ReadBytes(currentAddress, bytesToRead);

                for (int i = 0; i <= bytesToRead - pattern.Length; i++)
                {
                    bool found = true;

                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (j < mask.Length && mask[j] == '?')
                            continue;

                        if (buffer[i + j] != pattern[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        addresses.Add(IntPtr.Add(currentAddress, i));
                    if (addresses.Count == size) break;
                }

                currentAddress = IntPtr.Add(currentAddress, bytesToRead - pattern.Length + 1);
            }

            return addresses;
        }

        private void FindMultipleCallsInFunction(Pattern basePattern, Dictionary<Action<long>, int> callMappings)
        {
            var baseInstructionAddr = FindAddressByPattern(basePattern);

            foreach (var mapping in callMappings)
            {
                var callInstructionAddr = IntPtr.Add(baseInstructionAddr, mapping.Value);

                int callOffset = memoryService.ReadInt32(IntPtr.Add(callInstructionAddr, 1));
                var callTarget = IntPtr.Add(callInstructionAddr, callOffset + 5);

                mapping.Key(callTarget.ToInt64());
            }
        }

        public IntPtr FindAddressByChain(Pattern pattern, params AddressJump[] chain)
        {
            var baseAddress = FindAddressByPattern(pattern);
            if (baseAddress == IntPtr.Zero)
                return IntPtr.Zero;

            return FollowChain(baseAddress, chain);
        }

        private IntPtr FollowChain(IntPtr baseAddress, params AddressJump[] chain)
        {
            IntPtr currentAddress = baseAddress;

            foreach (var jump in chain)
            {
                IntPtr instructionAddress = IntPtr.Add(currentAddress, jump.Offset);

                currentAddress = jump.Type switch
                {
                    AddressJump.JumpType.Relative32 => IntPtr.Add(
                        instructionAddress,
                        memoryService.ReadInt32(IntPtr.Add(instructionAddress, jump.ImmediatePosition)) + jump.InstructionLength),

                    AddressJump.JumpType.Absolute64 => new IntPtr(
                        memoryService.ReadInt64(IntPtr.Add(instructionAddress, jump.ImmediatePosition))),

                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return currentAddress;
        }
    }
}