using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TarnishedTool.Interfaces;
using static TarnishedTool.Memory.Offsets;
#if DEBUG
using System.Diagnostics;
#endif

namespace TarnishedTool.Memory
{
    public class AobScanner(IMemoryService memoryService)
    {
        private const int HistogramSampleStep = 16;

        private byte[]? _module;
        private nint _moduleBase;

        private readonly List<Request> _requests = new();

        private readonly byte[] _bitmap = new byte[65536 / 8];
        private readonly List<Request>?[] _pairBuckets = new List<Request>[65536];

        private readonly List<Request>?[] _singleBuckets = new List<Request>[256];
        private bool _hasSingleFallback;

        private readonly Dictionary<string, nint> _savedAddresses = new();
        private readonly long[] _pairHistogram = new long[65536];
        private long _histogramSamples;
        private bool _histogramBuilt;

        private static readonly string BackupPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TarnishedTool",
            "backup_addresses.txt");

        private sealed class Request(int id, string? name, Pattern pattern, Action<nint> setter)
        {
            public int Id { get; } = id;
            public string? Name { get; } = name;
            public Pattern Pattern { get; } = pattern;
            public Action<nint> Setter { get; } = setter;
            public int[] NonWildcardIndices { get; } = BuildNonWildcardIndices(pattern);
            public int AnchorOffset;
            public long AnchorFrequency = -1;
            public bool IsSingle;

            private static int[] BuildNonWildcardIndices(Pattern p)
            {
                var len = p.Bytes.Length;
                var list = new List<int>(len);
                for (var j = 0; j < len; j++)
                    if (IsConcrete(p.Mask, j, len))
                        list.Add(j);
                return list.ToArray();
            }
        }

        private static bool IsConcrete(string mask, int j, int len)
            => j < len && (j >= mask.Length || mask[j] != '?');

        public void LoadModule()
        {
            _moduleBase = memoryService.BaseAddress;
            _module = memoryService.ReadBytes(_moduleBase, memoryService.ModuleMemorySize);
        }
        
