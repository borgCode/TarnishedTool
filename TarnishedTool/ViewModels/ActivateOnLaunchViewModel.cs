using System.Threading.Tasks;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels;

public class ActivateOnLaunchViewModel : BaseViewModel
{
    private readonly PlayerViewModel _playerViewModel;
    private readonly EnemyViewModel _enemyViewModel;
    private readonly UtilityViewModel _utilityViewModel;
    private readonly TravelViewModel _travelViewModel;
    private readonly EventViewModel _eventViewModel;
    private readonly ItemViewModel _itemViewModel;
    private readonly ActivateOnLaunchManager _aol;

    public ActivateOnLaunchViewModel(
        PlayerViewModel playerViewModel,
        EnemyViewModel enemyViewModel,
        UtilityViewModel utilityViewModel,
        TravelViewModel travelViewModel,
        EventViewModel eventViewModel,
        ItemViewModel itemViewModel,
        ActivateOnLaunchManager activateOnLaunchManager,
        IStateService stateService)
    {
        _playerViewModel = playerViewModel;
        _enemyViewModel = enemyViewModel;
        _utilityViewModel = utilityViewModel;
        _travelViewModel = travelViewModel;
        _eventViewModel = eventViewModel;
        _itemViewModel = itemViewModel;
        _aol = activateOnLaunchManager;

        stateService.Subscribe(State.AppStart, OnAppStart);
        stateService.Subscribe(State.Attached, OnGameAttached);
        stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);

