using System;
using System.Windows.Threading;
using SilkyRing.Interfaces;

namespace SilkyRing.ViewModels
{
    public class TargetViewModel : BaseViewModel
    {
        private bool _areOptionsEnabled = true;
        private bool _isTargetOptionsEnabled;
        private bool _isValidTarget;
        private readonly DispatcherTimer _targetOptionsTimer;

        private int _targetCurrentHealth;
        private int _targetMaxHealth;

        private ulong _currentTargetId;

        // private float _targetSpeed;
        private bool _isFreezeHealthEnabled;
        private bool _isTargetingViewEnabled;
        private bool _isDisableTargetAiEnabled;
        private bool _isRepeatActEnabled;

        private int _lastAct;
        private int _forceAct;


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


        //
        // private Forlorn _selectedForlorn;
        // private ObservableCollection<Forlorn> _availableForlorns;
        //
        private readonly ITargetService _targetService;
        // private readonly DamageControlService _damageControlService;
        // private readonly HotkeyManager _hotkeyManager;

        public TargetViewModel(ITargetService targetService, IStateService stateService)
        {
            _targetService = targetService;

            // _hotkeyManager = hotkeyManager;
            RegisterHotkeys();

            _targetOptionsTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(64)
            };
            _targetOptionsTimer.Tick += TargetOptionsTimerTick;
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

