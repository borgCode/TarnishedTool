using System;
using System.Collections.Generic;
using System.IO;


namespace SilkyRing.Memory
{
    public class AoBScanner
    {
        private readonly MemoryIo _memoryIo;

        public AoBScanner(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
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


            Offsets.WorldChrMan.Base = FindAddressByPattern(Pattern.WorldChrMan);
            Offsets.FieldArea.Base = FindAddressByPattern(Pattern.FieldArea);
            
           


            // TryPatternWithFallback("NoLogo", Patterns.NoLogo, addr => Offsets.Patches.NoLogo = addr, saved);
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
                addr => Offsets.Hooks.UpdateCoords = addr.ToInt64(), saved);
            TryPatternWithFallback("InAirTimer", Pattern.InAirTimer,
                addr => Offsets.Hooks.InAirTimer = addr.ToInt64(), saved);
            TryPatternWithFallback("NoClipKb", Pattern.NoClipKb,
                addr => Offsets.Hooks.NoClipKb = addr.ToInt64(), saved);
            TryPatternWithFallback("NoClipTriggers", Pattern.NoClipTriggers,
                addr => Offsets.Hooks.NoClipTriggers = addr.ToInt64(), saved);
            // TryPatternWithFallback("WarpCoordWrite", Patterns.WarpCoordWrite,
            //     addr => Offsets.Hooks.WarpCoordWrite = addr.ToInt64(), saved);
            // TryPatternWithFallback("AddSubGoal", Patterns.AddSubGoal, addr => Offsets.Hooks.AddSubGoal = addr.ToInt64(),
            //     saved);
            // TryPatternWithFallback("InAirTimer", Patterns.NoClipInAirTimer,
            //     addr => Offsets.Hooks.InAirTimer = addr.ToInt64(), saved);
            // TryPatternWithFallback("NoClipKeyboard", Patterns.NoClipKeyboard,
            //     addr => Offsets.Hooks.NoClipKeyboard = addr.ToInt64(), saved);
            // TryPatternWithFallback("NoClipUpdateCoords", Patterns.NoClipUpdateCoords,
            //     addr => Offsets.Hooks.NoClipUpdateCoords = addr.ToInt64(), saved);
            // TryPatternWithFallback("CameraUpLimit", Patterns.CameraUpLimit,
            //     addr => Offsets.Hooks.CameraUpLimit = addr.ToInt64(), saved);
            // TryPatternWithFallback("ItemLotBase", Patterns.ItemLotBase,
            //     addr => Offsets.Hooks.ItemLotBase = addr.ToInt64(), saved);
            // TryPatternWithFallback("ArgoSpeed", Patterns.ArgoSpeed,
            //     addr => Offsets.Hooks.ArgoSpeed = addr.ToInt64(), saved);
            //
            // var triggers = FindAddressesByPattern(Patterns.NoClipTriggers, 2);
            // if (triggers[0] == IntPtr.Zero && saved.TryGetValue("NoClipTriggers", out var value))
            // {
            //     Offsets.Hooks.NoClipTriggers = value;
            //     Offsets.Hooks.NoClipTriggers2 = saved["NoClipTriggers2"];
            // }
            // else if (triggers[0] != IntPtr.Zero)
            // {
            //     Offsets.Hooks.NoClipTriggers = triggers[0].ToInt64();
            //     Offsets.Hooks.NoClipTriggers2 = triggers[1].ToInt64();
            //     saved["NoClipTriggers"] = triggers[0].ToInt64();
            //     saved["NoClipTriggers2"] = triggers[1].ToInt64();
            // }

            using (var writer = new StreamWriter(savePath))
            {
                foreach (var pair in saved)
                    writer.WriteLine($"{pair.Key}={pair.Value:X}");
            }

            // Offsets.Funcs.Warp = FindAddressByPattern(Patterns.WarpFunc).ToInt64();
            // Offsets.Funcs.ItemSpawn = FindAddressByPattern(Patterns.ItemSpawnFunc).ToInt64();
            // Offsets.Funcs.BreakAllObjects = FindAddressByPattern(Patterns.BreakAllObjects).ToInt64();
            // Offsets.Funcs.RestoreAllObjects = FindAddressByPattern(Patterns.RestoreAllObjects).ToInt64();
            // Offsets.Funcs.SetEvent = FindAddressByPattern(Patterns.SetEvent).ToInt64();
            // Offsets.Funcs.Travel = FindAddressByPattern(Patterns.TravelFunc).ToInt64();
            // Offsets.Funcs.GetEvent = FindAddressByPattern(Patterns.GetEvent).ToInt64();
            // Offsets.Funcs.SetSpEffect = FindAddressByPattern(Patterns.SetSpEffect).ToInt64();
            // Offsets.Funcs.LevelUp = Offsets.Funcs.Travel - 0x720;
            // Offsets.Funcs.ReinforceWeapon = Offsets.Funcs.Travel - 0x1620;
            // Offsets.Funcs.InfuseWeapon = Offsets.Funcs.Travel - 0x1CB0;
            // Offsets.Funcs.Repair = Offsets.Funcs.Travel - 0x14C0;
            // Offsets.Funcs.Attunement = Offsets.Funcs.Travel - 0xB10;
            // Offsets.Funcs.AllotEstus = Offsets.Funcs.Travel - 0x2010;
            // Offsets.Funcs.Transpose = Offsets.Funcs.Travel - 0x1A10;
            // Offsets.Funcs.RegularShop = Offsets.Funcs.Travel - 0x1B50;
            // Offsets.Funcs.CombineMenuFlagAndEventFlag =
            //     FindAddressByPattern(Patterns.CombineMenuFlagAndEventFlag).ToInt64();


#if DEBUG
            Console.WriteLine($"WorldChrMan.Base: 0x{Offsets.WorldChrMan.Base.ToInt64():X}");
            Console.WriteLine($"FieldArea.Base: 0x{Offsets.FieldArea.Base.ToInt64():X}");
           
//             Console.WriteLine($"Patches.NoLogo: 0x{Offsets.Patches.NoLogo.ToInt64():X}");
//            
             Console.WriteLine($"Hooks.UpdateCoords: 0x{Offsets.Hooks.UpdateCoords:X}");
             Console.WriteLine($"Hooks.InAirTimer: 0x{Offsets.Hooks.InAirTimer:X}");
             Console.WriteLine($"Hooks.NoClipKb: 0x{Offsets.Hooks.NoClipKb:X}");
             Console.WriteLine($"Hooks.NoClipTriggers: 0x{Offsets.Hooks.NoClipTriggers:X}");
//             
//             Console.WriteLine($"Funcs.SetSpEffect: 0x{Offsets.Funcs.SetSpEffect:X}");
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
                    int offset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                    addresses[i] = IntPtr.Add(instructionAddress, offset + pattern.InstructionLength);
                }
            }

            return addresses;
        }

        private List<IntPtr> PatternScanMultiple(byte[] pattern, string mask, int size)
        {
            const int chunkSize = 4096 * 16;
            byte[] buffer = new byte[chunkSize];

            IntPtr currentAddress = _memoryIo.BaseAddress;
            IntPtr endAddress = IntPtr.Add(currentAddress, 0x3200000);

            List<IntPtr> addresses = new List<IntPtr>();

            while (currentAddress.ToInt64() < endAddress.ToInt64())
            {
                int bytesRemaining = (int)(endAddress.ToInt64() - currentAddress.ToInt64());
                int bytesToRead = Math.Min(bytesRemaining, buffer.Length);

                if (bytesToRead < pattern.Length)
                    break;

                buffer = _memoryIo.ReadBytes(currentAddress, bytesToRead);

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