using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Models;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels;

public class GracePresetViewModel : BaseViewModel
{
    private readonly Dictionary<string, GracePresetTemplate> _customPresetTemplates;
    private readonly Func<string, string, string> _showInputDialog;

    public SearchableGroupedCollection<string, Grace> Graces { get; }

    public GracePresetViewModel(
        SearchableGroupedCollection<string, Grace> graces,
        Dictionary<string, GracePresetTemplate> customPresetTemplates,
        Func<string, string, string> showInputDialog)
    {
        Graces = graces;
        _customPresetTemplates = customPresetTemplates;
        _showInputDialog = showInputDialog;

        _customLoadouts = new ObservableCollection<GracePresetTemplate>(customPresetTemplates.Values);

        if (_customLoadouts.Count > 0)
        {
            SelectedLoadout = _customLoadouts[0];
        }

        CreateLoadoutCommand = new DelegateCommand(CreateLoadout);
        RenameLoadoutCommand = new DelegateCommand(RenameLoadout);
        DeleteLoadoutCommand = new DelegateCommand(DeleteLoadout);
        AddGraceCommand = new DelegateCommand(AddGrace);
        RemoveGraceCommand = new DelegateCommand(RemoveGrace);
    }

    #region Commands

    public ICommand CreateLoadoutCommand { get; }
    public ICommand RenameLoadoutCommand { get; }
    public ICommand DeleteLoadoutCommand { get; }
    public ICommand AddGraceCommand { get; }
    public ICommand RemoveGraceCommand { get; }

    #endregion

    #region Properties

    private ObservableCollection<GracePresetTemplate> _customLoadouts;

    public ObservableCollection<GracePresetTemplate> CustomLoadouts
    {
        get => _customLoadouts;
        set => SetProperty(ref _customLoadouts, value);
    }

    private GracePresetTemplate _selectedLoadout;

    public GracePresetTemplate SelectedLoadout
    {
        get => _selectedLoadout;
        set
        {
            if (!SetProperty(ref _selectedLoadout, value)) return;

            CurrentLoadoutGraces.Clear();
            if (_selectedLoadout?.Graces != null)
            {
                foreach (var grace in _selectedLoadout.Graces)
                {
                    CurrentLoadoutGraces.Add(grace);
                }
            }
        }
    }

    private ObservableCollection<GracePresetEntry> _currentLoadoutGraces = new();

    public ObservableCollection<GracePresetEntry> CurrentLoadoutGraces
    {
        get => _currentLoadoutGraces;
        set => SetProperty(ref _currentLoadoutGraces, value);
    }

    private GracePresetEntry _selectedLoadoutGrace;

    public GracePresetEntry SelectedLoadoutGrace
    {
        get => _selectedLoadoutGrace;
        set => SetProperty(ref _selectedLoadoutGrace, value);
    }

    #endregion

    #region Private Methods

    private void CreateLoadout()
    {
        string name = _showInputDialog("Enter name for new preset:", "");

        if (string.IsNullOrWhiteSpace(name)) return;

        if (_customPresetTemplates.ContainsKey(name)) return;

        var newLoadout = new GracePresetTemplate
        {
            Name = name,
            Graces = new List<GracePresetEntry>()
        };

        _customPresetTemplates[name] = newLoadout;
        _customLoadouts.Add(newLoadout);
        SelectedLoadout = newLoadout;
    }

    private void RenameLoadout()
    {
        if (SelectedLoadout == null) return;

        string newName = _showInputDialog("Enter new name for preset:", SelectedLoadout.Name);

        if (string.IsNullOrWhiteSpace(newName)) return;
        if (newName == SelectedLoadout.Name) return;
        if (_customPresetTemplates.ContainsKey(newName)) return;

        _customPresetTemplates.Remove(SelectedLoadout.Name);

        var renamedLoadout = new GracePresetTemplate
        {
            Name = newName,
            Graces = SelectedLoadout.Graces
        };

        int index = _customLoadouts.IndexOf(SelectedLoadout);
        _customLoadouts.RemoveAt(index);
        _customLoadouts.Insert(index, renamedLoadout);
        _customPresetTemplates[newName] = renamedLoadout;

        SelectedLoadout = renamedLoadout;
    }

    private void DeleteLoadout()
    {
        if (SelectedLoadout == null) return;

        _customPresetTemplates.Remove(SelectedLoadout.Name);
        _customLoadouts.Remove(SelectedLoadout);
        CurrentLoadoutGraces.Clear();
        SelectedLoadout = _customLoadouts.FirstOrDefault();
    }

    private void AddGrace()
    {
        if (SelectedLoadout == null || Graces.SelectedItem == null)
        {
            MsgBox.Show("Please select or create a preset and select a grace to add.");
            return;
        }


        if (SelectedLoadout.Graces.Any(g => g.FlagId == Graces.SelectedItem.FlagId))
        {
            return;
        }

        var entry = new GracePresetEntry
        {
            IsDlc = Graces.SelectedItem.IsDlc,
            Name = Graces.SelectedItem.Name,
            FlagId = Graces.SelectedItem.FlagId,
            MainArea = Graces.SelectedItem.MainArea
        };

        SelectedLoadout.Graces.Add(entry);
        CurrentLoadoutGraces.Add(entry);
    }

    private void RemoveGrace()
    {
        if (SelectedLoadout == null || SelectedLoadoutGrace == null) return;

        SelectedLoadout.Graces.Remove(SelectedLoadoutGrace);
        CurrentLoadoutGraces.Remove(SelectedLoadoutGrace);
    }

    #endregion
}