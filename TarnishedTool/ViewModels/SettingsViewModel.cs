using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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

    public SearchableGroupedCollection<string, HotkeyBindingViewModel> Hotkeys { get; }

    public SettingsViewModel(ISettingsService settingsService, HotkeyManager hotkeyManager, IStateService stateService)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;

        stateService.Subscribe(State.AppStart, OnAppStart);
        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.Attached, OnGameAttached);
        stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);


        var groupedHotkeys = new Dictionary<string, List<HotkeyBindingViewModel>>
        {
            ["Player"] =
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
                new("No Hit", HotkeyActions.NoHit),
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
                new("Rune Arc", HotkeyActions.RuneArc),
                new("Rest Character", HotkeyActions.Rest)
            ],
            ["Enemies"] =
            [
                new("All No Death", HotkeyActions.AllNoDeath),
                new("All No Damage", HotkeyActions.AllNoDamage),
                new("All No Hit", HotkeyActions.AllNoHit),
                new("All No Attack", HotkeyActions.AllNoAttack),
                new("All No Move", HotkeyActions.AllNoMove),
                new("All Disable Ai", HotkeyActions.AllDisableAi),
                new("Targeting View", HotkeyActions.AllTargetingView),
                new("Force EB act sequence", HotkeyActions.ForceEbActSequence),
                new("Revive Boss", HotkeyActions.ReviveSelectedBoss),
                new("Revive Boss (1st Encounter)", HotkeyActions.ReviveSelectedBossFirstEncounter),
            ],

            ["Target"] =
            [
                new("Enable Target Options", HotkeyActions.EnableTargetOptions),
                new("Kill Target", HotkeyActions.KillTarget),
                new("Set Max Hp", HotkeyActions.SetTargetMaxHp),
                new("Set Custom Hp", HotkeyActions.SetTargetCustomHp),
                new("Freeze Hp", HotkeyActions.FreezeTargetHp),
                new("Show all resistances", HotkeyActions.ShowAllResistances),
                new("Pop out resistances", HotkeyActions.PopoutResistances),
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
                new("Open Special Effects", HotkeyActions.ShowTargetSpEffects),
                new("No Move", HotkeyActions.TargetNoMove),
                new("No Attack", HotkeyActions.TargetNoAttack),
                new("Force Act Sequence", HotkeyActions.ForceActSequence),
                new("Kill All Except Target", HotkeyActions.KillAllExceptTarget),
                new("Reset Position", HotkeyActions.ResetTargetPosition)
            ],
            ["Utility"] =
            [
                new("Quitout", HotkeyActions.Quitout),
                new("Force Save", HotkeyActions.ForceSave),
                new("Noclip", HotkeyActions.Noclip),
                new("Increase Game Speed", HotkeyActions.IncreaseGameSpeed),
                new("Decrease Game Speed", HotkeyActions.DecreaseGameSpeed),
                new("Toggle Game Speed", HotkeyActions.ToggleGameSpeed),
                new("Toggle 7x Game Speed", HotkeyActions.ToggleSevenSpeed),
                new("Increase NoClip Speed", HotkeyActions.IncreaseNoClipSpeed),
                new("Decrease NoClip Speed", HotkeyActions.DecreaseNoClipSpeed),
                new("Toggle Free Cam", HotkeyActions.ToggleFreeCam),
                new("Toggle Freeze World", HotkeyActions.ToggleFreezeWorld),
                new("Move Free Cam to Player", HotkeyActions.MoveCamToPlayer),
                new("Move Player to Free Cam", HotkeyActions.MovePlayerToCam),
                new("Move Player when Free Cam", HotkeyActions.ToggleFreeCamPlayerMovement),
                new("Draw Hitbox", HotkeyActions.DrawHitbox),
                new("Draw Player Sound", HotkeyActions.DrawPlayerSound),
                new("Draw Ragdolls", HotkeyActions.DrawRagdolls),
                new("Draw Poise Bars", HotkeyActions.DrawPoiseBars),
                new("Draw Low Hit", HotkeyActions.DrawLowHit),
                new("Draw High Hit", HotkeyActions.DrawHighHit),
                new("Level Up", HotkeyActions.LevelUp),
                new("Allot Flasks", HotkeyActions.AllotFlasks),
                new("Memorize Spells", HotkeyActions.MemorizeSpells),
                new("Mix Physick", HotkeyActions.MixPhysick),
                new("Open Chest", HotkeyActions.OpenChest),
                new("Great Runes", HotkeyActions.GreatRunes),
                new("Ashes of War", HotkeyActions.AshesOfWar),
                new("Alter Garments", HotkeyActions.AlterGarments),
                new("Upgrade", HotkeyActions.Upgrade),
                new("Sell", HotkeyActions.Sell),
                new("Rebirth", HotkeyActions.Rebirth),
                new("Upgrade Flask", HotkeyActions.UpgradeFlask),
                new("Increase Flask Charges", HotkeyActions.IncreaseFlaskCharges),
                new("Open Shop Window", HotkeyActions.OpenShopWindow),
                new("Set 30 FPS", HotkeyActions.Set30Fps),
                new("Set 60 FPS", HotkeyActions.Set60Fps),
                new("Set 90 FPS", HotkeyActions.Set90Fps),
                new("Set 120 FPS", HotkeyActions.Set120Fps),
                new("Set 180 FPS", HotkeyActions.Set180Fps),
                new("Set 240 FPS", HotkeyActions.Set240Fps),
            ],

            ["Event"] =
            [
                new("Draw Events", HotkeyActions.DrawEvent),
                new("Set Morning", HotkeyActions.SetMorning),
                new("Set Noon", HotkeyActions.SetNoon),
                new("Set Night", HotkeyActions.SetNight)
            ],
        };

        Hotkeys = new SearchableGroupedCollection<string, HotkeyBindingViewModel>(
            groupedHotkeys,
            (hotkey, search) => hotkey.DisplayName.ToLower().Contains(search)
        );
        
        _hotkeyLookup = Hotkeys.AllItems.ToDictionary(h => h.ActionId);
        
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

    private bool _isHotkeyReminderEnabled;

    public bool IsHotkeyReminderEnabled
    {
        get => _isHotkeyReminderEnabled;
        set
        {
            if (SetProperty(ref _isHotkeyReminderEnabled, value))
            {
                SettingsManager.Default.HotkeyReminder = value;
                SettingsManager.Default.Save();
            }
        }
    }

    private bool _isBlockGameHotkeysEnabled;

    public bool IsBlockGameHotkeysEnabled
    {
        get => _isBlockGameHotkeysEnabled;
        set
        {
            if (SetProperty(ref _isBlockGameHotkeysEnabled, value))
            {
                SettingsManager.Default.BlockHotkeysFromGame = value;
                SettingsManager.Default.Save();
                _hotkeyManager.SetKeyboardHandling(_isBlockGameHotkeysEnabled);
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

    private bool _isMuteMusicEnabled;

    public bool IsMuteMusicEnabled
    {
        get => _isMuteMusicEnabled;
        set
        {
            if (!SetProperty(ref _isMuteMusicEnabled, value)) return;
            SettingsManager.Default.MuteMusic = value;
            SettingsManager.Default.Save();
            if (AreOptionsEnabled) _settingsService.ToggleMuteMusic(_isMuteMusicEnabled);
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
        var actionId = _currentSettingHotkeyId;

        if (actionId != null && _hotkeyLookup.TryGetValue(actionId, out var binding))
        {
            binding.HotkeyText = "None";
            _hotkeyManager.SetHotkey(actionId, new Keys());
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
        IsBlockGameHotkeysEnabled = SettingsManager.Default.BlockHotkeysFromGame;

        _isStutterFixEnabled = SettingsManager.Default.StutterFix;
        OnPropertyChanged(nameof(IsStutterFixEnabled));

        _isDisableAchievementsEnabled = SettingsManager.Default.DisableAchievements;
        OnPropertyChanged(nameof(IsDisableAchievementsEnabled));

        _isNoLogoEnabled = SettingsManager.Default.NoLogo;
        OnPropertyChanged(nameof(IsNoLogoEnabled));
        _isMuteMusicEnabled = SettingsManager.Default.MuteMusic;
        OnPropertyChanged(nameof(IsMuteMusicEnabled));

        _isHotkeyReminderEnabled = SettingsManager.Default.HotkeyReminder;
        OnPropertyChanged(nameof(IsHotkeyReminderEnabled));
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsStutterFixEnabled) _settingsService.ToggleStutterFix(true);
        if (IsDisableAchievementsEnabled) _settingsService.ToggleDisableAchievements(true);
        if (IsMuteMusicEnabled) _settingsService.ToggleMuteMusic(true);
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void OnGameAttached()
    {
        if (IsNoLogoEnabled) _settingsService.ToggleNoLogo(true);
    }

    private void OnNewGameStart()
    {
        if (!IsHotkeyReminderEnabled) return;
        if (!IsEnableHotkeysEnabled) return;
        MsgBox.Show("Hotkeys are enabled");
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
        var hook = _tempHook;
        _tempHook = null;
        _currentSettingHotkeyId = null;
        _currentKeys = null;

        if (hook != null)
        {
            hook.Down -= TempHook_Down;
            try
            {
                hook.Dispose();
            }
            catch (COMException)
            {
                // Already stopped - harmless
            }
        }
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
