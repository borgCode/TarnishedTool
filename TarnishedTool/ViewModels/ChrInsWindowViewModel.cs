// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Data;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels;

internal class ChrInsWindowViewModel : BaseViewModel
{
    private readonly IStateService _stateService;
    private readonly IGameTickService _gameTickService;
    private readonly IPlayerService _playerService;
    private readonly IChrInsService _chrInsService;
    private readonly IAiWindowService _aiWindowService;

    private readonly Dictionary<int, string> _chrNames;

    private readonly Dictionary<long, ChrInsEntry> _entriesByHandle = new();

    public static readonly int[] DummyChrIds = [100, 1000];

    public ChrInsWindowViewModel(IStateService stateService, IGameTickService gameTickService,
        IPlayerService playerService, IChrInsService chrInsService, IAiWindowService aiWindowService)
    {
        _stateService = stateService;
        _gameTickService = gameTickService;
        _playerService = playerService;
        _chrInsService = chrInsService;
        _aiWindowService = aiWindowService;

        _chrNames = DataLoader.GetSimpleDict("ChrNames", int.Parse, s => s);

        _chrInsView = (ListCollectionView)CollectionViewSource.GetDefaultView(ChrInsEntries);
        _chrInsView.Filter = ChrInsFilter;
    }

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

    private readonly ListCollectionView _chrInsView;
    public ICollectionView ChrInsView => _chrInsView;

    private string _chrInsSearchText;

