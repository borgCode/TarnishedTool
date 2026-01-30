// 

using System.Collections.Generic;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class GoalWindowViewModel : BaseViewModel
{
    private readonly IAiService _aiService;
    private readonly IGameTickService _gameTickService;
    private readonly Dictionary<int, GoalInfo> _goalDict;

    private nint _chrIns;

    public GoalWindowViewModel(IAiService aiService, IGameTickService gameTickService,
        Dictionary<int, GoalInfo> goalDict, nint chrIns)
    {
        _aiService = aiService;
        _gameTickService = gameTickService;
        _goalDict = goalDict;
        _chrIns = chrIns;
    }

    #region Properties

    private GoalViewModel _topGoal;
    public GoalViewModel TopGoal
    {
        get => _topGoal;
        set => SetProperty(ref _topGoal, value);
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
        
    }

    #endregion
}