        public void QueueFallbackPatterns()
        {
            Queue(nameof(Pattern.WorldChrMan), Pattern.WorldChrMan, addr => WorldChrMan.Base = addr);
            Queue(nameof(Pattern.FieldArea), Pattern.FieldArea, addr => FieldArea.Base = addr);
            Queue(nameof(Pattern.LuaEventMan), Pattern.LuaEventMan, addr => LuaEventMan.Base = addr);
            Queue(nameof(Pattern.VirtualMemFlag), Pattern.VirtualMemFlag, addr => VirtualMemFlag.Base = addr);
            Queue(nameof(Pattern.DamageManager), Pattern.DamageManager, addr => DamageManager.Base = addr);
            Queue(nameof(Pattern.MenuMan), Pattern.MenuMan, addr => MenuMan.Base = addr);
            Queue(nameof(Pattern.TargetView), Pattern.TargetView, addr => TargetView.Base = addr);
            Queue(nameof(Pattern.GameMan), Pattern.GameMan, addr => GameMan.Base = addr);
            Queue(nameof(Pattern.WorldHitMan), Pattern.WorldHitMan, addr => WorldHitMan.Base = addr);
            Queue(nameof(Pattern.WorldChrManDbg), Pattern.WorldChrManDbg, addr => WorldChrManDbg.Base = addr);
            Queue(nameof(Pattern.GameDataMan), Pattern.GameDataMan, addr => GameDataMan.Base = addr);
            Queue(nameof(Pattern.CsDlcImp), Pattern.CsDlcImp, addr => CsDlcImp.Base = addr);
            Queue(nameof(Pattern.MapItemManImpl), Pattern.MapItemManImpl, addr => MapItemManImpl.Base = addr);
            Queue(nameof(Pattern.FD4PadManager), Pattern.FD4PadManager, addr => FD4PadManager.Base = addr);
            Queue(nameof(Pattern.CSEmkSystem), Pattern.CSEmkSystem, addr => CSEmkSystem.Base = addr);
            Queue(nameof(Pattern.WorldAreaTimeImpl), Pattern.WorldAreaTimeImpl, addr => WorldAreaTimeImpl.Base = addr);
            Queue(nameof(Pattern.GroupMask), Pattern.GroupMask, addr => GroupMask.Base = addr);
            Queue(nameof(Pattern.SoloParamRepositoryImp), Pattern.SoloParamRepositoryImp,
                addr => SoloParamRepositoryImp.Base = addr);
            Queue(nameof(Pattern.MsgRepository), Pattern.MsgRepository, addr => MsgRepository.Base = addr);
            Queue(nameof(Pattern.CSFlipperImp), Pattern.CSFlipperImp, addr => CSFlipperImp.Base = addr);
            Queue(nameof(Pattern.CSDbgEvent), Pattern.CSDbgEvent, addr => CSDbgEvent.Base = addr);
            Queue(nameof(Pattern.ChrDbgFlags), Pattern.ChrDbgFlags, addr => ChrDbgFlags.Base = addr);
            Queue(nameof(Pattern.UserInputManager), Pattern.UserInputManager, addr => UserInputManager.Base = addr);
            Queue(nameof(Pattern.CSTrophy), Pattern.CSTrophy, addr => CSTrophy.Base = addr);
            Queue(nameof(Pattern.DrawPathing), Pattern.DrawPathing, addr => DrawPathing.Base = addr - 0x10);
            Queue(nameof(Pattern.MapDebugFlags), Pattern.MapDebugFlags, addr => MapDebugFlags.Base = addr - 1);
            Queue(nameof(Pattern.WorldAiManagerImp), Pattern.WorldAiManagerImp, addr => WorldAiManagerImp.Base = addr);

            Queue(nameof(Pattern.GraceWarp), Pattern.GraceWarp, addr => Functions.GraceWarp = addr);
            Queue(nameof(Pattern.SetEvent), Pattern.SetEvent, addr => Functions.SetEvent = addr);
            Queue(nameof(Pattern.SetSpEffect), Pattern.SetSpEffect, addr => Functions.SetSpEffect = addr);
            Queue(nameof(Pattern.GiveRunes), Pattern.GiveRunes, addr => Functions.GiveRunes = addr);
            Queue(nameof(Pattern.LookupByFieldInsHandle), Pattern.LookupByFieldInsHandle,
                addr => Functions.LookupByFieldInsHandle = addr);
            Queue(nameof(Pattern.WarpToBlock), Pattern.WarpToBlock, addr => Functions.WarpToBlock = addr);
            Queue(nameof(Pattern.ExternalEventTempCtor), Pattern.ExternalEventTempCtor,
                addr => Functions.ExternalEventTempCtor = addr);
            Queue(nameof(Pattern.ExecuteTalkCommand), Pattern.ExecuteTalkCommand,
                addr => Functions.ExecuteTalkCommand = addr);
            Queue(nameof(Pattern.GetEvent), Pattern.GetEvent, addr => Functions.GetEvent = addr);
            Queue(nameof(Pattern.GetPlayerItemQuantityById), Pattern.GetPlayerItemQuantityById,
                addr => Functions.GetPlayerItemQuantityById = addr);
            Queue(nameof(Pattern.ItemSpawn), Pattern.ItemSpawn, addr => Functions.ItemSpawn = addr);
            Queue(nameof(Pattern.MatrixVectorProduct), Pattern.MatrixVectorProduct,
                addr => Functions.MatrixVectorProduct = addr);
            Queue(nameof(Pattern.ChrInsByHandle), Pattern.ChrInsByHandle, addr => Functions.ChrInsByHandle = addr);
            Queue(nameof(Pattern.FindAndRemoveSpEffect), Pattern.FindAndRemoveSpEffect,
                addr => Functions.FindAndRemoveSpEffect = addr);
            Queue(nameof(Pattern.EmevdSwitch), Pattern.EmevdSwitch, addr => Functions.EmevdSwitch = addr);
            Queue(nameof(Pattern.EmkEventInsCtor), Pattern.EmkEventInsCtor, addr => Functions.EmkEventInsCtor = addr);
            Queue(nameof(Pattern.GetMovement), Pattern.GetMovement, addr => Functions.GetMovement = addr);
            Queue(nameof(Pattern.GetChrInsByEntityId), Pattern.GetChrInsByEntityId,
                addr => Functions.GetChrInsByEntityId = addr);
            Queue(nameof(Pattern.NpcEzStateTalkCtor), Pattern.NpcEzStateTalkCtor,
                addr => Functions.NpcEzStateTalkCtor = addr);
            Queue(nameof(Pattern.EzStateEnvQueryImplCtor), Pattern.EzStateEnvQueryImplCtor,
                addr => Functions.EzStateEnvQueryImplCtor = addr);
            Queue(nameof(Pattern.LocalToMapCoords), Pattern.LocalToMapCoords,
                addr => Functions.LocalToMapCoords = addr);
            Queue(nameof(Pattern.LuaDoString), Pattern.LuaDoString, addr => Functions.LuaDoString = addr);
            Queue(nameof(Pattern.RefreshFromStorage), Pattern.RefreshFromStorage,
                addr => Functions.RefreshFromStorage = addr);

            Queue(nameof(Pattern.CanFastTravel), Pattern.CanFastTravel, addr => Patches.CanFastTravel = addr);
            Queue(nameof(Pattern.NoRunesFromEnemies), Pattern.NoRunesFromEnemies,
                addr => Patches.NoRunesFromEnemies = addr);
            Queue(nameof(Pattern.NoRuneArcLoss), Pattern.NoRuneArcLoss, addr => Patches.NoRuneArcLoss = addr);
            Queue(nameof(Pattern.NoRuneLossOnDeath), Pattern.NoRuneLossOnDeath,
                addr => Patches.NoRuneLossOnDeath = addr);
            Queue(nameof(Pattern.IsWorldPaused), Pattern.IsWorldPaused, addr => Patches.IsWorldPaused = addr);
            Queue(nameof(Pattern.GetItemChance), Pattern.GetItemChance, addr => Patches.GetItemChance = addr);
            Queue(nameof(Pattern.OpenMap), Pattern.OpenMap, addr => Patches.OpenMap = addr);
            Queue(nameof(Pattern.CloseMap), Pattern.CloseMap, addr => Patches.CloseMap = addr);
            Queue(nameof(Pattern.NoLogo), Pattern.NoLogo, addr => Patches.NoLogo = addr);
            Queue(nameof(Pattern.PlayerSound), Pattern.PlayerSound, addr => Patches.PlayerSound = addr);
            Queue(nameof(Pattern.IsTorrentDisabledInUnderworld), Pattern.IsTorrentDisabledInUnderworld,
                addr => Patches.IsTorrentDisabledInUnderworld = addr);
            Queue(nameof(Pattern.IsWhistleDisabled), Pattern.IsWhistleDisabled,
                addr => Patches.IsWhistleDisabled = addr);
            Queue(nameof(Pattern.EnableFreeCam), Pattern.EnableFreeCam, addr => Patches.EnableFreeCam = addr);
            Queue(nameof(Pattern.GetShopEvent), Pattern.GetShopEvent, addr => Patches.GetShopEvent = addr);
            Queue(nameof(Pattern.DebugFont), Pattern.DebugFont, addr => Patches.DebugFont = addr);
            Queue(nameof(Pattern.FpsCap), Pattern.FpsCap, addr => Patches.FpsCap = addr);
            Queue(nameof(Pattern.CanDrawEvents1), Pattern.CanDrawEvents1, addr => Patches.CanDrawEvents1 = addr);
            Queue(nameof(Pattern.CanDrawEvents2), Pattern.CanDrawEvents2, addr => Patches.CanDrawEvents2 = addr);

            Queue(nameof(Pattern.UpdateCoords), Pattern.UpdateCoords, addr => Hooks.UpdateCoords = addr);
            Queue(nameof(Pattern.InAirTimer), Pattern.InAirTimer, addr => Hooks.InAirTimer = addr);
            Queue(nameof(Pattern.NoClipKb), Pattern.NoClipKb, addr => Hooks.NoClipKb = addr);
            Queue(nameof(Pattern.NoClipTriggers), Pattern.NoClipTriggers, addr => Hooks.NoClipTriggers = addr);
            Queue(nameof(Pattern.HasSpEffect), Pattern.HasSpEffect, addr => Hooks.HasSpEffect = addr);
            Queue(nameof(Pattern.BlueTargetViewHook), Pattern.BlueTargetViewHook, addr => Hooks.BlueTargetView = addr);
            Queue(nameof(Pattern.LockedTargetPtr), Pattern.LockedTargetPtr, addr => Hooks.LockedTargetPtr = addr);
            Queue(nameof(Pattern.InfinitePoise), Pattern.InfinitePoise, addr => Hooks.InfinitePoise = addr);
            Queue(nameof(Pattern.ShouldUpdateAi), Pattern.ShouldUpdateAi, addr => Hooks.ShouldUpdateAi = addr);
            Queue(nameof(Pattern.GetForceActIdx), Pattern.GetForceActIdx, addr => Hooks.GetForceActIdx = addr);
            Queue(nameof(Pattern.TargetNoStagger), Pattern.TargetNoStagger, addr => Hooks.TargetNoStagger = addr);
            Queue(nameof(Pattern.AttackInfo), Pattern.AttackInfo, addr => Hooks.AttackInfo = addr);
            Queue(nameof(Pattern.NoTimePassOnDeath), Pattern.NoTimePassOnDeath, addr => Hooks.NoTimePassOnDeath = addr);
            Queue(nameof(Pattern.WarpCoordWrite), Pattern.WarpCoordWrite, addr => Hooks.WarpCoordWrite = addr);
            Queue(nameof(Pattern.WarpAngleWrite), Pattern.WarpAngleWrite, addr => Hooks.WarpAngleWrite = addr);
            Queue(nameof(Pattern.LionCooldownHook), Pattern.LionCooldownHook, addr => Hooks.LionCooldownHook = addr);
            Queue(nameof(Pattern.SetActionRequested), Pattern.SetActionRequested,
                addr => Hooks.SetActionRequested = addr);
            Queue(nameof(Pattern.NoMapAcquiredPopup), Pattern.NoMapAcquiredPopup,
                addr => Hooks.NoMapAcquiredPopup = addr);
            Queue(nameof(Pattern.LoadScreenMsgLookup), Pattern.LoadScreenMsgLookup,
                addr => Hooks.LoadScreenMsgLookup = addr);
            Queue(nameof(Pattern.NoGrab), Pattern.NoGrab, addr => Hooks.NoGrab = addr);
            Queue(nameof(Pattern.NoHeal), Pattern.NoHeal, addr => Hooks.NoHeal = addr);
            Queue(nameof(Pattern.PlayerLockHp), Pattern.PlayerLockHp, addr => Hooks.PlayerLockHp = addr);
        }
        
