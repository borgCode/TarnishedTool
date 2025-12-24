using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Models;
using SilkyRing.Utilities;
using SilkyRing.Views.Windows;

namespace SilkyRing.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private const float DefaultNoclipMultiplier = 1f;

        private float _desiredGameSpeed = -1f;
        private const float DefaultGameSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _wasNoDeathEnabled;
        
        private ShopSelectorWindow _shopSelectorWindow;

        private readonly IUtilityService _utilityService;
        private readonly IEzStateService _ezStateService;
        private readonly IPlayerService _playerService;
        private readonly HotkeyManager _hotkeyManager;
        private readonly IEmevdService _emevdService;
        private readonly PlayerViewModel _playerViewModel;
        private readonly IDlcService _dlcService;
        
        private readonly List<ShopCommand> _allShops;

        public UtilityViewModel(IUtilityService utilityService, IStateService stateService,
            IEzStateService ezStateService, IPlayerService playerService, HotkeyManager hotkeyManager,
            IEmevdService emevdService, PlayerViewModel playerViewModel, IDlcService dlcService)
        {
            _utilityService = utilityService;
            _ezStateService = ezStateService;
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;
            _emevdService = emevdService;
            _playerViewModel = playerViewModel;
            _dlcService = dlcService;

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);

            SaveCommand = new DelegateCommand(Save);
            TriggerNgCycleCommand = new DelegateCommand(TriggerNgCycle);
            SetMorningCommand = new DelegateCommand(SetMorning);
            SetNoonCommand = new DelegateCommand(SetNoon);
            SetNightCommand = new DelegateCommand(SetNight);
            OpenLevelUpCommand = new DelegateCommand(OpenLevelUp);
            OpenAllotCommand = new DelegateCommand(OpenAllot);
            AttunementCommand = new DelegateCommand(OpenAttunement);
            OpenPhysickCommand = new DelegateCommand(OpenPhysick);
            OpenChestCommand = new DelegateCommand(OpenChest);
            OpenGreatRunesCommand = new DelegateCommand(OpenGreatRunes);
            OpenAowCommand = new DelegateCommand(OpenAow);
            OpenAlterGarmentsCommand = new DelegateCommand(OpenAlterGarments);
            OpenUpgradeCommand = new DelegateCommand(OpenUpgrade);
            OpenSellCommand = new DelegateCommand(OpenSell);
            OpenRebirthCommand = new DelegateCommand(OpenRebirth);
            OpenShopSelectorCommand = new DelegateCommand(OpenShopSelector);
            OpenShopCommand = new DelegateCommand<ShopCommand>(OpenShop);

            _allShops = DataLoader.GetShops();
            FilteredShops = new ObservableCollection<ShopCommand>();
            
            RegisterHotkeys();
            ApplyPrefs();
        }

        #region Commands

        public ICommand SaveCommand { get; set; }
        public ICommand TriggerNgCycleCommand { get; set; }
        public ICommand SetMorningCommand { get; set; }
        public ICommand SetNoonCommand { get; set; }
        public ICommand SetNightCommand { get; set; }
        public ICommand OpenLevelUpCommand { get; set; }
        public ICommand OpenAllotCommand { get; set; }
        public ICommand AttunementCommand { get; set; }
        public ICommand OpenPhysickCommand { get; set; }
        public ICommand OpenChestCommand { get; set; }
        public ICommand OpenGreatRunesCommand { get; set; }
        public ICommand OpenAowCommand { get; set; }
        public ICommand OpenAlterGarmentsCommand { get; set; }
        public ICommand OpenUpgradeCommand { get; set; }
        public ICommand OpenSellCommand { get; set; }
        public ICommand OpenRebirthCommand { get; set; }
        public ICommand OpenShopSelectorCommand { get; set; }
        public ICommand OpenShopCommand { get; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }
        
        private bool _isDlcAvailable;
        
        public bool IsDlcAvailable
        {
            get => _isDlcAvailable;
            set => SetProperty(ref _isDlcAvailable, value);
        }

        private bool _isNoClipEnabled;

        public bool IsNoClipEnabled
        {
            get => _isNoClipEnabled;
            set
            {
                if (!SetProperty(ref _isNoClipEnabled, value)) return;
                if (_isNoClipEnabled)
                {
                    _utilityService.WriteNoClipSpeed(NoClipSpeed);
                    _wasNoDeathEnabled = _playerViewModel.IsNoDeathEnabled;
                    _playerViewModel.IsNoDeathEnabled = true;
                    _utilityService.ToggleNoClip(_isNoClipEnabled);
                }
                else
                {
                    _utilityService.ToggleNoClip(_isNoClipEnabled);
                    _playerViewModel.IsNoDeathEnabled = _wasNoDeathEnabled;
                }
            }
        }

        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;

        public float NoClipSpeed
        {
            get => _noClipSpeedMultiplier;
            set
            {
                if (SetProperty(ref _noClipSpeedMultiplier, value))
                {
                    if (!IsNoClipEnabled) return;
                    _utilityService.WriteNoClipSpeed(_noClipSpeedMultiplier);
                }
            }
        }

        private bool _isCombatMapEnabled;

        public bool IsCombatMapEnabled
        {
            get => _isCombatMapEnabled;
            set
            {
                if (!SetProperty(ref _isCombatMapEnabled, value)) return;
                _utilityService.ToggleCombatMap(_isCombatMapEnabled);
            }
        }

        private bool _isDungeonWarpEnabled;

        public bool IsDungeonWarpEnabled
        {
            get => _isDungeonWarpEnabled;
            set
            {
                if (!SetProperty(ref _isDungeonWarpEnabled, value)) return;
                _utilityService.ToggleDungeonWarp(_isDungeonWarpEnabled);
            }
        }

        private float _gameSpeed;

        public float GameSpeed
        {
            get => _gameSpeed;
            set
            {
                if (SetProperty(ref _gameSpeed, value))
                {
                    _utilityService.SetSpeed(value);
                    if (IsRememberSpeedEnabled && Math.Abs(value - DefaultGameSpeed) > Epsilon)
                    {
                        SettingsManager.Default.GameSpeed = value;
                    }
                }
            }
        }

        private bool _isRememberSpeedEnabled;

        public bool IsRememberSpeedEnabled
        {
            get => _isRememberSpeedEnabled;
            set
            {
                if (SetProperty(ref _isRememberSpeedEnabled, value))
                {
                    if (_isRememberSpeedEnabled)
                    {
                        SettingsManager.Default.RememberGameSpeed = _isRememberSpeedEnabled;

                        if (Math.Abs(GameSpeed - DefaultGameSpeed) > Epsilon)
                        {
                            SettingsManager.Default.GameSpeed = GameSpeed;
                        }
                    }
                    else
                    {
                        SettingsManager.Default.GameSpeed = DefaultGameSpeed;
                        SettingsManager.Default.RememberGameSpeed = _isRememberSpeedEnabled;
                    }
                }
            }
        }

        private bool _isFreeCamEnabled;

        public bool IsFreeCamEnabled
        {
            get => _isFreeCamEnabled;
            set
            {
                if (!SetProperty(ref _isFreeCamEnabled, value)) return;
                if (_isFreeCamEnabled)
                {
                    IsNoClipEnabled = false;
                }

                _utilityService.ToggleFreeCam(_isFreeCamEnabled);
            }
        }

        private bool _isFreezeWorldEnabled;

        public bool IsFreezeWorldEnabled
        {
            get => _isFreeCamEnabled;
            set
            {
                if (!SetProperty(ref _isFreezeWorldEnabled, value)) return;
                _utilityService.ToggleFreezeWorld(_isFreezeWorldEnabled);
            }
        }

        private bool _isDrawHitboxEnabled;

        public bool IsDrawHitboxEnabled
        {
            get => _isDrawHitboxEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHitboxEnabled, value)) return;
                _utilityService.ToggleDrawHitbox(_isDrawHitboxEnabled);
            }
        }

        private bool _isDrawLowHitEnabled;

        public bool IsDrawLowHitEnabled
        {
            get => _isDrawLowHitEnabled;
            set
            {
                if (!SetProperty(ref _isDrawLowHitEnabled, value)) return;
                _utilityService.ToggleWorldHitDraw(Offsets.WorldHitMan.LowHit, _isDrawLowHitEnabled);
                // _utilityService.SetColDrawMode(ColDrawMode);
            }
        }

        private bool _isDrawHighHitEnabled;

        public bool IsDrawHighHitEnabled
        {
            get => _isDrawHighHitEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHighHitEnabled, value)) return;
                _utilityService.ToggleWorldHitDraw(Offsets.WorldHitMan.HighHit, _isDrawHighHitEnabled);
            }
        }

        private int _colDrawMode = 1;

        public int ColDrawMode
        {
            get => _colDrawMode;
            set
            {
                if (!SetProperty(ref _colDrawMode, value)) return;
                if (!IsDrawHighHitEnabled && !IsDrawLowHitEnabled) return;
                // _utilityService.SetColDrawMode(_colDrawMode);
            }
        }

        private bool _isDrawRagdollEnabled;

        public bool IsDrawRagdollsEnabled
        {
            get => _isDrawRagdollEnabled;
            set
            {
                if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
                _utilityService.ToggleWorldHitDraw(Offsets.WorldHitMan.Ragdoll, _isDrawRagdollEnabled);
            }
        }
        
        private string _shopsSearchText = string.Empty;

        public string ShopsSearchText
        {
            get => _shopsSearchText;
            set
            {
                if (SetProperty(ref _shopsSearchText, value))
                {
                    ApplyFilter();
                }
            }
        }
        
        public ObservableCollection<ShopCommand> FilteredShops { get; }
        
        private ShopCommand _selectedShop;

        public ShopCommand SelectedShop
        {
            get => _selectedShop;
            set => SetProperty(ref _selectedShop, value);
        }

        #endregion

        #region Public Methods

        public void SetSpeed(float value) => GameSpeed = value;

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            GameSpeed = _utilityService.GetSpeed();
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
        }

        private void OnGameFirstLoaded()
        {
            if (IsCombatMapEnabled) _utilityService.ToggleCombatMap(true);
            if (IsDungeonWarpEnabled) _utilityService.ToggleDungeonWarp(true);
            if (IsDrawHitboxEnabled) _utilityService.ToggleDrawHitbox(true);
            IsDlcAvailable = _dlcService.IsDlcAvailable;
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.Noclip, () => { IsNoClipEnabled = !IsNoClipEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseNoClipSpeed, () =>
            {
                if (IsNoClipEnabled) NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
            });

            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseNoClipSpeed, () =>
            {
                if (IsNoClipEnabled) NoClipSpeed = Math.Max(0.5f, NoClipSpeed - 0.50f);
            });

            _hotkeyManager.RegisterAction(HotkeyActions.ForceSave, () => _utilityService.ForceSave());
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleGameSpeed, ToggleSpeed);
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseGameSpeed,
                () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseGameSpeed,
                () => SetSpeed(Math.Max(0.5f, GameSpeed - 0.50f)));
        }

        private void ApplyPrefs()
        {
            _isRememberSpeedEnabled = SettingsManager.Default.RememberGameSpeed;
            OnPropertyChanged(nameof(IsRememberSpeedEnabled));
            if (_isRememberSpeedEnabled) _desiredGameSpeed = SettingsManager.Default.GameSpeed;
        }

        private void Save() => _utilityService.ForceSave();
        private void TriggerNgCycle() => _utilityService.TriggerNewNgCycle();

        private void ToggleSpeed()
        {
            if (!AreOptionsEnabled) return;

            if (!IsApproximately(GameSpeed, DefaultGameSpeed))
            {
                _desiredGameSpeed = GameSpeed;
                SetSpeed(DefaultGameSpeed);
            }
            else if (_desiredGameSpeed >= 0)
            {
                SetSpeed(_desiredGameSpeed);
            }
        }

        private bool IsApproximately(float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        private void SetMorning() => _emevdService.ExecuteEmevdCommand(GameIds.Emevd.EmevdCommands.SetMorning);
        private void SetNoon() => _emevdService.ExecuteEmevdCommand(GameIds.Emevd.EmevdCommands.SetNoon);
        private void SetNight() => _emevdService.ExecuteEmevdCommand(GameIds.Emevd.EmevdCommands.SetNight);

        private void OpenLevelUp() => _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.LevelUp);
        private void OpenAllot() => _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenAllot);

        private void OpenAttunement() =>
            _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenAttunement);

        private void OpenPhysick() => _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenPhysick);
        private void OpenChest() => _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenChest);

        private void OpenGreatRunes() =>
            _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenGreatRunes);

        private void OpenAow() => _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenAow);

        private void OpenAlterGarments() =>
            _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenAlterGarments);

        private void OpenRebirth() => _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.Rebirth);

        private void OpenUpgrade()
        {
            foreach (var upgradeMenuFlag in GameIds.EzState.TalkCommands.UpgradeMenuFlags)
            {
                _ezStateService.ExecuteTalkCommand(upgradeMenuFlag);
            }

            _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenUpgrade);
        }

        private void OpenSell()
        {
            var playerHandle = _playerService.GetHandle();
            _ezStateService.ExecuteTalkCommand(GameIds.EzState.TalkCommands.OpenSell, playerHandle);
        }
        
        private void OpenShopSelector()
        {
            if (_shopSelectorWindow != null && _shopSelectorWindow.IsVisible)
            {
                _shopSelectorWindow.Activate();
                return;
            }

            _shopSelectorWindow = new ShopSelectorWindow
            {
                DataContext = this
            };

            ApplyFilter();
            _shopSelectorWindow.Closed += (sender, args) => _shopSelectorWindow = null;
            _shopSelectorWindow.Show();
        }
        
        private void ApplyFilter()
        {
            FilteredShops.Clear();

            var filtered = _allShops.AsEnumerable();

            if (!IsDlcAvailable)
            {
                filtered = filtered.Where(s => !s.IsDlc);
            }

            if (!string.IsNullOrWhiteSpace(ShopsSearchText))
            {
                var searchLower = ShopsSearchText.ToLowerInvariant();
                filtered = filtered.Where(s => 
                    s.Name.ToLowerInvariant().Contains(searchLower));
            }

            foreach (var shop in filtered)
            {
                FilteredShops.Add(shop);
            }
        }

        private void OpenShop(ShopCommand shop) => _ezStateService.ExecuteTalkCommand(shop.Command);


        #endregion

        // public bool IsDrawEventGeneralEnabled
        // {
        //     get => _isDrawEventGeneralEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventGeneralEnabled, value)) return;
        //         _utilityService.ToggleDrawEvent(DrawType.EventGeneral, _isDrawEventGeneralEnabled);
        //     }
        // }
        //

        // public bool IsDrawSoundEnabled
        // {
        //     get => _isDrawSoundEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawSoundEnabled, value)) return;
        //         _utilityService.ToggleDrawSound(_isDrawSoundEnabled);
        //     }
        // }
        //

        //
        // public bool IsHideCharactersEnabled
        // {
        //     get => _isHideCharactersEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isHideCharactersEnabled, value)) return;
        //         _utilityService.ToggleHideChr(_isHideCharactersEnabled);
        //     }
        // }
        //
        // public bool IsHideMapEnabled
        // {
        //     get => _isHideMapEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isHideMapEnabled, value)) return;
        //         _utilityService.ToggleHideMap(_isHideMapEnabled);
        //     }
        // }
    }
}