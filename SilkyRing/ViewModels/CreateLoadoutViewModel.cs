using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Models;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels;

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
    }

    #region Commands

    public ICommand CreateLoadoutCommand { get; }
    public ICommand RenameLoadoutCommand { get; }
    public ICommand DeleteLoadoutCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }

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

    #endregion
}