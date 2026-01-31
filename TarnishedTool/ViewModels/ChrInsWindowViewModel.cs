// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels;

internal class ChrInsWindowViewModel : BaseViewModel
{
    private readonly IAiService _aiService;
    private readonly IStateService _stateService;
    private readonly IGameTickService _gameTickService;
    private readonly IPlayerService _playerService;
    private readonly IChrInsService _chrInsService;

    private readonly Dictionary<int, string> _chrNames;
    private readonly Dictionary<int, string> _aiTargetEnums;
    private readonly Dictionary<int, string> _aiInterruptEnums;
    private readonly Dictionary<int, GoalInfo> _goalInfos;

    private readonly Dictionary<long, ChrInsEntry> _entriesByHandle = new();

    private readonly Dictionary<nint, AiWindow> _openAiWindows = new();
    private const int MaxAiWindows = 4;

    public ChrInsWindowViewModel(IAiService aiService, IStateService stateService, IGameTickService gameTickService,
        IPlayerService playerService, IChrInsService chrInsService)
    {
        _aiService = aiService;
        _stateService = stateService;
        _gameTickService = gameTickService;
        _playerService = playerService;
        _chrInsService = chrInsService;

        _goalInfos = DataLoader.LoadGoalInfo();
        _chrNames = DataLoader.GetSimpleDict("ChrNames", int.Parse, s => s);
        _aiTargetEnums = DataLoader.GetSimpleDict("AiTargetEnum", int.Parse, s => s);
        _aiInterruptEnums = DataLoader.GetSimpleDict("AiInterruptEnum", int.Parse, s => s);
    }

    #region Commands

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
            if (_selectedChrInsEntry == value) return;

            var previous = _selectedChrInsEntry;
            SetProperty(ref _selectedChrInsEntry, value);

            if (previous != null)
                _chrInsService.SetSelected(previous.ChrIns, false);

            if (value != null)
                _chrInsService.SetSelected(value.ChrIns, true);
        }
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        _gameTickService.Subscribe(ChrInsEntriesTick);
        var entries = _chrInsService.GetNearbyChrInsEntries()
            .Where(e => _aiService.GetNpcThinkParamIdByChrIns(e.ChrIns) != 0)
            .ToList();

        foreach (var entry in entries)
        {
            var position = _chrInsService.GetChrInsMapCoords(entry.ChrIns);
            Console.WriteLine(position.Coords);

            entry.NpcThinkParamId = _aiService.GetNpcThinkParamIdByChrIns(entry.ChrIns);


            entry.ChrId = _chrInsService.GetChrIdByChrIns(entry.ChrIns);

            entry.Name = _chrNames.TryGetValue(entry.ChrId, out var chrName) ? chrName : "Unknown";
        }
    }

    private void OnGameNotLoaded()
    {
        _gameTickService.Unsubscribe(ChrInsEntriesTick);
        foreach (var window in _openAiWindows.Values.ToList())
        {
            window.Close();
        }
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

            entry.OnOptionChanged = HandleEntryOptionChanged;
            entry.OnCommandExecuted = HandleEntryCommand;
            entry.OnExpanded = HandleEntryExpanded;
            entry.NpcThinkParamId = _aiService.GetNpcThinkParamIdByChrIns(entry.ChrIns);

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

    private void OpenAiWindow(ChrInsEntry entry)
    {
        if (entry == null) return;
        var chrIns = entry.ChrIns;

        if (_openAiWindows.TryGetValue(chrIns, out var existing))
        {
            existing.Activate();
            return;
        }

        if (_openAiWindows.Count >= MaxAiWindows)
        {
            MsgBox.Show("Only four AI windows can be open at once, close one to open another", "Too many AI windows");
            return;
        }

        var window = new AiWindow();
        var vm = new AiWindowViewModel(_aiService, _gameTickService, _goalInfos, chrIns, _aiTargetEnums,
            _aiInterruptEnums, _aiService.GetAiThinkPtr(chrIns));
        window.DataContext = vm;
        window.Closed += (_, _) => _openAiWindows.Remove(chrIns);
        _openAiWindows[chrIns] = window;
        window.Show();
    }

    private void HandleEntryOptionChanged(ChrInsEntry entry, string propertyName, bool value)
    {
        switch (propertyName)
        {
            case nameof(ChrInsEntry.IsAiDisabled):
                _chrInsService.ToggleTargetAi(entry.ChrIns, value);
                break;
            case nameof(ChrInsEntry.IsTargetViewEnabled):
                //TODO
                break;
        }
    }

    private void HandleEntryCommand(ChrInsEntry entry, string commandName)
    {
        switch (commandName)
        {
            case nameof(ChrInsEntry.WarpCommand):
                var targetPosition = _chrInsService.GetChrInsMapCoords(entry.ChrIns);

                //TODO offset a bit to not end up inside big enemies
                _playerService.MoveToPosition(targetPosition);
                break;
            case nameof(ChrInsEntry.OpenAiWindowCommand):
                OpenAiWindow(entry);
                break;
        }
    }

    private void HandleEntryExpanded(ChrInsEntry entry)
    {
        entry.IsAiDisabled = _chrInsService.IsAiDisabled(entry.ChrIns);
        // entry.IsTargetViewEnabled = _chrInsService.IsTargetViewEnabled(entry.ChrIns);
        // entry.IsAiDisabled = _chrInsService.IsAiDisabled(entry.ChrIns);
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