using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels
{
    public class TargetViewModel : BaseViewModel
    {
        private bool _customHpHasBeenSet = true;
        private nint _currentTargetChrIns;

        private float _targetDesiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private AttackInfoWindow _attackInfoWindow;
        private AttackInfoViewModel _attackInfoViewModel;

        private ResistancesWindow _resistancesWindowWindow;

        private DefensesWindow _defensesWindow;

        private readonly SpEffectViewModel _spEffectViewModel = new();
        private SpEffectsWindow _spEffectsWindow;

        private readonly ITargetService _targetService;
        private readonly IEnemyService _enemyService;
        private readonly IAttackInfoService _attackInfoService;

        private readonly HotkeyManager _hotkeyManager;
        private readonly ISpEffectService _spEffectService;
        private readonly IEmevdService _emevdService;
        private readonly IGameTickService _gameTickService;
        private readonly IAiWindowService _aiWindowService;

        private DateTime _forceActSequenceLastExecuted = DateTime.MinValue;
        private static readonly TimeSpan ForceActSequenceCooldown = TimeSpan.FromSeconds(2);

        public TargetViewModel(ITargetService targetService, IStateService stateService, IEnemyService enemyService,
            IAttackInfoService attackInfoService, HotkeyManager hotkeyManager, ISpEffectService spEffectService,
            IEmevdService emevdService, IGameTickService gameTickService, IAiWindowService aiWindowService)
        {
            _targetService = targetService;
            _enemyService = enemyService;
            _attackInfoService = attackInfoService;

            _attackInfoViewModel = new AttackInfoViewModel();

            _hotkeyManager = hotkeyManager;
            _spEffectService = spEffectService;
            _emevdService = emevdService;
            _gameTickService = gameTickService;
            _aiWindowService = aiWindowService;
            RegisterHotkeys();

            ShowPoise = SettingsManager.Default.ResistancesShowPoise;
            ShowSleep = SettingsManager.Default.ResistancesShowSleep;
            ShowPoison = SettingsManager.Default.ResistancesShowPoison;
            ShowRot = SettingsManager.Default.ResistancesShowRot;
            ShowFrost = SettingsManager.Default.ResistancesShowFrost;
            ShowBleed = SettingsManager.Default.ResistancesShowBleed;
            ShowMadness = SettingsManager.Default.ResistancesShowMadness;
            ShowDeathBlight = SettingsManager.Default.ResistancesShowDeathBlight;
            ShowCombatInfo = SettingsManager.Default.ResistancesShowCombatInfo;

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

            SetHpCommand = new DelegateCommand(SetHp);
            SetHpPercentageCommand = new DelegateCommand(SetHpPercentage);
            SetCustomHpCommand = new DelegateCommand(SetCustomHp);
            ForActSequenceCommand = new DelegateCommand(ForceActSequence);
            KillAllCommand = new DelegateCommand(KillAllBesidesTarget);
            ResetPositionCommand = new DelegateCommand(ResetPosition);
        }

        #region Commands

        public ICommand SetHpCommand { get; set; }
        public ICommand SetHpPercentageCommand { get; set; }
        public ICommand SetCustomHpCommand { get; set; }
        public ICommand ForActSequenceCommand { get; set; }
        public ICommand KillAllCommand { get; set; }
        public ICommand ResetPositionCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

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
                    _gameTickService.Subscribe(TargetTick);
                }
                else
                {
                    _gameTickService.Unsubscribe(TargetTick);
                    IsShowAiInfoEnabled = false;
                    IsShowDefensesEnabled = false;
                    IsShowAttackInfoEnabled = false;
                    IsShowSpEffectEnabled = false;
                    IsRepeatActEnabled = false;
                    IsResistancesWindowOpen = false;
                    IsFreezeHealthEnabled = false;
                    IsDisableAllExceptTargetEnabled = false;
                    IsNoStaggerEnabled = false;
                    _targetService.ToggleTargetHook(false);
                }

                RefreshResistancesWindow();
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
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowPoise = value;
                SettingsManager.Default.Save();
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
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowPoison = value;
                SettingsManager.Default.Save();
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
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowBleed = value;
                SettingsManager.Default.Save();
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
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowRot = value;
                SettingsManager.Default.Save();
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
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowFrost = value;
                SettingsManager.Default.Save();
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

        private int _currentMadness;

        public int CurrentMadness
        {
            get => _currentMadness;
            set => SetProperty(ref _currentMadness, value);
        }

        private int _maxMadness;

        public int MaxMadness
        {
            get => _maxMadness;
            set => SetProperty(ref _maxMadness, value);
        }

        private bool _showMadness;

        public bool ShowMadness
        {
            get => _showMadness;
            set
            {
                SetProperty(ref _showMadness, value);
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowMadness = value;
                SettingsManager.Default.Save();
            }
        }

        private int _currentDeathBlight;

        public int CurrentDeathBlight
        {
            get => _currentDeathBlight;
            set => SetProperty(ref _currentDeathBlight, value);
        }

        private int _maxDeathBlight;

        public int MaxDeathBlight
        {
            get => _maxDeathBlight;
            set => SetProperty(ref _maxDeathBlight, value);
        }

        private bool _showDeathBlight;

        public bool ShowDeathBlight
        {
            get => _showDeathBlight;
            set
            {
                SetProperty(ref _showDeathBlight, value);
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowDeathBlight = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _showSleep;

        public bool ShowSleep
        {
            get => _showSleep;
            set
            {
                SetProperty(ref _showSleep, value);
                RefreshResistancesWindow();
                SettingsManager.Default.ResistancesShowSleep = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isSleepImmune;

        public bool IsSleepImmune
        {
            get => _isSleepImmune;
            set => SetProperty(ref _isSleepImmune, value);
        }

        private bool _isMadnessImmune;

        public bool IsMadnessImmune
        {
            get => _isMadnessImmune;
            set => SetProperty(ref _isMadnessImmune, value);
        }

        private bool _isDeathBlightImmune;

        public bool IsDeathBlightImmune
        {
            get => _isDeathBlightImmune;
            set => SetProperty(ref _isDeathBlightImmune, value);
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

        private bool _isNoAttackEnabled;

        public bool IsNoAttackEnabled
        {
            get => _isNoAttackEnabled;
            set
            {
                if (SetProperty(ref _isNoAttackEnabled, value))
                {
                    _targetService.ToggleNoAttack(_isNoAttackEnabled);
                }
            }
        }

        private bool _isNoMoveEnabled;

        public bool IsNoMoveEnabled
        {
            get => _isNoMoveEnabled;
            set
            {
                if (SetProperty(ref _isNoMoveEnabled, value))
                {
                    _targetService.ToggleNoMove(_isNoMoveEnabled);
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

        private int _currentAnimation;

        public int CurrentAnimation
        {
            get => _currentAnimation;
            set => SetProperty(ref _currentAnimation, value);
        }

        private string _customHp = "1";

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
                _targetService.ToggleNoHeal(_isFreezeHealthEnabled);
            }
        }

        private bool _showCombatInfo;

        public bool ShowCombatInfo
        {
            get => _showCombatInfo;
            set
            {
                if (SetProperty(ref _showCombatInfo, value))
                {
                    SettingsManager.Default.ResistancesShowCombatInfo = value;
                    SettingsManager.Default.Save();
                }
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

        private bool _isShowDefensesEnabled;

        public bool IsShowDefensesEnabled
        {
            get => _isShowDefensesEnabled;
            set
            {
                if (!SetProperty(ref _isShowDefensesEnabled, value)) return;
                if (_isShowDefensesEnabled) OpenDefenseWindow();
                else CloseDefensesWindow();
            }
        }

        private bool _isShowAttackInfoEnabled;

        public bool IsShowAttackInfoEnabled
        {
            get => _isShowAttackInfoEnabled;
            set
            {
                if (SetProperty(ref _isShowAttackInfoEnabled, value))
                {
                    if (_isShowAttackInfoEnabled) OpenAttackInfoWindow();
                    else CloseAttackInfoWindow();

                    _attackInfoService.ToggleAttackInfoHook(_isShowAttackInfoEnabled);
                }
            }
        }

        private bool _isShowSpEffectEnabled;

        public bool IsShowSpEffectEnabled
        {
            get => _isShowSpEffectEnabled;
            set
            {
                if (SetProperty(ref _isShowSpEffectEnabled, value))
                {
                    if (_isShowSpEffectEnabled) OpenSpEffectsWindow();
                    else CloseSpEffectsWindow();
                }
            }
        }

        private bool _isShowAiInfoEnabled;

        public bool IsShowAiInfoEnabled
        {
            get => _isShowAiInfoEnabled;
            set
            {
                if (SetProperty(ref _isShowAiInfoEnabled, value))
                {
                    if (_isShowAiInfoEnabled) OpenAiWindow();
                    else CloseTargetAiWindow();
                }
            }
        }

        private float _dist;

        public float Dist
        {
            get => _dist;
            set => SetProperty(ref _dist, value);
        }

        private bool _isResistancesWindowOpen;

        public bool IsResistancesWindowOpen
        {
            get => _isResistancesWindowOpen;
            set
            {
                if (!SetProperty(ref _isResistancesWindowOpen, value)) return;
                if (value)
                    OpenResistancesWindow();
                else
                    CloseResistancesWindow();
            }
        }

        public bool ShowSleepAndNotImmune => ShowSleep && !IsSleepImmune;
        public bool ShowPoisonAndNotImmune => ShowPoison && !IsPoisonImmune;
        public bool ShowRotAndNotImmune => ShowRot && !IsRotImmune;
        public bool ShowFrostAndNotImmune => ShowFrost && !IsFrostImmune;
        public bool ShowBleedAndNotImmune => ShowBleed && !IsBleedImmune;
        public bool ShowMadnessAndNotImmune => ShowMadness && !IsMadnessImmune;
        public bool ShowDeathBlightAndNotImmune => ShowDeathBlight && !IsDeathBlightImmune;

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            if (IsTargetOptionsEnabled)
            {
                _targetService.ToggleTargetHook(true);
                _gameTickService.Subscribe(TargetTick);
            }

            _targetService.ToggleTargetAi(false);
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            _gameTickService.Unsubscribe(TargetTick);
            LastAct = 0;
            ForceAct = 0;
            AreOptionsEnabled = false;
            IsDisableAllExceptTargetEnabled = false;
            _targetService.ToggleNoHeal(false);
            _enemyService.UnhookForceAct();
            IsShowAiInfoEnabled = false;
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.EnableTargetOptions,
                () => { IsTargetOptionsEnabled = !IsTargetOptionsEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.KillTarget, () =>
                ExecuteTargetAction(() => SetHp(0)));
            _hotkeyManager.RegisterAction(HotkeyActions.SetTargetMaxHp, () =>
                ExecuteTargetAction(() => SetHpPercentage(100)));
            _hotkeyManager.RegisterAction(HotkeyActions.SetTargetCustomHp, () =>
                ExecuteTargetAction(() => SetHpPercentage(CustomHp)));
            _hotkeyManager.RegisterAction(HotkeyActions.ShowAllResistances, () =>
            {
                if (!IsTargetOptionsEnabled) IsTargetOptionsEnabled = true;
                _showAllResistances = !_showAllResistances;
                UpdateResistancesDisplay();
            });
            _hotkeyManager.RegisterAction(HotkeyActions.FreezeTargetHp,
                () => ExecuteTargetAction(() => IsFreezeHealthEnabled = !IsFreezeHealthEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseTargetSpeed, () =>
                ExecuteTargetAction(() => SetSpeed(Math.Min(5, TargetSpeed + 0.25f))));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseTargetSpeed, () =>
                ExecuteTargetAction(() => SetSpeed(Math.Max(0, TargetSpeed - 0.25f))));
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleTargetSpeed, () =>
                ExecuteTargetAction(ToggleTargetSpeed));
            _hotkeyManager.RegisterAction(HotkeyActions.IncrementForceAct, () =>
                ExecuteTargetAction(() =>
                {
                    if (ForceAct + 1 > 99) ForceAct = 0;
                    else ForceAct += 1;
                }));
            _hotkeyManager.RegisterAction(HotkeyActions.DecrementForceAct, () =>
                ExecuteTargetAction(() =>
                {
                    if (ForceAct - 1 < 0) ForceAct = 99;
                    else ForceAct -= 1;
                }));

            _hotkeyManager.RegisterAction(HotkeyActions.SetForceActToZero, () =>
                ExecuteTargetAction(() => ForceAct = 0));
            _hotkeyManager.RegisterAction(HotkeyActions.DisableTargetAi,
                () => ExecuteTargetAction(() => IsFreezeAiEnabled = !IsFreezeAiEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.DisableAllExceptTargetAi,
                () => ExecuteTargetAction(() => IsDisableAllExceptTargetEnabled = !IsDisableAllExceptTargetEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.TargetNoStagger,
                () => ExecuteTargetAction(() => IsNoStaggerEnabled = !IsNoStaggerEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.TargetRepeatAct,
                () => ExecuteTargetAction(() => IsRepeatActEnabled = !IsRepeatActEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.TargetTargetingView,
                () => ExecuteTargetAction(() => IsTargetingViewEnabled = !IsTargetingViewEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.ShowAttackInfo,
                () => ExecuteWindowAction(() => IsShowAttackInfoEnabled = !IsShowAttackInfoEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.ShowDefenses,
                () => ExecuteWindowAction(() => IsShowDefensesEnabled = !IsShowDefensesEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.ShowTargetSpEffects,
                () => ExecuteWindowAction(() => IsShowSpEffectEnabled = !IsShowSpEffectEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.TargetNoMove,
                () => ExecuteTargetAction(() => IsNoMoveEnabled = !IsNoMoveEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.TargetNoAttack,
                () => ExecuteTargetAction(() => IsNoAttackEnabled = !IsNoAttackEnabled));
            _hotkeyManager.RegisterAction(HotkeyActions.PopoutResistances,
                () => ExecuteWindowAction(() => IsResistancesWindowOpen = !IsResistancesWindowOpen));
            _hotkeyManager.RegisterAction(HotkeyActions.ForceActSequence,
                () => ExecuteTargetAction(ForceActSequence));
            _hotkeyManager.RegisterAction(HotkeyActions.KillAllExceptTarget,
                () => ExecuteTargetAction(KillAllBesidesTarget));
            _hotkeyManager.RegisterAction(HotkeyActions.ResetTargetPosition,
                () => ExecuteTargetAction(ResetPosition));
            _hotkeyManager.RegisterAction(HotkeyActions.TogglePoise, () => ShowPoise = !ShowPoise);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleSleep, () => ShowSleep = !ShowSleep);
            _hotkeyManager.RegisterAction(HotkeyActions.TogglePoison, () => ShowPoison = !ShowPoison);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleRot, () => ShowRot = !ShowRot);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleFrost, () => ShowFrost = !ShowFrost);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleBleed, () => ShowBleed = !ShowBleed);
            _hotkeyManager.RegisterAction(HotkeyActions.AiInfo, () => ExecuteTargetAction(OpenAiWindow));
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleMadness, () => ShowMadness = !ShowMadness);
            _hotkeyManager.RegisterAction(HotkeyActions.ToggleDeathblight, () => ShowDeathBlight = !ShowDeathBlight);
        }

        private void ExecuteTargetAction(Action action)
        {
            if (!IsTargetOptionsEnabled)
            {
                IsTargetOptionsEnabled = true;
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    if (EnsureValidTarget()) action();
                });
                return;
            }

            if (!IsValidTarget) return;
            action();
        }

        private void ExecuteWindowAction(Action action)
        {
            if (!IsTargetOptionsEnabled)
            {
                IsTargetOptionsEnabled = true;
            }

            action();
        }

        private bool EnsureValidTarget() => IsValidTarget || IsTargetValid();

        private void ToggleTargetSpeed()
        {
            if (!AreOptionsEnabled) return;

            if (!IsApproximately(TargetSpeed, DefaultSpeed))
            {
                _targetDesiredSpeed = TargetSpeed;
                SetSpeed(DefaultSpeed);
            }
            else if (_targetDesiredSpeed >= 0)
            {
                SetSpeed(_targetDesiredSpeed);
            }
        }

        private bool IsApproximately(float a, float b) => Math.Abs(a - b) < Epsilon;

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
            var (CustomHp, error) = ParseCustomHp();
            if (CustomHp == null)
            {
                MsgBox.Show(error, "Invalid Input");
                return;
            }

            {
                if (CustomHp > MaxHealth)
                    CustomHp = MaxHealth;

                _targetService.SetHp(CustomHp.Value);
            }
        }

        private (int? value, string error) ParseCustomHp()
        {
            var input = CustomHp?.Trim();
            if (string.IsNullOrEmpty(input))
                return (null, "Please enter a Value");

            if (input.EndsWith("%"))
            {
                if (double.TryParse(input.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var percent))
                    return ((int)(percent / 100.0 * MaxHealth), null);
                return (null, "Invalid percentage format");
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var absolute))
                return (absolute, null);

            return (null, "Enter a number or percentage (e.g. 545 or 40%)");
        }

        private void TargetTick()
        {
            if (!IsTargetValid())
            {
                IsValidTarget = false;
                return;
            }

            IsValidTarget = true;
            nint chrIns = _targetService.GetTargetChrIns();
            if (chrIns != _currentTargetChrIns)
            {
#if DEBUG

                uint entityId = _targetService.GetEntityId();
                int npcThinkParamId = _targetService.GetNpcThinkParamId();
                int chrId = _targetService.GetNpcChrId();
                uint npcParamId = _targetService.GetNpcParamId();
                Console.WriteLine(
                    $@"EntityId: {entityId} NpcThinkParamId: {npcThinkParamId} NpcParamId: {npcParamId} ChrId: {chrId}");
                var aiThink = _targetService.GetAiThinkPtr();
                Console.WriteLine($@"Locked on target chrIns: 0x{(long)chrIns:X} AiThink: 0x{(long)aiThink:X}");

#endif
                IsFreezeAiEnabled = _targetService.IsAiDisabled();
                IsTargetingViewEnabled = _targetService.IsTargetViewEnabled();
                IsNoMoveEnabled = _targetService.IsNoMoveEnabled();
                IsNoAttackEnabled = _targetService.IsNoAttackEnabled();

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
                _targetService.ToggleNoHeal(IsFreezeHealthEnabled);

                if (IsShowAiInfoEnabled) UpdateAiWindow(_currentTargetChrIns, chrIns);

                _currentTargetChrIns = chrIns;
                MaxPoise = _targetService.GetMaxPoise();

                UpdateImmunities();
                UpdateDefenses();
                RefreshResistancesWindow();
            }


            CurrentHealth = _targetService.GetCurrentHp();
            MaxHealth = _targetService.GetMaxHp();
            LastAct = _targetService.GetLastAct();
            CurrentAnimation = _targetService.GetCurrentAnimation();
            TargetSpeed = _targetService.GetSpeed();
            CurrentPoise = _targetService.GetCurrentPoise();
            PoiseTimer = _targetService.GetPoiseTimer();

            Dist = _targetService.GetDist();

            UpdateResistances();


            if (IsShowAttackInfoEnabled)
            {
                var attackInfo = _attackInfoService.PollAttackInfo();
                if (attackInfo.Count > 0)
                {
                    _attackInfoViewModel.AddAttacks(attackInfo);
                }
            }

            if (IsShowSpEffectEnabled)
            {
                var spEffects = _spEffectService.GetActiveSpEffectList(_targetService.GetTargetChrIns());
                _spEffectViewModel.RefreshEffects(spEffects);
            }
        }

        private void UpdateResistances()
        {
            var resistData = _targetService.GetAllResistances();
            CurrentPoison = resistData.PoisonCurrent;
            MaxPoison = resistData.PoisonMax;
            CurrentRot = resistData.RotCurrent;
            MaxRot = resistData.RotMax;
            CurrentBleed = resistData.BleedCurrent;
            MaxBleed = resistData.BleedMax;
            CurrentFrost = resistData.FrostCurrent;
            MaxFrost = resistData.FrostMax;
            CurrentSleep = resistData.SleepCurrent;
            MaxSleep = resistData.SleepMax;
            CurrentMadness = resistData.MadnessCurrent;
            MaxMadness = resistData.MadnessMax;
            CurrentDeathBlight = resistData.DeathBlightCurrent;
            MaxDeathBlight = resistData.DeathBlightMax;
        }

        private void UpdateImmunities()
        {
            bool[] immunities = _targetService.GetImmunities();
            IsSleepImmune = immunities[0];
            IsPoisonImmune = immunities[1];
            IsRotImmune = immunities[2];
            IsFrostImmune = immunities[3];
            IsBleedImmune = immunities[4];
            IsMadnessImmune = immunities[5];
            IsDeathBlightImmune = immunities[6];
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
            long chrIns = _targetService.GetTargetChrIns();
            if (chrIns == 0)
                return false;

            float health = _targetService.GetCurrentHp();
            float maxHealth = _targetService.GetMaxHp();
            if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000)
                return false;

            if (health > maxHealth * 1.5) return false;

            var position = _targetService.GetLocalCoords();

            if (float.IsNaN(position.X) || float.IsNaN(position.Y) || float.IsNaN(position.Z))
                return false;

            if (Math.Abs(position.X) > 10000 || Math.Abs(position.Y) > 10000 || Math.Abs(position.Z) > 10000)
                return false;

            return true;
        }

        private void KillAllBesidesTarget() => _targetService.KillAllBesidesTarget();

        private void ForceActSequence()
        {
            var now = DateTime.UtcNow;
            if (now - _forceActSequenceLastExecuted < ForceActSequenceCooldown) return;
            _forceActSequenceLastExecuted = now;

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
                if (!int.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out int act) ||
                    act < 0 || act > 99)
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
            _defensesWindow = new DefensesWindow
            {
                DataContext = this
            };

            _defensesWindow.Closed += (s, e) =>
            {
                _defensesWindow = null;
                _isShowDefensesEnabled = false; 
            };
            _defensesWindow.Show();
        }

        private void CloseDefensesWindow()
        {
            if (_defensesWindow == null || !_defensesWindow.IsVisible) return;
            _defensesWindow.Close();
            _defensesWindow = null;
        }

        private void OpenAttackInfoWindow()
        {
            _attackInfoWindow = new AttackInfoWindow
            {
                DataContext = _attackInfoViewModel
            };

            _attackInfoWindow.Closed += (s, e) =>
            {
                _attackInfoWindow = null;
                IsShowAttackInfoEnabled = false;
            };
            _attackInfoWindow.Show();
        }

        private void CloseAttackInfoWindow()
        {
            if (_attackInfoWindow == null || !_attackInfoWindow.IsVisible) return;
            _attackInfoWindow.Close();
            _attackInfoWindow = null;
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
                ShowMadness = true;
                ShowDeathBlight = true;
            }
            else
            {
                ShowPoise = false;
                ShowSleep = false;
                ShowPoison = false;
                ShowRot = false;
                ShowFrost = false;
                ShowBleed = false;
                ShowMadness = false;
                ShowDeathBlight = false;
            }

            RefreshResistancesWindow();
        }
        
        private void OpenResistancesWindow()
        {
            if (_resistancesWindowWindow != null && _resistancesWindowWindow.IsVisible) return;

            ShowCombatInfo = SettingsManager.Default.ResistancesShowCombatInfo;

            _resistancesWindowWindow = new ResistancesWindow
            {
                DataContext = this
            };
            _resistancesWindowWindow.Closed += (s, e) => _isResistancesWindowOpen = false;
            _resistancesWindowWindow.Show();
        }

        private void CloseResistancesWindow()
        {
            if (_resistancesWindowWindow == null || !_resistancesWindowWindow.IsVisible) return;
            _resistancesWindowWindow.Close();
            _resistancesWindowWindow = null;
        }

        private void RefreshResistancesWindow()
        {
            if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            _resistancesWindowWindow.DataContext = null;
            _resistancesWindowWindow.DataContext = this;
        }

        private void OpenSpEffectsWindow()
        {
            _spEffectsWindow = new SpEffectsWindow
            {
                DataContext = _spEffectViewModel,
                Title = "Target Active Special Effects"
            };
            _spEffectsWindow.Closed += (s, e) =>
            {
                _spEffectsWindow = null;
                IsShowSpEffectEnabled = false;
            };
            _spEffectsWindow.Show();
        }

        private void CloseSpEffectsWindow()
        {
            if (_spEffectsWindow == null || !_spEffectsWindow.IsVisible) return;
            _spEffectsWindow.Close();
            _spEffectsWindow = null;
        }

        private void ResetPosition()
        {
            var entityId = _targetService.GetEntityId();
            _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ResetCharacterPosition(entityId));
        }

        private void OpenAiWindow()
        {
            var chrInsEntry = CreateChrInsEntry(_currentTargetChrIns);

            _aiWindowService.OpenAiWindow(chrInsEntry);
        }

        private void UpdateAiWindow(nint oldTarget, nint newTarget)
        {
            var newEntry = CreateChrInsEntry(newTarget);
            _aiWindowService.UpdateAiWindow(oldTarget, newEntry);
        }

        private ChrInsEntry CreateChrInsEntry(nint currentTargetChrIns)
        {
            return new ChrInsEntry(currentTargetChrIns)
            {
                ChrId = _targetService.GetNpcChrId(),
                NpcParamId = _targetService.GetNpcParamId(),
                NpcThinkParamId = _targetService.GetNpcThinkParamId(),
                Name = "Current Target"
            };
        }

        private void CloseTargetAiWindow() => _aiWindowService.CloseSpecificWindow(_currentTargetChrIns);

        #endregion

        #region Public Methods

        public void SetSpeed(double value) => TargetSpeed = (float)value;

        #endregion
    }
}