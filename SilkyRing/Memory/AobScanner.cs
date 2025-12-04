using System;
using System.Collections.Generic;
using System.IO;
using SilkyRing.Services;
using static SilkyRing.Memory.Offsets;


namespace SilkyRing.Memory
{
    public class AoBScanner
    {
        private readonly MemoryService _memoryService;

        public AoBScanner(MemoryService memoryService)
        {
            _memoryService = memoryService;
        }

        public void Scan()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SilkyRing");
            Directory.CreateDirectory(appData);
            string savePath = Path.Combine(appData, "backup_addresses.txt");

            Dictionary<string, long> saved = new Dictionary<string, long>();
            if (File.Exists(savePath))
            {
                foreach (string line in File.ReadAllLines(savePath))
                {
                    string[] parts = line.Split('=');
                    saved[parts[0]] = Convert.ToInt64(parts[1], 16);
                }
            }


            WorldChrMan.Base = FindAddressByPattern(Pattern.WorldChrMan);
            FieldArea.Base = FindAddressByPattern(Pattern.FieldArea);
            LuaEventMan.Base = FindAddressByPattern(Pattern.LuaEventMan);
            VirtualMemFlag.Base = FindAddressByPattern(Pattern.VirtualMemFlag);
            DamageManager.Base = FindAddressByPattern(Pattern.DamageManager);
            MenuMan.Base = FindAddressByPattern(Pattern.MenuMan);
            TargetView.Base = FindAddressByPattern(Pattern.TargetView);
            GameMan.Base = FindAddressByPattern(Pattern.GameMan);
            WorldHitMan.Base = FindAddressByPattern(Pattern.WorldHitMan);
            WorldChrManDbg.Base = FindAddressByPattern(Pattern.WorldChrManDbg);
            GameDataMan.Base = FindAddressByPattern(Pattern.GameDataMan);


            TryPatternWithFallback("DungeonWarp", Pattern.DungeonWarp, addr => Patches.DungeonWarp = addr, saved);
            TryPatternWithFallback("NoRunesFromEnemies", Pattern.NoRunesFromEnemies,
                addr => Patches.NoRunesFromEnemies = addr, saved);
            TryPatternWithFallback("NoRuneArcLoss", Pattern.NoRuneArcLoss, addr => Patches.NoRuneArcLoss = addr, saved);
            TryPatternWithFallback("NoRuneLossOnDeath", Pattern.NoRuneLossOnDeath,
                addr => Patches.NoRuneLossOnDeath = addr, saved);
            // TryPatternWithFallback("AccessFullShop", Patterns.AccessFullShop, addr => Offsets.Patches.AccessFullShop = addr, saved);
            // TryPatternWithFallback("RepeatAct", Patterns.RepeatAct, addr => Offsets.Patches.RepeatAct = addr, saved);
            // TryPatternWithFallback("GameSpeed", Patterns.GameSpeed, addr => Offsets.Patches.GameSpeed = addr, saved);
            // TryPatternWithFallback("InfiniteDurability", Patterns.InfiniteDurability,
            //     addr => Offsets.Patches.InfiniteDurability = addr, saved);
            // TryPatternWithFallback("PlayerSoundView", Patterns.PlayerSoundView,
            //     addr => Offsets.Patches.PlayerSoundView = addr, saved);
            // TryPatternWithFallback("DebugFont", Patterns.DebugFont, addr => Offsets.Patches.DebugFont = addr, saved);
            // TryPatternWithFallback("NoRoll", Patterns.NoRoll, addr => Offsets.Patches.NoRoll = addr, saved);
            // TryPatternWithFallback("TargetingView", Patterns.DbgDrawFlag, addr => Offsets.Patches.DbgDrawFlag = addr,
            //     saved);
            // TryPatternWithFallback("FreeCam", Patterns.FreeCamPatch, addr => Offsets.Patches.FreeCam = addr, saved);
            //
            TryPatternWithFallback("UpdateCoords", Pattern.UpdateCoords,
                addr => Hooks.UpdateCoords = addr.ToInt64(), saved);
            TryPatternWithFallback("InAirTimer", Pattern.InAirTimer,
                addr => Hooks.InAirTimer = addr.ToInt64(), saved);
            TryPatternWithFallback("NoClipKb", Pattern.NoClipKb,
                addr => Hooks.NoClipKb = addr.ToInt64(), saved);
            TryPatternWithFallback("NoClipTriggers", Pattern.NoClipTriggers,
                addr => Hooks.NoClipTriggers = addr.ToInt64(), saved);
            TryPatternWithFallback("AddSubGoal", Pattern.CreateGoalObj,
                addr => Hooks.CreateGoalObj = addr.ToInt64(), saved);
            TryPatternWithFallback("HasSpEffect", Pattern.HasSpEffect,
                addr => Hooks.HasSpEffect = addr.ToInt64(), saved);
            TryPatternWithFallback("BlueTargetView", Pattern.BlueTargetViewHook,
                addr => Hooks.BlueTargetView = addr.ToInt64(), saved);
            TryPatternWithFallback("LockedTargetPtr", Pattern.LockedTargetPtr,
                addr => Hooks.LockedTargetPtr = addr.ToInt64(), saved);
            TryPatternWithFallback("InfinitePoise", Pattern.InfinitePoise,
                addr => Hooks.InfinitePoise = addr.ToInt64(), saved);
            TryPatternWithFallback("ShouldUpdateAi", Pattern.ShouldUpdateAi,
                addr => Hooks.ShouldUpdateAi = addr.ToInt64(), saved);
            TryPatternWithFallback("GetForceActIdx", Pattern.GetForceActIdx,
                addr => Hooks.GetForceActIdx = addr.ToInt64(), saved);
            TryPatternWithFallback("TargetNoStagger", Pattern.TargetNoStagger,
                addr => Hooks.TargetNoStagger = addr.ToInt64(), saved);
            TryPatternWithFallback("AttackInfo", Pattern.AttackInfo,
                addr => Hooks.AttackInfo = addr.ToInt64(), saved);

