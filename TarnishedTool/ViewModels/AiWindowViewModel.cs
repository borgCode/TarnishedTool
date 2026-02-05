// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Views.Windows;
using TarnishedTool.Views.Windows.AiOverlay;

namespace TarnishedTool.ViewModels;

public class AiWindowViewModel : BaseViewModel, IDisposable
{
    private readonly IAiService _aiService;
    private readonly IGameTickService _gameTickService;
    private readonly Dictionary<int, GoalInfo> _goalDict;
    private readonly Dictionary<string, Dictionary<int, string>> _enumDicts;
    private readonly Dictionary<int, string> _aiTargetEnums;
    private readonly Dictionary<int, string> _aiInterruptEnums;
    private readonly nint _aiThink;
    private readonly ISpEffectService _spEffectService;
    private readonly AiWindow _parentWindow;
    private AiOverlayToolbar _overlayToolbar;

    public AiWindowViewModel(IAiService aiService, IGameTickService gameTickService,
        Dictionary<int, GoalInfo> goalDict, ChrInsEntry chrInsEntry,
        Dictionary<string, Dictionary<int, string>> enumDicts,
        Dictionary<int, string> aiInterruptEnums, nint aiThink,
        ISpEffectService spEffectService, AiWindow window)
    {
        _aiService = aiService;
        _gameTickService = gameTickService;
        _goalDict = goalDict;
        ChrInsEntry = chrInsEntry;
        _enumDicts = enumDicts;
        _aiInterruptEnums = aiInterruptEnums;
        _aiThink = aiThink;
        _spEffectService = spEffectService;
        _parentWindow = window;

        _enumDicts.TryGetValue("target", out var targetEnums);
        _aiTargetEnums = targetEnums;

        _goalDict.TryGetValue(_aiService.GetMainScriptGoalId(aiThink), out var goalInfo);
        if (goalInfo != null) ScriptName = goalInfo.GoalName;

        EnterOverlayModeCommand = new DelegateCommand(EnterOverlayMode);

        _gameTickService.Subscribe(UpdateTick);
    }

    #region Commands

    public ICommand EnterOverlayModeCommand { get; set; }
    
    #endregion

    
    #region Properties

    public ChrInsEntry ChrInsEntry { get; }

    private GoalViewModel _topGoal;

    public GoalViewModel TopGoal
    {
        get => _topGoal;
        set => SetProperty(ref _topGoal, value);
    }

    public IEnumerable<GoalViewModel> FlatGoals
    {
        get
        {
            if (TopGoal == null) yield break;
            foreach (var goal in FlattenTree(TopGoal))
                yield return goal;
        }
    }

    private bool _isShowGoalsEnabled;

    public bool IsShowGoalsEnabled
    {
        get => _isShowGoalsEnabled;
        set => SetProperty(ref _isShowGoalsEnabled, value);
    }

    private bool _isShowCoolTimesEnabled;

    public bool IsShowCoolTimesEnabled
    {
        get => _isShowCoolTimesEnabled;
        set => SetProperty(ref _isShowCoolTimesEnabled, value);
    }

    public ObservableCollection<CoolTimeEntry> CoolTimes { get; } = new();

    private bool _isShowLuaNumbersEnabled;

    public bool IsShowLuaNumbersEnabled
    {
        get => _isShowLuaNumbersEnabled;
        set => SetProperty(ref _isShowLuaNumbersEnabled, value);
    }

    public LuaNumberViewModel[] LuaNumbers { get; } = Enumerable.Range(0, 64)
        .Select(i => new LuaNumberViewModel { Index = i })
        .ToArray();

    private bool _isShowLuaTimersEnabled;

    public bool IsShowLuaTimersEnabled
    {
        get => _isShowLuaTimersEnabled;
        set => SetProperty(ref _isShowLuaTimersEnabled, value);
    }

    public LuaTimerViewModel[] LuaTimers { get; } = Enumerable.Range(0, 16)
        .Select(i => new LuaTimerViewModel { Index = i })
        .ToArray();

    private bool _isShowSpEffectObservesEnabled;

    public bool IsShowSpEffectObservesEnabled
    {
        get => _isShowSpEffectObservesEnabled;
        set => SetProperty(ref _isShowSpEffectObservesEnabled, value);
    }

    public ObservableCollection<SpEffectObserve> SpEffectObserves { get; } = new();

    private bool _isShowInterruptsEnabled;

    public bool IsShowInterruptsEnabled
    {
        get => _isShowInterruptsEnabled;
        set
        {
            if (!SetProperty(ref _isShowInterruptsEnabled, value)) return;
            if (_isShowInterruptsEnabled) _aiService.RegisterInterruptListener(UpdateInterrupt);
            else _aiService.UnregisterInterruptListener(UpdateInterrupt);
        }
    }

