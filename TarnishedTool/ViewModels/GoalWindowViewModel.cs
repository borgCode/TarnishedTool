// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class GoalWindowViewModel : BaseViewModel
{
    #region Properties

    private GoalViewModel _topGoal;
    private readonly IAiService _aiService;
    private readonly Dictionary<int, GoalInfo> _goalDict;
    private readonly nint _chrIns;

    public GoalWindowViewModel(IAiService aiService, IGameTickService gameTickService, Dictionary<int, GoalInfo> goalDict, nint chrIns)
    {
        _aiService = aiService;
        _goalDict = goalDict;
        _chrIns = chrIns;
        
        gameTickService.Subscribe(GoalTick);
    }

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

    #endregion

    #region Private Methods

    private void GoalTick()
    {
        var goalPtr = _aiService.GetTopGoal(_chrIns);
        UpdateGoalTree(goalPtr);
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

    #endregion
}