            using (var writer = new StreamWriter(savePath))
            {
                foreach (var pair in saved)
                    writer.WriteLine($"{pair.Key}={pair.Value:X}");
            }

            Funcs.GraceWarp = FindAddressByPattern(Pattern.GraceWarp).ToInt64();
            Funcs.SetEvent = FindAddressByPattern(Pattern.SetEvent).ToInt64();
            Funcs.SetSpEffect = FindAddressByPattern(Pattern.SetSpEffect).ToInt64();
            Funcs.GiveRunes = FindAddressByPattern(Pattern.GiveRunes).ToInt64();
            Funcs.LookupByFieldInsHandle = FindAddressByPattern(Pattern.LookupByFieldInsHandle).ToInt64();


#if DEBUG
            Console.WriteLine($"WorldChrMan.Base: 0x{WorldChrMan.Base.ToInt64():X}");
            Console.WriteLine($"FieldArea.Base: 0x{FieldArea.Base.ToInt64():X}");
            Console.WriteLine($"LuaEventMan.Base: 0x{LuaEventMan.Base.ToInt64():X}");
            Console.WriteLine($"VirtualMemFlag.Base: 0x{VirtualMemFlag.Base.ToInt64():X}");
            Console.WriteLine($"DamageManager.Base: 0x{DamageManager.Base.ToInt64():X}");
            Console.WriteLine($"MenuMan.Base: 0x{MenuMan.Base.ToInt64():X}");
            Console.WriteLine($"TargetView.Base: 0x{TargetView.Base.ToInt64():X}");
            Console.WriteLine($"GameMan.Base: 0x{GameMan.Base.ToInt64():X}");
            Console.WriteLine($"WorldHitMan.Base: 0x{WorldHitMan.Base.ToInt64():X}");
            Console.WriteLine($"WorldChrManDbg.Base: 0x{WorldChrManDbg.Base.ToInt64():X}");
            Console.WriteLine($"GameDataMan.Base: 0x{GameDataMan.Base.ToInt64():X}");

