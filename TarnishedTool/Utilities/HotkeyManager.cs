using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using H.Hooks;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;

namespace TarnishedTool.Utilities;

public class HotkeyManager
{
    private readonly IMemoryService _memoryService;
    private readonly LowLevelKeyboardHook _keyboardHook = new();
    private readonly Dictionary<Keys, List<string>> _hotkeyMappings = new();
    private readonly Dictionary<string, Action> _actions = new();

    public HotkeyManager(IMemoryService memoryService)
    {
        _memoryService = memoryService;

        _keyboardHook.HandleModifierKeys = true;
        _keyboardHook.Down += KeyboardHook_Down;
        LoadHotkeys();
        if (SettingsManager.Default.EnableHotkeys) _keyboardHook.Start();
    }

    public void Start()
    {
        _keyboardHook.Start();
    }

    public void Stop()
    {
        _keyboardHook.Stop();
    }

    public void RegisterAction(HotkeyActions actionId, Action action)
    {
        _actions[actionId.ToString()] = action;
    }

    private void KeyboardHook_Down(object sender, KeyboardEventArgs e)
    {
        if (!IsGameFocused()) return;
        foreach (var mapping in _hotkeyMappings)
        {
            if (!e.Keys.Are(mapping.Key.Values.ToArray())) continue;
            if (_keyboardHook.Handling) e.IsHandled = true;
            foreach (var actionId in mapping.Value)
            {
                if (_actions.TryGetValue(actionId, out var action))
                {
                    Application.Current.Dispatcher.BeginInvoke(action);
                }
            }

            break;
        }
    }

    private bool IsGameFocused()
    {
        if (_memoryService.TargetProcess == null || _memoryService.TargetProcess.Id == 0) return false;

        IntPtr foregroundWindow = User32.GetForegroundWindow();
        User32.GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
        return foregroundProcessId == (uint)_memoryService.TargetProcess.Id;
    }

    public void SetHotkey(string actionId, Keys keys)
    {
        ClearHotkey(actionId);

        if (!_hotkeyMappings.TryGetValue(keys, out var actions))
        {
            actions = new List<string>();
            _hotkeyMappings[keys] = actions;
        }

        actions.Add(actionId);
        SaveHotkeys();
    }

    public void ClearHotkey(string actionId)
    {
        var toRemove = _hotkeyMappings
            .FirstOrDefault(m => m.Value.Contains(actionId));

        if (toRemove.Key == null) return;

        toRemove.Value.Remove(actionId);
        if (toRemove.Value.Count == 0)
            _hotkeyMappings.Remove(toRemove.Key);

        SaveHotkeys();
    }

    public Keys GetHotkey(string actionId)
    {
        foreach (var mapping in _hotkeyMappings)
        {
            if (mapping.Value.Contains(actionId))
                return mapping.Key;
        }

        return null;
    }

    public string GetActionIdByKeys(Keys keys)
    {
        return _hotkeyMappings.TryGetValue(keys, out var actions)
            ? actions.FirstOrDefault()
            : null;
    }

    public void SaveHotkeys()
    {
        try
        {
            var mappingPairs = new List<string>();
            foreach (var mapping in _hotkeyMappings)
            {
                foreach (var actionId in mapping.Value)
                {
                    mappingPairs.Add($"{actionId}={mapping.Key}");
                }
            }

            SettingsManager.Default.HotkeyActionIds = string.Join(";", mappingPairs);
            SettingsManager.Default.Save();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error saving hotkeys: {ex.Message}");
        }
    }

    public void LoadHotkeys()
    {
        try
        {
            _hotkeyMappings.Clear();
            string mappingsString = SettingsManager.Default.HotkeyActionIds;
            if (!string.IsNullOrEmpty(mappingsString))
            {
                string[] pairs = mappingsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in pairs)
                {
                    int separatorIndex = pair.IndexOf('=');
                    if (separatorIndex <= 0) continue;
                    string actionId = pair.Substring(0, separatorIndex);
                    string keyValue = pair.Substring(separatorIndex + 1);
                    var keys = Keys.Parse(keyValue);
                    if (!_hotkeyMappings.TryGetValue(keys, out var actions))
                    {
                        actions = new List<string>();
                        _hotkeyMappings[keys] = actions;
                    }

                    actions.Add(actionId);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error loading hotkeys: {ex.Message}");
        }
    }

    public void ClearAll()
    {
        _hotkeyMappings.Clear();
        SaveHotkeys();
    }

    public void SetKeyboardHandling(bool isEnabled) => _keyboardHook.Handling = isEnabled;
}