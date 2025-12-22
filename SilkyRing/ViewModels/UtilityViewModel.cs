using System;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private const float DefaultNoclipMultiplier = 1f;
        
        private float _desiredGameSpeed = -1f;
        private const float DefaultGameSpeed = 1f;
        private const float Epsilon = 0.0001f;

   
        private bool _isDrawLowHitEnabled;
        private bool _isDrawHighHitEnabled;
        private int _colDrawMode = 1;
        private bool _isDrawRagdollEnabled;

        private readonly IUtilityService _utilityService;
        private readonly IEzStateService _ezStateService;
        private readonly IPlayerService _playerService;
        private readonly HotkeyManager _hotkeyManager;
        private readonly IEmevdService _emevdService;

        public UtilityViewModel(IUtilityService utilityService, IStateService stateService,
            IEzStateService ezStateService, IPlayerService playerService, HotkeyManager hotkeyManager,
            IEmevdService emevdService)
        {
            _utilityService = utilityService;
            _ezStateService = ezStateService;
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;
            _emevdService = emevdService;

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);

            SaveCommand = new DelegateCommand(Save);
            SetMorningCommand = new DelegateCommand(SetMorning);
            SetNoonCommand = new DelegateCommand(SetNoon);
            SetNightCommand = new DelegateCommand(SetNight);


            RegisterHotkeys();
            ApplyPrefs();
        }

        
        private void ApplyPrefs()
        {
            _isRememberSpeedEnabled = SettingsManager.Default.RememberGameSpeed;
            OnPropertyChanged(nameof(IsRememberSpeedEnabled));
            if (_isRememberSpeedEnabled) _desiredGameSpeed = SettingsManager.Default.GameSpeed;
        }

        #region Commands
        
        public ICommand SaveCommand { get; set; }
        public ICommand SetMorningCommand { get; set; }
        public ICommand SetNoonCommand { get; set; }
        public ICommand SetNightCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isNoClipEnabled;

        public bool IsNoClipEnabled
        {
            get => _isNoClipEnabled;
            set
            {
                if (!SetProperty(ref _isNoClipEnabled, value)) return;
                _utilityService.WriteNoClipSpeed(NoClipSpeed);
                _utilityService.ToggleNoClip(_isNoClipEnabled);
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
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseGameSpeed, () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseGameSpeed, () => SetSpeed(Math.Max(0.5f, GameSpeed - 0.50f)));
        }
        
        private void Save() => _utilityService.ForceSave();
        
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

        #endregion
        
        

      
        //
        // public bool IsDrawLowHitEnabled
        // {
        //     get => _isDrawLowHitEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawLowHitEnabled, value)) return;
        //         // _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.LowHit, _isDrawLowHitEnabled);
        //         _utilityService.SetColDrawMode(ColDrawMode);
        //     }
        // }
        //
        // public bool IsDrawHighHitEnabled
        // {
        //     get => _isDrawHighHitEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawHighHitEnabled, value)) return;
        //         // _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.HighHit, _isDrawHighHitEnabled);
        //     }
        // }
        //
        // public int ColDrawMode
        // {
        //     get => _colDrawMode;
        //     set
        //     {
        //         if (!SetProperty(ref _colDrawMode, value)) return;
        //         if (!IsDrawHighHitEnabled && !IsDrawLowHitEnabled) return;
        //         _utilityService.SetColDrawMode(_colDrawMode);
        //     }
        // }
        //

        
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
        // //
        //
        // public bool IsDrawRagdollsEnabled
        // {
        //     get => _isDrawRagdollEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
        //         // _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.Ragdoll, _isDrawRagdollEnabled);
        //     }
        // }
        //
      
        // public bool IsDrawCollisionEnabled
        // {
        //     get => _isDrawCollisionEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawCollisionEnabled, value)) return;
        //         _utilityService.ToggleDrawCol(_isDrawCollisionEnabled);
        //         if (!_isDrawCollisionEnabled) IsColWireframeEnabled = false;
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