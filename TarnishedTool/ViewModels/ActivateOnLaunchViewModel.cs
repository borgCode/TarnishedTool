using System.Threading.Tasks;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels
{
    public class ActivateOnLaunchViewModel : BaseViewModel
    {
        private readonly PlayerViewModel _playerViewModel;
        private readonly EnemyViewModel _enemyViewModel;
        private readonly UtilityViewModel _utilityViewModel;
        private readonly TravelViewModel _travelViewModel;
        private readonly EventViewModel _eventViewModel;
        private readonly ItemViewModel _itemViewModel;

        public ActivateOnLaunchViewModel(
            PlayerViewModel playerViewModel,
            EnemyViewModel enemyViewModel,
            UtilityViewModel utilityViewModel,
            TravelViewModel travelViewModel,
            EventViewModel eventViewModel,
            ItemViewModel itemViewModel,
            IStateService stateService)
        {
            _playerViewModel = playerViewModel;
            _enemyViewModel = enemyViewModel;
            _utilityViewModel = utilityViewModel;
            _travelViewModel = travelViewModel;
            _eventViewModel = eventViewModel;
            _itemViewModel = itemViewModel;


            stateService.Subscribe(State.AppStart, OnAppStart);
            stateService.Subscribe(State.Attached, OnGameAttached);
            stateService.Subscribe(State.FirstLoaded, OnFirstLoaded);
            stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);
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

        private void OnNewGameStart()
        {
            // Event
            if (IsStartingFlasksChecked && _eventViewModel.GiveStartingFlasksCommand.CanExecute(null))
            {
                _eventViewModel.GiveStartingFlasksCommand.Execute(null);
            }
        }

        private void OnGameAttached()
        {
            if (!IsEnabled) return;
            // FPS 
            if (LaunchFps > 0)
            {
                _utilityViewModel.Fps = LaunchFps;
            }
        }

        private async void OnFirstLoaded()
        {
            if (!IsEnabled) return;
            if (!_travelViewModel.AreOptionsEnabled) return;
            await Task.Delay(1500);

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

        // Player

        private bool _isNoDeathChecked = SettingsManager.Default.AolNoDeath;

        public bool IsNoDeathChecked
        {
            get => _isNoDeathChecked;
            set
            {
                if (SetProperty(ref _isNoDeathChecked, value))
                {
                    SettingsManager.Default.AolNoDeath = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isNoDamageChecked = SettingsManager.Default.AolNoDamage;

        public bool IsNoDamageChecked
        {
            get => _isNoDamageChecked;
            set
            {
                if (SetProperty(ref _isNoDamageChecked, value))
                {
                    SettingsManager.Default.AolNoDamage = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isNoHitChecked = SettingsManager.Default.AolNoHit;

        public bool IsNoHitChecked
        {
            get => _isNoHitChecked;
            set
            {
                if (SetProperty(ref _isNoHitChecked, value))
                {
                    SettingsManager.Default.AolNoHit = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isInfiniteStaminaChecked = SettingsManager.Default.AolInfiniteStamina;

        public bool IsInfiniteStaminaChecked
        {
            get => _isInfiniteStaminaChecked;
            set
            {
                if (SetProperty(ref _isInfiniteStaminaChecked, value))
                {
                    SettingsManager.Default.AolInfiniteStamina = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isInfiniteConsumablesChecked = SettingsManager.Default.AolInfiniteConsumables;

        public bool IsInfiniteConsumablesChecked
        {
            get => _isInfiniteConsumablesChecked;
            set
            {
                if (SetProperty(ref _isInfiniteConsumablesChecked, value))
                {
                    SettingsManager.Default.AolInfiniteConsumables = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isInfiniteArrowsChecked = SettingsManager.Default.AolInfiniteArrows;

        public bool IsInfiniteArrowsChecked
        {
            get => _isInfiniteArrowsChecked;
            set
            {
                if (SetProperty(ref _isInfiniteArrowsChecked, value))
                {
                    SettingsManager.Default.AolInfiniteArrows = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isInfiniteFpChecked = SettingsManager.Default.AolInfiniteFp;

        public bool IsInfiniteFpChecked
        {
            get => _isInfiniteFpChecked;
            set
            {
                if (SetProperty(ref _isInfiniteFpChecked, value))
                {
                    SettingsManager.Default.AolInfiniteFp = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isOneShotChecked = SettingsManager.Default.AolOneShot;

        public bool IsOneShotChecked
        {
            get => _isOneShotChecked;
            set
            {
                if (SetProperty(ref _isOneShotChecked, value))
                {
                    SettingsManager.Default.AolOneShot = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isInfinitePoiseChecked = SettingsManager.Default.AolInfinitePoise;

        public bool IsInfinitePoiseChecked
        {
            get => _isInfinitePoiseChecked;
            set
            {
                if (SetProperty(ref _isInfinitePoiseChecked, value))
                {
                    SettingsManager.Default.AolInfinitePoise = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isSilentChecked = SettingsManager.Default.AolSilent;

        public bool IsSilentChecked
        {
            get => _isSilentChecked;
            set
            {
                if (SetProperty(ref _isSilentChecked, value))
                {
                    SettingsManager.Default.AolSilent = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isHiddenChecked = SettingsManager.Default.AolHidden;

        public bool IsHiddenChecked
        {
            get => _isHiddenChecked;
            set
            {
                if (SetProperty(ref _isHiddenChecked, value))
                {
                    SettingsManager.Default.AolHidden = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isFasterDeathChecked = SettingsManager.Default.AolFasterDeath;

        public bool IsFasterDeathChecked
        {
            get => _isFasterDeathChecked;
            set
            {
                if (SetProperty(ref _isFasterDeathChecked, value))
                {
                    SettingsManager.Default.AolFasterDeath = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isTorrentAnywhereChecked = SettingsManager.Default.AolTorrentAnywhere;

        public bool IsTorrentAnywhereChecked
        {
            get => _isTorrentAnywhereChecked;
            set
            {
                if (SetProperty(ref _isTorrentAnywhereChecked, value))
                {
                    SettingsManager.Default.AolTorrentAnywhere = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isTorrentNoDeathChecked = SettingsManager.Default.AolTorrentNoDeath;

        public bool IsTorrentNoDeathChecked
        {
            get => _isTorrentNoDeathChecked;
            set
            {
                if (SetProperty(ref _isTorrentNoDeathChecked, value))
                {
                    SettingsManager.Default.AolTorrentNoDeath = value;
                    SettingsManager.Default.Save();
                }
            }
        }
        
        private bool _isRfbsOnLoadChecked = SettingsManager.Default.AolRfbsOnLoad;

        public bool IsRfbsOnLoadChecked
        {
            get => _isRfbsOnLoadChecked;
            set
            {
                if (SetProperty(ref _isRfbsOnLoadChecked, value))
                {
                    SettingsManager.Default.AolRfbsOnLoad = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isNoRunesFromEnemiesChecked = SettingsManager.Default.AolNoRunesFromEnemies;

        public bool IsNoRunesFromEnemiesChecked
        {
            get => _isNoRunesFromEnemiesChecked;
            set
            {
                if (SetProperty(ref _isNoRunesFromEnemiesChecked, value))
                {
                    SettingsManager.Default.AolNoRunesFromEnemies = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isNoRuneLossChecked = SettingsManager.Default.AolNoRuneLoss;

        public bool IsNoRuneLossChecked
        {
            get => _isNoRuneLossChecked;
            set
            {
                if (SetProperty(ref _isNoRuneLossChecked, value))
                {
                    SettingsManager.Default.AolNoRuneLoss = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isNoRuneArcLossChecked = SettingsManager.Default.AolNoRuneArcLoss;

        public bool IsNoRuneArcLossChecked
        {
            get => _isNoRuneArcLossChecked;
            set
            {
                if (SetProperty(ref _isNoRuneArcLossChecked, value))
                {
                    SettingsManager.Default.AolNoRuneArcLoss = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isNoTimeChangeOnDeathChecked = SettingsManager.Default.AolNoTimeChangeOnDeath;

        public bool IsNoTimeChangeOnDeathChecked
        {
            get => _isNoTimeChangeOnDeathChecked;
            set
            {
                if (SetProperty(ref _isNoTimeChangeOnDeathChecked, value))
                {
                    SettingsManager.Default.AolNoTimeChangeOnDeath = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isHpRegenChecked = SettingsManager.Default.AolHpRegen;

        public bool IsHpRegenChecked
        {
            get => _isHpRegenChecked;
            set
            {
                if (SetProperty(ref _isHpRegenChecked, value))
                {
                    SettingsManager.Default.AolHpRegen = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isFpRegenChecked = SettingsManager.Default.AolFpRegen;

        public bool IsFpRegenChecked
        {
            get => _isFpRegenChecked;
            set
            {
                if (SetProperty(ref _isFpRegenChecked, value))
                {
                    SettingsManager.Default.AolFpRegen = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        // Enemies

        private bool _isAllNoDeathChecked = SettingsManager.Default.AolAllNoDeath;

        public bool IsAllNoDeathChecked
        {
            get => _isAllNoDeathChecked;
            set
            {
                if (SetProperty(ref _isAllNoDeathChecked, value))
                {
                    SettingsManager.Default.AolAllNoDeath = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isAllNoDamageChecked = SettingsManager.Default.AolAllNoDamage;

        public bool IsAllNoDamageChecked
        {
            get => _isAllNoDamageChecked;
            set
            {
                if (SetProperty(ref _isAllNoDamageChecked, value))
                {
                    SettingsManager.Default.AolAllNoDamage = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isAllNoHitChecked = SettingsManager.Default.AolAllNoHit;

        public bool IsAllNoHitChecked
        {
            get => _isAllNoHitChecked;
            set
            {
                if (SetProperty(ref _isAllNoHitChecked, value))
                {
                    SettingsManager.Default.AolAllNoHit = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isAllNoAttackChecked = SettingsManager.Default.AolAllNoAttack;

        public bool IsAllNoAttackChecked
        {
            get => _isAllNoAttackChecked;
            set
            {
                if (SetProperty(ref _isAllNoAttackChecked, value))
                {
                    SettingsManager.Default.AolAllNoAttack = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isAllNoMoveChecked = SettingsManager.Default.AolAllNoMove;

        public bool IsAllNoMoveChecked
        {
            get => _isAllNoMoveChecked;
            set
            {
                if (SetProperty(ref _isAllNoMoveChecked, value))
                {
                    SettingsManager.Default.AolAllNoMove = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isAllDisableAiChecked = SettingsManager.Default.AolAllDisableAi;

        public bool IsAllDisableAiChecked
        {
            get => _isAllDisableAiChecked;
            set
            {
                if (SetProperty(ref _isAllDisableAiChecked, value))
                {
                    SettingsManager.Default.AolAllDisableAi = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        // Utility

        private bool _isNoUpgradeCostChecked = SettingsManager.Default.AolNoUpgradeCost;

        public bool IsNoUpgradeCostChecked
        {
            get => _isNoUpgradeCostChecked;
            set
            {
                if (SetProperty(ref _isNoUpgradeCostChecked, value))
                {
                    SettingsManager.Default.AolNoUpgradeCost = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isOpenMapInCombatChecked = SettingsManager.Default.AolOpenMapInCombat;

        public bool IsOpenMapInCombatChecked
        {
            get => _isOpenMapInCombatChecked;
            set
            {
                if (SetProperty(ref _isOpenMapInCombatChecked, value))
                {
                    SettingsManager.Default.AolOpenMapInCombat = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isWarpInDungeonsChecked = SettingsManager.Default.AolWarpInDungeons;

        public bool IsWarpInDungeonsChecked
        {
            get => _isWarpInDungeonsChecked;
            set
            {
                if (SetProperty(ref _isWarpInDungeonsChecked, value))
                {
                    SettingsManager.Default.AolWarpInDungeons = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isDropRateChecked = SettingsManager.Default.AolDropRate;

        public bool IsDropRateChecked
        {
            get => _isDropRateChecked;
            set
            {
                if (SetProperty(ref _isDropRateChecked, value))
                {
                    SettingsManager.Default.AolDropRate = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        // FPS

        private int _launchFps = SettingsManager.Default.AolFps;

        public int LaunchFps
        {
            get => _launchFps;
            set
            {
                if (SetProperty(ref _launchFps, value))
                {
                    SettingsManager.Default.AolFps = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        // Event
        private bool _isStartingFlasksChecked = SettingsManager.Default.AolStartingFlasks;

        public bool IsStartingFlasksChecked
        {
            get => _isStartingFlasksChecked;
            set
            {
                if (SetProperty(ref _isStartingFlasksChecked, value))
                {
                    SettingsManager.Default.AolStartingFlasks = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        // Travel
        private bool _isNoMapAcquiredPopupsChecked = SettingsManager.Default.AolNoMapAcquiredPopups;

        public bool IsNoMapAcquiredPopupsChecked
        {
            get => _isNoMapAcquiredPopupsChecked;
            set
            {
                if (SetProperty(ref _isNoMapAcquiredPopupsChecked, value))
                {
                    SettingsManager.Default.AolNoMapAcquiredPopups = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isUnlockPresetGracesOnStartChecked = SettingsManager.Default.AolUnlockPresetGracesOnStart;

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
                    SettingsManager.Default.AolUnlockPresetGracesOnStart = value;
                    SettingsManager.Default.Save();
                }
            }
        }
        

        private bool _isBaseGameMapsChecked = SettingsManager.Default.AolBaseGameMaps;

        public bool IsBaseGameMapsChecked
        {
            get => _isBaseGameMapsChecked;
            set
            {
                if (SetProperty(ref _isBaseGameMapsChecked, value))
                {
                    SettingsManager.Default.AolBaseGameMaps = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isDlcMapsChecked = SettingsManager.Default.AolDlcMaps;

        public bool IsDlcMapsChecked
        {
            get => _isDlcMapsChecked;
            set
            {
                if (SetProperty(ref _isDlcMapsChecked, value))
                {
                    SettingsManager.Default.AolDlcMaps = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isMainGracesChecked = SettingsManager.Default.AolMainGraces;

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
                    SettingsManager.Default.AolMainGraces = value;
                    SettingsManager.Default.Save();
                }
            }
        }


        private bool _isDlcGracesChecked = SettingsManager.Default.AolDlcGraces;

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
                    SettingsManager.Default.AolDlcGraces = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isMainArGracesChecked = SettingsManager.Default.AolMainArGraces;

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
                    SettingsManager.Default.AolMainArGraces = value;
                    SettingsManager.Default.Save();
                }
            }
        }

        private bool _isDlcArGracesChecked = SettingsManager.Default.AolDlcArGraces;

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
                    SettingsManager.Default.AolDlcArGraces = value;
                    SettingsManager.Default.Save();
                }
            }
        }
        private bool _isUnlockWeaponOnStartChecked = SettingsManager.Default.AolUnlockWeaponOnStart;

        public bool IsUnlockWeaponOnStartChecked
        {
            get => _isUnlockWeaponOnStartChecked;
            set
            {
                if (SetProperty(ref _isUnlockWeaponOnStartChecked, value))
                {
                    SettingsManager.Default.AolUnlockWeaponOnStart = value;
                    SettingsManager.Default.Save();
                }
            }
        }
        
        // Items
        private bool _isUnlockLoadoutOnStartChecked = SettingsManager.Default.AolUnlockLoadoutOnStart;

        public bool IsUnlockLoadoutOnStartChecked
        {
            get => _isUnlockLoadoutOnStartChecked;
            set
            {
                if (SetProperty(ref _isUnlockLoadoutOnStartChecked, value))
                {
                    SettingsManager.Default.AolUnlockLoadoutOnStart = value;
                    SettingsManager.Default.Save();
                }
            }
        }
    }
}