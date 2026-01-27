using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private const float DefaultNoclipMultiplier = 1f;

        private float _desiredGameSpeed = -1f;
        private const float DefaultGameSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _wasNoDeathEnabled;
        private bool _wasTorrentNoDeathEnabled;

        private ShopSelectorWindow _shopSelectorWindow;

        private readonly IUtilityService _utilityService;
        private readonly IEzStateService _ezStateService;
        private readonly IPlayerService _playerService;
        private readonly HotkeyManager _hotkeyManager;
        private readonly PlayerViewModel _playerViewModel;
        private readonly IDlcService _dlcService;
        private readonly ISpEffectService _spEffectService;
        private readonly IFlaskService _flaskService;
        private readonly IParamService _paramService;

        public const int MaterialId01Offset = 0x0;
        public const int ItemNum01Offset = 0x20;
        private List<byte[]>? _originalMaterialIds;
        private List<byte[]>? _originalItemNums;

        public const int ReinforcePriceRateOffset = 0x68;
        public const int BaseChangePriceRateOffset = 0x6C;
        private List<byte[]>? _originalReinforcePriceRate;
        private List<byte[]>? _originalBaseChangePriceRate;

        private static readonly uint[] DisableGraceWarpIds = [4270, 4271, 4272, 4282, 4286, 4288];

        private readonly List<ShopCommand> _allShops;

        public UtilityViewModel(IUtilityService utilityService, IStateService stateService,
            IEzStateService ezStateService, IPlayerService playerService, HotkeyManager hotkeyManager,
            PlayerViewModel playerViewModel, IDlcService dlcService,
            ISpEffectService spEffectService, IFlaskService flaskService, IParamService paramService)
        {
            _utilityService = utilityService;
            _ezStateService = ezStateService;
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;
            _playerViewModel = playerViewModel;
            _dlcService = dlcService;
            _spEffectService = spEffectService;
            _flaskService = flaskService;
            _paramService = paramService;

            stateService.Subscribe(State.AppStart, OnAppStart);
            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);
            stateService.Subscribe(State.FadedIn, OnFadedIn);

            SaveCommand = new DelegateCommand(Save);
            TriggerNgCycleCommand = new DelegateCommand(TriggerNgCycle);
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
            MoveCamToPlayerCommand = new DelegateCommand(MoveCamToPlayer);
            MovePlayerToCamCommand = new DelegateCommand(MovePlayerToCam);
            UpgradeFlaskCommand = new DelegateCommand(UpgradeFlask);
            IncreaseChargesCommand = new DelegateCommand(IncreaseCharges);
            OpenMirrorCommand = new DelegateCommand(OpenMirror);

            _allShops = DataLoader.GetShops();
            FilteredShops = new ObservableCollection<ShopCommand>();

            RegisterHotkeys();
            ApplyPrefs();
        }

        #region Commands

        public ICommand SaveCommand { get; set; }
        public ICommand TriggerNgCycleCommand { get; set; }

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
        public ICommand MoveCamToPlayerCommand { get; }
        public ICommand MovePlayerToCamCommand { get; }
        public ICommand UpgradeFlaskCommand { get; }
        public ICommand IncreaseChargesCommand { get; }
        public ICommand OpenMirrorCommand { get; }

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

                    _wasTorrentNoDeathEnabled = _playerViewModel.IsTorrentNoDeathEnabled;
                    _playerViewModel.IsTorrentNoDeathEnabled = true;

                    _utilityService.ToggleNoClip(_isNoClipEnabled, IsNoClipKeyboardDisableEnabled);
                }
                else
                {
                    _utilityService.ToggleNoClip(_isNoClipEnabled, IsNoClipKeyboardDisableEnabled);

                    _playerViewModel.IsNoDeathEnabled = _wasNoDeathEnabled;
                    _playerViewModel.IsTorrentNoDeathEnabled = _wasTorrentNoDeathEnabled;
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

        private bool _isNoClipKeyboardDisableEnabled;

        public bool IsNoClipKeyboardDisableEnabled
        {
            get => _isNoClipKeyboardDisableEnabled;
            set
            {
                if (!SetProperty(ref _isNoClipKeyboardDisableEnabled, value)) return;
                if (IsNoClipEnabled)
                {
                    _utilityService.ToggleNoclipKeyboardHook(_isNoClipKeyboardDisableEnabled);
                }

                SettingsManager.Default.IsNoClipKeyboardDisabled = _isNoClipKeyboardDisableEnabled;
                SettingsManager.Default.Save();
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
                if (_isDungeonWarpEnabled && AreOptionsEnabled)
                {
                    var playerIns = _playerService.GetPlayerIns();
                    foreach (var disableGraceWarpId in DisableGraceWarpIds)
                    {
                        _spEffectService.RemoveSpEffect(playerIns, disableGraceWarpId);
                    }
                }

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

        private int _fps;

        public int Fps
        {
            get => _fps;
            set
            {
                if (SetProperty(ref _fps, value))
                {
                    _utilityService.SetFps(_fps);
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
                else
                {
                    _isPlayerMovementEnabled = false;
                    OnPropertyChanged(nameof(IsPlayerMovementEnabled));
                }

                _utilityService.ToggleFreeCam(_isFreeCamEnabled);
            }
        }

        private bool _isPlayerMovementEnabled;

        public bool IsPlayerMovementEnabled
        {
            get => _isPlayerMovementEnabled;
            set
            {
                if (!SetProperty(ref _isPlayerMovementEnabled, value)) return;
                if (!IsFreeCamEnabled) return;
                _utilityService.TogglePlayerMovementForFreeCam(_isPlayerMovementEnabled);
            }
        }

        private bool _isFreezeWorldEnabled;

        public bool IsFreezeWorldEnabled
        {
            get => _isFreezeWorldEnabled;
            set
            {
                if (!SetProperty(ref _isFreezeWorldEnabled, value)) return;
                _utilityService.ToggleFreezeWorld(_isFreezeWorldEnabled);
            }
        }

        private bool _isGuaranteedDropEnabled;

        public bool IsGuaranteedDropEnabled
        {
            get => _isGuaranteedDropEnabled;
            set
            {
                if (!SetProperty(ref _isGuaranteedDropEnabled, value)) return;
                _utilityService.ToggleGuaranteedDrop(_isGuaranteedDropEnabled);
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
                _utilityService.SetColDrawMode(_isDrawLowHitEnabled ? ColDrawMode : 0);
                _utilityService.ToggleDrawLowHit(_isDrawLowHitEnabled);
            }
        }

        private bool _isDrawHighHitEnabled;

        public bool IsDrawHighHitEnabled
        {
            get => _isDrawHighHitEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHighHitEnabled, value)) return;
                _utilityService.SetColDrawMode(_isDrawHighHitEnabled ? ColDrawMode : 0);
                _utilityService.ToggleDrawHighHit(_isDrawHighHitEnabled);
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
                _utilityService.SetColDrawMode(_colDrawMode);
            }
        }

        private bool _isDrawRagdollEnabled;

        public bool IsDrawRagdollsEnabled
        {
            get => _isDrawRagdollEnabled;
            set
            {
                if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
                _utilityService.ToggleDrawRagdolls(_isDrawRagdollEnabled);
            }
        }

        private bool _isDrawPoiseBarsEnabled;

        public bool IsDrawPoiseBarsEnabled
        {
            get => _isDrawPoiseBarsEnabled;
            set
            {
                if (!SetProperty(ref _isDrawPoiseBarsEnabled, value)) return;

                if (_isDrawPoiseBarsEnabled)
                {
                    _utilityService.PatchDebugFont();
                }

                _utilityService.ToggleDrawPoiseBars(_isDrawPoiseBarsEnabled);
            }
        }

        private bool _isDrawPlayerSoundEnabled;

        public bool IsDrawPlayerSoundEnabled
        {
            get => _isDrawPlayerSoundEnabled;
            set
            {
                if (!SetProperty(ref _isDrawPlayerSoundEnabled, value)) return;
                if (_isDrawPlayerSoundEnabled)
                {
                    _utilityService.PatchDebugFont();
                }

                _utilityService.TogglePlayerSound(_isDrawPlayerSoundEnabled);
            }
        }

        private bool _isDrawMapTiles1Enabled;

        public bool IsDrawMapTiles1Enabled
        {
            get => _isDrawMapTiles1Enabled;
            set
            {
                if (!SetProperty(ref _isDrawMapTiles1Enabled, value)) return;
                _utilityService.ToggleDrawMapTiles1(_isDrawMapTiles1Enabled);
            }
        }

        private bool _isDrawMapTiles2Enabled;

        public bool IsDrawMapTiles2Enabled
        {
            get => _isDrawMapTiles2Enabled;
            set
            {
                if (!SetProperty(ref _isDrawMapTiles2Enabled, value)) return;
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawMapTiles2(_isDrawMapTiles2Enabled);
            }
        }

        private bool _isDrawMiniMapEnabled;

        public bool IsDrawMiniMapEnabled
        {
            get => _isDrawMiniMapEnabled;
            set
            {
                if (!SetProperty(ref _isDrawMiniMapEnabled, value)) return;
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawMiniMap(_isDrawMiniMapEnabled);
            }
        }

        private bool _isDrawTilesOnMapEnabled;

        public bool IsDrawTilesOnMapEnabled
        {
            get => _isDrawTilesOnMapEnabled;
            set
            {
                if (!SetProperty(ref _isDrawTilesOnMapEnabled, value)) return;
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawTilesOnMap(_isDrawTilesOnMapEnabled);
            }
        }

        private bool _isHideCharactersEnabled;

        public bool IsHideCharactersEnabled
        {
            get => _isHideCharactersEnabled;
            set
            {
                if (!SetProperty(ref _isHideCharactersEnabled, value)) return;
                _utilityService.ToggleHideChr(_isHideCharactersEnabled);
            }
        }

        private bool _isHideMapEnabled;

        public bool IsHideMapEnabled
        {
            get => _isHideMapEnabled;
            set
            {
                if (!SetProperty(ref _isHideMapEnabled, value)) return;
                _utilityService.ToggleHideMap(_isHideMapEnabled);
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

        private bool _isShowFullShopLineupEnabled;

        public bool IsShowFullShopLineupEnabled
        {
            get => _isShowFullShopLineupEnabled;
            set
            {
                if (!SetProperty(ref _isShowFullShopLineupEnabled, value)) return;
                _utilityService.ToggleFullShopLineup(_isShowFullShopLineupEnabled);
            }
        }

        private bool _isUpgradingFlask;

        public bool IsUpgradingFlask
        {
            get => _isUpgradingFlask;
            set => SetProperty(ref _isUpgradingFlask, value);
        }

        private bool _isIncreasingCharges;

        public bool IsIncreasingCharges
        {
            get => _isIncreasingCharges;
            set => SetProperty(ref _isIncreasingCharges, value);
        }

        private const float FastGameSpeed = 7f;
        private bool _is7xSpeedActive;

        private void Toggle7xSpeed()
        {
            if (!AreOptionsEnabled) return;

            if (_is7xSpeedActive)
            {
                SetSpeed(DefaultGameSpeed);
                _is7xSpeedActive = false;
            }
            else
            {
                SetSpeed(FastGameSpeed);
                _is7xSpeedActive = true;
            }
        }

        private bool _isNoUpgradeCostEnabled;

        public bool IsNoUpgradeCostEnabled
        {
            get => _isNoUpgradeCostEnabled;
            set
            {
                if (!SetProperty(ref _isNoUpgradeCostEnabled, value)) return;

                _ = Task.Run(() =>
                {
                    var (equipMtrlSetParamTableIndex, equipMtrlSetParamSlotIndex) =
                        ParamIndices.All["EquipMtrlSetParam"];
                    var (reinforceParamWeaponTableIndex, reinforceParamWeaponSlotIndex) =
                        ParamIndices.All["ReinforceParamWeapon"];

                    if (_isNoUpgradeCostEnabled)
                    {
                        _originalMaterialIds ??= _paramService.ReadFieldFromAllRows(equipMtrlSetParamTableIndex,
                            equipMtrlSetParamSlotIndex, MaterialId01Offset, sizeof(int));

                        _originalItemNums ??= _paramService.ReadFieldFromAllRows(equipMtrlSetParamTableIndex,
                            equipMtrlSetParamSlotIndex, ItemNum01Offset, sizeof(int));

                        _originalReinforcePriceRate ??= _paramService.ReadFieldFromAllRows(
                            reinforceParamWeaponTableIndex,
                            reinforceParamWeaponSlotIndex, ReinforcePriceRateOffset, sizeof(float));

                        _originalBaseChangePriceRate ??= _paramService.ReadFieldFromAllRows(
                            reinforceParamWeaponTableIndex,
                            reinforceParamWeaponSlotIndex, BaseChangePriceRateOffset, sizeof(float));

                        _paramService.WriteFieldToAllRows(equipMtrlSetParamTableIndex, equipMtrlSetParamSlotIndex,
                            MaterialId01Offset,
                            BitConverter.GetBytes(-1));
                        _paramService.WriteFieldToAllRows(equipMtrlSetParamTableIndex, equipMtrlSetParamSlotIndex,
                            ItemNum01Offset,
                            BitConverter.GetBytes(-1));
                        _paramService.WriteFieldToAllRows(reinforceParamWeaponTableIndex, reinforceParamWeaponSlotIndex,
                            ReinforcePriceRateOffset,
                            BitConverter.GetBytes(0f));
                        _paramService.WriteFieldToAllRows(reinforceParamWeaponTableIndex, reinforceParamWeaponSlotIndex,
                            BaseChangePriceRateOffset,
                            BitConverter.GetBytes(0f));
                    }
                    else
                    {
                        _paramService.RestoreFieldToAllRows(equipMtrlSetParamTableIndex, equipMtrlSetParamSlotIndex,
                            MaterialId01Offset,
                            _originalMaterialIds);
                        _paramService.RestoreFieldToAllRows(equipMtrlSetParamTableIndex, equipMtrlSetParamSlotIndex,
                            ItemNum01Offset,
                            _originalItemNums);
                        _paramService.RestoreFieldToAllRows(reinforceParamWeaponTableIndex,
                            reinforceParamWeaponSlotIndex,
                            ReinforcePriceRateOffset,
                            _originalReinforcePriceRate);
                        _paramService.RestoreFieldToAllRows(reinforceParamWeaponTableIndex,
                            reinforceParamWeaponSlotIndex,
                            BaseChangePriceRateOffset,
                            _originalBaseChangePriceRate);
                    }
                });
            }
        }

        #endregion

        #region Public Methods

        public void SetSpeed(float value) => GameSpeed = value;

        #endregion

        #region Private Methods

        private void OnAppStart()
        {
            IsNoClipKeyboardDisableEnabled = SettingsManager.Default.IsNoClipKeyboardDisabled;
        }

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            GameSpeed = _utilityService.GetSpeed();
            Fps = _utilityService.GetFps();
            if (IsDungeonWarpEnabled)
            {
                var playerIns = _playerService.GetPlayerIns();
                foreach (var disableGraceWarpId in DisableGraceWarpIds)
                {
                    _spEffectService.RemoveSpEffect(playerIns, disableGraceWarpId);
                }
            }

            if (IsDrawHitboxEnabled) _utilityService.ToggleDrawHitbox(true);
            if (IsDrawPoiseBarsEnabled)
            {
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawPoiseBars(true);
            }

            if (IsDrawMapTiles1Enabled) _utilityService.ToggleDrawMapTiles1(true);
            if (IsDrawMapTiles2Enabled)
            {
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawMapTiles2(true);
            }

            if (IsHideCharactersEnabled) _utilityService.ToggleHideChr(true);
            if (IsHideMapEnabled) _utilityService.ToggleHideMap(true);
            if (IsDrawRagdollsEnabled) _utilityService.ToggleDrawRagdolls(true);

            _ezStateService.RequestNewNpcTalk();
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
            IsFreeCamEnabled = false;
            if (IsDrawLowHitEnabled)
            {
                _utilityService.ToggleDrawLowHit(false);
                _utilityService.SetColDrawMode(0);
            }

            if (IsDrawHighHitEnabled)
            {
                _utilityService.ToggleDrawHighHit(false);
                _utilityService.SetColDrawMode(0);
            }
        }

        private void OnGameFirstLoaded()
        {
            if (IsCombatMapEnabled) _utilityService.ToggleCombatMap(true);
            if (IsDungeonWarpEnabled) _utilityService.ToggleDungeonWarp(true);
            if (IsGuaranteedDropEnabled) _utilityService.ToggleGuaranteedDrop(true);
            if (IsShowFullShopLineupEnabled) _utilityService.ToggleFullShopLineup(true);
            if (IsDrawPlayerSoundEnabled)
            {
                _utilityService.PatchDebugFont();
                _utilityService.TogglePlayerSound(true);
            }

            if (IsDrawMiniMapEnabled)
            {
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawMiniMap(true);
            }

            if (IsDrawTilesOnMapEnabled)
            {
                _utilityService.PatchDebugFont();
                _utilityService.ToggleDrawTilesOnMap(true);
            }
            
            IsDlcAvailable = _dlcService.IsDlcAvailable;
        }

        private void OnFadedIn()
        {
            if (IsDrawLowHitEnabled)
            {
                _utilityService.ToggleDrawLowHit(true);
                _utilityService.SetColDrawMode(ColDrawMode);
            }

            if (IsDrawHighHitEnabled)
            {
                _utilityService.ToggleDrawHighHit(true);
                _utilityService.SetColDrawMode(ColDrawMode);
            }
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
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleSevenSpeed, Toggle7xSpeed);
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseGameSpeed,
                () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseGameSpeed,
                () => SetSpeed(Math.Max(0.5f, GameSpeed - 0.50f)));

            _hotkeyManager.RegisterAction(HotkeyActions.ToggleFreeCam, () => IsFreeCamEnabled = !IsFreeCamEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleFreezeWorld,
                () => IsFreezeWorldEnabled = !IsFreezeWorldEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.MoveCamToPlayer, () =>
            {
                if (!IsFreeCamEnabled) return;
                MoveCamToPlayer();
            });
            _hotkeyManager.RegisterAction(HotkeyActions.MovePlayerToCam, () =>
            {
                if (!IsFreeCamEnabled) return;
                MovePlayerToCam();
            });
            _hotkeyManager.RegisterAction(HotkeyActions.DrawHitbox, () => IsDrawHitboxEnabled = !IsDrawHitboxEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.DrawPlayerSound,
                () => IsDrawPlayerSoundEnabled = !IsDrawPlayerSoundEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.DrawRagdolls,
                () => IsDrawRagdollsEnabled = !IsDrawRagdollsEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.DrawLowHit, () => IsDrawLowHitEnabled = !IsDrawLowHitEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.DrawHighHit,
                () => IsDrawHighHitEnabled = !IsDrawHighHitEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.LevelUp, () => SafeExecute(OpenLevelUp));
            _hotkeyManager.RegisterAction(HotkeyActions.AllotFlasks, () => SafeExecute(OpenAllot));
            _hotkeyManager.RegisterAction(HotkeyActions.MemorizeSpells, () => SafeExecute(OpenAttunement));
            _hotkeyManager.RegisterAction(HotkeyActions.MixPhysick, () => SafeExecute(OpenPhysick));
            _hotkeyManager.RegisterAction(HotkeyActions.OpenChest, () => SafeExecute(OpenChest));
            _hotkeyManager.RegisterAction(HotkeyActions.GreatRunes, () => SafeExecute(OpenGreatRunes));
            _hotkeyManager.RegisterAction(HotkeyActions.AshesOfWar, () => SafeExecute(OpenAow));
            _hotkeyManager.RegisterAction(HotkeyActions.AlterGarments, () => SafeExecute(OpenAlterGarments));
            _hotkeyManager.RegisterAction(HotkeyActions.Upgrade, () => SafeExecute(OpenUpgrade));
            _hotkeyManager.RegisterAction(HotkeyActions.Sell, () => SafeExecute(OpenSell));
            _hotkeyManager.RegisterAction(HotkeyActions.Rebirth, () => SafeExecute(OpenRebirth));
            _hotkeyManager.RegisterAction(HotkeyActions.UpgradeFlask,
                () => SafeExecuteIfNotBusy(UpgradeFlask, IsUpgradingFlask));
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseFlaskCharges,
                () => SafeExecuteIfNotBusy(IncreaseCharges, IsIncreasingCharges));
            _hotkeyManager.RegisterAction(HotkeyActions.OpenShopWindow, OpenShopSelector);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleFreeCamPlayerMovement, () =>
            {
                if (!IsFreeCamEnabled) return;
                IsPlayerMovementEnabled = !IsPlayerMovementEnabled;
            });
            _hotkeyManager.RegisterAction(HotkeyActions.DrawPoiseBars,
                () => IsDrawPoiseBarsEnabled = !IsDrawPoiseBarsEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.Set30Fps, () => SafeExecute(() => Fps = 30));
            _hotkeyManager.RegisterAction(HotkeyActions.Set60Fps, () => SafeExecute(() => Fps = 60));
            _hotkeyManager.RegisterAction(HotkeyActions.Set90Fps, () => SafeExecute(() => Fps = 90));
            _hotkeyManager.RegisterAction(HotkeyActions.Set120Fps, () => SafeExecute(() => Fps = 120));
            _hotkeyManager.RegisterAction(HotkeyActions.Set180Fps, () => SafeExecute(() => Fps = 180));
            _hotkeyManager.RegisterAction(HotkeyActions.Set240Fps, () => SafeExecute(() => Fps = 240));
        }

        private void SafeExecute(Action action)
        {
            if (!AreOptionsEnabled) return;
            action();
        }

        private void SafeExecuteIfNotBusy(Action action, bool isBusy)
        {
            if (!AreOptionsEnabled || isBusy) return;
            action();
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

        private void OpenLevelUp() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.LevelUp);
        private void OpenAllot() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenAllot);

        private void OpenAttunement() =>
            _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenAttunement);

        private void OpenPhysick() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenPhysick);
        private void OpenChest() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenChest);

        private void OpenGreatRunes() =>
            _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenGreatRunes);

        private void OpenAow() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenAow);

        private void OpenAlterGarments() =>
            _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenAlterGarments);

        private void OpenRebirth() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.Rebirth);

        private void OpenMirror() => _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenMirror);

        private void OpenUpgrade()
        {
            foreach (var upgradeMenuFlag in EzState.TalkCommands.UpgradeMenuFlags)
            {
                _ezStateService.ExecuteTalkCommand(upgradeMenuFlag);
            }

            _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenUpgrade);
        }

        private void OpenSell()
        {
            var playerHandle = _playerService.GetHandle();
            _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.OpenSell, playerHandle);
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

        private void MoveCamToPlayer() => _utilityService.MoveCamToPlayer();
        private void MovePlayerToCam() => _utilityService.MovePlayerToCam();

        private async void UpgradeFlask()
        {
            IsUpgradingFlask = true;
            try
            {
                await _flaskService.TryUpgradeFlask();
            }
            finally
            {
                IsUpgradingFlask = false;
            }
        }

        private async void IncreaseCharges()
        {
            IsIncreasingCharges = true;
            try
            {
                await _flaskService.TryIncreaseCharges();
            }
            finally
            {
                IsIncreasingCharges = false;
            }
        }

        #endregion
    }
}