        public void Queue(string? name, Pattern pattern, Action<nint> setter) =>
            _requests.Add(new Request(_requests.Count, name, pattern, setter));

        private void LoadSavedAddresses()
        {
            _savedAddresses.Clear();
            if (!File.Exists(BackupPath)) return;
            foreach (var line in File.ReadAllLines(BackupPath))
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;
                if (long.TryParse(parts[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var val))
                    _savedAddresses[parts[0]] = (nint)val;
            }
        }

        private void WriteSavedAddresses()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BackupPath)!);
            using var writer = new StreamWriter(BackupPath);
            foreach (var kvp in _savedAddresses)
                writer.WriteLine($"{kvp.Key}={(long)kvp.Value:X}");
        }

        private void BuildHistogram()
        {
            Array.Clear(_pairHistogram, 0, _pairHistogram.Length);
            var buf = _module!;
            var end = buf.Length - 1;
            long samples = 0;
            for (var i = 0; i < end; i += HistogramSampleStep)
            {
                var key = buf[i] | (buf[i + 1] << 8);
                _pairHistogram[key]++;
                samples++;
            }

            _histogramSamples = samples;
            _histogramBuilt = true;
        }
        
        private void AssignAnchors()
        {
            var needHistogram = _requests.Any(r => r.Pattern.AnchorOffset < 0);
#if DEBUG
            needHistogram = true;
#endif
            if (needHistogram) BuildHistogram();

            long[]? singleMarginal = null; 

            foreach (var req in _requests)
            {
                var bytes = req.Pattern.Bytes;
                var hardOffset = req.Pattern.AnchorOffset;

                if (hardOffset >= 0 && hardOffset + 1 < bytes.Length)
                {
                    AssignPair(req, hardOffset);
                    continue;
                }
                
                var mask = req.Pattern.Mask;
                var len = bytes.Length;
                var bestOffset = -1;
                var bestFreq = long.MaxValue;
                for (var j = 0; j + 1 < len; j++)
                {
                    if (!IsConcrete(mask, j, len) || !IsConcrete(mask, j + 1, len)) continue;
                    var freq = _pairHistogram[bytes[j] | (bytes[j + 1] << 8)];
                    if (freq < bestFreq)
                    {
                        bestFreq = freq;
                        bestOffset = j;
                    }
                }

                if (bestOffset >= 0)
                {
                    AssignPair(req, bestOffset);
                }
                else
                {
                    singleMarginal ??= BuildSingleByteMarginal();
                    var bestByteOffset = req.NonWildcardIndices.Length > 0 ? req.NonWildcardIndices[0] : 0;
                    var bestByteFreq = long.MaxValue;
                    foreach (var j in req.NonWildcardIndices)
                    {
                        var freq = singleMarginal[bytes[j]];
                        if (freq < bestByteFreq)
                        {
                            bestByteFreq = freq;
                            bestByteOffset = j;
                        }
                    }

                    req.IsSingle = true;
                    req.AnchorOffset = bestByteOffset;
                    req.AnchorFrequency = bestByteFreq;
                    _hasSingleFallback = true;
                    (_singleBuckets[bytes[bestByteOffset]] ??= new List<Request>()).Add(req);
                }
            }
        }

        private void AssignPair(Request req, int offset)
        {
            var bytes = req.Pattern.Bytes;
            var key = bytes[offset] | (bytes[offset + 1] << 8);
            req.AnchorOffset = offset;
            req.IsSingle = false;
            req.AnchorFrequency = _histogramBuilt ? _pairHistogram[key] : -1;
            _bitmap[key >> 3] |= (byte)(1 << (key & 7));
            (_pairBuckets[key] ??= new List<Request>()).Add(req);
        }
        
        private long[] BuildSingleByteMarginal()
        {
            var marginal = new long[256];
            for (var key = 0; key < _pairHistogram.Length; key++)
                marginal[key & 0xFF] += _pairHistogram[key];
            return marginal;
        }

        public void Run()
        {
            if (_module is null) LoadModule();
            LoadSavedAddresses();
            AssignAnchors();

#if DEBUG
            LogAnchors();
            var scan = Stopwatch.StartNew();
#endif
            var buf = _module!;
            var bufLen = buf.Length;
            var end = bufLen - 1; 
            ref var bufRef = ref buf[0];

            var found = new bool[_requests.Count];
            var remaining = _requests.Count;

            for (var i = 0; i < end && remaining > 0; i++)
            {
                var b0 = Unsafe.Add(ref bufRef, i);
                var key = b0 | (Unsafe.Add(ref bufRef, i + 1) << 8);

                if ((_bitmap[key >> 3] & (1 << (key & 7))) != 0)
                {
                    var bucket = _pairBuckets[key];
                    if (bucket != null)
                    {
                        foreach (var req in bucket)
                        {
                            if (found[req.Id]) continue;
                            var start = i - req.AnchorOffset;
                            if (start < 0) continue;
                            if (!Matches(ref bufRef, bufLen, start, req)) continue;
                            ResolveAndInvoke(req, start);
                            found[req.Id] = true;
                            remaining--;
                        }
                    }
                }

                if (!_hasSingleFallback) continue;
                {
                    var sb = _singleBuckets[b0];
                    if (sb == null) continue;
                    foreach (var req in sb)
                    {
                        if (found[req.Id]) continue;
                        var start = i - req.AnchorOffset;
                        if (start < 0) continue;
                        if (!Matches(ref bufRef, bufLen, start, req)) continue;
                        ResolveAndInvoke(req, start);
                        found[req.Id] = true;
                        remaining--;
                    }
                }
            }

            for (var i = 0; i < _requests.Count; i++)
            {
                if (found[i]) continue;
                var req = _requests[i];
                if (req.Name != null && _savedAddresses.TryGetValue(req.Name, out var saved))
                {
                    req.Setter(saved);
#if DEBUG
                    Console.WriteLine($"[AobScanner] MISS (using saved): {req.Name}");
#endif
                }
                else
                {
                    req.Setter(0);
#if DEBUG
                    Console.WriteLine($"[AobScanner] MISS (no saved): {req.Name}");
#endif
                }
            }

            WriteSavedAddresses();

#if DEBUG
            scan.Stop();
            var foundCount = _requests.Count - remaining;
            Console.WriteLine(
                $"[AobScanner] scan done in {scan.ElapsedMilliseconds} ms ({foundCount}/{_requests.Count} found)");
#endif
        }

        private static bool Matches(ref byte bufRef, int bufLen, int start, Request req)
        {
            var bytes = req.Pattern.Bytes;
            var indices = req.NonWildcardIndices;
            if (start + bytes.Length > bufLen) return false;

            foreach (var j in indices)
            {
                if (Unsafe.Add(ref bufRef, start + j) != bytes[j]) return false;
            }

            return true;
        }

        private void ResolveAndInvoke(Request req, int startIndex)
        {
            var p = req.Pattern;
            var instructionAddress = _moduleBase + startIndex + p.InstructionOffset;

            var final = p.AddressingMode == AddressingMode.Absolute
                ? instructionAddress
                : instructionAddress + ReadInt32(instructionAddress + p.OffsetLocation) + p.InstructionLength;

            if (req.Name != null) _savedAddresses[req.Name] = final;
            req.Setter(final);
        }

        private int ReadInt32(nint address)
        {
            var idx = (int)(address - _moduleBase);
            return Unsafe.ReadUnaligned<int>(ref _module![idx]);
        }

