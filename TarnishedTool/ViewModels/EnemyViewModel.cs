using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels;

public class EnemyViewModel : BaseViewModel
{
    private readonly IEnemyService _enemyService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IEmevdService _emevdService;
    private readonly IDlcService _dlcService;
    private readonly ISpEffectService _spEffectService;
    private readonly IParamService _paramService;
    private readonly IPlayerService _playerService;
    private readonly IEventService _eventService;
    private readonly IReminderService _reminderService;
    private readonly ITravelService _travelService;
    private readonly IChrInsService _chrInsService;

    public const uint LionMainBossEntityId = 20000800;
    public const int LionMainBossNpcParamId = 52100088;

    public const uint LionMinibossEntityId = 2046460800;
    public const int LionMinibossNpcParamId = 52100094;

    public const int LightningAnimationId = 20002;
    public const int DeathblightAnimationId = 20003;
    public const int FrostAnimationId = 20004;
    public const int WindAnimationId = 20006;

    public const int NpcParamTableIndex = 6;
    public const int NpcParamSlotIndex = 0;
    public static readonly BitFlag InitializeDead = new(0x14D, 1 << 3);

    private bool _shouldSetNight;

    public const int PhaseTransitionCooldownSpEffectId = 20011216;

    private const int EbNpcThinkParamId = 22000000;
    private const int EldenStarsActIdx = 22;
    private DateTime _ebLastExecuted = DateTime.MinValue;
    private static readonly TimeSpan EbCooldownDuration = TimeSpan.FromSeconds(2);

    public SearchableGroupedCollection<string, BossRevive> BossRevives { get; }

    public EnemyViewModel(IEnemyService enemyService, IStateService stateService, HotkeyManager hotkeyManager,
        IEmevdService emevdService, IDlcService dlcService, ISpEffectService spEffectService,
        IParamService paramService, IPlayerService playerService, IEventService eventService,
        IReminderService reminderService, ITravelService travelService, IChrInsService chrInsService)
    {
        _enemyService = enemyService;
        _hotkeyManager = hotkeyManager;
        _emevdService = emevdService;
        _dlcService = dlcService;
        _spEffectService = spEffectService;
        _paramService = paramService;
        _playerService = playerService;
        _eventService = eventService;
        _reminderService = reminderService;
        _travelService = travelService;
        _chrInsService = chrInsService;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);

        EbForceActSequenceCommand = new DelegateCommand(ForceEbActSequence);
        SetLionMainBossLightningPhaseCommand = new DelegateCommand(ForceLionMainBossLightningPhase);
        SetLionMainBossFrostPhaseCommand = new DelegateCommand(ForceLionMainBossFrostPhase);
        SetLionMainBossWindPhaseCommand = new DelegateCommand(ForceLionMainBossWindPhase);
        SetLionMiniBossDeathblightPhaseCommand = new DelegateCommand(ForceLionMiniBossDeathblightPhase);
        SetLionMiniBossFrostPhaseCommand = new DelegateCommand(ForceLionMiniBossFrostPhase);
        SetLionMiniBossWindPhaseCommand = new DelegateCommand(ForceLionMiniBossWindPhase);
        SetLionMiniBossLightningPhaseCommand = new DelegateCommand(ForceLionMiniBossLightningPhase);

        ReviveBossCommand = new DelegateCommand(ReviveBoss);
        ReviveBossFirstEncounterCommand = new DelegateCommand(ReviveBossFirstEncounter);
        ReviveAllBossesCommand = new DelegateCommand(ReviveAllBosses);
        ReviveAllBossesAsFirstEncounterCommand = new DelegateCommand(ReviveAllBossesAsFirstEncounter);

        BossRevives = new SearchableGroupedCollection<string, BossRevive>(
            DataLoader.GetBossRevives(),
            (bossRevive, search) => bossRevive.BossName.ToLower().Contains(search) ||
                                    bossRevive.Area.ToLower().Contains(search));

        SelectedAct = Acts.FirstOrDefault();

