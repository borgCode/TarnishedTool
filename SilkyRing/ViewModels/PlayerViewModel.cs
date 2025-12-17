using System;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Threading;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.GameIds;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        
        
        // private int _soulMemory;

        
        private Vector3 _coords;
        private int _currentRuneLevel;

        
        private float _playerDesiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _pauseUpdates = true;

        private readonly DispatcherTimer _playerTick;

        private readonly IPlayerService _playerService;

        private readonly CharacterState _saveState1 = new();
        private readonly CharacterState _saveState2 = new();

        private readonly HotkeyManager _hotkeyManager;

        public PlayerViewModel(IPlayerService playerService, IStateService stateService, HotkeyManager hotkeyManager)
        {
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;

            RegisterHotkeys();

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);

            SetRfbsCommand = new DelegateCommand(SetRfbs);
            SetMaxHpCommand = new DelegateCommand(SetMaxHp);

            SavePositionCommand = new DelegateCommand(SavePosition);
            RestorePositionCommand = new DelegateCommand(RestorePosition);

            GiveRunesCommand = new DelegateCommand(GiveRunes);
            ApplyRuneArcCommand = new DelegateCommand(ApplyRuneArc);


            _playerTick = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(64)
            };
            _playerTick.Tick += PlayerTick;
        }

        #region Commands

        public ICommand SetRfbsCommand { get; set; }
        public ICommand SetMaxHpCommand { get; set; }

        public ICommand SavePositionCommand { get; set; }
        public ICommand RestorePositionCommand { get; set; }

        public ICommand GiveRunesCommand { get; set; }
        public ICommand ApplyRuneArcCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.PlayerNoDeath, true);
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

        private bool _isInfiniteStaminaEnabled;

        public bool IsInfiniteStaminaEnabled
        {
            get => _isInfiniteStaminaEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteStaminaEnabled, value))
                {
                    _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteStam, _isInfiniteStaminaEnabled);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteGoods, _isInfiniteConsumablesEnabled);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteArrows, _isInfiniteArrowsEnabled);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteFp, _isInfiniteFpEnabled);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.OneShot, _isOneShotEnabled);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.Silent, _isSilentEnabled);
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
                    _playerService.ToggleDebugFlag(WorldChrManDbg.Hidden, _isHiddenEnabled);
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
                    _playerService.SetNewGame(value);
                }
            }
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
                }
            }
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
        
        public void SetSpeed(float value) => PlayerSpeed = value;

        #endregion
        
        #region Private Methods

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            if (IsSetRfbsOnLoadEnabled) SetRfbs();
            if (IsTorrentNoDeathEnabled) _playerService.ToggleTorrentNoDeath(true);
            LoadStats();
            _playerTick.Start();
            _pauseUpdates = false;
        }

        private void OnGameFirstLoaded()
        {
            if (IsNoDeathEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.PlayerNoDeath, true);
            if (IsInfiniteStaminaEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteStam, true);
            if (IsInfiniteConsumablesEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteGoods, true);
            if (IsInfiniteArrowsEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteArrows, true);
            if (IsInfiniteFpEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.InfiniteFp, true);
            if (IsOneShotEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.OneShot, true);
            if (IsInfinitePoiseEnabled) _playerService.ToggleInfinitePoise(true);
            if (IsSilentEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.Silent, true);
            if (IsHiddenEnabled) _playerService.ToggleDebugFlag(WorldChrManDbg.Hidden, true);
            if (IsNoRuneGainEnabled) _playerService.ToggleNoRuneGain(true);
            if (IsNoRuneArcLossEnabled) _playerService.ToggleNoRuneArcLoss(true);
            if (IsNoRuneLossEnabled) _playerService.ToggleNoRuneLoss(true);
            _pauseUpdates = false;
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.SetRfbs, SetRfbs);
            _hotkeyManager.RegisterAction(HotkeyActions.SetMaxHp, SetMaxHp);
            _hotkeyManager.RegisterAction(HotkeyActions.SavePos1, () => SavePosition(0));
            _hotkeyManager.RegisterAction(HotkeyActions.SavePos2, () => SavePosition(1));
            _hotkeyManager.RegisterAction(HotkeyActions.RestorePos1, () => RestorePosition(0));
            _hotkeyManager.RegisterAction(HotkeyActions.RestorePos2, () => RestorePosition(1));
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteStamina, () => { IsInfiniteStaminaEnabled = !IsInfiniteStaminaEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteConsumables, () => { IsInfiniteConsumablesEnabled = !IsInfiniteConsumablesEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteArrows, () => { IsInfiniteArrowsEnabled = !IsInfiniteArrowsEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfiniteFp, () => { IsInfiniteFpEnabled = !IsInfiniteFpEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.OneShot, () => { IsOneShotEnabled = !IsOneShotEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.InfinitePoise, () => { IsInfinitePoiseEnabled = !IsInfinitePoiseEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.Silent, () => { IsSilentEnabled = !IsSilentEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.Hidden, () => { IsHiddenEnabled = !IsHiddenEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.TogglePlayerSpeed, ToggleSpeed);
            _hotkeyManager.RegisterAction(HotkeyActions.IncreasePlayerSpeed, () => SetSpeed(Math.Min(10, PlayerSpeed + 0.25f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreasePlayerSpeed, () => SetSpeed(Math.Max(0, PlayerSpeed - 0.25f)));
        }

        private void PlayerTick(object sender, EventArgs e)
        {
            if (_pauseUpdates) return;

            CurrentHp = _playerService.GetCurrentHp();
            CurrentMaxHp = _playerService.GetMaxHp();
            PlayerSpeed = _playerService.GetSpeed();
            int newRuneLevel = _playerService.GetRuneLevel();
            // SoulMemory = _playerService.GetSoulMemory();
            _coords = _playerService.GetPlayerPos();
            PosX = _coords.X;
            PosY = _coords.Y;
            PosZ = _coords.Z;
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
            // PlayerSpeed = _playerService.GetPlayerSpeed();
        }

        private void SetRfbs() => _playerService.SetRfbs();
        private void SetMaxHp() => _playerService.SetFullHp();

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
        
        private void ApplyRuneArc() =>  _playerService.ApplySpEffect(SpEffect.RuneArc);
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

        #endregion
        
    }
}