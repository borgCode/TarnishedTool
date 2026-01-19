using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

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
        ImportLoadoutCommand = new DelegateCommand(ImportLoadout);
        ExportLoadoutCommand = new DelegateCommand(ExportLoadout);
    }

    #region Commands

    public ICommand CreateLoadoutCommand { get; }
    public ICommand RenameLoadoutCommand { get; }
    public ICommand DeleteLoadoutCommand { get; }
    public ICommand AddGraceCommand { get; }
    public ICommand RemoveGraceCommand { get; }
    public ICommand ImportLoadoutCommand { get; }
    public ICommand ExportLoadoutCommand { get; }

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
                OnPropertyChanged(nameof(FilteredCurrentLoadoutGraces));
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
    
    private string _presetGracesSearchText = "";
    public string PresetGracesSearchText
    {
        get => _presetGracesSearchText;
        set
        {
            if (SetProperty(ref _presetGracesSearchText, value))
                OnPropertyChanged(nameof(FilteredCurrentLoadoutGraces));
        }
    }
    
    public IEnumerable<GracePresetEntry> FilteredCurrentLoadoutGraces =>
        string.IsNullOrWhiteSpace(PresetGracesSearchText)
            ? CurrentLoadoutGraces
            : CurrentLoadoutGraces.Where(g => 
                g.Name.IndexOf(PresetGracesSearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                g.MainArea.IndexOf(PresetGracesSearchText, StringComparison.OrdinalIgnoreCase) >= 0);

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
        OnPropertyChanged(nameof(FilteredCurrentLoadoutGraces)); 
    }

    private void RemoveGrace()
    {
        if (SelectedLoadout == null || SelectedLoadoutGrace == null) return;

        SelectedLoadout.Graces.Remove(SelectedLoadoutGrace);
        CurrentLoadoutGraces.Remove(SelectedLoadoutGrace);
        OnPropertyChanged(nameof(FilteredCurrentLoadoutGraces));
    }

    private void ImportLoadout()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Import Grace Presets"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            string json = File.ReadAllText(dialog.FileName);
            var presets = JsonSerializer.Deserialize<List<GracePresetTemplate>>(json);

            if (presets == null || presets.Count == 0)
            {
                MsgBox.Show("No presets found in file.");
                return;
            }

            var selectionWindow = new GraceImportPresentSelectionWindow(presets, _customPresetTemplates);
            if (selectionWindow.ShowDialog() != true) return;

            var selectedPresets = selectionWindow.ViewModel.GetSelectedPresets();
            var conflictResolution = selectionWindow.ViewModel.SelectedConflictResolution;

            int imported = 0;
            int skipped = 0;

            foreach (var preset in selectedPresets)
            {
                if (string.IsNullOrWhiteSpace(preset.Name))
                {
                    skipped++;
                    continue;
                }

                if (_customPresetTemplates.ContainsKey(preset.Name))
                {
                    switch (conflictResolution)
                    {
                        case ConflictResolution.Skip:
                            skipped++;
                            continue;

                        case ConflictResolution.Overwrite:
                            var existing = _customLoadouts.FirstOrDefault(l => l.Name == preset.Name);
                            if (existing != null)
                            {
                                int index = _customLoadouts.IndexOf(existing);
                                _customLoadouts.RemoveAt(index);
                                _customLoadouts.Insert(index, preset);
                            }

                            _customPresetTemplates[preset.Name] = preset;
                            imported++;
                            break;

                        case ConflictResolution.Rename:
                            string newName = GenerateUniqueName(preset.Name);
                            preset.Name = newName;
                            _customPresetTemplates[newName] = preset;
                            _customLoadouts.Add(preset);
                            imported++;
                            break;
                    }
                }
                else
                {
                    _customPresetTemplates[preset.Name] = preset;
                    _customLoadouts.Add(preset);
                    imported++;
                }
            }

            if (imported > 0)
            {
                SelectedLoadout = _customLoadouts.Last();
            }

            string message = $"Imported {imported} preset{(imported != 1 ? "s" : "")}";
            if (skipped > 0)
                message += $" ({skipped} skipped)";

            MsgBox.Show(message);
        }
        catch (Exception ex)
        {
            MsgBox.Show($"Failed to import presets: {ex.Message}");
        }
    }

    private string GenerateUniqueName(string baseName)
    {
        string newName = baseName;
        int counter = 2;

        while (_customPresetTemplates.ContainsKey(newName))
        {
            newName = $"{baseName} ({counter})";
            counter++;
        }

        return newName;
    }

    private void ExportLoadout()
    {
        if (_customLoadouts.Count == 0)
        {
            MsgBox.Show("No presets to export.");
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            Title = "Export Grace Presets",
            FileName = "GracePresets.json"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_customLoadouts.ToList(), options);
            File.WriteAllText(dialog.FileName, json);
            MsgBox.Show($"Exported {_customLoadouts.Count} preset{(_customLoadouts.Count != 1 ? "s" : "")}.");
        }
        catch (Exception ex)
        {
            MsgBox.Show($"Failed to export presets: {ex.Message}");
        }
    }
    
    #endregion

    #region Public Methods

    public void AddGraces(List<Grace> graces)
    {
        if (SelectedLoadout == null) return;

        foreach (var grace in graces)
        {
            if (SelectedLoadout.Graces.Any(g => g.FlagId == grace.FlagId))
                continue;

            var entry = new GracePresetEntry
            {
                IsDlc = grace.IsDlc,
                Name = grace.Name,
                FlagId = grace.FlagId,
                MainArea = grace.MainArea
            };

            SelectedLoadout.Graces.Add(entry);
            CurrentLoadoutGraces.Add(entry);
        }
        
        OnPropertyChanged(nameof(FilteredCurrentLoadoutGraces));
    }

    #endregion
}