        RegisterHotkeys();
    }

    #region Commands

    public ICommand EbForceActSequenceCommand { get; set; }
    public ICommand SetLionMainBossLightningPhaseCommand { get; set; }
    public ICommand SetLionMainBossFrostPhaseCommand { get; set; }
    public ICommand SetLionMainBossWindPhaseCommand { get; set; }
    public ICommand SetLionMiniBossDeathblightPhaseCommand { get; set; }
    public ICommand SetLionMiniBossFrostPhaseCommand { get; set; }
    public ICommand SetLionMiniBossWindPhaseCommand { get; set; }
    public ICommand SetLionMiniBossLightningPhaseCommand { get; set; }

    public ICommand ReviveBossCommand { get; set; }
    public ICommand ReviveBossFirstEncounterCommand { get; set; }
    public ICommand ReviveAllBossesCommand { get; set; }
    public ICommand ReviveAllBossesAsFirstEncounterCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled = true;

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

    private bool _isNoDeathEnabled;

    public bool IsNoDeathEnabled
    {
        get => _isNoDeathEnabled;
        set
        {
            SetProperty(ref _isNoDeathEnabled, value);
            _enemyService.ToggleNoDeath(_isNoDeathEnabled);
        }
    }

    private bool _isNoDamageEnabled;

    public bool IsNoDamageEnabled
    {
        get => _isNoDamageEnabled;
        set
        {
            SetProperty(ref _isNoDamageEnabled, value);
            _enemyService.ToggleNoDamage(_isNoDamageEnabled);
        }
    }

    private bool _isNoHitEnabled;

    public bool IsNoHitEnabled
    {
        get => _isNoHitEnabled;
        set
        {
            SetProperty(ref _isNoHitEnabled, value);
            _enemyService.ToggleNoHit(_isNoHitEnabled);
        }
    }

    private bool _isNoAttackEnabled;

    public bool IsNoAttackEnabled
    {
        get => _isNoAttackEnabled;
        set
        {
            SetProperty(ref _isNoAttackEnabled, value);
            _enemyService.ToggleNoAttack(_isNoAttackEnabled);
        }
    }

    private bool _isNoMoveEnabled;

    public bool IsNoMoveEnabled
    {
        get => _isNoMoveEnabled;
        set
        {
            SetProperty(ref _isNoMoveEnabled, value);
            _enemyService.ToggleNoMove(_isNoMoveEnabled);
        }
    }

    private bool _isDisableAiEnabled;

    public bool IsDisableAiEnabled
    {
        get => _isDisableAiEnabled;
        set
        {
            SetProperty(ref _isDisableAiEnabled, value);
            _enemyService.ToggleDisableAi(_isDisableAiEnabled);
        }
    }

    private bool _isTargetingViewEnabled;

    public bool IsTargetingViewEnabled
    {
        get => _isTargetingViewEnabled;
        set
        {
            if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
            _enemyService.ToggleTargetingView(_isTargetingViewEnabled);
            if (!_isTargetingViewEnabled)
            {
                IsDrawReducedTargetViewEnabled = false;
            }
        }
    }

    private bool _isDrawReducedTargetViewEnabled;

    public bool IsDrawReducedTargetViewEnabled
    {
        get => _isDrawReducedTargetViewEnabled;
        set
        {
            if (!SetProperty(ref _isDrawReducedTargetViewEnabled, value)) return;
            _enemyService.ToggleReducedTargetingView(_isDrawReducedTargetViewEnabled);
            _enemyService.SetTargetViewMaxDist(ReducedTargetViewDistance);
        }
    }

    private float _reducedTargetViewDistance = 100;

    public float ReducedTargetViewDistance
    {
        get => _reducedTargetViewDistance;
        set
        {
            if (!SetProperty(ref _reducedTargetViewDistance, value)) return;
            if (!IsDrawReducedTargetViewEnabled) return;
            _enemyService.SetTargetViewMaxDist(_reducedTargetViewDistance);
        }
    }

    private bool _isDrawNavigationRouteEnabled;

    public bool IsDrawNavigationRouteEnabled
    {
        get => _isDrawNavigationRouteEnabled;
        set
        {
            SetProperty(ref _isDrawNavigationRouteEnabled, value);
            _enemyService.ToggleDrawNavigationRoute(_isDrawNavigationRouteEnabled);
        }
    }

    private bool _isRykardNoMegaEnabled;

    public bool IsRykardNoMegaEnabled
    {
        get => _isRykardNoMegaEnabled;
        set
        {
            SetProperty(ref _isRykardNoMegaEnabled, value);
            _enemyService.ToggleRykardMega(_isRykardNoMegaEnabled);
        }
    }

    private bool _isLionMainBossPhaseLockEnabled;

    public bool IsLionMainBossPhaseLockEnabled
    {
        get => _isLionMainBossPhaseLockEnabled;
        set
        {
            SetProperty(ref _isLionMainBossPhaseLockEnabled, value);
            if (_isLionMainBossPhaseLockEnabled) IsLionMiniBossPhaseLockEnabled = false;
            _enemyService.ToggleLionCooldownHook(_isLionMainBossPhaseLockEnabled, LionMainBossNpcParamId);
            if (_isLionMainBossPhaseLockEnabled) ApplyLionSpEffects(LionMainBossEntityId);
            else RemoveLionSpEffects(LionMainBossEntityId);
        }
    }

    private bool _isLionMiniBossPhaseLockEnabled;

    public bool IsLionMiniBossPhaseLockEnabled
    {
        get => _isLionMiniBossPhaseLockEnabled;
        set
        {
            SetProperty(ref _isLionMiniBossPhaseLockEnabled, value);
            if (_isLionMiniBossPhaseLockEnabled) IsLionMainBossPhaseLockEnabled = false;
            _enemyService.ToggleLionCooldownHook(_isLionMiniBossPhaseLockEnabled, LionMinibossNpcParamId);
            if (_isLionMiniBossPhaseLockEnabled) ApplyLionSpEffects(LionMinibossEntityId);
            else RemoveLionSpEffects(LionMinibossEntityId);
        }
    }

    public IReadOnlyList<Act> Acts { get; } = DataLoader.GetEbActs().ToList();

    private Act _selectedAct;

    public Act SelectedAct
    {
        get => _selectedAct;
        set => SetProperty(ref _selectedAct, value);
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        IsDlcAvailable = _dlcService.IsDlcAvailable;
        if (_shouldSetNight)
        {
            _shouldSetNight = false;
            _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetNight);
        }
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        IsLionMainBossPhaseLockEnabled = false;
        IsLionMiniBossPhaseLockEnabled = false;
    }

    private void OnGameFirstLoaded()
    {
        if (IsNoDeathEnabled) _enemyService.ToggleNoDeath(true);
        if (IsNoDamageEnabled) _enemyService.ToggleNoDamage(true);
        if (IsNoHitEnabled) _enemyService.ToggleNoHit(true);
        if (IsNoAttackEnabled) _enemyService.ToggleNoAttack(true);
        if (IsNoMoveEnabled) _enemyService.ToggleNoMove(true);
        if (IsDisableAiEnabled) _enemyService.ToggleDisableAi(true);
        if (IsRykardNoMegaEnabled) _enemyService.ToggleRykardMega(true);
        if (IsTargetingViewEnabled) _enemyService.ToggleTargetingView(true);
        if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled)
            _enemyService.ToggleReducedTargetingView(true);
        if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled)
            _enemyService.SetTargetViewMaxDist(ReducedTargetViewDistance);
    }

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoDeath, () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoDamage, () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoHit, () => { IsNoHitEnabled = !IsNoHitEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoAttack, () => { IsNoAttackEnabled = !IsNoAttackEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoMove, () => { IsNoMoveEnabled = !IsNoMoveEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllDisableAi, () => { IsDisableAiEnabled = !IsDisableAiEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllTargetingView,
            () => { IsTargetingViewEnabled = !IsTargetingViewEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.ForceEbActSequence, () => SafeExecute(ForceEbActSequence));
        _hotkeyManager.RegisterAction(HotkeyActions.ReviveSelectedBoss, () => SafeExecute(ReviveBoss));
        _hotkeyManager.RegisterAction(HotkeyActions.ReviveSelectedBossFirstEncounter,
            () => SafeExecute(ReviveBossFirstEncounter));
    }

    private void SafeExecute(Action action)
    {
        if (!AreOptionsEnabled) return;
        action();
    }

    private void ForceEbActSequence()
    {
        var now = DateTime.UtcNow;
        if (now - _ebLastExecuted < EbCooldownDuration) return;
        _ebLastExecuted = now;
        int[] acts = [EldenStarsActIdx, SelectedAct.ActIdx];
        _enemyService.ForceActSequence(acts, EbNpcThinkParamId);
    }

    private void ApplyLionSpEffects(uint entityId)
    {
        var chrIns = _chrInsService.ChrInsByEntityId(entityId);
        if (chrIns == IntPtr.Zero) return;
        _spEffectService.ApplySpEffect(chrIns, PhaseTransitionCooldownSpEffectId);
        _spEffectService.ApplySpEffect(chrIns,
            20011237); //Some 15sec duration speffect, needed for no triple phase attack in lightning phase
        _spEffectService.ApplySpEffect(chrIns, 20011245); // Phase 2 active
    }

    private void RemoveLionSpEffects(uint entityId)
    {
        var chrIns = _chrInsService.ChrInsByEntityId(entityId);
        if (chrIns == IntPtr.Zero) return;
        _spEffectService.RemoveSpEffect(chrIns, PhaseTransitionCooldownSpEffectId);
        _spEffectService.RemoveSpEffect(chrIns, 20011237);
    }

    private void ForceLionMainBossLightningPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMainBossEntityId, LightningAnimationId));
    }

    private void ForceLionMainBossFrostPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMainBossEntityId, FrostAnimationId));
    }

    private void ForceLionMainBossWindPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMainBossEntityId, WindAnimationId));
    }

    private void ForceLionMiniBossDeathblightPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMinibossEntityId, DeathblightAnimationId));
    }

    private void ForceLionMiniBossFrostPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMinibossEntityId, FrostAnimationId));
    }

    private void ForceLionMiniBossWindPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMinibossEntityId, WindAnimationId));
    }
    
    private void ForceLionMiniBossLightningPhase()
    {
        _reminderService.TrySetReminder();
        _emevdService.ExecuteEmevdCommand(
            Emevd.EmevdCommands.ForcePlaybackAnimation(LionMinibossEntityId, LightningAnimationId));
    }

    private void ReviveBoss()
    {
        var bossRevive = BossRevives.SelectedItem;
        SetBossFlags(bossRevive, isFirstEncounter: false);
        var playerBlockId = _playerService.GetBlockId();
        if (playerBlockId != bossRevive.BlockId &&
            (bossRevive.BossBlockIds == null || !bossRevive.BossBlockIds.Contains(playerBlockId)))
            return;

        if (bossRevive.ShouldSetNight) _shouldSetNight = true;

        _ = Task.Run(() => _travelService.WarpToBlockId(bossRevive.Position));
    }

    private void ReviveBossFirstEncounter()
    {
        var bossRevive = BossRevives.SelectedItem;
        SetBossFlags(bossRevive, isFirstEncounter: true);
        var playerBlockId = _playerService.GetBlockId();
        if (playerBlockId != bossRevive.BlockId &&
            (bossRevive.BossBlockIds == null || !bossRevive.BossBlockIds.Contains(playerBlockId)))
            return;

        if (bossRevive.ShouldSetNight) _shouldSetNight = true;

        _ = Task.Run(() => _travelService.WarpToBlockId(bossRevive.PositionFirstEncounter));
    }

    private void ReviveAllBosses()
    {
        foreach (var bossRevive in BossRevives.AllItems)
            SetBossFlags(bossRevive, isFirstEncounter: false);

        _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ReloadArea);
    }

    private void ReviveAllBossesAsFirstEncounter()
    {
        foreach (var bossRevive in BossRevives.AllItems)
            SetBossFlags(bossRevive, isFirstEncounter: true);

        _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ReloadArea);
    }

    private void SetBossFlags(BossRevive bossRevive, bool isFirstEncounter)
    {
        if (bossRevive.IsDlc && !IsDlcAvailable) return;
        if (!bossRevive.IsInitializeDeadSet) SetInitializeDead(bossRevive.NpcParamIds);

        if (isFirstEncounter)
        {
            foreach (var flag in bossRevive.FirstEncounterFlags)
                _eventService.SetEvent(flag.EventId, flag.SetValue);
        }

        foreach (var flag in bossRevive.BossFlags)
            _eventService.SetEvent(flag.EventId, flag.SetValue);
    }

    private void SetInitializeDead(List<uint> npcParamIds)
    {
        foreach (var npcParamId in npcParamIds)
        {
            var paramRow = _paramService.GetParamRow(NpcParamTableIndex, NpcParamSlotIndex, npcParamId);
            _paramService.SetBit(paramRow, InitializeDead.Offset, InitializeDead.Bit, true);
        }
    }

    #endregion
}