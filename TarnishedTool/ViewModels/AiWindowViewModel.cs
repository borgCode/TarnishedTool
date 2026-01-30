// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels;

internal class AiWindowViewModel : BaseViewModel
{
    private readonly IAiService _aiService;
    private readonly IStateService _stateService;
    private readonly IGameTickService _gameTickService;
    private readonly IPlayerService _playerService;

    private readonly Dictionary<long, ChrInsEntry> _entriesByHandle = new();
    private readonly Dictionary<int, GoalInfo> _goalInfos;

    public AiWindowViewModel(IAiService aiService, IStateService stateService, IGameTickService gameTickService,
        IPlayerService playerService)
    {
        _aiService = aiService;
        _stateService = stateService;
        _gameTickService = gameTickService;
        _playerService = playerService;

        _goalInfos = DataLoader.LoadGoalInfo();

        WarpToSelectedCommand = new DelegateCommand(WarpToSelected);
        OpenGoalWindowForSelectedCommand = new DelegateCommand(OpenGoalWindow);
    }
    
    #region Commands
    
    public ICommand WarpToSelectedCommand { get; set; }
    public ICommand OpenGoalWindowForSelectedCommand { get; set; }

    #endregion

    #region Properties

    private ObservableCollection<ChrInsEntry> _chrInsEntries = new();

    public ObservableCollection<ChrInsEntry> ChrInsEntries

    {
        get => _chrInsEntries;
        set => SetProperty(ref _chrInsEntries, value);
    }

    private ChrInsEntry _selectedChrInsEntry;

    public ChrInsEntry SelectedChrInsEntry
    {
        get => _selectedChrInsEntry;
        set
        {
            var previousSelectedChrInsEntry = _selectedChrInsEntry;
            SetProperty(ref _selectedChrInsEntry, value);
            if (previousSelectedChrInsEntry != null)
            {
                _aiService.SetSelected(previousSelectedChrInsEntry.ChrIns, false);
            }

            if (value != null)
            {
                _aiService.SetSelected(_selectedChrInsEntry.ChrIns, true);
            }
            
        } 
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        _gameTickService.Subscribe(ChrInsEntriesTick);
    }

    private void OnGameNotLoaded()
    {
        _gameTickService.Unsubscribe(ChrInsEntriesTick);
    }

    private void ChrInsEntriesTick()
    {
        var entries = _aiService.GetNearbyChrInsEntries();
        var seenHandles = new HashSet<long>();

        foreach (var entry in entries)
        {
            long handle = _aiService.GetHandleByChrIns(entry.ChrIns);

            seenHandles.Add(handle);
            if (_entriesByHandle.TryGetValue(handle, out _))
            {
                continue;
            }

            entry.NpcThinkParamId = _aiService.GetNpcThinkParamIdByChrIns(entry.ChrIns);

            if (entry.NpcThinkParamId == 0) continue;

            entry.Handle = handle;
            entry.ChrId = _aiService.GetChrIdByChrIns(entry.ChrIns);
            entry.NpcParamId = _aiService.GetNpcParamIdByChrIns(entry.ChrIns);

            _entriesByHandle[handle] = entry;
            ChrInsEntries.Add(entry);
        }


        var toRemove = _entriesByHandle.Keys.Where(h => !seenHandles.Contains(h)).ToList();
        foreach (var handle in toRemove)
        {
            var entry = _entriesByHandle[handle];
            _entriesByHandle.Remove(handle);
            ChrInsEntries.Remove(entry);
        }
    }
    
    private void WarpToSelected()
    {
        var targetPosition = _aiService.GetChrInsPos(SelectedChrInsEntry.ChrIns);
        _playerService.MoveToPosition(targetPosition);
    }
    
    private void OpenGoalWindow()
    {
        var window = new GoalWindow();
        var vm = new GoalWindowViewModel(_aiService, _gameTickService, _goalInfos, SelectedChrInsEntry.ChrIns);
        window.DataContext = vm;
        window.Show();
    }

    #endregion

    #region Public Methods

    public void NotifyWindowOpen()
    {
        _stateService.Subscribe(State.Loaded, OnGameLoaded);
        _gameTickService.Subscribe(ChrInsEntriesTick);
    }

    public void NotifyWindowClosed()
    {
        _stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        _gameTickService.Unsubscribe(ChrInsEntriesTick);
    }
    
    public void ClearSelected()
    {
        SelectedChrInsEntry = null;
    }

    #endregion

    
}