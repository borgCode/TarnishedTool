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

namespace TarnishedTool.ViewModels;

public class CreateLoadoutViewModel : BaseViewModel
{
    private readonly Dictionary<string, LoadoutTemplate> _customLoadoutTemplates;
    private readonly Func<string, string, string> _showInputDialog;

    public ItemSelectionViewModel ItemSelection { get; }

    public CreateLoadoutViewModel(
        Dictionary<string, List<Item>> itemsByCategory,
        List<AshOfWar> ashesOfWar,
        Dictionary<string, LoadoutTemplate> customLoadoutTemplates,
        bool hasDlc,
        Func<string, string, string> showInputDialog)
    {
        _customLoadoutTemplates = customLoadoutTemplates;
        _showInputDialog = showInputDialog;

        ItemSelection = new ItemSelectionViewModel(itemsByCategory, ashesOfWar);
        ItemSelection.SetDlcAvailable(hasDlc);

        _customLoadouts = new ObservableCollection<LoadoutTemplate>(customLoadoutTemplates.Values);

        if (_customLoadouts.Count > 0)
        {
            SelectedLoadout = _customLoadouts[0];
        }

        CreateLoadoutCommand = new DelegateCommand(CreateLoadout);
        RenameLoadoutCommand = new DelegateCommand(RenameLoadout);
        DeleteLoadoutCommand = new DelegateCommand(DeleteLoadout);
        AddItemCommand = new DelegateCommand(AddItem);
        RemoveItemCommand = new DelegateCommand(RemoveItem);
        ImportLoadoutCommand = new DelegateCommand(ImportLoadout);
        ExportLoadoutCommand = new DelegateCommand(ExportLoadout);
    }

    #region Commands

    public ICommand CreateLoadoutCommand { get; }
    public ICommand RenameLoadoutCommand { get; }
    public ICommand DeleteLoadoutCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand ImportLoadoutCommand { get; }
    public ICommand ExportLoadoutCommand { get; }

    #endregion

    #region Properties

    private ObservableCollection<LoadoutTemplate> _customLoadouts;

    public ObservableCollection<LoadoutTemplate> CustomLoadouts
    {
        get => _customLoadouts;
        set => SetProperty(ref _customLoadouts, value);
    }

    private LoadoutTemplate _selectedLoadout;

    public LoadoutTemplate SelectedLoadout
    {
        get => _selectedLoadout;
        set
        {
            if (!SetProperty(ref _selectedLoadout, value)) return;

            CurrentLoadoutItems.Clear();
            if (_selectedLoadout?.Items != null)
            {
                foreach (var item in _selectedLoadout.Items)
                {
                    CurrentLoadoutItems.Add(item);
                }
            }
        }
    }

    private ObservableCollection<ItemTemplate> _currentLoadoutItems = new();

    public ObservableCollection<ItemTemplate> CurrentLoadoutItems
    {
        get => _currentLoadoutItems;
        set => SetProperty(ref _currentLoadoutItems, value);
    }

    private ItemTemplate _selectedLoadoutItem;

    public ItemTemplate SelectedLoadoutItem
    {
        get => _selectedLoadoutItem;
        set => SetProperty(ref _selectedLoadoutItem, value);
    }

    #endregion

    #region Private Methods

    private void CreateLoadout()
    {
        string name = _showInputDialog("Enter name for new loadout:", "");

        if (string.IsNullOrWhiteSpace(name)) return;

        if (_customLoadoutTemplates.ContainsKey(name))
        {
            // Could show a message, for now just return
            return;
        }

        var newLoadout = new LoadoutTemplate
        {
            Name = name,
            Items = new List<ItemTemplate>()
        };

        _customLoadoutTemplates[name] = newLoadout;
        _customLoadouts.Add(newLoadout);
        SelectedLoadout = newLoadout;
    }

