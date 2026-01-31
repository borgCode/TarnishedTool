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
    private readonly IChrInsService _chrInsService;

    private readonly Dictionary<int, string> _chrNames;
    private readonly Dictionary<long, ChrInsEntry> _entriesByHandle = new();
    private readonly Dictionary<int, GoalInfo> _goalInfos;
    
    private readonly Dictionary<nint, GoalWindow> _openGoalWindows = new();
    private const int MaxGoalWindows = 4;

    public AiWindowViewModel(IAiService aiService, IStateService stateService, IGameTickService gameTickService,
        IPlayerService playerService, IChrInsService chrInsService)
    {
        _aiService = aiService;
        _stateService = stateService;
        _gameTickService = gameTickService;
        _playerService = playerService;
        _chrInsService = chrInsService;

        _goalInfos = DataLoader.LoadGoalInfo();
        _chrNames = DataLoader.GetSimpleDict("ChrNames", int.Parse, s => s);

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
                _chrInsService.SetSelected(previousSelectedChrInsEntry.ChrIns, false);
            }

            if (value != null)
            {
                _chrInsService.SetSelected(_selectedChrInsEntry.ChrIns, true);
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
        var entries = _chrInsService.GetNearbyChrInsEntries();
        var seenHandles = new HashSet<long>();

        foreach (var entry in entries)
        {
            long handle = _chrInsService.GetHandleByChrIns(entry.ChrIns);

            seenHandles.Add(handle);
            if (_entriesByHandle.TryGetValue(handle, out _))
            {
                continue;
            }

            entry.NpcThinkParamId = _aiService.GetNpcThinkParamIdByChrIns(entry.ChrIns);

            if (entry.NpcThinkParamId == 0) continue;
            
            entry.ChrId = _chrInsService.GetChrIdByChrIns(entry.ChrIns);

            entry.Name = _chrNames.TryGetValue(entry.ChrId, out var chrName) ? chrName : "Unknown";

            entry.Handle = handle;
            entry.NpcParamId = _chrInsService.GetNpcParamIdByChrIns(entry.ChrIns);

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
        var targetPosition = _chrInsService.GetChrInsPos(SelectedChrInsEntry.ChrIns);
        _playerService.MoveToPosition(targetPosition);
    }
    
    private void OpenGoalWindow()
    {
        var chrIns = SelectedChrInsEntry.ChrIns;
    
        if (_openGoalWindows.TryGetValue(chrIns, out var existing))
        {
            existing.Activate();
            return;
        }

        if (_openGoalWindows.Count >= MaxGoalWindows)
        {
            MsgBox.Show("Only four Goal windows can be open at once, close one to open another", "Too many Goal windows");
            return;
        }
        
        var window = new GoalWindow();
        var vm = new GoalWindowViewModel(_aiService, _gameTickService, _goalInfos, chrIns);
        window.DataContext = vm;
        window.Closed += (_, _) => _openGoalWindows.Remove(chrIns);
        _openGoalWindows[chrIns] = window;
        window.Show();
    }

    #endregion

    #region Public Methods

    public void NotifyWindowOpen()
    {
        _stateService.Subscribe(State.Loaded, OnGameLoaded);
        _stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        _gameTickService.Subscribe(ChrInsEntriesTick);
    }

    public void NotifyWindowClosed()
    {
        _stateService.Unsubscribe(State.Loaded, OnGameLoaded);
        _stateService.Unsubscribe(State.NotLoaded, OnGameNotLoaded);
        _gameTickService.Unsubscribe(ChrInsEntriesTick);
    }
    
    public void ClearSelected()
    {
        SelectedChrInsEntry = null;
    }

    #endregion

    
}