            Console.WriteLine($"Patches.NoLogo: 0x{Patches.DungeonWarp.ToInt64():X}");
            Console.WriteLine($"NoRunesFromEnemies.NoLogo: 0x{Patches.NoRunesFromEnemies.ToInt64():X}");
            Console.WriteLine($"NoRuneArcLoss.NoLogo: 0x{Patches.NoRuneArcLoss.ToInt64():X}");
            Console.WriteLine($"NoRuneLossOnDeath.NoLogo: 0x{Patches.NoRuneLossOnDeath.ToInt64():X}");

            Console.WriteLine($"Hooks.UpdateCoords: 0x{Hooks.UpdateCoords:X}");
            Console.WriteLine($"Hooks.InAirTimer: 0x{Hooks.InAirTimer:X}");
            Console.WriteLine($"Hooks.NoClipKb: 0x{Hooks.NoClipKb:X}");
            Console.WriteLine($"Hooks.NoClipTriggers: 0x{Hooks.NoClipTriggers:X}");
            Console.WriteLine($"Hooks.AddSubGoal: 0x{Hooks.CreateGoalObj:X}");
            Console.WriteLine($"Hooks.HasSpEffect: 0x{Hooks.HasSpEffect:X}");
            Console.WriteLine($"Hooks.BlueTargetView: 0x{Hooks.BlueTargetView:X}");
            Console.WriteLine($"Hooks.LockedTargetPtr: 0x{Hooks.LockedTargetPtr:X}");
            Console.WriteLine($"Hooks.InfinitePoise: 0x{Hooks.InfinitePoise:X}");
            Console.WriteLine($"Hooks.ShouldUpdateAi: 0x{Hooks.ShouldUpdateAi:X}");
            Console.WriteLine($"Hooks.GetForceActIdx: 0x{Hooks.GetForceActIdx:X}");
            Console.WriteLine($"Hooks.TargetNoStagger: 0x{Hooks.TargetNoStagger:X}");
            Console.WriteLine($"Hooks.AttackInfo: 0x{Hooks.AttackInfo:X}");

            Console.WriteLine($"Funcs.GraceWarp: 0x{Funcs.GraceWarp:X}");
            Console.WriteLine($"Funcs.SetEvent: 0x{Funcs.SetEvent:X}");
            Console.WriteLine($"Funcs.SetSpEffect: 0x{Funcs.SetSpEffect:X}");
            Console.WriteLine($"Funcs.GiveRunes: 0x{Funcs.GiveRunes:X}");
            Console.WriteLine($"Funcs.LookupByFieldInsHandle: 0x{Funcs.LookupByFieldInsHandle:X}");
#endif
        }

        private void TryPatternWithFallback(string name, Pattern pattern, Action<IntPtr> setter,
            Dictionary<string, long> saved)
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
                    int offset = _memoryService.ReadInt32(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                    addresses[i] = IntPtr.Add(instructionAddress, offset + pattern.InstructionLength);
                }
            }

            return addresses;
        }

        private List<IntPtr> PatternScanMultiple(byte[] pattern, string mask, int size)
        {
            const int chunkSize = 4096 * 16;
            byte[] buffer = new byte[chunkSize];

            IntPtr currentAddress = _memoryService.BaseAddress;
            int memSize = _memoryService.ModuleMemorySize;
            IntPtr endAddress = IntPtr.Add(currentAddress, memSize);

            List<IntPtr> addresses = new List<IntPtr>();

            while (currentAddress.ToInt64() < endAddress.ToInt64())
            {
                int bytesRemaining = (int)(endAddress.ToInt64() - currentAddress.ToInt64());
                int bytesToRead = Math.Min(bytesRemaining, buffer.Length);

                if (bytesToRead < pattern.Length)
                    break;

                buffer = _memoryService.ReadBytes(currentAddress, bytesToRead);

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
    }
}