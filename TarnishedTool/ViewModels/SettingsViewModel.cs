using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using H.Hooks;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;
using Key = H.Hooks.Key;
using KeyboardEventArgs = H.Hooks.KeyboardEventArgs;

namespace TarnishedTool.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly HotkeyManager _hotkeyManager;

    private readonly Dictionary<string, HotkeyBindingViewModel> _hotkeyLookup;

    private string _currentSettingHotkeyId;
    private LowLevelKeyboardHook _tempHook;
    private Keys _currentKeys;

    public ObservableCollection<HotkeyBindingViewModel> PlayerHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> EnemiesHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> TargetHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> UtilityHotkeys { get; }

    public SettingsViewModel(ISettingsService settingsService, HotkeyManager hotkeyManager, IStateService stateService)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;

        stateService.Subscribe(State.AppStart, OnAppStart);
        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.Attached, OnGameAttached);


        PlayerHotkeys =
        [
            new("Set RFBS", HotkeyActions.SetRfbs),
            new("Set Max Hp", HotkeyActions.SetMaxHp),
            new("Set Custom Hp", HotkeyActions.PlayerSetCustomHp),
            new("Save Position 1", HotkeyActions.SavePos1),
            new("Save Position 2", HotkeyActions.SavePos2),
            new("Restore Position 1", HotkeyActions.RestorePos1),
            new("Restore Position 2", HotkeyActions.RestorePos2),
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
            new("Toggle Speed", HotkeyActions.TogglePlayerSpeed),
            new("Increase Speed", HotkeyActions.IncreasePlayerSpeed),
            new("Decrease Speed", HotkeyActions.DecreasePlayerSpeed),
            new("Apply Special Effect", HotkeyActions.ApplySpEffect),
            new("Remove Special Effect", HotkeyActions.RemoveSpEffect),
        ];
        EnemiesHotkeys =
        [
            new("All No Death", HotkeyActions.AllNoDeath),
            new("All No Damage", HotkeyActions.AllNoDamage),
            new("All No Hit", HotkeyActions.AllNoHit),
            new("All No Attack", HotkeyActions.AllNoAttack),
            new("All No Move", HotkeyActions.AllNoMove),
            new("All Disable Ai", HotkeyActions.AllDisableAi),
            new("Targeting View", HotkeyActions.AllTargetingView),
            new("Force EB act sequence", HotkeyActions.ForceEbActSequence),
        ];

        TargetHotkeys =
        [
            new("Enable Target Options", HotkeyActions.EnableTargetOptions),
            new("Kill Target", HotkeyActions.KillTarget),
            new("Set Max Hp", HotkeyActions.SetTargetMaxHp),
            new("Set Custom Hp", HotkeyActions.SetTargetCustomHp),
            new("Freeze Hp", HotkeyActions.FreezeTargetHp),
            new("Show all resistances", HotkeyActions.ShowAllResistances),
            new("Increase Speed", HotkeyActions.IncreaseTargetSpeed),
            new("Decrease Speed", HotkeyActions.DecreaseTargetSpeed),
            new("Toggle Speed", HotkeyActions.ToggleTargetSpeed),
            new("Increment Force Act", HotkeyActions.IncrementForceAct),
            new("Decrement Force Act", HotkeyActions.DecrementForceAct),
            new("Set Force Act to 0", HotkeyActions.SetForceActToZero),
            new("Disable Ai", HotkeyActions.DisableTargetAi),
            new("Disable All Except Target", HotkeyActions.DisableAllExceptTargetAi),
            new("No Stagger", HotkeyActions.TargetNoStagger),
            new("Repeat Act", HotkeyActions.TargetRepeatAct),
            new("Targeting View", HotkeyActions.TargetTargetingView),
            new("Open Attack Info", HotkeyActions.ShowAttackInfo),
            new("Open Defenses", HotkeyActions.ShowDefenses),
            new("No Move", HotkeyActions.TargetNoMove),
            new("Force Act Sequence", HotkeyActions.ForceActSequence),
            new("Kill All Except Target", HotkeyActions.KillAllExceptTarget),
        ];

        UtilityHotkeys =
        [
            new("Quitout", HotkeyActions.Quitout),
            new("Force Save", HotkeyActions.ForceSave),
            new("Noclip", HotkeyActions.Noclip),
            new("Increase Game Speed", HotkeyActions.IncreaseGameSpeed),
            new("Decrease Game Speed", HotkeyActions.DecreaseGameSpeed),
            new("Toggle Game Speed", HotkeyActions.ToggleGameSpeed),
            new("Increase NoClip Speed", HotkeyActions.IncreaseNoClipSpeed),
            new("Decrease NoClip Speed", HotkeyActions.DecreaseNoClipSpeed),
            new("Toggle Free Cam", HotkeyActions.ToggleFreeCam),
            new("Toggle Freeze World", HotkeyActions.ToggleFreezeWorld),
            new("Move Free Cam to Player", HotkeyActions.MoveCamToPlayer),
            new("Move Player to Free Cam", HotkeyActions.MovePlayerToCam),
        ];


        _hotkeyLookup = PlayerHotkeys
            .Concat(EnemiesHotkeys)
            .Concat(TargetHotkeys)
            .Concat(UtilityHotkeys)
            .ToDictionary(h => h.ActionId);

        LoadHotkeyDisplays();
        RegisterHotkeys();

        ClearHotkeysCommand = new DelegateCommand(ClearHotkeys);
    }
    
    #region Commands

    public ICommand ClearHotkeysCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled = true;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

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
            if (SetProperty(ref _isNoLogoEnabled, value))
            {
                SettingsManager.Default.NoLogo = value;
                SettingsManager.Default.Save();
                _settingsService.ToggleNoLogo(_isNoLogoEnabled);
            }
        }
    }

    private bool _isAlwaysOnTopEnabled;

    public bool IsAlwaysOnTopEnabled
    {
        get => _isAlwaysOnTopEnabled;
        set
        {
            if (!SetProperty(ref _isAlwaysOnTopEnabled, value)) return;
            SettingsManager.Default.AlwaysOnTop = value;
            SettingsManager.Default.Save();
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null) mainWindow.Topmost = _isAlwaysOnTopEnabled;
        }
    }

    private bool _isStutterFixEnabled;

    public bool IsStutterFixEnabled
    {
        get => _isStutterFixEnabled;
        set
        {
            if (!SetProperty(ref _isStutterFixEnabled, value)) return;
            SettingsManager.Default.StutterFix = value;
            SettingsManager.Default.Save();
            if (AreOptionsEnabled) _settingsService.ToggleStutterFix(_isStutterFixEnabled);
        }
    }
    
    private bool _isDisableAchievementsEnabled;

    public bool IsDisableAchievementsEnabled
    {
        get => _isDisableAchievementsEnabled;
        set
        {
            if (!SetProperty(ref _isDisableAchievementsEnabled, value)) return;
            SettingsManager.Default.DisableAchievements = value;
            SettingsManager.Default.Save();
            if (AreOptionsEnabled) _settingsService.ToggleDisableAchievements(_isDisableAchievementsEnabled);
        }
    }

    #endregion

    #region Public Methods

    public void StartSettingHotkey(string actionId)
    {
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

    private void OnAppStart()
    {
        _isEnableHotkeysEnabled = SettingsManager.Default.EnableHotkeys;
        if (_isEnableHotkeysEnabled) _hotkeyManager.Start();
        else _hotkeyManager.Stop();
        OnPropertyChanged(nameof(IsEnableHotkeysEnabled));

        IsAlwaysOnTopEnabled = SettingsManager.Default.AlwaysOnTop;

        _isStutterFixEnabled = SettingsManager.Default.StutterFix;
        OnPropertyChanged(nameof(IsStutterFixEnabled));
        
        _isDisableAchievementsEnabled = SettingsManager.Default.DisableAchievements;
        OnPropertyChanged(nameof(IsDisableAchievementsEnabled));
        
        _isNoLogoEnabled = SettingsManager.Default.NoLogo;
        OnPropertyChanged(nameof(IsNoLogoEnabled));
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsStutterFixEnabled) _settingsService.ToggleStutterFix(true);
        if (IsDisableAchievementsEnabled) _settingsService.ToggleDisableAchievements(true);
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void OnGameAttached()
    {
        if (IsNoLogoEnabled) _settingsService.ToggleNoLogo(true);
    }
    

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

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.Quitout, () => _settingsService.Quitout());
    }

    private void ClearHotkeys()
    {
        _hotkeyManager.ClearAll();
        LoadHotkeyDisplays();
    }

    #endregion
}