        RegisterActions();
    }

    private void OnAppStart()
    {
        if (!IsEnabled) return;
        // Player
        if (IsNoDeathChecked) _playerViewModel.IsNoDeathEnabled = true;
        if (IsNoDamageChecked) _playerViewModel.IsNoDamageEnabled = true;
        if (IsNoHitChecked) _playerViewModel.IsNoHitEnabled = true;
        if (IsInfiniteStaminaChecked) _playerViewModel.IsInfiniteStaminaEnabled = true;
        if (IsInfiniteConsumablesChecked) _playerViewModel.IsInfiniteConsumablesEnabled = true;
        if (IsInfiniteArrowsChecked) _playerViewModel.IsInfiniteArrowsEnabled = true;
        if (IsInfiniteFpChecked) _playerViewModel.IsInfiniteFpEnabled = true;
        if (IsOneShotChecked) _playerViewModel.IsOneShotEnabled = true;
        if (IsInfinitePoiseChecked) _playerViewModel.IsInfinitePoiseEnabled = true;
        if (IsSilentChecked) _playerViewModel.IsSilentEnabled = true;
        if (IsHiddenChecked) _playerViewModel.IsHiddenEnabled = true;
        if (IsFasterDeathChecked) _playerViewModel.IsFasterDeathEnabled = true;
        if (IsTorrentAnywhereChecked) _playerViewModel.IsTorrentAnywhereEnabled = true;
        if (IsTorrentNoDeathChecked) _playerViewModel.IsTorrentNoDeathEnabled = true;
        if (IsRfbsOnLoadChecked) _playerViewModel.IsSetRfbsOnLoadEnabled = true;
        if (IsNoRunesFromEnemiesChecked) _playerViewModel.IsNoRuneGainEnabled = true;
        if (IsNoRuneLossChecked) _playerViewModel.IsNoRuneLossEnabled = true;
        if (IsNoRuneArcLossChecked) _playerViewModel.IsNoRuneArcLossEnabled = true;
        if (IsNoTimeChangeOnDeathChecked) _playerViewModel.IsNoTimePassOnDeathEnabled = true;
        if (IsHpRegenChecked) _playerViewModel.IsHotEnabled = true;
        if (IsFpRegenChecked) _playerViewModel.IsFpRegenEnabled = true;

        // Travel
        if (IsNoMapAcquiredPopupsChecked) _travelViewModel.IsNoMapAcquiredPopupsEnabled = true;
        if (IsUnlockPresetGracesOnStartChecked) _travelViewModel.IsAutoUnlockPresetEnabled = true;

        // Enemies 
        if (IsAllNoDeathChecked) _enemyViewModel.IsNoDeathEnabled = true;
        if (IsAllNoDamageChecked) _enemyViewModel.IsNoDamageEnabled = true;
        if (IsAllNoHitChecked) _enemyViewModel.IsNoHitEnabled = true;
        if (IsAllNoAttackChecked) _enemyViewModel.IsNoAttackEnabled = true;
        if (IsAllNoMoveChecked) _enemyViewModel.IsNoMoveEnabled = true;
        if (IsAllDisableAiChecked) _enemyViewModel.IsDisableAiEnabled = true;

        // Utility 
        if (IsNoUpgradeCostChecked) _utilityViewModel.IsNoUpgradeCostEnabled = true;
        if (IsOpenMapInCombatChecked) _utilityViewModel.IsCombatMapEnabled = true;
        if (IsWarpInDungeonsChecked) _utilityViewModel.IsDungeonWarpEnabled = true;
        if (IsDropRateChecked) _utilityViewModel.IsGuaranteedDropEnabled = true;

        // Item
        if (IsUnlockWeaponOnStartChecked) _itemViewModel.AutoSpawnEnabled = true;
        if (IsUnlockLoadoutOnStartChecked) _itemViewModel.AutoLoadoutSpawnEnabled = true;
    }

    private async void OnNewGameStart()
    {
        if (!IsEnabled) return;
        if (!_travelViewModel.AreOptionsEnabled) return;
        await Task.Delay(1500);
        
        // Event
        if (IsStartingFlasksChecked && _eventViewModel.GiveStartingFlasksCommand.CanExecute(null))
            _eventViewModel.GiveStartingFlasksCommand.Execute(null);

        // Travel
        if (IsBaseGameMapsChecked && _travelViewModel.UnlockBaseGameMapsCommand.CanExecute(null))
        {
            _travelViewModel.UnlockBaseGameMapsCommand.Execute(null);
            await Task.Delay(300);
        }

        if (IsDlcMapsChecked && _travelViewModel.UnlockDlcMapsCommand.CanExecute(null))
        {
            _travelViewModel.UnlockDlcMapsCommand.Execute(null);
            await Task.Delay(300);
        }

        if (IsMainGracesChecked && _travelViewModel.UnlockMainGameGracesCommand.CanExecute(null))
        {
            _travelViewModel.UnlockMainGameGracesCommand.Execute(null);
            await Task.Delay(300);
        }

        if (IsDlcGracesChecked && _travelViewModel.UnlockDlcGracesCommand.CanExecute(null))
        {
            _travelViewModel.UnlockDlcGracesCommand.Execute(null);
            await Task.Delay(300);
        }

        if (IsMainArGracesChecked && _travelViewModel.UnlockBaseArGracesCommand.CanExecute(null))
        {
            _travelViewModel.UnlockBaseArGracesCommand.Execute(null);
            await Task.Delay(300);
        }

        if (IsDlcArGracesChecked && _travelViewModel.UnlockDlcArGracesCommand.CanExecute(null))
        {
            _travelViewModel.UnlockDlcArGracesCommand.Execute(null);
            await Task.Delay(300);
        }
    }

    private void OnGameAttached()
    {
        // FPS
        if (!IsEnabled) return;
        if (LaunchFps > 0) _utilityViewModel.Fps = LaunchFps;
    }
    
    
    private void RegisterActions()
    {
        _isNoDeathChecked = Get(nameof(IsNoDeathChecked));
        _isNoDamageChecked = Get(nameof(IsNoDamageChecked));
        _isNoHitChecked = Get(nameof(IsNoHitChecked));
        _isInfiniteStaminaChecked = Get(nameof(IsInfiniteStaminaChecked));
        _isInfiniteConsumablesChecked = Get(nameof(IsInfiniteConsumablesChecked));
        _isInfiniteArrowsChecked = Get(nameof(IsInfiniteArrowsChecked));
        _isInfiniteFpChecked = Get(nameof(IsInfiniteFpChecked));
        _isOneShotChecked = Get(nameof(IsOneShotChecked));
        _isInfinitePoiseChecked = Get(nameof(IsInfinitePoiseChecked));
        _isSilentChecked = Get(nameof(IsSilentChecked));
        _isHiddenChecked = Get(nameof(IsHiddenChecked));
        _isFasterDeathChecked = Get(nameof(IsFasterDeathChecked));
        _isTorrentAnywhereChecked = Get(nameof(IsTorrentAnywhereChecked));
        _isTorrentNoDeathChecked = Get(nameof(IsTorrentNoDeathChecked));
        _isRfbsOnLoadChecked = Get(nameof(IsRfbsOnLoadChecked));
        _isNoRunesFromEnemiesChecked = Get(nameof(IsNoRunesFromEnemiesChecked));
        _isNoRuneLossChecked = Get(nameof(IsNoRuneLossChecked));
        _isNoRuneArcLossChecked = Get(nameof(IsNoRuneArcLossChecked));
        _isNoTimeChangeOnDeathChecked = Get(nameof(IsNoTimeChangeOnDeathChecked));
        _isHpRegenChecked = Get(nameof(IsHpRegenChecked));
        _isFpRegenChecked = Get(nameof(IsFpRegenChecked));
        _isAllNoDeathChecked = Get(nameof(IsAllNoDeathChecked));
        _isAllNoDamageChecked = Get(nameof(IsAllNoDamageChecked));
        _isAllNoHitChecked = Get(nameof(IsAllNoHitChecked));
        _isAllNoAttackChecked = Get(nameof(IsAllNoAttackChecked));
        _isAllNoMoveChecked = Get(nameof(IsAllNoMoveChecked));
        _isAllDisableAiChecked = Get(nameof(IsAllDisableAiChecked));
        _isNoUpgradeCostChecked = Get(nameof(IsNoUpgradeCostChecked));
        _isOpenMapInCombatChecked = Get(nameof(IsOpenMapInCombatChecked));
        _isWarpInDungeonsChecked = Get(nameof(IsWarpInDungeonsChecked));
        _isDropRateChecked = Get(nameof(IsDropRateChecked));
        _isStartingFlasksChecked = Get(nameof(IsStartingFlasksChecked));
        _isNoMapAcquiredPopupsChecked = Get(nameof(IsNoMapAcquiredPopupsChecked));
        _isUnlockPresetGracesOnStartChecked = Get(nameof(IsUnlockPresetGracesOnStartChecked));
        _isBaseGameMapsChecked = Get(nameof(IsBaseGameMapsChecked));
        _isDlcMapsChecked = Get(nameof(IsDlcMapsChecked));
        _isMainGracesChecked = Get(nameof(IsMainGracesChecked));
        _isDlcGracesChecked = Get(nameof(IsDlcGracesChecked));
        _isMainArGracesChecked = Get(nameof(IsMainArGracesChecked));
        _isDlcArGracesChecked = Get(nameof(IsDlcArGracesChecked));
        _isUnlockWeaponOnStartChecked = Get(nameof(IsUnlockWeaponOnStartChecked));
        _isUnlockLoadoutOnStartChecked = Get(nameof(IsUnlockLoadoutOnStartChecked));
        _launchFps = _aol.GetInt(nameof(LaunchFps), defaultValue: 60);
    }

    // Master toggle
    private bool _isEnabled = SettingsManager.Default.ActivateOnLaunchEnabled;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (SetProperty(ref _isEnabled, value))
            {
                SettingsManager.Default.ActivateOnLaunchEnabled = value;
                SettingsManager.Default.Save();
            }
        }
    }

    // Helper macros
    private bool Get(string id) => _aol.GetBool(id);
    private void Set(string id, bool value) => _aol.SetBool(id, value);

    // Player
    private bool _isNoDeathChecked;

    public bool IsNoDeathChecked
    {
        get => _isNoDeathChecked;
        set
        {
            if (SetProperty(ref _isNoDeathChecked, value)) Set(nameof(IsNoDeathChecked), value);
        }
    }

    private bool _isNoDamageChecked;

    public bool IsNoDamageChecked
    {
        get => _isNoDamageChecked;
        set
        {
            if (SetProperty(ref _isNoDamageChecked, value)) Set(nameof(IsNoDamageChecked), value);
        }
    }

    private bool _isNoHitChecked;

    public bool IsNoHitChecked
    {
        get => _isNoHitChecked;
        set
        {
            if (SetProperty(ref _isNoHitChecked, value)) Set(nameof(IsNoHitChecked), value);
        }
    }

    private bool _isInfiniteStaminaChecked;

    public bool IsInfiniteStaminaChecked
    {
        get => _isInfiniteStaminaChecked;
        set
        {
            if (SetProperty(ref _isInfiniteStaminaChecked, value)) Set(nameof(IsInfiniteStaminaChecked), value);
        }
    }

    private bool _isInfiniteConsumablesChecked;

    public bool IsInfiniteConsumablesChecked
    {
        get => _isInfiniteConsumablesChecked;
        set
        {
            if (SetProperty(ref _isInfiniteConsumablesChecked, value)) Set(nameof(IsInfiniteConsumablesChecked), value);
        }
    }

    private bool _isInfiniteArrowsChecked;

    public bool IsInfiniteArrowsChecked
    {
        get => _isInfiniteArrowsChecked;
        set
        {
            if (SetProperty(ref _isInfiniteArrowsChecked, value)) Set(nameof(IsInfiniteArrowsChecked), value);
        }
    }

    private bool _isInfiniteFpChecked;

    public bool IsInfiniteFpChecked
    {
        get => _isInfiniteFpChecked;
        set
        {
            if (SetProperty(ref _isInfiniteFpChecked, value)) Set(nameof(IsInfiniteFpChecked), value);
        }
    }

    private bool _isOneShotChecked;

    public bool IsOneShotChecked
    {
        get => _isOneShotChecked;
        set
        {
            if (SetProperty(ref _isOneShotChecked, value)) Set(nameof(IsOneShotChecked), value);
        }
    }

    private bool _isInfinitePoiseChecked;

    public bool IsInfinitePoiseChecked
    {
        get => _isInfinitePoiseChecked;
        set
        {
            if (SetProperty(ref _isInfinitePoiseChecked, value)) Set(nameof(IsInfinitePoiseChecked), value);
        }
    }

    private bool _isSilentChecked;

    public bool IsSilentChecked
    {
        get => _isSilentChecked;
        set
        {
            if (SetProperty(ref _isSilentChecked, value)) Set(nameof(IsSilentChecked), value);
        }
    }

    private bool _isHiddenChecked;

    public bool IsHiddenChecked
    {
        get => _isHiddenChecked;
        set
        {
            if (SetProperty(ref _isHiddenChecked, value)) Set(nameof(IsHiddenChecked), value);
        }
    }

    private bool _isFasterDeathChecked;

    public bool IsFasterDeathChecked
    {
        get => _isFasterDeathChecked;
        set
        {
            if (SetProperty(ref _isFasterDeathChecked, value)) Set(nameof(IsFasterDeathChecked), value);
        }
    }

    private bool _isTorrentAnywhereChecked;

    public bool IsTorrentAnywhereChecked
    {
        get => _isTorrentAnywhereChecked;
        set
        {
            if (SetProperty(ref _isTorrentAnywhereChecked, value)) Set(nameof(IsTorrentAnywhereChecked), value);
        }
    }

    private bool _isTorrentNoDeathChecked;

    public bool IsTorrentNoDeathChecked
    {
        get => _isTorrentNoDeathChecked;
        set
        {
            if (SetProperty(ref _isTorrentNoDeathChecked, value)) Set(nameof(IsTorrentNoDeathChecked), value);
        }
    }

    private bool _isRfbsOnLoadChecked;

    public bool IsRfbsOnLoadChecked
    {
        get => _isRfbsOnLoadChecked;
        set
        {
            if (SetProperty(ref _isRfbsOnLoadChecked, value)) Set(nameof(IsRfbsOnLoadChecked), value);
        }
    }

    private bool _isNoRunesFromEnemiesChecked;

    public bool IsNoRunesFromEnemiesChecked
    {
        get => _isNoRunesFromEnemiesChecked;
        set
        {
            if (SetProperty(ref _isNoRunesFromEnemiesChecked, value)) Set(nameof(IsNoRunesFromEnemiesChecked), value);
        }
    }

    private bool _isNoRuneLossChecked;

    public bool IsNoRuneLossChecked
    {
        get => _isNoRuneLossChecked;
        set
        {
            if (SetProperty(ref _isNoRuneLossChecked, value)) Set(nameof(IsNoRuneLossChecked), value);
        }
    }

    private bool _isNoRuneArcLossChecked;

    public bool IsNoRuneArcLossChecked
    {
        get => _isNoRuneArcLossChecked;
        set
        {
            if (SetProperty(ref _isNoRuneArcLossChecked, value)) Set(nameof(IsNoRuneArcLossChecked), value);
        }
    }

    private bool _isNoTimeChangeOnDeathChecked;

    public bool IsNoTimeChangeOnDeathChecked
    {
        get => _isNoTimeChangeOnDeathChecked;
        set
        {
            if (SetProperty(ref _isNoTimeChangeOnDeathChecked, value)) Set(nameof(IsNoTimeChangeOnDeathChecked), value);
        }
    }

    private bool _isHpRegenChecked;

    public bool IsHpRegenChecked
    {
        get => _isHpRegenChecked;
        set
        {
            if (SetProperty(ref _isHpRegenChecked, value)) Set(nameof(IsHpRegenChecked), value);
        }
    }

    private bool _isFpRegenChecked;

    public bool IsFpRegenChecked
    {
        get => _isFpRegenChecked;
        set
        {
            if (SetProperty(ref _isFpRegenChecked, value)) Set(nameof(IsFpRegenChecked), value);
        }
    }

    // Enemies
    private bool _isAllNoDeathChecked;

    public bool IsAllNoDeathChecked
    {
        get => _isAllNoDeathChecked;
        set
        {
            if (SetProperty(ref _isAllNoDeathChecked, value)) Set(nameof(IsAllNoDeathChecked), value);
        }
    }

    private bool _isAllNoDamageChecked;

    public bool IsAllNoDamageChecked
    {
        get => _isAllNoDamageChecked;
        set
        {
            if (SetProperty(ref _isAllNoDamageChecked, value)) Set(nameof(IsAllNoDamageChecked), value);
        }
    }

    private bool _isAllNoHitChecked;

    public bool IsAllNoHitChecked
    {
        get => _isAllNoHitChecked;
        set
        {
            if (SetProperty(ref _isAllNoHitChecked, value)) Set(nameof(IsAllNoHitChecked), value);
        }
    }

    private bool _isAllNoAttackChecked;

    public bool IsAllNoAttackChecked
    {
        get => _isAllNoAttackChecked;
        set
        {
            if (SetProperty(ref _isAllNoAttackChecked, value)) Set(nameof(IsAllNoAttackChecked), value);
        }
    }

    private bool _isAllNoMoveChecked;

    public bool IsAllNoMoveChecked
    {
        get => _isAllNoMoveChecked;
        set
        {
            if (SetProperty(ref _isAllNoMoveChecked, value)) Set(nameof(IsAllNoMoveChecked), value);
        }
    }

    private bool _isAllDisableAiChecked;

    public bool IsAllDisableAiChecked
    {
        get => _isAllDisableAiChecked;
        set
        {
            if (SetProperty(ref _isAllDisableAiChecked, value)) Set(nameof(IsAllDisableAiChecked), value);
        }
    }

    // Utility
    private bool _isNoUpgradeCostChecked;

    public bool IsNoUpgradeCostChecked
    {
        get => _isNoUpgradeCostChecked;
        set
        {
            if (SetProperty(ref _isNoUpgradeCostChecked, value)) Set(nameof(IsNoUpgradeCostChecked), value);
        }
    }

    private bool _isOpenMapInCombatChecked;

    public bool IsOpenMapInCombatChecked
    {
        get => _isOpenMapInCombatChecked;
        set
        {
            if (SetProperty(ref _isOpenMapInCombatChecked, value)) Set(nameof(IsOpenMapInCombatChecked), value);
        }
    }

    private bool _isWarpInDungeonsChecked;

    public bool IsWarpInDungeonsChecked
    {
        get => _isWarpInDungeonsChecked;
        set
        {
            if (SetProperty(ref _isWarpInDungeonsChecked, value)) Set(nameof(IsWarpInDungeonsChecked), value);
        }
    }

    private bool _isDropRateChecked;

    public bool IsDropRateChecked
    {
        get => _isDropRateChecked;
        set
        {
            if (SetProperty(ref _isDropRateChecked, value)) Set(nameof(IsDropRateChecked), value);
        }
    }

    // FPS
    private int _launchFps;

    public int LaunchFps
    {
        get => _launchFps;
        set
        {
            if (SetProperty(ref _launchFps, value))
                _aol.SetInt(nameof(LaunchFps), value);
        }
    }

    // Event
    private bool _isStartingFlasksChecked;

    public bool IsStartingFlasksChecked
    {
        get => _isStartingFlasksChecked;
        set
        {
            if (SetProperty(ref _isStartingFlasksChecked, value)) Set(nameof(IsStartingFlasksChecked), value);
        }
    }

    // Travel
    private bool _isNoMapAcquiredPopupsChecked;

    public bool IsNoMapAcquiredPopupsChecked
    {
        get => _isNoMapAcquiredPopupsChecked;
        set
        {
            if (SetProperty(ref _isNoMapAcquiredPopupsChecked, value)) Set(nameof(IsNoMapAcquiredPopupsChecked), value);
        }
    }

    private bool _isUnlockPresetGracesOnStartChecked;

    public bool IsUnlockPresetGracesOnStartChecked
    {
        get => _isUnlockPresetGracesOnStartChecked;
        set
        {
            if (SetProperty(ref _isUnlockPresetGracesOnStartChecked, value))
            {
                if (value)
                {
                    IsMainGracesChecked = false;
                    IsMainArGracesChecked = false;
                    IsDlcArGracesChecked = false;
                    IsDlcGracesChecked = false;
                }

                Set(nameof(IsUnlockPresetGracesOnStartChecked), value);
            }
        }
    }

    private bool _isBaseGameMapsChecked;

    public bool IsBaseGameMapsChecked
    {
        get => _isBaseGameMapsChecked;
        set
        {
            if (SetProperty(ref _isBaseGameMapsChecked, value)) Set(nameof(IsBaseGameMapsChecked), value);
        }
    }

    private bool _isDlcMapsChecked;

    public bool IsDlcMapsChecked
    {
        get => _isDlcMapsChecked;
        set
        {
            if (SetProperty(ref _isDlcMapsChecked, value)) Set(nameof(IsDlcMapsChecked), value);
        }
    }

    private bool _isMainGracesChecked;

    public bool IsMainGracesChecked
    {
        get => _isMainGracesChecked;
        set
        {
            if (SetProperty(ref _isMainGracesChecked, value))
            {
                if (value)
                {
                    IsMainArGracesChecked = false;
                    IsUnlockPresetGracesOnStartChecked = false;
                }

                Set(nameof(IsMainGracesChecked), value);
            }
        }
    }

    private bool _isDlcGracesChecked;

    public bool IsDlcGracesChecked
    {
        get => _isDlcGracesChecked;
        set
        {
            if (SetProperty(ref _isDlcGracesChecked, value))
            {
                if (value)
                {
                    IsDlcArGracesChecked = false;
                    IsUnlockPresetGracesOnStartChecked = false;
                }

                Set(nameof(IsDlcGracesChecked), value);
            }
        }
    }

    private bool _isMainArGracesChecked;

    public bool IsMainArGracesChecked
    {
        get => _isMainArGracesChecked;
        set
        {
            if (SetProperty(ref _isMainArGracesChecked, value))
            {
                if (value)
                {
                    IsMainGracesChecked = false;
                    IsUnlockPresetGracesOnStartChecked = false;
                }

                Set(nameof(IsMainArGracesChecked), value);
            }
        }
    }

    private bool _isDlcArGracesChecked;

    public bool IsDlcArGracesChecked
    {
        get => _isDlcArGracesChecked;
        set
        {
            if (SetProperty(ref _isDlcArGracesChecked, value))
            {
                if (value)
                {
                    IsDlcGracesChecked = false;
                    IsUnlockPresetGracesOnStartChecked = false;
                }

                Set(nameof(IsDlcArGracesChecked), value);
            }
        }
    }

    // Items
    private bool _isUnlockWeaponOnStartChecked;

    public bool IsUnlockWeaponOnStartChecked
    {
        get => _isUnlockWeaponOnStartChecked;
        set
        {
            if (SetProperty(ref _isUnlockWeaponOnStartChecked, value)) Set(nameof(IsUnlockWeaponOnStartChecked), value);
        }
    }

    private bool _isUnlockLoadoutOnStartChecked;

    public bool IsUnlockLoadoutOnStartChecked
    {
        get => _isUnlockLoadoutOnStartChecked;
        set
        {
            if (SetProperty(ref _isUnlockLoadoutOnStartChecked, value))
                Set(nameof(IsUnlockLoadoutOnStartChecked), value);
        }
    }
}