    public string ChrInsSearchText
    {
        get => _chrInsSearchText;
        set
        {
            if (SetProperty(ref _chrInsSearchText, value))
                _chrInsView.Refresh();
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
        _aiWindowService.CloseAllAiWindows();
    }

    private void ChrInsEntriesTick()
    {
        var entries = _chrInsService.GetNearbyChrInsEntries();
        var seenHandles = new HashSet<long>();
        var playerBlockId = _playerService.GetBlockId();
        byte playerArea = (byte)((playerBlockId >> 24) & 0xFF);
        var playerAbsolute = PositionUtils.ToAbsolute(_playerService.GetPlayerPos(), playerBlockId);

        foreach (var entry in entries)
        {
            long handle = _chrInsService.GetHandleByChrIns(entry.ChrIns);


            seenHandles.Add(handle);
            if (_entriesByHandle.TryGetValue(handle, out var existingEntry))
            {
                if (existingEntry.NpcThinkParamId == 0)
                    existingEntry.NpcThinkParamId = _chrInsService.GetNpcThinkParamId(entry.ChrIns);
                if (existingEntry.EntityId == 0)
                    existingEntry.EntityId = _chrInsService.GetEntityId(entry.ChrIns);

                if (existingEntry.IsExpanded)
                {
                    var entryBlockId = _chrInsService.GetBlockId(existingEntry.ChrIns);
                    byte entryArea = (byte)((entryBlockId >> 24) & 0xFF);

                    if (playerArea != entryArea)
                    {
                        existingEntry.Distance = -1f;
                    }
                    else
                    {
                        var entryAbsolute =
                            PositionUtils.ToAbsolute(_chrInsService.GetLocalCoords(existingEntry.ChrIns), entryBlockId);
                        existingEntry.Distance = Vector3.Distance(playerAbsolute, entryAbsolute);
                    }

                    existingEntry.CurrentHp = _chrInsService.GetCurrentHp(existingEntry.ChrIns);
                    existingEntry.MaxHp = _chrInsService.GetMaxHp(existingEntry.ChrIns);
                }

                continue;
            }

            entry.ChrId = _chrInsService.GetChrId(entry.ChrIns);
            if (DummyChrIds.Contains(entry.ChrId)) continue;

            var instanceId = _chrInsService.GetChrInstanceId(entry.ChrIns);
            entry.InternalName = $@"c{entry.ChrId}_{instanceId}";
            entry.OnOptionChanged = HandleEntryOptionChanged;
            entry.OnCommandExecuted = HandleEntryCommand;
            entry.OnExpanded = HandleEntryExpanded;
            entry.NpcThinkParamId = _chrInsService.GetNpcThinkParamId(entry.ChrIns);
            entry.Name = _chrNames.TryGetValue(entry.ChrId, out var chrName) ? chrName : "Unknown";
            entry.EntityId = _chrInsService.GetEntityId(entry.ChrIns);
            entry.Handle = handle;
            entry.NpcParamId = _chrInsService.GetNpcParamId(entry.ChrIns);
            entry.CurrentHp = _chrInsService.GetCurrentHp(entry.ChrIns);
            entry.MaxHp = _chrInsService.GetMaxHp(entry.ChrIns);

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

    private void HandleEntryOptionChanged(ChrInsEntry entry, string propertyName, bool value)
    {
        switch (propertyName)
        {
            case nameof(ChrInsEntry.IsAiDisabled):
                _chrInsService.ToggleTargetAi(entry.ChrIns, value);
                break;
            case nameof(ChrInsEntry.IsTargetViewEnabled):
                _chrInsService.ToggleTargetView(entry.ChrIns, value);
                break;
            case nameof(ChrInsEntry.IsNoAttackEnabled):
                _chrInsService.ToggleNoAttack(entry.ChrIns, value);
                break;
            case nameof(ChrInsEntry.IsNoMoveEnabled):
                _chrInsService.ToggleNoMove(entry.ChrIns, value);
                break;
            case nameof(ChrInsEntry.IsNoDamageEnabled):
                _chrInsService.ToggleNoDamage(entry.ChrIns, value);
                break;
        }
    }

    private void HandleEntryCommand(ChrInsEntry entry, string commandName)
    {
        switch (commandName)
        {
            case nameof(ChrInsEntry.WarpCommand):
                var targetPosition = _chrInsService.GetChrInsMapCoords(entry.ChrIns);
                _playerService.MoveToPosition(targetPosition);
                break;
            case nameof(ChrInsEntry.OpenAiWindowCommand):
                _aiWindowService.OpenAiWindow(entry);
                break;
            case nameof(ChrInsEntry.KillChrCommand):
                _chrInsService.SetHp(entry.ChrIns, 0);
                break;
            case nameof(ChrInsEntry.SetHpCommand):
                HandleSetHp(entry);
                break;
        }
    }

    private void HandleEntryExpanded(ChrInsEntry entry)
    {
        entry.IsAiDisabled = _chrInsService.IsAiDisabled(entry.ChrIns);
        entry.IsTargetViewEnabled = _chrInsService.IsTargetViewEnabled(entry.ChrIns);
        entry.IsNoAttackEnabled = _chrInsService.IsNoAttackEnabled(entry.ChrIns);
        entry.IsNoMoveEnabled = _chrInsService.IsNoMoveEnabled(entry.ChrIns);
        entry.IsNoDamageEnabled = _chrInsService.IsNoDamageEnabled(entry.ChrIns);
    }

    private void HandleSetHp(ChrInsEntry entry)
    {
        var input = MsgBox.ShowInput(
            "Enter HP (number or percentage, e.g. 5000 or 50%)",
            "",
            "Set Hp");

        if (string.IsNullOrEmpty(input))
            return;
        input = input.Trim();

        int targetHp;

        if (input.EndsWith("%"))
        {
            if (double.TryParse(input.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
            {
                targetHp = (int)(percent / 100.0 * entry.MaxHp);
            }
            else
            {
                MsgBox.Show("Invalid percentage format", "Error");
                return;
            }
        }
        else if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var absolute))
        {
            targetHp = absolute;
        }
        else
        {
            MsgBox.Show("Enter a number or percentage (e.g., 5000 or 50%)", "Error");
            return;
        }

        targetHp = Math.Min(targetHp, entry.MaxHp);
        _chrInsService.SetHp(entry.ChrIns, targetHp);
    }


    private bool ChrInsFilter(object obj)
    {
        if (obj is not ChrInsEntry e) return false;
        if (string.IsNullOrWhiteSpace(ChrInsSearchText)) return true;

        var s = ChrInsSearchText.ToLowerInvariant();
        return (e.Name?.ToLowerInvariant().Contains(s) ?? false)
               || (e.InternalName?.ToLowerInvariant().Contains(s) ?? false)
               || e.ChrId.ToString().Contains(s);
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

        _aiWindowService.CloseAllAiWindows();

        SelectedChrInsEntry = null;
        _entriesByHandle.Clear();
        ChrInsEntries.Clear();
    }

    #endregion
}