#if DEBUG
        private void LogAnchors()
        {
            double scale = HistogramSampleStep;
            long totalCandidateEst = 0;

            Console.WriteLine($"[AobScanner] --- anchor report ({_requests.Count} patterns, " +
                              $"{_requests.Count(r => r.IsSingle)} single-fallback) ---");
            Console.WriteLine("[AobScanner]   freq(ppm)  count~   combo      off  name");

            foreach (var req in _requests.OrderByDescending(r => r.AnchorFrequency))
            {
                var estCount = (long)(req.AnchorFrequency * scale);
                var ppm = _histogramSamples > 0 ? req.AnchorFrequency / (double)_histogramSamples * 1_000_000 : 0;
                if (req.AnchorFrequency >= 0) totalCandidateEst += estCount;

                if (req.IsSingle)
                {
                    var b = req.Pattern.Bytes[req.AnchorOffset];
                    Console.WriteLine(
                        $"[AobScanner]   {ppm,8:F1}  {estCount,8}  0x{b:X2}(1byte) {req.AnchorOffset,3}   {req.Name}  <-- SINGLE-BYTE FALLBACK");
                    continue;
                }

                var b0 = req.Pattern.Bytes[req.AnchorOffset];
                var b1 = req.Pattern.Bytes[req.AnchorOffset + 1];
                Console.WriteLine(
                    $"[AobScanner]   {ppm,8:F1}  {estCount,8}  0x{b0:X2} 0x{b1:X2}  {req.AnchorOffset,3}   {req.Name}");
            }
        }
#endif
    }
}