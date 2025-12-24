using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels;

public class EnemyViewModel : BaseViewModel
{
    private readonly IEnemyService _enemyService;
    private readonly HotkeyManager _hotkeyManager;

    private const int EbNpcThinkParamId = 22000000;
    private const int EldenStarsActIdx = 22;
    private DateTime _ebLastExecuted = DateTime.MinValue;
    private static readonly TimeSpan EbCooldownDuration = TimeSpan.FromSeconds(2);
    
    public EnemyViewModel(IEnemyService enemyService, IStateService stateService, HotkeyManager hotkeyManager)
    {
        _enemyService = enemyService;
        _hotkeyManager = hotkeyManager;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);

        EbForceActSequenceCommand = new DelegateCommand(ForceEbActSequence);

        _acts = new ObservableCollection<Act>(DataLoader.GetEbActs());
        SelectedAct = Acts.FirstOrDefault();

        RegisterHotkeys();
    }

    #region Commands

    public ICommand EbForceActSequenceCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled = true;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }
    
    private bool _isNoDeathEnabled;

    public bool IsNoDeathEnabled
    {
        get => _isNoDeathEnabled;
        set
        {
            SetProperty(ref _isNoDeathEnabled, value);
            _enemyService.ToggleNoDeath(_isNoDeathEnabled);
        }
    }

    private bool _isNoDamageEnabled;

    public bool IsNoDamageEnabled
    {
        get => _isNoDamageEnabled;
        set
        {
            SetProperty(ref _isNoDamageEnabled, value);
            _enemyService.ToggleNoDamage(_isNoDamageEnabled);
        }
    }

    private bool _isNoHitEnabled;

    public bool IsNoHitEnabled
    {
        get => _isNoHitEnabled;
        set
        {
            SetProperty(ref _isNoHitEnabled, value);
            _enemyService.ToggleNoHit(_isNoHitEnabled);
        }
    }

    private bool _isNoAttackEnabled;

    public bool IsNoAttackEnabled
    {
        get => _isNoAttackEnabled;
        set
        {
            SetProperty(ref _isNoAttackEnabled, value);
            _enemyService.ToggleNoAttack(_isNoAttackEnabled);
        }
    }

    private bool _isNoMoveEnabled;

    public bool IsNoMoveEnabled
    {
        get => _isNoMoveEnabled;
        set
        {
            SetProperty(ref _isNoMoveEnabled, value);
            _enemyService.ToggleNoMove(_isNoMoveEnabled);
        }
    }

    private bool _isDisableAiEnabled;

    public bool IsDisableAiEnabled
    {
        get => _isDisableAiEnabled;
        set
        {
            SetProperty(ref _isDisableAiEnabled, value);
            _enemyService.ToggleDisableAi(_isDisableAiEnabled);
        }
    }

    private bool _isTargetingViewEnabled;

    public bool IsTargetingViewEnabled
    {
        get => _isTargetingViewEnabled;
        set
        {
            if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
            _enemyService.ToggleTargetingView(_isTargetingViewEnabled);
            if (!_isTargetingViewEnabled)
            {
                IsDrawReducedTargetViewEnabled = false;
            }
        }
    }

    private bool _isDrawReducedTargetViewEnabled;

    public bool IsDrawReducedTargetViewEnabled
    {
        get => _isDrawReducedTargetViewEnabled;
        set
        {
            if (!SetProperty(ref _isDrawReducedTargetViewEnabled, value)) return;
            _enemyService.ToggleReducedTargetingView(_isDrawReducedTargetViewEnabled);
            _enemyService.SetTargetViewMaxDist(ReducedTargetViewDistance);
        }
    }

    private float _reducedTargetViewDistance = 100;

    public float ReducedTargetViewDistance
    {
        get => _reducedTargetViewDistance;
        set
        {
            if (!SetProperty(ref _reducedTargetViewDistance, value)) return;
            if (!IsDrawReducedTargetViewEnabled) return;
            _enemyService.SetTargetViewMaxDist(_reducedTargetViewDistance);
        }
    }
    
    private bool _isRykardNoMegaEnabled;

    public bool IsRykardNoMegaEnabled
    {
        get => _isRykardNoMegaEnabled;
        set
        {
            SetProperty(ref _isRykardNoMegaEnabled, value);
            _enemyService.ToggleRykardMega(_isRykardNoMegaEnabled);
        }
    }
    
    private ObservableCollection<Act> _acts;
    
    public ObservableCollection<Act> Acts
    {
        get => _acts;
        set => SetProperty(ref _acts, value);
    }
    
    private Act _selectedAct;

    public Act SelectedAct
    {
        get => _selectedAct;
        set => SetProperty(ref _selectedAct, value);
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void OnGameFirstLoaded()
    {
        if (IsNoDeathEnabled) _enemyService.ToggleNoDeath(true);
        if (IsNoDamageEnabled) _enemyService.ToggleNoDamage(true);
        if (IsNoHitEnabled) _enemyService.ToggleNoHit(true);
        if (IsNoAttackEnabled) _enemyService.ToggleNoAttack(true);
        if (IsNoMoveEnabled) _enemyService.ToggleNoMove(true);
        if (IsDisableAiEnabled) _enemyService.ToggleDisableAi(true);
        if (IsRykardNoMegaEnabled) _enemyService.ToggleRykardMega(true);
        if (IsTargetingViewEnabled) _enemyService.ToggleTargetingView(true);
        if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled)
            _enemyService.ToggleReducedTargetingView(true);
        if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled)
            _enemyService.SetTargetViewMaxDist(ReducedTargetViewDistance);
    }
    
    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoDeath, () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoDamage, () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoHit, () => { IsNoHitEnabled = !IsNoHitEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoAttack, () => { IsNoAttackEnabled = !IsNoAttackEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoMove, () => { IsNoMoveEnabled = !IsNoMoveEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllDisableAi, () => { IsDisableAiEnabled = !IsDisableAiEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.ForceEbActSequence, () => SafeExecute(ForceEbActSequence));
    }

    private void SafeExecute(Action action)
    {
        if (!AreOptionsEnabled) return;
        action();
    }

    private void ForceEbActSequence()
    {
        var now = DateTime.UtcNow;
        if (now - _ebLastExecuted < EbCooldownDuration) return;
        _ebLastExecuted = now;
        int[] acts = [EldenStarsActIdx, SelectedAct.ActIdx];
        _enemyService.ForceActSequence(acts, EbNpcThinkParamId);
    }

    #endregion
}