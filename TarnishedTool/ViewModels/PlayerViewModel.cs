using System;
using System.Globalization;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private int _currentRuneLevel;
        private bool _customHpHasBeenSet = !string.IsNullOrWhiteSpace(SettingsManager.Default.SaveCustomHp);

        private float _playerDesiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _pauseUpdates = true;
        
        private readonly IPlayerService _playerService;

        private readonly CharacterState _saveState1 = new();
        private readonly CharacterState _saveState2 = new();

        private readonly HotkeyManager _hotkeyManager;
        private readonly IEventService _eventService;
        private readonly ISpEffectService _spEffectService;
        private readonly IEmevdService _emevdService;
        private readonly IDlcService _dlcService;
        private readonly IEzStateService _ezStateService;
        private readonly IGameTickService _gameTickService;

        public static readonly long[] NewGameEventIds = [50, 51, 52, 53, 54, 55, 56, 57];

        public PlayerViewModel(IPlayerService playerService, IStateService stateService, HotkeyManager hotkeyManager,
            IEventService eventService, ISpEffectService spEffectService, IEmevdService emevdService,
            IDlcService dlcService, IEzStateService ezStateService, IGameTickService gameTickService)
        {
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;
            _eventService = eventService;
            _spEffectService = spEffectService;
            _emevdService = emevdService;
            _dlcService = dlcService;
            _ezStateService = ezStateService;
            _gameTickService = gameTickService;

            RegisterHotkeys();

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);
            stateService.Subscribe(State.FadedIn, OnFadedIn);

            SetRfbsCommand = new DelegateCommand(SetRfbs);
            SetMaxHpCommand = new DelegateCommand(SetMaxHp);
            SetCustomHpCommand = new DelegateCommand(SetCustomHp);
            DieCommand = new DelegateCommand(Die);

            SavePositionCommand = new DelegateCommand(SavePosition);
            RestorePositionCommand = new DelegateCommand(RestorePosition);

            GiveRunesCommand = new DelegateCommand(GiveRunes);
            ApplyRuneArcCommand = new DelegateCommand(ApplyRuneArc);
            RestCommand = new DelegateCommand(Rest);
            

            ApplyPrefs();
        }

        #region Commands

        public ICommand SetRfbsCommand { get; set; }
        public ICommand SetMaxHpCommand { get; set; }
        public ICommand SetCustomHpCommand { get; set; }
        public ICommand DieCommand { get; set; }

        public ICommand SavePositionCommand { get; set; }
        public ICommand RestorePositionCommand { get; set; }

        public ICommand GiveRunesCommand { get; set; }
        public ICommand ApplyRuneArcCommand { get; set; }
        public ICommand RestCommand { get; set; }



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

        private int _currentHp;

        public int CurrentHp
        {
            get => _currentHp;
            set => SetProperty(ref _currentHp, value);
        }

        private int _currentMaxHp;

        public int CurrentMaxHp
        {
            get => _currentMaxHp;
            set => SetProperty(ref _currentMaxHp, value);
        }
        
