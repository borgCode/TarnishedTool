using System;
using System.Windows.Input;
using System.Windows.Threading;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Utilities;
using SilkyRing.Views.Windows;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.ViewModels
{
    public class TargetViewModel : BaseViewModel
    {
        private readonly DispatcherTimer _targetTick;

        private bool _customHpHasBeenSet;

        private ulong _currentTargetId;
        
        private AttackInfoWindow _attackInfoWindow;
        private AttackInfoViewModel _attackInfoViewModel;

        // private ResistancesWindow _resistancesWindowWindow;
        // private bool _isResistancesWindowOpen;
        //
        private DefensesWindow _defensesWindow;
        //
        //
        //
        // private bool _showAllResistances;

        private readonly ITargetService _targetService;

        private readonly IEnemyService _enemyService;

        private readonly IAttackInfoService _attackInfoService;
        // private readonly HotkeyManager _hotkeyManager;

        public TargetViewModel(ITargetService targetService, IStateService stateService, IEnemyService enemyService,
            IAttackInfoService attackInfoService)
        {
            _targetService = targetService;
            _enemyService = enemyService;
            _attackInfoService = attackInfoService;
            
            _attackInfoViewModel = new AttackInfoViewModel();

            // _hotkeyManager = hotkeyManager;
            RegisterHotkeys();

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

            SetHpCommand = new DelegateCommand(SetHp);
            SetHpPercentageCommand = new DelegateCommand(SetHpPercentage);
            SetCustomHpCommand = new DelegateCommand(SetCustomHp);
            ForActSequenceCommand = new DelegateCommand(ForceActSequence);
            KillAllCommand = new DelegateCommand(KillAllBesidesTarget);
            OpenDefensesWindowCommand = new DelegateCommand(OpenDefenseWindow);

            _targetTick = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(64)
            };
            _targetTick.Tick += TargetTick;
        }

       

        #region Commands

        public ICommand SetHpCommand { get; set; }
        public ICommand SetHpPercentageCommand { get; set; }
        public ICommand SetCustomHpCommand { get; set; }
        public ICommand ForActSequenceCommand { get; set; }
        public ICommand KillAllCommand { get; set; }
        public ICommand OpenDefensesWindowCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled = true;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isValidTarget;

        public bool IsValidTarget
        {
            get => _isValidTarget;
            set => SetProperty(ref _isValidTarget, value);
        }

        private bool _isTargetOptionsEnabled;

        public bool IsTargetOptionsEnabled
        {
            get => _isTargetOptionsEnabled;
            set
            {
                if (!SetProperty(ref _isTargetOptionsEnabled, value)) return;
                if (value)
                {
                    _targetService.ToggleTargetHook(true);
                    _targetTick.Start();
                    ShowAllResistances = true;
                }
                else
                {
                    _targetTick.Stop();
                    IsRepeatActEnabled = false;
                    ShowAllResistances = false;
                    // IsResistancesWindowOpen = false;
                    IsFreezeHealthEnabled = false;
                    IsDisableAllExceptTargetEnabled = false;
                    _targetService.ToggleTargetHook(false);
                    ShowPoise = false;
                    ShowSleep = false;
                    ShowPoison = false;
                    ShowRot = false;
                    ShowFrost = false;
                    ShowBleed = false;
                }
            }
        }

        private float _currentPoise;

        public float CurrentPoise
        {
            get => _currentPoise;
            set => SetProperty(ref _currentPoise, value);
        }

        private float _maxPoise;

        public float MaxPoise
        {
            get => _maxPoise;
            set => SetProperty(ref _maxPoise, value);
        }

        private float _poiseTimer;

        public float PoiseTimer
        {
            get => _poiseTimer;
            set => SetProperty(ref _poiseTimer, value);
        }

        private bool _showPoise;

        public bool ShowPoise
        {
            get => _showPoise;
            set
            {
                SetProperty(ref _showPoise, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        }

        private int _currentPoison;

        public int CurrentPoison
        {
            get => _currentPoison;
            set => SetProperty(ref _currentPoison, value);
        }

        private int _maxPoison;

        public int MaxPoison
        {
            get => _maxPoison;
            set => SetProperty(ref _maxPoison, value);
        }

        private bool _showPoison;

        public bool ShowPoison
        {
            get => _showPoison;
            set
            {
                SetProperty(ref _showPoison, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isPoisonImmune;

        public bool IsPoisonImmune
        {
            get => _isPoisonImmune;
            set => SetProperty(ref _isPoisonImmune, value);
        }

        private int _currentBleed;

        public int CurrentBleed
        {
            get => _currentBleed;
            set => SetProperty(ref _currentBleed, value);
        }

        private int _maxBleed;

        public int MaxBleed
        {
            get => _maxBleed;
            set => SetProperty(ref _maxBleed, value);
        }

        private bool _showBleed;

        public bool ShowBleed
        {
            get => _showBleed;
            set
            {
                SetProperty(ref _showBleed, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isBleedImmune;

        public bool IsBleedImmune
        {
            get => _isBleedImmune;
            set => SetProperty(ref _isBleedImmune, value);
        }

        private int _currentRot;

        public int CurrentRot
        {
            get => _currentRot;
            set => SetProperty(ref _currentRot, value);
        }

        private int _maxRot;

        public int MaxRot
        {
            get => _maxRot;
            set => SetProperty(ref _maxRot, value);
        }

        private bool _showRot;

        public bool ShowRot
        {
            get => _showRot;
            set
            {
                SetProperty(ref _showRot, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isRotImmune;

        public bool IsRotImmune
        {
            get => _isRotImmune;
            set => SetProperty(ref _isRotImmune, value);
        }

        private int _currentFrost;

        public int CurrentFrost
        {
            get => _currentFrost;
            set => SetProperty(ref _currentFrost, value);
        }

        private int _maxFrost;

        public int MaxFrost
        {
            get => _maxFrost;
            set => SetProperty(ref _maxFrost, value);
        }

        private bool _showFrost;

        public bool ShowFrost
        {
            get => _showFrost;
            set
            {
                SetProperty(ref _showFrost, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isFrostImmune;

        public bool IsFrostImmune
        {
            get => _isFrostImmune;
            set => SetProperty(ref _isFrostImmune, value);
        }

        private int _currentSleep;

        public int CurrentSleep
        {
            get => _currentSleep;
            set => SetProperty(ref _currentSleep, value);
        }

        private int _maxSleep;

        public int MaxSleep
        {
            get => _maxSleep;
            set => SetProperty(ref _maxSleep, value);
        }

        private bool _showSleep;

        public bool ShowSleep
        {
            get => _showSleep;
            set
            {
                SetProperty(ref _showSleep, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isSleepImmune;

        public bool IsSleepImmune
        {
            get => _isSleepImmune;
            set => SetProperty(ref _isSleepImmune, value);
        }

        private bool _showAllResistances;

        public bool ShowAllResistances
        {
            get => _showAllResistances;
            set
            {
                if (SetProperty(ref _showAllResistances, value))
                {
                    UpdateResistancesDisplay();
                }
            }
        }

        private float _standardDefense;

        public float StandardDefense
        {
            get => _standardDefense;
            set => SetProperty(ref _standardDefense, value);
        }

        private float _slashDefense;

        public float SlashDefense
        {
            get => _slashDefense;
            set => SetProperty(ref _slashDefense, value);
        }

        private float _strikeDefense;

        public float StrikeDefense
        {
            get => _strikeDefense;
            set => SetProperty(ref _strikeDefense, value);
        }

        private float _thrustDefense;

        public float ThrustDefense
        {
            get => _thrustDefense;
            set => SetProperty(ref _thrustDefense, value);
        }

        private float _magicDefense;

        public float MagicDefense
        {
            get => _magicDefense;
            set => SetProperty(ref _magicDefense, value);
        }

        private float _fireDefense;

        public float FireDefense
        {
            get => _fireDefense;
            set => SetProperty(ref _fireDefense, value);
        }

        private float _lightningDefense;

        public float LightningDefense
        {
            get => _lightningDefense;
            set => SetProperty(ref _lightningDefense, value);
        }

        private float _holyDefense;

        public float HolyDefense
        {
            get => _holyDefense;
            set => SetProperty(ref _holyDefense, value);
        }

        private bool _isFreezeAiEnabled;

        public bool IsFreezeAiEnabled
        {
            get => _isFreezeAiEnabled;
            set
            {
                if (SetProperty(ref _isFreezeAiEnabled, value))
                {
                    _targetService.ToggleTargetAi(_isFreezeAiEnabled);
                }
            }
        }

        private bool _isRepeatActEnabled;

        public bool IsRepeatActEnabled
        {
            get => _isRepeatActEnabled;
            set
            {
                if (!SetProperty(ref _isRepeatActEnabled, value)) return;

                bool isRepeating = _targetService.IsTargetRepeating();

                switch (value)
                {
                    case true when !isRepeating:
                        _targetService.ToggleRepeatAct(true);
                        ForceAct = _targetService.GetLastAct();
                        break;
                    case false when isRepeating:
                        _targetService.ToggleRepeatAct(false);
                        ForceAct = 0;
                        break;
                }
            }
        }

        private int _forceAct;

        public int ForceAct
        {
            get => _forceAct;
            set
            {
                if (!SetProperty(ref _forceAct, value)) return;
                _targetService.ForceAct(_forceAct);
                if (_forceAct == 0) IsRepeatActEnabled = false;
            }
        }

        private int _lastAct;

        public int LastAct
        {
            get => _lastAct;
            set => SetProperty(ref _lastAct, value);
        }

        private int _customHp;

        public int CustomHp
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

        private int _currentHealth;

        public int CurrentHealth
        {
            get => _currentHealth;
            set => SetProperty(ref _currentHealth, value);
        }

        private int _maxHealth;

        public int MaxHealth
        {
            get => _maxHealth;
            set => SetProperty(ref _maxHealth, value);
        }

        private bool _isFreezeHealthEnabled;

        public bool IsFreezeHealthEnabled
        {
            get => _isFreezeHealthEnabled;
            set
            {
                SetProperty(ref _isFreezeHealthEnabled, value);
                _targetService.ToggleTargetNoDamage(_isFreezeHealthEnabled);
            }
        }

        private float _targetSpeed;

        public float TargetSpeed
        {
            get => _targetSpeed;
            set
            {
                if (SetProperty(ref _targetSpeed, value))
                {
                    _targetService.SetSpeed(value);
                }
            }
        }

        private bool _isTargetingViewEnabled;

        public bool IsTargetingViewEnabled
        {
            get => _isTargetingViewEnabled;
            set
            {
                if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
                _targetService.ToggleTargetingView(_isTargetingViewEnabled);
            }
        }

        private bool _isNoStaggerEnabled;

        public bool IsNoStaggerEnabled
        {
            get => _isNoStaggerEnabled;
            set
            {
                if (!SetProperty(ref _isNoStaggerEnabled, value)) return;
                _targetService.ToggleNoStagger(_isNoStaggerEnabled);
            }
        }

        private bool _isDisableAllExceptTargetEnabled;

        public bool IsDisableAllExceptTargetEnabled
        {
            get => _isDisableAllExceptTargetEnabled;
            set
            {
                if (!SetProperty(ref _isDisableAllExceptTargetEnabled, value)) return;
                _targetService.ToggleDisableAllExceptTarget(_isDisableAllExceptTargetEnabled);
            }
        }

        private string _actSequence;

        public string ActSequence
        {
            get => _actSequence;
            set => SetProperty(ref _actSequence, value);
        }
        
        private bool _isShowAttackInfoEnabled;
        
        public bool IsShowAttackInfoEnabled
        {
            get => _isShowAttackInfoEnabled;
            set
            {
                if (!SetProperty(ref _isShowAttackInfoEnabled, value))
                {
                    if (_isShowAttackInfoEnabled)
                    {
                        OpenAttackInfoWindow();
                    }
                    _attackInfoService.ToggleAttackInfoHook(_isShowAttackInfoEnabled);
                    
                }
                
            }
        }

        

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            if (IsTargetOptionsEnabled)
            {
                _targetService.ToggleTargetHook(true);
                _targetTick.Start();
            }

            _targetService.ToggleTargetAi(false);
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            _targetTick.Stop();
            // IsFreezeHealthEnabled = false;
            LastAct = 0;
            ForceAct = 0;
            AreOptionsEnabled = false;
            _enemyService.UnhookForceAct();
        }

        private void RegisterHotkeys()
        {
            // _hotkeyManager.RegisterAction("EnableTargetOptions",
            //     () => { IsTargetOptionsEnabled = !IsTargetOptionsEnabled; });
            // _hotkeyManager.RegisterAction("ShowAllResistances", () =>
            // {
            //     ShowAllResistances = !ShowAllResistances;
            //     UpdateResistancesDisplay();
            // });
            // _hotkeyManager.RegisterAction("FreezeHp", () =>
            // {
            //     if (!IsValidTarget) return;
            //     IsFreezeHealthEnabled = !IsFreezeHealthEnabled;
            // });
            // _hotkeyManager.RegisterAction("KillTarget", () => {
            //     if (!IsValidTarget) return;
            //     SetTargetHealth(0);
            // });
            // _hotkeyManager.RegisterAction("DisableTargetAi",
            //     () =>
            //     {
            //         if (!IsValidTarget) return;
            //         IsDisableTargetAiEnabled = !IsDisableTargetAiEnabled;
            //     });
            // _hotkeyManager.RegisterAction("IncreaseTargetSpeed", () =>
            // {
            //     if (!IsValidTarget) return;
            //     SetSpeed(Math.Min(5, TargetSpeed + 0.25f));
            // });
            // _hotkeyManager.RegisterAction("DecreaseTargetSpeed", () =>
            // {
            //     if (!IsValidTarget) return;
            //     SetSpeed(Math.Max(0, TargetSpeed - 0.25f));
            // });
            // _hotkeyManager.RegisterAction("TargetRepeatAct", () =>
            // {
            //     if (!IsValidTarget) return;
            //     IsRepeatActEnabled = !IsRepeatActEnabled;
            // });
            // _hotkeyManager.RegisterAction("DisableAi", () => { IsAllDisableAiEnabled = !IsAllDisableAiEnabled; });
        }

        private void SetHp(object parameter) =>
            _targetService.SetHp(Convert.ToInt32(parameter));

        private void SetHpPercentage(object parameter)
        {
            int healthPercentage = Convert.ToInt32(parameter);
            int newHealth = MaxHealth * healthPercentage / 100;
            _targetService.SetHp(newHealth);
        }

        private void SetCustomHp()
        {
            if (!_customHpHasBeenSet) return;
            if (CustomHp > MaxHealth) CustomHp = MaxHealth;
            _targetService.SetHp(CustomHp);
        }

        private void TargetTick(object sender, EventArgs e)
        {
            if (!IsTargetValid())
            {
                IsValidTarget = false;
                return;
            }

            IsValidTarget = true;
            ulong targetId = _targetService.GetTargetAddr();
            if (targetId != _currentTargetId)
            {
                IsFreezeAiEnabled = _targetService.IsAiDisabled();
                IsTargetingViewEnabled = _targetService.IsTargetViewEnabled();
                int forceActValue = _targetService.GetForceAct();
                if (forceActValue != 0)
                {
                    IsRepeatActEnabled = true;
                    ForceAct = forceActValue;
                }
                else
                {
                    ForceAct = 0;
                    IsRepeatActEnabled = false;
                }

                IsFreezeHealthEnabled = _targetService.IsNoDamageEnabled();
                _currentTargetId = targetId;
                MaxPoise = _targetService.GetMaxPoise();
                UpdateImmunities();
                UpdateDefenses();
                //
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }

            CurrentHealth = _targetService.GetCurrentHp();
            MaxHealth = _targetService.GetMaxHp();
            LastAct = _targetService.GetLastAct();
            TargetSpeed = _targetService.GetSpeed();
            CurrentPoise = _targetService.GetCurrentPoise();
            PoiseTimer = _targetService.GetPoiseTimer();

            CurrentPoison = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.PoisonCurrent);
            MaxPoison = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.PoisonMax);
            CurrentRot = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.RotCurrent);
            MaxRot = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.RotMax);
            CurrentBleed = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.BleedCurrent);
            MaxBleed = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.BleedMax);
            CurrentFrost = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.FrostCurrent);
            MaxFrost = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.FrostMax);
            CurrentSleep = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.SleepCurrent);
            MaxSleep = _targetService.GetResistance((int)ChrIns.ChrResistOffsets.SleepMax);

            if (IsShowAttackInfoEnabled)
            {
                var attackInfo = _attackInfoService.PollAttackInfo();
                if (attackInfo.Count > 0)
                {
                    _attackInfoViewModel.AddAttacks(attackInfo);
                }
            }
        }

        private void UpdateImmunities()
        {
            bool[] immunities = _targetService.GetImmunities();
            IsSleepImmune = immunities[0];
            IsPoisonImmune = immunities[1];
            IsRotImmune = immunities[2];
            IsFrostImmune = immunities[3];
            IsBleedImmune = immunities[4];
        }

        private void UpdateDefenses()
        {
            float[] defenses = _targetService.GetDefenses();
            StandardDefense = (1 - defenses[0]) * 100;
            SlashDefense = (1 - defenses[1]) * 100;
            StrikeDefense = (1 - defenses[2]) * 100;
            ThrustDefense = (1 - defenses[3]) * 100;
            MagicDefense = (1 - defenses[4]) * 100;
            FireDefense = (1 - defenses[5]) * 100;
            LightningDefense = (1 - defenses[6]) * 100;
            HolyDefense = (1 - defenses[7]) * 100;
        }

        private bool IsTargetValid()
        {
            ulong targetId = _targetService.GetTargetAddr();
            if (targetId == 0)
                return false;

            float health = _targetService.GetCurrentHp();
            float maxHealth = _targetService.GetMaxHp();
            if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000)
                return false;

            if (health > maxHealth * 1.5) return false;

            var position = _targetService.GetPosition();

            if (float.IsNaN(position[0]) || float.IsNaN(position[1]) || float.IsNaN(position[2]))
                return false;

            if (Math.Abs(position[0]) > 10000 || Math.Abs(position[1]) > 10000 || Math.Abs(position[2]) > 10000)
                return false;

            return true;
        }

        private void UpdateResistancesDisplay()
        {
            if (!IsTargetOptionsEnabled) return;
            if (_showAllResistances)
            {
                ShowPoise = true;
                ShowSleep = true;
                ShowPoison = true;
                ShowRot = true;
                ShowFrost = true;
                ShowBleed = true;
            }
            else
            {
                ShowPoise = false;
                ShowSleep = false;
                ShowPoison = false;
                ShowRot = false;
                ShowFrost = false;
                ShowBleed = false;
            }
            //
            // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            // _resistancesWindowWindow.DataContext = null;
            // _resistancesWindowWindow.DataContext = this;
        }

        private void KillAllBesidesTarget() => _targetService.KillAllBesidesTarget();

        private void ForceActSequence()
        {
            if (string.IsNullOrWhiteSpace(ActSequence))
            {
                MsgBox.Show("Sequence of acts is empty");
                return;
            }

            string actSequence = ActSequence.Trim();
            string[] parts = actSequence.Split(' ');
            int[] acts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out int act) || act < 0 || act > 99)
                {
                    MsgBox.Show("Invalid act: " + parts[i]);
                    return;
                }

                acts[i] = act;
            }

            var npcThinkParamId = _targetService.GetNpcThinkParamId();
            _enemyService.ForceActSequence(acts, npcThinkParamId);
        }
        
        private void OpenDefenseWindow()
        {
            if (_defensesWindow != null && _defensesWindow.IsVisible) 
            {
                _defensesWindow.Activate(); 
                return;
            }
            
            _defensesWindow = new DefensesWindow
            {
                DataContext = this
            };
            
            _defensesWindow.Closed += (s, e) => _defensesWindow = null;
            _defensesWindow.Show();
        }
        
        private void OpenAttackInfoWindow()
        {
            _attackInfoWindow = new AttackInfoWindow
            {
                DataContext = _attackInfoViewModel
            };
            
            _attackInfoWindow.Closed += (s, e) => _attackInfoWindow = null;
            _attackInfoWindow.Show();
        }
        
        #endregion

        #region Public Methods

        public void SetSpeed(double value) => TargetSpeed = (float)value;

        #endregion

        //
        // private void UpdateResistancesDisplay()
        // {
        //     if (!IsTargetOptionsEnabled) return;
        //     if (_showAllResistances)
        //     {
        //         ShowBleed = true;
        //         ShowHeavyPoise = true;
        //         ShowLightPoise = true;
        //         ShowPoison = true;
        //         ShowToxic = true;
        //     }
        //     else
        //     {
        //         ShowBleed = false;
        //         ShowHeavyPoise = false;
        //         ShowLightPoise = false;
        //         ShowPoison = false;
        //         ShowToxic = false;
        //     }
        //
        //     if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //     _resistancesWindowWindow.DataContext = null;
        //     _resistancesWindowWindow.DataContext = this;
        // }
        //
        // public bool IsResistancesWindowOpen
        // {
        //     get => _isResistancesWindowOpen;
        //     set
        //     {
        //         if (!SetProperty(ref _isResistancesWindowOpen, value)) return;
        //         if (value)
        //             OpenResistancesWindow();
        //         else
        //             CloseResistancesWindow();
        //     }
        // }
        //
        // private void OpenResistancesWindow()
        // {
        //     if (_resistancesWindowWindow != null && _resistancesWindowWindow.IsVisible) return;
        //     _resistancesWindowWindow = new ResistancesWindow
        //     {
        //         DataContext = this
        //     };
        //     _resistancesWindowWindow.Closed += (s, e) => _isResistancesWindowOpen = false;
        //     _resistancesWindowWindow.Show();
        // }
        //
        // private void CloseResistancesWindow()
        // {
        //     if (_resistancesWindowWindow == null || !_resistancesWindowWindow.IsVisible) return;
        //     _resistancesWindowWindow.Close();
        //     _resistancesWindowWindow = null;
        // }
        //


        //
        // public bool ShowLightPoiseAndNotImmune => ShowLightPoise && !IsLightPoiseImmune;
        // public bool ShowBleedAndNotImmune => ShowBleed && !IsBleedImmune;
        // public bool ShowPoisonAndNotImmune => ShowPoison && !IsPoisonToxicImmune;
        // public bool ShowToxicAndNotImmune => ShowToxic && !IsPoisonToxicImmune;
        
    }
}