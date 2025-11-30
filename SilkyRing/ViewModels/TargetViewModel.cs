using System;
using System.Windows.Input;
using System.Windows.Threading;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels
{
    public class TargetViewModel : BaseViewModel
    {
        private readonly DispatcherTimer _targetTick;

        private const int MaxNumOfActs = 10;

        private bool _customHpHasBeenSet;

        private ulong _currentTargetId;

        // private ResistancesWindow _resistancesWindowWindow;
        // private bool _isResistancesWindowOpen;
        //
        // private DefenseWindow _defenseWindow;
        //
        // private float _targetCurrentHeavyPoise;
        // private float _targetMaxHeavyPoise;
        // private bool _showHeavyPoise;
        // private float _targetCurrentLightPoise;
        // private float _targetMaxLightPoise;
        // private bool _showLightPoise;
        // private bool _isLightPoiseImmune;
        //
        // private float _targetCurrentBleed;
        // private float _targetMaxBleed;
        // private bool _showBleed;
        // private bool _isBleedImmune;
        //
        // private float _targetCurrentPoison;
        // private float _targetMaxPoison;
        // private bool _showPoison;
        // private bool _isPoisonToxicImmune;
        //
        // private float _targetCurrentToxic;
        // private float _targetMaxToxic;
        // private bool _showToxic;
        //
        // private bool _showAllResistances;
        //
        // private bool _isAllDisableAiEnabled;
        //

        private readonly ITargetService _targetService;

        private readonly IEnemyService _enemyService;
        // private readonly HotkeyManager _hotkeyManager;

        public TargetViewModel(ITargetService targetService, IStateService stateService, IEnemyService enemyService)
        {
            _targetService = targetService;
            _enemyService = enemyService;

            // _hotkeyManager = hotkeyManager;
            RegisterHotkeys();

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

            SetHpCommand = new DelegateCommand(SetHp);
            SetHpPercentageCommand = new DelegateCommand(SetHpPercentage);
            SetCustomHpCommand = new DelegateCommand(SetCustomHp);

            ForActSequenceCommand = new DelegateCommand(ForceActSequence);

            KillAllCommand = new DelegateCommand(KillAllBesidesTarget);

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
                    // ShowAllResistances = true;
                }
                else
                {
                    _targetTick.Stop();
                    IsRepeatActEnabled = false;
                    // ShowAllResistances = false;
                    // IsResistancesWindowOpen = false;
                    IsFreezeHealthEnabled = false;
                    IsDisableAllExceptTargetEnabled = false;
                    _targetService.ToggleTargetHook(false);
                    // ShowHeavyPoise = false;
                    // ShowLightPoise = false;
                    // ShowBleed = false;
                    // ShowPoison = false;
                    // ShowToxic = false;
                }
            }
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

                IsFreezeHealthEnabled = _targetService.IsTargetNoDamageEnabled();
                _currentTargetId = targetId;
                // TargetMaxHeavyPoise = _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.HeavyPoiseMax);
                // TargetMaxLightPoise = _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.LightPoiseMax);
                // (IsPoisonToxicImmune, IsBleedImmune) = _enemyService.GetImmunities();
                // TargetMaxPoison = IsPoisonToxicImmune
                //     ? 0
                //     : _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.PoisonMax);
                // TargetMaxToxic = IsPoisonToxicImmune
                //     ? 0
                //     : _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.ToxicMax);
                // TargetMaxBleed = IsBleedImmune
                //     ? 0
                //     : _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.BleedMax);
                //
                // IsLightPoiseImmune = _enemyService.IsLightPoiseImmune();
                // UpdateDefenses();
                //
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }

            CurrentHealth = _targetService.GetCurrentHp();
            MaxHealth = _targetService.GetMaxHp();
            LastAct = _targetService.GetLastAct();
            TargetSpeed = _targetService.GetSpeed();
            // TargetCurrentHeavyPoise = _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.HeavyPoiseCurrent);
            // TargetCurrentLightPoise = _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.LightPoiseCurrent);
            // TargetCurrentPoison = IsPoisonToxicImmune
            //     ? 0
            //     : _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.PoisonCurrent);
            // TargetCurrentToxic = IsPoisonToxicImmune
            //     ? 0
            //     : _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.ToxicCurrent);
            // TargetCurrentBleed = IsBleedImmune
            //     ? 0
            //     : _enemyService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.BleedCurrent);
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

        private void UpdateDefenses()
        {
            // MagicResist = _enemyService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.MagicResist);
            // LightningResist = _enemyService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.LightningResist);
            // FireResist = _enemyService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.FireResist);
            // DarkResist = _enemyService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.DarkResist);
            // PoisonToxicResist = _enemyService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.PoisonToxicResist);
            // BleedResist = _enemyService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.BleedResist);
            // SlashDefense = _enemyService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommon.Slash);
            // ThrustDefense = _enemyService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommon.Thrust);
            // StrikeDefense = _enemyService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommon.Strike);
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
            int[] acts = new int[MaxNumOfActs + 1]; // +1 for trailing zero

            for (int i = 0; i < MaxNumOfActs; i++)
            {
                if (i >= parts.Length) continue;
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
        
        public void SetSpeed(double value) => TargetSpeed = (float)value;

        //
        // public bool ShowLightPoiseAndNotImmune => ShowLightPoise && !IsLightPoiseImmune;
        // public bool ShowBleedAndNotImmune => ShowBleed && !IsBleedImmune;
        // public bool ShowPoisonAndNotImmune => ShowPoison && !IsPoisonToxicImmune;
        // public bool ShowToxicAndNotImmune => ShowToxic && !IsPoisonToxicImmune;
        //
        //
        // public float TargetCurrentHeavyPoise
        // {
        //     get => _targetCurrentHeavyPoise;
        //     set => SetProperty(ref _targetCurrentHeavyPoise, value);
        // }
        //
        // public float TargetMaxHeavyPoise
        // {
        //     get => _targetMaxHeavyPoise;
        //     set => SetProperty(ref _targetMaxHeavyPoise, value);
        // }
        //
        //
        //
        // public bool ShowHeavyPoise
        // {
        //     get => _showHeavyPoise;
        //     set
        //     {
        //         SetProperty(ref _showHeavyPoise, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool ShowLightPoise
        // {
        //     get => _showLightPoise;
        //     set
        //     {
        //         SetProperty(ref _showLightPoise, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public float TargetCurrentLightPoise
        // {
        //     get => _targetCurrentLightPoise;
        //     set => SetProperty(ref _targetCurrentLightPoise, value);
        // }
        //
        // public float TargetMaxLightPoise
        // {
        //     get => _targetMaxLightPoise;
        //     set => SetProperty(ref _targetMaxLightPoise, value);
        // }
        //
        // public bool IsLightPoiseImmune
        // {
        //     get => _isLightPoiseImmune;
        //     set => SetProperty(ref _isLightPoiseImmune, value);
        // }
        //
        // public float TargetCurrentBleed
        // {
        //     get => _targetCurrentBleed;
        //     set => SetProperty(ref _targetCurrentBleed, value);
        // }
        //
        // public float TargetMaxBleed
        // {
        //     get => _targetMaxBleed;
        //     set => SetProperty(ref _targetMaxBleed, value);
        // }
        //
        // public bool ShowBleed
        // {
        //     get => _showBleed;
        //     set
        //     {
        //         SetProperty(ref _showBleed, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool IsBleedImmune
        // {
        //     get => _isBleedImmune;
        //     set => SetProperty(ref _isBleedImmune, value);
        // }
        //
        // public float TargetCurrentPoison
        // {
        //     get => _targetCurrentPoison;
        //     set => SetProperty(ref _targetCurrentPoison, value);
        // }
        //
        // public float TargetMaxPoison
        // {
        //     get => _targetMaxPoison;
        //     set => SetProperty(ref _targetMaxPoison, value);
        // }
        //
        // public bool ShowPoison
        // {
        //     get => _showPoison;
        //     set
        //     {
        //         SetProperty(ref _showPoison, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool IsPoisonToxicImmune
        // {
        //     get => _isPoisonToxicImmune;
        //     set => SetProperty(ref _isPoisonToxicImmune, value);
        // }
        //
        // public float TargetCurrentToxic
        // {
        //     get => _targetCurrentToxic;
        //     set => SetProperty(ref _targetCurrentToxic, value);
        // }
        //
        // public float TargetMaxToxic
        // {
        //     get => _targetMaxToxic;
        //     set => SetProperty(ref _targetMaxToxic, value);
        // }
        //
        // public bool ShowToxic
        // {
        //     get => _showToxic;
        //     set
        //     {
        //         SetProperty(ref _showToxic, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        //
        // public bool ShowAllResistances
        // {
        //     get => _showAllResistances;
        //     set
        //     {
        //         if (SetProperty(ref _showAllResistances, value))
        //         {
        //             UpdateResistancesDisplay();
        //         }
        //     }
        // }
        //



        //
        // public void TryApplyOneTimeFeatures()
        // {
        //     if (IsAllDisableAiEnabled) _enemyService.ToggleDisableAi(true);
        //     IsScholar = GameVersion.Current.Edition == GameEdition.Scholar;
        // }
        //
        //
        // private float _slashDefense;
        // public float SlashDefense
        // {
        //     get => _slashDefense;
        //     set => SetProperty(ref _slashDefense, value);
        // }
        //
        // private float _thrustDefense;
        // public float ThrustDefense
        // {
        //     get => _thrustDefense;
        //     set => SetProperty(ref _thrustDefense, value);
        // }
        //
        // private float _strikeDefense;
        // public float StrikeDefense
        // {
        //     get => _strikeDefense;
        //     set => SetProperty(ref _strikeDefense, value);
        // }
        //
        // private int _magicResist;
        // public int MagicResist
        // {
        //     get => _magicResist;
        //     set => SetProperty(ref _magicResist, value);
        // }
        //
        // private int _lightningResist;
        // public int LightningResist
        // {
        //     get => _lightningResist;
        //     set => SetProperty(ref _lightningResist, value);
        // }
        //
        // private int _fireResist;
        // public int FireResist
        // {
        //     get => _fireResist;
        //     set => SetProperty(ref _fireResist, value);
        // }
        //
        // private int _darkResist;
        // public int DarkResist
        // {
        //     get => _darkResist;
        //     set => SetProperty(ref _darkResist, value);
        // }
        //
        // private int _poisonToxicResist;
        // public int PoisonToxicResist
        // {
        //     get => _poisonToxicResist;
        //     set => SetProperty(ref _poisonToxicResist, value);
        // }
        //
        // private int _bleedResist;
        // public int BleedResist
        // {
        //     get => _bleedResist;
        //     set => SetProperty(ref _bleedResist, value);
        // }
        //
        public void OpenDefenseWindow()
        {
            // if (_defenseWindow != null && _defenseWindow.IsVisible) 
            // {
            //     _defenseWindow.Activate(); 
            //     return;
            // }
            //
            // _defenseWindow = new DefenseWindow
            // {
            //     DataContext = this
            // };
            //
            // _defenseWindow.Closed += (s, e) => _defenseWindow = null;
            //
            // _defenseWindow.Show();
        }
    }
}