// save custom hp throughout sessions, maybe I should've made it a toggle
        private string _customHp = SettingsManager.Default.SaveCustomHp;
        
        public string CustomHp
        {
            get => _customHp;
            set
            {
                if (SetProperty(ref _customHp, value))
                {
                    _customHpHasBeenSet = true;
                    
                }
            }
        }

        private bool _isSetRfbsOnLoadEnabled;

        public bool IsSetRfbsOnLoadEnabled
        {
            get => _isSetRfbsOnLoadEnabled;
            set => SetProperty(ref _isSetRfbsOnLoadEnabled, value);
        }

        private bool _isPos1Saved;

        public bool IsPos1Saved
        {
            get => _isPos1Saved;
            set => SetProperty(ref _isPos1Saved, value);
        }

        private bool _isPos2Saved;

        public bool IsPos2Saved
        {
            get => _isPos2Saved;
            set => SetProperty(ref _isPos2Saved, value);
        }

        private bool _isStateIncluded;

        public bool IsStateIncluded
        {
            get => _isStateIncluded;
            set => SetProperty(ref _isStateIncluded, value);
        }

        private float _posX;

        public float PosX
        {
            get => _posX;
            set => SetProperty(ref _posX, value);
        }

        private float _posY;

        public float PosY
        {
            get => _posY;
            set => SetProperty(ref _posY, value);
        }

        private float _posZ;

        public float PosZ
        {
            get => _posZ;
            set => SetProperty(ref _posZ, value);
        }

        private bool _isNoDeathEnabled;

        public bool IsNoDeathEnabled
        {
            get => _isNoDeathEnabled;
            set
            {
                if (SetProperty(ref _isNoDeathEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.PlayerNoDeath, _isNoDeathEnabled);
                }
            }
        }

        private bool _isNoDamageEnabled;

        public bool IsNoDamageEnabled
        {
            get => _isNoDamageEnabled;
            set
            {
                if (SetProperty(ref _isNoDamageEnabled, value))
                {
                    _playerService.ToggleNoDamage(_isNoDamageEnabled);
                }
            }
        }

        private bool _isNoHitEnabled;

        public bool IsNoHitEnabled
        {
            get => _isNoHitEnabled;
            set
            {
                if (SetProperty(ref _isNoHitEnabled, value))
                {
                    _playerService.ToggleNoHit(_isNoHitEnabled);
                }
            }
        }

        private bool _isInfiniteStaminaEnabled;

        public bool IsInfiniteStaminaEnabled
        {
            get => _isInfiniteStaminaEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteStaminaEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteStam, _isInfiniteStaminaEnabled);
                }
            }
        }

        private bool _isInfiniteConsumablesEnabled;

        public bool IsInfiniteConsumablesEnabled
        {
            get => _isInfiniteConsumablesEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteConsumablesEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteGoods, _isInfiniteConsumablesEnabled);
                }
            }
        }

        private bool _isInfiniteArrowsEnabled;

        public bool IsInfiniteArrowsEnabled
        {
            get => _isInfiniteArrowsEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteArrowsEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteArrows, _isInfiniteArrowsEnabled);
                }
            }
        }

        private bool _isInfiniteFpEnabled;

        public bool IsInfiniteFpEnabled
        {
            get => _isInfiniteFpEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteFpEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteFp, _isInfiniteFpEnabled);
                }
            }
        }

        private bool _isOneShotEnabled;

        public bool IsOneShotEnabled
        {
            get => _isOneShotEnabled;
            set
            {
                if (SetProperty(ref _isOneShotEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.OneShot, _isOneShotEnabled);
                }
            }
        }

        private bool _isInfinitePoiseEnabled;

        public bool IsInfinitePoiseEnabled
        {
            get => _isInfinitePoiseEnabled;
            set
            {
                if (SetProperty(ref _isInfinitePoiseEnabled, value))
                {
                    _playerService.ToggleInfinitePoise(_isInfinitePoiseEnabled);
                }
            }
        }

        private bool _isSilentEnabled;

        public bool IsSilentEnabled
        {
            get => _isSilentEnabled;
            set
            {
                if (SetProperty(ref _isSilentEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.Silent, _isSilentEnabled, true);
                }
            }
        }

        private bool _isHiddenEnabled;

        public bool IsHiddenEnabled
        {
            get => _isHiddenEnabled;
            set
            {
                if (SetProperty(ref _isHiddenEnabled, value))
                {
                    _playerService.ToggleDebugFlag(ChrDbgFlags.Hidden, _isHiddenEnabled, true);
                }
            }
        }

        private bool _isTorrentNoDeathEnabled;

        public bool IsTorrentNoDeathEnabled
        {
            get => _isTorrentNoDeathEnabled;
            set
            {
                if (SetProperty(ref _isTorrentNoDeathEnabled, value))
                {
                    _playerService.ToggleTorrentNoDeath(_isTorrentNoDeathEnabled);
                }
            }
        }

        private bool _isTorrentAnywhereEnabled;

        public bool IsTorrentAnywhereEnabled
        {
            get => _isTorrentAnywhereEnabled;
            set
            {
                if (SetProperty(ref _isTorrentAnywhereEnabled, value))
                {
                    _playerService.ToggleTorrentAnywhere(_isTorrentAnywhereEnabled);
                }
            }
        }

        private bool _isNoRuneLossEnabled;

        public bool IsNoRuneLossEnabled
        {
            get => _isNoRuneLossEnabled;
            set
            {
                if (SetProperty(ref _isNoRuneLossEnabled, value))
                {
                    _playerService.ToggleNoRuneLoss(_isNoRuneLossEnabled);
                }
            }
        }

        private bool _isNoRuneArcLossEnabled;

        public bool IsNoRuneArcLossEnabled
        {
            get => _isNoRuneArcLossEnabled;
            set
            {
                if (SetProperty(ref _isNoRuneArcLossEnabled, value))
                {
                    _playerService.ToggleNoRuneArcLoss(_isNoRuneArcLossEnabled);
                }
            }
        }

        private bool _isNoRuneGainEnabled;

        public bool IsNoRuneGainEnabled
        {
            get => _isNoRuneGainEnabled;
            set
            {
                if (SetProperty(ref _isNoRuneGainEnabled, value))
                {
                    _playerService.ToggleNoRuneGain(_isNoRuneGainEnabled);
                }
            }
        }

        private bool _isNoTimePassOnDeathEnabled;

        public bool IsNoTimePassOnDeathEnabled
        {
            get => _isNoTimePassOnDeathEnabled;
            set
            {
                if (SetProperty(ref _isNoTimePassOnDeathEnabled, value))
                {
                    _playerService.ToggleNoTimePassOnDeath(_isNoTimePassOnDeathEnabled);
                }
            }
        }

        private int _runeLevel;

        public int RuneLevel
        {
            get => _runeLevel;
            private set => SetProperty(ref _runeLevel, value);
        }

        private int _vigor;

        public int Vigor
        {
            get => _vigor;
            set => SetProperty(ref _vigor, value);
        }

        private int _mind;

        public int Mind
        {
            get => _mind;
            set => SetProperty(ref _mind, value);
        }

        private int _endurance;

        public int Endurance
        {
            get => _endurance;
            set => SetProperty(ref _endurance, value);
        }

        private int _strength;

        public int Strength
        {
            get => _strength;
            set => SetProperty(ref _strength, value);
        }

        private int _dexterity;

        public int Dexterity
        {
            get => _dexterity;
            set => SetProperty(ref _dexterity, value);
        }

        private int _intelligence;

        public int Intelligence
        {
            get => _intelligence;
            set => SetProperty(ref _intelligence, value);
        }

        private int _faith;

        public int Faith
        {
            get => _faith;
            set => SetProperty(ref _faith, value);
        }

        private int _arcane;

        public int Arcane
        {
            get => _arcane;
            set => SetProperty(ref _arcane, value);
        }

        private int _scadu;

        public int Scadu
        {
            get => _scadu;
            set => SetProperty(ref _scadu, value);
        }

        private int _spiritAsh;

        public int SpiritAsh
        {
            get => _spiritAsh;
            set => SetProperty(ref _spiritAsh, value);
        }

        private int _runes = 10000;

        public int Runes
        {
            get => _runes;
            set => SetProperty(ref _runes, value);
        }

        private int _newGame;

        public int NewGame
        {
            get => _newGame;
            set
            {
                if (SetProperty(ref _newGame, value))
                {
                    SetNewGame(value);
                }
            }
        }

        private int _currentAnimation;

        public int CurrentAnimation
        {
            get => _currentAnimation;
            set => SetProperty(ref _currentAnimation, value);
        }

        private float _playerSpeed;

        public float PlayerSpeed
        {
            get => _playerSpeed;
            set
            {
                if (SetProperty(ref _playerSpeed, value))
                {
                    _playerService.SetSpeed(value);
                    if (IsRememberSpeedEnabled && Math.Abs(value - DefaultSpeed) > Epsilon)
                    {
                        SettingsManager.Default.PlayerSpeed = value;
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
                        SettingsManager.Default.RememberPlayerSpeed = _isRememberSpeedEnabled;

                        if (Math.Abs(PlayerSpeed - DefaultSpeed) > Epsilon)
                        {
                            SettingsManager.Default.PlayerSpeed = PlayerSpeed;
                        }
                    }
                    else
                    {
                        SettingsManager.Default.PlayerSpeed = DefaultSpeed;
                        SettingsManager.Default.RememberPlayerSpeed = _isRememberSpeedEnabled;
                    }
                }
            }
        }

        private bool _isAutoSetNewGameSevenEnabled;

        public bool IsAutoSetNewGameSevenEnabled
        {
            get => _isAutoSetNewGameSevenEnabled;
            set => SetProperty(ref _isAutoSetNewGameSevenEnabled, value);
        }

        private bool _isResetWorldIncluded;

        public bool IsResetWorldIncluded
        {
            get => _isResetWorldIncluded;
            set => SetProperty(ref _isResetWorldIncluded, value);
        }

        private MapLocation _mapLocation;

        public MapLocation MapLocation
        {
            get => _mapLocation;
            set => SetProperty(ref _mapLocation, value);
        }

        private bool _showPlayerLocation;

        public bool ShowPlayerLocation
        {
            get => _showPlayerLocation;
            set => SetProperty(ref _showPlayerLocation, value);
        }

        #endregion

        #region Public Methods

        public void PauseUpdates() => _pauseUpdates = true;
        public void ResumeUpdates() => _pauseUpdates = false;
        public void SetHp(int hp) => _playerService.SetHp(hp);

        public void SetStat(string statName, int value)
        {
            if (Enum.TryParse<GameDataMan.PlayerGameDataOffsets>(statName, out var offset))
            {
                _playerService.SetStat((int)offset, value);
            }
        }

        public void SetScadu(int value) => _playerService.SetScadu(value);
        public void SetSpiritAsh(int value) => _playerService.SetSpiritAsh(value);
        public void SetSpeed(float value) => PlayerSpeed = value;

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            
            LoadStats();
            _gameTickService.Subscribe(PlayerTick);
            _pauseUpdates = false;
            IsDlcAvailable = _dlcService.IsDlcAvailable;
        }

        private void OnFadedIn()
        {
            if (IsSetRfbsOnLoadEnabled) SetRfbs();
            if (IsTorrentNoDeathEnabled) _playerService.ToggleTorrentNoDeath(true);
            if (IsNoDamageEnabled) _playerService.ToggleNoDamage(true);
            if (IsNoHitEnabled) _playerService.ToggleNoHit(true);
        }

        private void OnGameFirstLoaded()
        {
            if (IsNoDeathEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.PlayerNoDeath, true);
            if (IsInfiniteStaminaEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteStam, true);
            if (IsInfiniteConsumablesEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteGoods, true);
            if (IsInfiniteArrowsEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteArrows, true);
            if (IsInfiniteFpEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.InfiniteFp, true);
            if (IsOneShotEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.OneShot, true);
            if (IsInfinitePoiseEnabled) _playerService.ToggleInfinitePoise(true);
            if (IsSilentEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.Silent, true);
            if (IsHiddenEnabled) _playerService.ToggleDebugFlag(ChrDbgFlags.Hidden, true);
            if (IsNoRuneGainEnabled) _playerService.ToggleNoRuneGain(true);
            if (IsNoRuneArcLossEnabled) _playerService.ToggleNoRuneArcLoss(true);
            if (IsNoRuneLossEnabled) _playerService.ToggleNoRuneLoss(true);
            if (IsNoTimePassOnDeathEnabled) _playerService.ToggleNoTimePassOnDeath(true);
            _pauseUpdates = false;
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
            _gameTickService.Unsubscribe(PlayerTick);
        }

        private void OnNewGameStart()
        {
            if (!IsAutoSetNewGameSevenEnabled) return;
            SetNewGame(7);
            NewGame = _playerService.GetNewGame();
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.SetRfbs, SetRfbs);
            _hotkeyManager.RegisterAction(HotkeyActions.SetMaxHp, SetMaxHp);
            _hotkeyManager.RegisterAction(HotkeyActions.SavePos1, () => SavePosition(0));
            _hotkeyManager.RegisterAction(HotkeyActions.SavePos2, () => SavePosition(1));
            _hotkeyManager.RegisterAction(HotkeyActions.RestorePos1, () => RestorePosition(0));
            _hotkeyManager.RegisterAction(HotkeyActions.RestorePos2, () => RestorePosition(1));
            _hotkeyManager.RegisterAction(HotkeyActions.NoDeath,
                () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.NoDamage,
                () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteStamina,
                () => { IsInfiniteStaminaEnabled = !IsInfiniteStaminaEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteConsumables,
                () => { IsInfiniteConsumablesEnabled = !IsInfiniteConsumablesEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteArrows,
                () => { IsInfiniteArrowsEnabled = !IsInfiniteArrowsEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteFp,
                () => { IsInfiniteFpEnabled = !IsInfiniteFpEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.OneShot, () => { IsOneShotEnabled = !IsOneShotEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfinitePoise,
                () => { IsInfinitePoiseEnabled = !IsInfinitePoiseEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.Silent, () => { IsSilentEnabled = !IsSilentEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.Hidden, () => { IsHiddenEnabled = !IsHiddenEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.TogglePlayerSpeed, ToggleSpeed);
            _hotkeyManager.RegisterAction(HotkeyActions.IncreasePlayerSpeed,
                () => SetSpeed(Math.Min(10, PlayerSpeed + 0.25f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreasePlayerSpeed,
                () => SetSpeed(Math.Max(0, PlayerSpeed - 0.25f)));
            _hotkeyManager.RegisterAction(HotkeyActions.RuneArc, () => SafeExecute(ApplyRuneArc));
            _hotkeyManager.RegisterAction(HotkeyActions.Rest, () => SafeExecute(Rest));
            _hotkeyManager.RegisterAction(HotkeyActions.PlayerSetCustomHp, SetCustomHp);
            _hotkeyManager.RegisterAction(HotkeyActions.NoHit, () => { IsNoHitEnabled = !IsNoHitEnabled; });
        }

        private void SafeExecute(Action action)
        {
            if (!AreOptionsEnabled) return;
            action();
        }

        private void PlayerTick()
        {
            if (_pauseUpdates) return;

            CurrentHp = _playerService.GetCurrentHp();
            CurrentMaxHp = _playerService.GetMaxHp();
            PlayerSpeed = _playerService.GetSpeed();
            int newRuneLevel = _playerService.GetRuneLevel();
            Scadu = _playerService.GetScadu();
            SpiritAsh = _playerService.GetSpiritAsh();
            CurrentAnimation = _playerService.GetCurrentAnimation();
            if (ShowPlayerLocation) MapLocation = _playerService.GetMapLocation();

            if (_currentRuneLevel == newRuneLevel) return;
            RuneLevel = newRuneLevel;
            _currentRuneLevel = newRuneLevel;
            LoadStats();
        }

        private void LoadStats()
        {
            Stats stats = _playerService.GetStats();
            Vigor = stats.Vigor;
            Mind = stats.Mind;
            Endurance = stats.Endurance;
            Strength = stats.Strength;
            Dexterity = stats.Dexterity;
            Intelligence = stats.Intelligence;
            Faith = stats.Faith;
            Arcane = stats.Arcane;
            RuneLevel = _playerService.GetRuneLevel();
            NewGame = _playerService.GetNewGame();
        }

        private void SetRfbs() => _playerService.SetRfbs();
        private void SetMaxHp() => _playerService.SetFullHp();
        private void Die() => _playerService.SetHp(0);

        private void SetCustomHp()
        {
            if (!_customHpHasBeenSet) return;
            var (customHp, error) = ParseCustomHp();
            if (customHp == null)
            {
                MsgBox.Show(error, "Invalid Input");
                return;
            }

            if (customHp > CurrentMaxHp)
                customHp = CurrentMaxHp;

            _playerService.SetHp(customHp.Value);
            SettingsManager.Default.SaveCustomHp = CustomHp;
            SettingsManager.Default.Save();            
        }

        private (int? value, string error) ParseCustomHp()
        {
            var input = CustomHp?.Trim();
            if (string.IsNullOrEmpty(input))
                return (null, "Please enter a value");

            if (input.EndsWith("%"))
            {
                if (double.TryParse(input.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
                    return ((int)(percent / 100.0 * CurrentMaxHp), null);
                return (null, "Invalid percentage format");
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var absolute))
                return (absolute, null);

            return (null, "Enter a number or percentage (e.g. 545 or 40%)");
        }

        private void SavePosition(object parameter)
        {
            int index = Convert.ToInt32(parameter);
            var state = index == 0 ? _saveState1 : _saveState2;
            if (index == 0) IsPos1Saved = true;
            else IsPos2Saved = true;

            state.IncludesState = IsStateIncluded;
            if (IsStateIncluded)
            {
                state.Hp = CurrentHp;
                state.Fp = _playerService.GetCurrentFp();
                state.Sp = _playerService.GetCurrentSp();
            }

            _playerService.SavePos(index);
        }

        private void RestorePosition(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (index == 0 && !IsPos1Saved) return;
            if (index == 1 && !IsPos2Saved) return;
            _playerService.RestorePos(index);
            if (!IsStateIncluded) return;

            var state = index == 0 ? _saveState1 : _saveState2;
            if (IsStateIncluded && state.IncludesState)
            {
                _playerService.SetHp(state.Hp);
                _playerService.SetFp(state.Fp);
                _playerService.SetSp(state.Sp);
            }
        }

        private void ApplyRuneArc()
        {
            var playerIns = _playerService.GetPlayerIns();
            _spEffectService.ApplySpEffect(playerIns, SpEffect.RuneArc);
        }

        private void GiveRunes() => _playerService.GiveRunes(Runes);

        private void ToggleSpeed()
        {
            if (!AreOptionsEnabled) return;

            if (!IsApproximately(PlayerSpeed, DefaultSpeed))
            {
                _playerDesiredSpeed = PlayerSpeed;
                SetSpeed(DefaultSpeed);
            }
            else if (_playerDesiredSpeed >= 0)
            {
                SetSpeed(_playerDesiredSpeed);
            }
        }

        private bool IsApproximately(float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        private void Rest()
        {
            if (IsResetWorldIncluded) _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.FadeOutAndPassTime(true));
            else _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.Rest);
        }
        

        private void ApplyPrefs()
        {
            _isRememberSpeedEnabled = SettingsManager.Default.RememberPlayerSpeed;
            OnPropertyChanged(nameof(IsRememberSpeedEnabled));
            if (_isRememberSpeedEnabled) _playerDesiredSpeed = SettingsManager.Default.PlayerSpeed;
        }

        private void SetNewGame(int value)
        {
            _playerService.SetNewGame(value);
            var activeIndex = Math.Min(_newGame, NewGameEventIds.Length - 1);
            for (var i = 0; i < NewGameEventIds.Length; i++)
            {
                _eventService.SetEvent(NewGameEventIds[i], i == activeIndex);
            }
        }
        

        #endregion
    }
}