        private void TargetOptionsTimerTick(object sender, EventArgs e)
        {
            if (!IsTargetValid())
            {
                IsValidTarget = false;
                _targetService.ClearLockedTarget();
                _isDisableTargetAiEnabled = false;
                OnPropertyChanged(nameof(IsDisableTargetAiEnabled));
                TargetCurrentHealth = 0;
                // TargetCurrentLightPoise = 0;
                // TargetCurrentHeavyPoise = 0;
                // TargetCurrentBleed = 0;
                // TargetCurrentPoison = 0;
                // TargetCurrentToxic = 0;
                return;
            }

            IsValidTarget = true;
            ulong targetId = _targetService.GetTargetAddr();
            if (targetId != _currentTargetId)
            {
                IsDisableTargetAiEnabled = _targetService.IsAiDisabled();
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

            TargetCurrentHealth = _targetService.GetCurrentHp();
            TargetMaxHealth = _targetService.GetMaxHp();
            LastAct = _targetService.GetLastAct();
            //
            // TargetSpeed = _enemyService.GetTargetSpeed();
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

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        public bool IsValidTarget
        {
            get => _isValidTarget;
            set => SetProperty(ref _isValidTarget, value);
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

        public bool IsTargetOptionsEnabled
        {
            get => _isTargetOptionsEnabled;
            set
            {
                if (!SetProperty(ref _isTargetOptionsEnabled, value)) return;
                if (value)
                {
                    _targetService.ToggleTargetHook(true);
                    _targetOptionsTimer.Start();
                    // ShowAllResistances = true;
                }
                else
                {
                    _targetOptionsTimer.Stop();
                    IsRepeatActEnabled = false;
                    // ShowAllResistances = false;
                    // IsResistancesWindowOpen = false;
                    IsFreezeHealthEnabled = false;
                    _targetService.ToggleTargetHook(false);
                    // ShowHeavyPoise = false;
                    // ShowLightPoise = false;
                    // ShowBleed = false;
                    // ShowPoison = false;
                    // ShowToxic = false;
                }
            }
        }

        public bool IsDisableTargetAiEnabled
        {
            get => _isDisableTargetAiEnabled;
            set
            {
                if (SetProperty(ref _isDisableTargetAiEnabled, value))
                {
                    _targetService.ToggleTargetAi(_isDisableTargetAiEnabled);
                }
            }
        }

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

        public int LastAct
        {
            get => _lastAct;
            set => SetProperty(ref _lastAct, value);
        }

        public int TargetCurrentHealth
        {
            get => _targetCurrentHealth;
            set => SetProperty(ref _targetCurrentHealth, value);
        }

        public int TargetMaxHealth
        {
            get => _targetMaxHealth;
            set => SetProperty(ref _targetMaxHealth, value);
        }

        public void SetTargetHealth(int value)
        {
            int health = TargetMaxHealth * value / 100;
            _targetService.SetHp(health);
        }

        //
        // public float TargetSpeed
        // {
        //     get => _targetSpeed;
        //     set
        //     {
        //         if (SetProperty(ref _targetSpeed, value))
        //         {
        //             _enemyService.SetTargetSpeed(value);
        //         }
        //     }
        // }
        //
        // public void SetSpeed(float value)
        // {
        //     TargetSpeed = value;
        // }
        //
        public bool IsFreezeHealthEnabled
        {
            get => _isFreezeHealthEnabled;
            set
            {
                SetProperty(ref _isFreezeHealthEnabled, value);
                _targetService.ToggleTargetNoDamage(_isFreezeHealthEnabled);
            }
        }

        public bool IsTargetingViewEnabled
        {
            get => _isTargetingViewEnabled;
            set
            {
                if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
                _targetService.ToggleTargetingView(_isTargetingViewEnabled);
            }
        }

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
        // public bool IsAllDisableAiEnabled
        // {
        //     get => _isAllDisableAiEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isAllDisableAiEnabled, value))
        //         {
        //             _enemyService.ToggleDisableAi(_isAllDisableAiEnabled);
        //         }
        //     }
        // }
        //
        //
        // public ObservableCollection<Forlorn> AvailableForlorns 
        // {
        //     get => _availableForlorns;
        //     private set => SetProperty(ref _availableForlorns, value);
        // }
        //
        // public Forlorn SelectedForlorn
        // {
        //     get => _selectedForlorn;
        //     set
        //     {
        //         if (!SetProperty(ref _selectedForlorn, value)) return;
        //         CurrentAreaName = value?.AreaName ?? "No Forlorn selected";
        //         IsForlornAvailable = value != null;
        //         OnPropertyChanged(nameof(ForlornIndexes));
        //         
        //         Application.Current.Dispatcher.BeginInvoke(new Action(() => {
        //             SelectedForlornIndex = 0;
        //         }));
        //     }
        // }
        //
        // public bool IsScholar
        // {
        //     get => _isScholar;
        //     private set => SetProperty(ref _isScholar, value);
        // }
        //
        // public bool IsForlornAvailable
        // {
        //     get => _isForlornAvailable;
        //     private set => SetProperty(ref _isForlornAvailable, value);
        // }
        //
        // public string CurrentAreaName
        // {
        //     get => _currentAreaName;
        //     private set => SetProperty(ref _currentAreaName, value);
        // }
        //
        //
        // public int SelectedForlornIndex
        // {
        //     get => _selectedForlornIndex; 
        //     set
        //     {
        //         if (!SetProperty(ref _selectedForlornIndex, value)) return;
        //         if (SelectedForlorn == null || !IsGuaranteedSpawnEnabled) return;
        //         _enemyService.UpdateForlornIndex(_selectedForlornIndex + 1);
        //         _enemyService.ToggleForlornSpawn(true, 
        //             SelectedForlorn.EsdFuncId,
        //             _selectedForlornIndex + 1);
        //     }
        // }
        //
        //
        // public bool IsGuaranteedSpawnEnabled
        // {
        //     get => _isGuaranteedSpawnEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isGuaranteedSpawnEnabled, value)) return;
        //         if (SelectedForlorn != null)
        //         {
        //             _enemyService.ToggleForlornSpawn(_isGuaranteedSpawnEnabled, 
        //                 SelectedForlorn.EsdFuncId,
        //                 SelectedForlornIndex + 1);
        //         }
        //     }
        // }
        //
        // public IEnumerable<string> ForlornIndexes 
        // {
        //     get
        //     {
        //         if (SelectedForlorn?.SpawnNames == null)
        //             return Enumerable.Empty<string>();
        //         return SelectedForlorn.SpawnNames
        //             .Select((name, i) => $"{i + 1}: {name}")
        //             .ToArray();
        //     }
        // }
        //
        public void TryEnableFeatures()
        {
            if (IsTargetOptionsEnabled)
            {
                _targetService.ToggleTargetHook(true);
                _targetOptionsTimer.Start();
            }

            _targetService.ToggleTargetAi(false);
            AreOptionsEnabled = true;
        }

        public void DisableFeatures()
        {
            _targetOptionsTimer.Stop();
            // IsFreezeHealthEnabled = false;
            LastAct = 0;
            ForceAct = 0;
            AreOptionsEnabled = false;
        }

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