    public ObservableCollection<string> InterruptHistory { get; } = new();

    private string _scriptName;

    public string ScriptName
    {
        get => _scriptName;
        set => SetProperty(ref _scriptName, value);
    }

    private bool _isShowSpEffectsEnabled;

    public bool IsShowSpEffectsEnabled
    {
        get => _isShowSpEffectsEnabled;
        set => SetProperty(ref _isShowSpEffectsEnabled, value);
    }

    private readonly SpEffectViewModel _spEffectViewModel = new();
    public SpEffectViewModel SpEffectViewModel => _spEffectViewModel;
    
    private bool _isOverlayMode;

    public bool IsOverlayMode
    {
        get => _isOverlayMode;
        set => SetProperty(ref _isOverlayMode, value);
    }

    #endregion

    #region Private Methods

    private void UpdateTick()
    {
        if (IsShowGoalsEnabled)
        {
            var goalPtr = _aiService.GetTopGoal(_aiThink);
            UpdateGoalTree(goalPtr);
        }

        if (IsShowCoolTimesEnabled) UpdateCoolTimes();
        if (IsShowLuaNumbersEnabled) UpdateLuaNumbers();
        if (IsShowLuaTimersEnabled) UpdateLuaTimers();
        if (IsShowSpEffectObservesEnabled) UpdateSpEffectObserves();
        if (IsShowSpEffectsEnabled) UpdateSpEffects();
    }

    private void UpdateGoalTree(nint goalPtr)
    {
        var topGoal = _aiService.GetGoalInfo(goalPtr);

        if (TopGoal == null || TopGoal.GoalId != topGoal.GoalId)
        {
            TopGoal = CreateGoalViewModel(topGoal);
        }
        else
        {
            UpdateGoalViewModel(TopGoal, topGoal);
        }

        UpdateChildren(TopGoal, goalPtr);

        OnPropertyChanged(nameof(FlatGoals));
    }

    private GoalViewModel CreateGoalViewModel(GoalIns ins, int indentLevel = 0)
    {
        var vm = new GoalViewModel
        {
            GoalId = ins.GoalId,
            Life = ins.Life,
            TurnTime = ins.TurnTime,
            Name = GetGoalName(ins.GoalId),
            Params = BuildParamViewModels(ins).ToList(),
            IndentLevel = indentLevel,
            Children = new ObservableCollection<GoalViewModel>()
        };
        return vm;
    }

    private void UpdateGoalViewModel(GoalViewModel vm, GoalIns ins)
    {
        vm.Life = ins.Life;
        vm.TurnTime = ins.TurnTime;
        vm.Params = BuildParamViewModels(ins).ToList();
    }

    private void UpdateChildren(GoalViewModel parent, nint goalPtr)
    {
        if (!_aiService.HasSubGoals(goalPtr))
        {
            parent.Children.Clear();
            return;
        }

        var subGoalPtrs = _aiService.GetSubGoals(goalPtr);

        while (parent.Children.Count > subGoalPtrs.Count)
            parent.Children.RemoveAt(parent.Children.Count - 1);

        for (int i = 0; i < subGoalPtrs.Count; i++)
        {
            var childPtr = subGoalPtrs[i];
            var childIns = _aiService.GetGoalInfo(childPtr);

            if (i < parent.Children.Count)
            {
                var existing = parent.Children[i];
                if (existing.GoalId == childIns.GoalId)
                {
                    UpdateGoalViewModel(existing, childIns);
                }
                else
                {
                    parent.Children[i] = CreateGoalViewModel(childIns, parent.IndentLevel + 1);
                }
            }
            else
            {
                parent.Children.Add(CreateGoalViewModel(childIns, parent.IndentLevel + 1));
            }

            UpdateChildren(parent.Children[i], childPtr);
        }
    }

    private string GetGoalName(int goalId) =>
        _goalDict.TryGetValue(goalId, out var info) ? info.GoalName : $"Goal_{goalId}";

    private IEnumerable<GoalParamViewModel> BuildParamViewModels(GoalIns goal)
    {
        _goalDict.TryGetValue(goal.GoalId, out var info);

        for (int i = 0; i < goal.Params.Length; i++)
        {
            if (goal.Params[i] == 0) continue;

            var paramDef = (info != null && i < info.ParamNames.Count)
                ? info.ParamNames[i]
                : new GoalParamDef($"param{i}", ParamType.Float, null);

            var displayValue = FormatParamValue(paramDef, goal.Params[i]);

            yield return new GoalParamViewModel { Label = paramDef.Name, Value = displayValue };
        }
    }

    private string FormatParamValue(GoalParamDef def, float rawValue)
    {
        return def.ParamType switch
        {
            ParamType.Int => ((int)rawValue).ToString(),
            ParamType.UInt => ((uint)rawValue).ToString(),
            ParamType.Bool => (rawValue != 0).ToString(),
            ParamType.Enum => FormatEnum(def.EnumDictName, (int)rawValue),
            _ => rawValue.ToString("F2")
        };
    }

