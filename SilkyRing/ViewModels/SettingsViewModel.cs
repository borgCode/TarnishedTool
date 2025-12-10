using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Hooks;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly HotkeyManager _hotkeyManager;
    
    private Dictionary<string, HotkeyBindingViewModel> _hotkeyLookup;

    private string _currentSettingHotkeyId;
    private LowLevelKeyboardHook _tempHook;
    private Keys _currentKeys;

    public ObservableCollection<HotkeyBindingViewModel> PlayerHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> EnemyHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> TargetHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> UtilityHotkeys { get; }

    public SettingsViewModel(ISettingsService settingsService, HotkeyManager hotkeyManager)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;
        
        PlayerHotkeys =
        [
            new("Set RFBS", HotkeyActions.SetRfbs),
            new("Set Max Hp", HotkeyActions.SetMaxHp),
            new("Save Position 1", HotkeyActions.SavePos1),
            new("Save Position 2", HotkeyActions.SavePos2),
            new("Restore Position 2", HotkeyActions.RestorePos1),
            new("No Death", HotkeyActions.NoDeath),
            new("No Damage", HotkeyActions.NoDamage),
            new("Infinite Stamina", HotkeyActions.InfiniteStamina),
            new("Infinite Consumables", HotkeyActions.InfiniteConsumables),
            new("Infinite Arrows", HotkeyActions.InfiniteArrows),
            new("Infinite Fp", HotkeyActions.InfiniteFp),
            new("One Shot", HotkeyActions.OneShot),
            new("Infinite Poise", HotkeyActions.InfinitePoise),
            new("Silent", HotkeyActions.Silent),
            new("Hidden", HotkeyActions.Hidden),
        ];
        EnemyHotkeys = new ObservableCollection<HotkeyBindingViewModel>
        {
            // Add enemy hotkeys
        };

        TargetHotkeys = new ObservableCollection<HotkeyBindingViewModel>
        {
            // Add target hotkeys
        };

        UtilityHotkeys = new ObservableCollection<HotkeyBindingViewModel>
        {
            // Add utility hotkeys
        };

        // Build lookup dictionary
        _hotkeyLookup = PlayerHotkeys
            .Concat(EnemyHotkeys)
            .Concat(TargetHotkeys)
            .Concat(UtilityHotkeys)
            .ToDictionary(h => h.ActionId);

        LoadHotkeyDisplays();
    }

    #region Properties

    private bool _isEnableHotkeysEnabled;

    public bool IsEnableHotkeysEnabled
    {
        get => _isEnableHotkeysEnabled;
        set
        {
            if (SetProperty(ref _isEnableHotkeysEnabled, value))
            {
                SettingsManager.Default.EnableHotkeys = value;
                SettingsManager.Default.Save();
                if (_isEnableHotkeysEnabled) _hotkeyManager.Start();
                else _hotkeyManager.Stop();
            }
        }
    }
    
    private bool _isNoLogoEnabled;

    public bool IsNoLogoEnabled
    {
        get => _isNoLogoEnabled;
        set
        {
            // if (SetProperty(ref _isNoLogoEnabled, value))
            // {
            //     SettingsManager.Default.NoLogo = value;
            //     SettingsManager.Default.Save();
            //
            //     _settingsService.ToggleNoLogo(_isNoLogoEnabled);
            // }
        }
    }

    private bool _isAlwaysOnTopEnabled;

    public bool IsAlwaysOnTopEnabled
    {
        get => _isAlwaysOnTopEnabled;
        set
        {
            // if (!SetProperty(ref _isAlwaysOnTopEnabled, value)) return;
            // SettingsManager.Default.AlwaysOnTop = value;
            // SettingsManager.Default.Save();
            // var mainWindow = Application.Current.MainWindow;
            // if (mainWindow != null) mainWindow.Topmost = _isAlwaysOnTopEnabled;
        }
    }

    #endregion

    #region Public Methods

    public void StartSettingHotkey(string actionId)
    {
        // Reset previous if we were already setting one
        if (_currentSettingHotkeyId != null && 
            _hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var prev))
        {
            prev.HotkeyText = GetHotkeyDisplayText(_currentSettingHotkeyId);
        }

        _currentSettingHotkeyId = actionId;

        if (_hotkeyLookup.TryGetValue(actionId, out var current))
        {
            current.HotkeyText = "Press keys...";
        }

        _tempHook = new LowLevelKeyboardHook();
        _tempHook.IsExtendedMode = true;
        _tempHook.Down += TempHook_Down;
        _tempHook.Start();
    }

    public void ConfirmHotkey()
    {
        var currentSettingHotkeyId = _currentSettingHotkeyId;
        var currentKeys = _currentKeys;
        if (currentSettingHotkeyId == null || currentKeys == null || currentKeys.IsEmpty)
        {
            CancelSettingHotkey();
            return;
        }

        HandleExistingHotkey(currentKeys);
        SetNewHotkey(currentSettingHotkeyId, currentKeys);

        StopSettingHotkey();
    }

    public void CancelSettingHotkey()
    {
        if (_currentSettingHotkeyId != null && 
            _hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
        {
            binding.HotkeyText = "None";
            _hotkeyManager.SetHotkey(_currentSettingHotkeyId, new Keys());
        }

        StopSettingHotkey();
    }

    #endregion
    
    #region Private Methods
    
    private void LoadHotkeyDisplays()
    {
        foreach (var hotkey in _hotkeyLookup.Values)
        {
            hotkey.HotkeyText = GetHotkeyDisplayText(hotkey.ActionId);
        }
    }

    private string GetHotkeyDisplayText(string actionId)
    {
        Keys keys = _hotkeyManager.GetHotkey(actionId);
        return keys != null && keys.Values.ToArray().Length > 0 ? string.Join(" + ", keys) : "None";
    }

    private void TempHook_Down(object sender, KeyboardEventArgs e)
    {
        if (_currentSettingHotkeyId == null || e.Keys.IsEmpty)
            return;

        try
        {
            bool containsEnter = e.Keys.Values.Contains(Key.Enter) || e.Keys.Values.Contains(Key.Return);

            if (containsEnter && _currentKeys != null)
            {
                _hotkeyManager.SetHotkey(_currentSettingHotkeyId, _currentKeys);
                StopSettingHotkey();
                e.IsHandled = true;
                return;
            }

            if (e.Keys.Values.Contains(Key.Escape))
            {
                CancelSettingHotkey();
                e.IsHandled = true;
                return;
            }

            if (containsEnter)
            {
                e.IsHandled = true;
                return;
            }

            if (e.Keys.IsEmpty)
                return;

            _currentKeys = e.Keys;

            if (_hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
            {
                binding.HotkeyText = e.Keys.ToString();
            }
        }
        catch (Exception ex)
        {
            if (_hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
            {
                binding.HotkeyText = "Error: Invalid key combination";
            }
        }

        e.IsHandled = true;
    }

    private void StopSettingHotkey()
    {
        if (_tempHook != null)
        {
            _tempHook.Down -= TempHook_Down;
            _tempHook.Dispose();
            _tempHook = null;
        }

        _currentSettingHotkeyId = null;
        _currentKeys = null;
    }

    private void HandleExistingHotkey(Keys currentKeys)
    {
        string existingHotkeyId = _hotkeyManager.GetActionIdByKeys(currentKeys);
        if (string.IsNullOrEmpty(existingHotkeyId)) return;

        _hotkeyManager.ClearHotkey(existingHotkeyId);
        if (_hotkeyLookup.TryGetValue(existingHotkeyId, out var binding))
        {
            binding.HotkeyText = "None";
        }
    }

    private void SetNewHotkey(string currentSettingHotkeyId, Keys currentKeys)
    {
        _hotkeyManager.SetHotkey(currentSettingHotkeyId, currentKeys);

        if (_hotkeyLookup.TryGetValue(currentSettingHotkeyId, out var binding))
        {
            binding.HotkeyText = new Keys(currentKeys.Values.ToArray()).ToString();
        }
    }
    
    #endregion
}