    private void RenameLoadout()
    {
        if (SelectedLoadout == null) return;

        string newName = _showInputDialog("Enter new name for loadout:", SelectedLoadout.Name);

        if (string.IsNullOrWhiteSpace(newName)) return;
        if (newName == SelectedLoadout.Name) return;
        if (_customLoadoutTemplates.ContainsKey(newName)) return;

        _customLoadoutTemplates.Remove(SelectedLoadout.Name);

        var renamedLoadout = new LoadoutTemplate
        {
            Name = newName,
            Items = SelectedLoadout.Items
        };

        int index = _customLoadouts.IndexOf(SelectedLoadout);
        _customLoadouts.RemoveAt(index);
        _customLoadouts.Insert(index, renamedLoadout);
        _customLoadoutTemplates[newName] = renamedLoadout;

        SelectedLoadout = renamedLoadout;
    }

    private void DeleteLoadout()
    {
        if (SelectedLoadout == null) return;

        _customLoadoutTemplates.Remove(SelectedLoadout.Name);
        _customLoadouts.Remove(SelectedLoadout);
        CurrentLoadoutItems.Clear();
        SelectedLoadout = _customLoadouts.FirstOrDefault();
    }

    private void AddItem()
    {
        if (SelectedLoadout == null || ItemSelection.SelectedItem == null)
        {
            MsgBox.Show("Please select or create a loadout to add an item.");
            return;
        }

        var template = new ItemTemplate
        {
            ItemName = ItemSelection.SelectedItem.Name,
            Quantity = ItemSelection.SelectedQuantity,
            Upgrade = ItemSelection.SelectedUpgrade
        };

        if (ItemSelection.SelectedItem is Weapon && ItemSelection.ShowAowOptions)
        {
            template.AshOfWarName = ItemSelection.SelectedAshOfWar?.Name;
            template.AffinityName = ItemSelection.SelectedAffinity.ToString();
        }

        SelectedLoadout.Items.Add(template);
        CurrentLoadoutItems.Add(template);
    }

    private void RemoveItem()
    {
        if (SelectedLoadout == null || SelectedLoadoutItem == null) return;

        SelectedLoadout.Items.Remove(SelectedLoadoutItem);
        CurrentLoadoutItems.Remove(SelectedLoadoutItem);
    }

    private void ImportLoadout()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Import Loadouts"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            string json = File.ReadAllText(dialog.FileName);
            var loadouts = JsonSerializer.Deserialize<List<LoadoutTemplate>>(json);

            if (loadouts == null || loadouts.Count == 0)
            {
                MsgBox.Show("No loadouts found in file.");
                return;
            }

            int imported = 0;
            int skipped = 0;

            foreach (var loadout in loadouts)
            {
                if (string.IsNullOrWhiteSpace(loadout.Name))
                {
                    skipped++;
                    continue;
                }

                if (_customLoadoutTemplates.ContainsKey(loadout.Name))
                {
                    string newName = GenerateUniqueName(loadout.Name);
                    loadout.Name = newName;
                }

                _customLoadoutTemplates[loadout.Name] = loadout;
                _customLoadouts.Add(loadout);
                imported++;
            }

            if (imported > 0)
            {
                SelectedLoadout = _customLoadouts.Last();
            }

            string message = $"Imported {imported} loadout{(imported != 1 ? "s" : "")}";
            if (skipped > 0)
                message += $" ({skipped} skipped)";

            MsgBox.Show(message);
        }
        catch (Exception ex)
        {
            MsgBox.Show($"Failed to import loadouts: {ex.Message}");
        }
    }

    private void ExportLoadout()
    {
        if (SelectedLoadout == null)
        {
            MsgBox.Show("No loadout selected to export.");
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            Title = "Export Loadout",
            FileName = $"{SelectedLoadout.Name}.json"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(new List<LoadoutTemplate> { SelectedLoadout }, options);
            File.WriteAllText(dialog.FileName, json);
            MsgBox.Show($"Exported loadout: {SelectedLoadout.Name}");
        }
        catch (Exception ex)
        {
            MsgBox.Show($"Failed to export loadout: {ex.Message}");
        }
    }

    private string GenerateUniqueName(string baseName)
    {
        string newName = baseName;
        int counter = 1;

        while (_customLoadoutTemplates.ContainsKey(newName))
        {
            newName = $"{baseName} ({counter})";
            counter++;
        }

        return newName;
    }

    #endregion
}