    private string FormatEnum(string dictName, int value)
    {
        if (dictName == null || !_enumDicts.TryGetValue(dictName, out var dict))
            return value.ToString();

        return dict.TryGetValue(value, out var name)
            ? $"{name} ({value})"
            : value.ToString();
    }

    private IEnumerable<GoalViewModel> FlattenTree(GoalViewModel goal)
    {
        yield return goal;
        foreach (var child in goal.Children)
        foreach (var descendant in FlattenTree(child))
            yield return descendant;
    }

    private void UpdateCoolTimes()
    {
        var entries = _aiService.GetCoolTimeItemList(_aiThink);

        while (CoolTimes.Count > entries.Count)
            CoolTimes.RemoveAt(CoolTimes.Count - 1);

        for (int i = 0; i < entries.Count; i++)
        {
            if (i < CoolTimes.Count)
            {
                CoolTimes[i].AnimationId = entries[i].AnimationId;
                CoolTimes[i].TimeSinceLastAttack = entries[i].TimeSinceLastAttack;
                CoolTimes[i].Cooldown = entries[i].Cooldown;
            }
            else
            {
                CoolTimes.Add(entries[i]);
            }
        }
    }

    private void UpdateLuaNumbers()
    {
        var numbers = _aiService.GetLuaNumbers(_aiThink);
        for (int i = 0; i < 64; i++)
        {
            LuaNumbers[i].Value = numbers[i];
        }
    }

    private void UpdateLuaTimers()
    {
        var timers = _aiService.GetLuaTimers(_aiThink);
        for (int i = 0; i < 16; i++)
        {
            LuaTimers[i].Value = timers[i];
        }
    }

    private void UpdateSpEffectObserves()
    {
        var entries = _aiService.GetSpEffectObserveList(_aiThink);

        while (SpEffectObserves.Count > entries.Count)
            SpEffectObserves.RemoveAt(SpEffectObserves.Count - 1);

        for (int i = 0; i < entries.Count; i++)
        {
            var targetName = _aiTargetEnums.TryGetValue(entries[i].Target, out var name) ? name : null;

            if (i < SpEffectObserves.Count)
            {
                SpEffectObserves[i].Target = entries[i].Target;
                SpEffectObserves[i].SpEffectId = entries[i].SpEffectId;
                SpEffectObserves[i].TargetName = targetName;
            }
            else
            {
                var entry = entries[i];
                entry.TargetName = targetName;
                SpEffectObserves.Add(entry);
            }
        }
    }

    private ulong _lastInterrupts;

    private void UpdateInterrupt()
    {
        ulong interrupts = _aiService.GetInterrupts(_aiThink);
        var newInterrupts = interrupts & ~_lastInterrupts;
        _lastInterrupts = interrupts;

        for (int i = 0; i < 64; i++)
        {
            if ((newInterrupts & (1UL << i)) != 0 && _aiInterruptEnums.TryGetValue(i, out var name))
            {
                if (InterruptHistory.Count >= 8) InterruptHistory.RemoveAt(0);
                InterruptHistory.Add(name);
            }
                
        }
    }

    private void UpdateSpEffects()
    {
        var spEffects = _spEffectService.GetActiveSpEffectList(ChrInsEntry.ChrIns);
        _spEffectViewModel.RefreshEffects(spEffects);
    }
    
    
    private void EnterOverlayMode()
    {
        if (IsOverlayMode) return;

        IsOverlayMode = true;

        _overlayToolbar = new AiOverlayToolbar(this, ChrInsEntry.Name, ExitOverlayMode);
        _overlayToolbar.Closed += (s, e) =>
        {
            _overlayToolbar = null;
            if (IsOverlayMode) ExitOverlayMode();
        };
        _overlayToolbar.Show();

        _parentWindow?.Hide();
    }

    private void ExitOverlayMode()
    {
        if (!IsOverlayMode) return;

        IsOverlayMode = false;

        if (_overlayToolbar != null)
        {
            var toolbar = _overlayToolbar;
            _overlayToolbar = null;
            toolbar.Close();
        }

        if (!_isDisposed)
            _parentWindow?.Show();
    }

    #endregion

    #region Public Methods
    
    private bool _isDisposed;
    public void Dispose()
    {
        _isDisposed = true;
        
        if (IsOverlayMode)
        {
            var toolbar = _overlayToolbar;
            _overlayToolbar = null;
            IsOverlayMode = false;
            toolbar?.Close();
        }
        
        _gameTickService.Unsubscribe(UpdateTick);
        if (_isShowInterruptsEnabled)
            _aiService.UnregisterInterruptListener(UpdateInterrupt);
    }

    #endregion
}