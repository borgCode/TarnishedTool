// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class AiWindowViewModel : BaseViewModel, IDisposable
{
    private readonly IAiService _aiService;
    private readonly IGameTickService _gameTickService;
    private readonly Dictionary<int, GoalInfo> _goalDict;
    private readonly Dictionary<int, string> _aiTargetEnums;
    private readonly Dictionary<int, string> _aiInterruptEnums;
    private readonly nint _chrIns;
    private readonly nint _aiThink;

    public AiWindowViewModel(IAiService aiService, IGameTickService gameTickService,
        Dictionary<int, GoalInfo> goalDict, nint chrIns, Dictionary<int, string> aiTargetEnums,
        Dictionary<int, string> aiInterruptEnums, nint aiThink)
    {
        _aiService = aiService;
        _gameTickService = gameTickService;
        _goalDict = goalDict;
        _chrIns = chrIns;
        _aiTargetEnums = aiTargetEnums;
        _aiInterruptEnums = aiInterruptEnums;
        _aiThink = aiThink;

        _gameTickService.Subscribe(UpdateTick);
    }

    #region Properties

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
    
    #endregion

    #region Private Methods

    private void UpdateTick()
    {
        if (IsShowGoalsEnabled)
        {
            var goalPtr = _aiService.GetTopGoal(_chrIns);
            UpdateGoalTree(goalPtr);
        }

        if (IsShowLuaNumbersEnabled) UpdateLuaNumbers();
        if (IsShowLuaTimersEnabled) UpdateLuaTimers();
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

            var label = (info != null && i < info.ParamNames.Count && !string.IsNullOrEmpty(info.ParamNames[i]))
                ? info.ParamNames[i]
                : $"param{i}";

            yield return new GoalParamViewModel { Label = label, Value = goal.Params[i] };
        }
    }

    private IEnumerable<GoalViewModel> FlattenTree(GoalViewModel goal)
    {
        yield return goal;
        foreach (var child in goal.Children)
        foreach (var descendant in FlattenTree(child))
            yield return descendant;
    }

    private void UpdateLuaNumbers()
    {
        var numbers = _aiService.GetLuaNumbers(_chrIns);
        for (int i = 0; i < 64; i++)
        {
            LuaNumbers[i].Value = numbers[i];
        }

        _aiService.GetSpEffectObserveList(_chrIns);
    }

    private void UpdateLuaTimers()
    {
        var timers = _aiService.GetLuaTimers(_chrIns);
        for (int i = 0; i < 16; i++)
        {
            LuaTimers[i].Value = timers[i];
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
                InterruptHistory.Add(name);
        }
    }

    #endregion

    #region Public Methods

    public void Dispose()
    {
        _gameTickService.Unsubscribe(UpdateTick);
        if (_isShowInterruptsEnabled)
            _aiService.UnregisterInterruptListener(UpdateInterrupt);
    }

    #endregion
}