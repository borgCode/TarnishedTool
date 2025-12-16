// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels;

public class ItemViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly IDlcService _dlcService;
    private readonly IEventService _eventService;

    private readonly Dictionary<string, List<Item>> _itemsByCategory = new();
    private readonly List<Item> _allItems = new();
    private List<AshOfWar> _allAshesOfWar;

    private bool _hasDlc;
    private string _preSearchCategory;
    private bool _isSearchActive;

    public ItemViewModel(IItemService itemService, IDlcService dlcService, IStateService stateService,
        IEventService eventService)
    {
        _itemService = itemService;
        _dlcService = dlcService;
        _eventService = eventService;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

        LoadItems();

        SpawnItemCommand = new DelegateCommand(SpawnItem);
    }
    
    #region Commands
    
    public ICommand SpawnItemCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private ObservableCollection<string> _categories;
    public ObservableCollection<string> Categories => _categories;

    private ObservableCollection<Item> _items = new();

    public ObservableCollection<Item> Items
    {
        get => _items;
        private set => SetProperty(ref _items, value);
    }

    private string _selectedCategory;

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (!SetProperty(ref _selectedCategory, value) || value == null) return;

            if (_isSearchActive)
            {
                _isSearchActive = false;
                _searchText = string.Empty;
                OnPropertyChanged(nameof(SearchText));
                _preSearchCategory = null;
            }

            UpdateItemsList();
        }
    }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!SetProperty(ref _searchText, value)) return;

            if (string.IsNullOrEmpty(value))
            {
                _isSearchActive = false;
                if (_preSearchCategory != null)
                {
                    _selectedCategory = _preSearchCategory;
                    OnPropertyChanged(nameof(SelectedCategory));
                    UpdateItemsList();
                    _preSearchCategory = null;
                }
            }
            else
            {
                if (!_isSearchActive)
                {
                    _preSearchCategory = _selectedCategory;
                    _isSearchActive = true;
                }

                ApplyFilter();
            }
        }
    }

    private Item _selectedItem;

    public Item SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value) || value == null) return;

            MaxQuantity = value.MaxStorage;
            SelectedQuantity = value.StackSize;
            QuantityEnabled = value.StackSize > 1;

            if (value is Weapon weapon)
            {
                ConfigureForWeapon(weapon);
            }
            else
            {
                ShowWeaponOptions = false;
                ShowAowOptions = false;
            }
        }
    }

    private bool _quantityEnabled;

    public bool QuantityEnabled
    {
        get => _quantityEnabled;
        private set => SetProperty(ref _quantityEnabled, value);
    }

    private int _maxQuantity = 1;

    public int MaxQuantity
    {
        get => _maxQuantity;
        private set => SetProperty(ref _maxQuantity, value);
    }

    private int _selectedQuantity = 1;

    public int SelectedQuantity
    {
        get => _selectedQuantity;
        set
        {
            int clampedValue = Math.Max(1, Math.Min(value, MaxQuantity));
            SetProperty(ref _selectedQuantity, clampedValue);
        }
    }

    private bool _showWeaponOptions;

    public bool ShowWeaponOptions
    {
        get => _showWeaponOptions;
        private set => SetProperty(ref _showWeaponOptions, value);
    }

    private int _maxUpgradeLevel = 25;

    public int MaxUpgradeLevel
    {
        get => _maxUpgradeLevel;
        private set => SetProperty(ref _maxUpgradeLevel, value);
    }

    private int _selectedUpgrade;

    public int SelectedUpgrade
    {
        get => _selectedUpgrade;
        set
        {
            int clampedValue = Math.Max(0, Math.Min(value, MaxUpgradeLevel));
            SetProperty(ref _selectedUpgrade, clampedValue);
        } 
    }
    
    private bool _canUpgrade;

    public bool CanUpgrade
    {
        get => _canUpgrade;
        private set => SetProperty(ref _canUpgrade, value);
    }
    

    private bool _showAowOptions;

    public bool ShowAowOptions
    {
        get => _showAowOptions;
        private set => SetProperty(ref _showAowOptions, value);
    }

    private ObservableCollection<AshOfWar> _availableAshesOfWar = new();

    public ObservableCollection<AshOfWar> AvailableAshesOfWar
    {
        get => _availableAshesOfWar;
        private set => SetProperty(ref _availableAshesOfWar, value);
    }

    private AshOfWar _selectedAshOfWar;

    public AshOfWar SelectedAshOfWar
    {
        get => _selectedAshOfWar;
        set
        {
            if (!SetProperty(ref _selectedAshOfWar, value) || value == null) return;

            AvailableAffinities = new ObservableCollection<Affinity>(value.GetAvailableAffinities());
            SelectedAffinity = AvailableAffinities.FirstOrDefault();
        }
    }

    private ObservableCollection<Affinity> _availableAffinities = new();

    public ObservableCollection<Affinity> AvailableAffinities
    {
        get => _availableAffinities;
        private set => SetProperty(ref _availableAffinities, value);
    }

    private Affinity _selectedAffinity;

    public Affinity SelectedAffinity
    {
        get => _selectedAffinity;
        set => SetProperty(ref _selectedAffinity, value);
    }

    #endregion

    #region Private Methods

    
    private void OnGameLoaded()
    {
        _hasDlc = _dlcService.IsDlcAvailable;
        UpdateItemsList();
        AreOptionsEnabled = true;
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void LoadItems()
    {
        _itemsByCategory["Weapons"] = DataLoader.GetWeapons().Cast<Item>().ToList();
        _itemsByCategory["Talismans"] = DataLoader.GetItems("Talismans");
        _itemsByCategory["Crafting Materials"] = DataLoader.GetItems("CraftingMaterials");
        _itemsByCategory["Arrows"] = DataLoader.GetItems("Arrows");
        _itemsByCategory["Consumables"] = DataLoader.GetItems("Consumables");
        _itemsByCategory["Armor"] = DataLoader.GetItems("Armor");
        _itemsByCategory["UpgradeMaterials"] = DataLoader.GetItems("UpgradeMaterials");
        _itemsByCategory["Cookbooks"] = DataLoader.GetEventItems("Cookbooks").Cast<Item>().ToList();
        _itemsByCategory["KeyItems"] = DataLoader.GetEventItems("KeyItems").Cast<Item>().ToList();
     
        
        
        _allItems.AddRange(_itemsByCategory.Values.SelectMany(x => x));
        _allAshesOfWar = DataLoader.GetAshOfWars();
        
        _categories = new ObservableCollection<string>(_itemsByCategory.Keys);
        SelectedCategory = Categories.FirstOrDefault();
    }

    private void UpdateItemsList()
    {
        if (_selectedCategory == null || !_itemsByCategory.ContainsKey(_selectedCategory))
        {
            Items = new ObservableCollection<Item>();
            return;
        }

        var items = _itemsByCategory[_selectedCategory];
        Items = new ObservableCollection<Item>(
            _hasDlc ? items : items.Where(i => !i.IsDlc));
        SelectedItem = Items.FirstOrDefault();
    }

    private void ApplyFilter()
    {
        var searchLower = _searchText.ToLower();
        var filtered = _allItems.Where(i => i.Name.ToLower().Contains(searchLower));

        if (!_hasDlc)
            filtered = filtered.Where(i => !i.IsDlc);

        Items = new ObservableCollection<Item>(filtered);
        SelectedItem = Items.FirstOrDefault();
    }

    private void ConfigureForWeapon(Weapon weapon)
    {
        ShowWeaponOptions = true;
        CanUpgrade = weapon.UpgradeType < 2;
        if (CanUpgrade) MaxUpgradeLevel = weapon.UpgradeType == 0 ? 25 : 10;
        
        if (SelectedUpgrade > MaxUpgradeLevel) SelectedUpgrade = MaxUpgradeLevel;

        if (weapon.CanApplyAow)
        {
            ShowAowOptions = true;
            
            var aows = _allAshesOfWar.Where(aow => aow.SupportsWeaponType(weapon.WeaponType));
            
            AvailableAshesOfWar = new ObservableCollection<AshOfWar>(aows);
            SelectedAshOfWar = AvailableAshesOfWar.FirstOrDefault();
        }
        else
        {
            ShowAowOptions = false;
            SelectedAshOfWar = null;
            AvailableAffinities = new ObservableCollection<Affinity>();
        }
    }
    
    private void SpawnItem()
    {
        if (_selectedItem == null) return;
        
        int itemId = SelectedItem.Id;
        int quantity = SelectedQuantity;
        int aowId = -1;
        int maxQuantity = SelectedItem.MaxStorage + SelectedItem.StackSize;
        bool shouldQuantityAdjust = SelectedItem.StackSize > 1;

        if (SelectedItem is Weapon weapon)
        {
            if (CanUpgrade) itemId += SelectedUpgrade;
        
            if (weapon.CanApplyAow && SelectedAshOfWar != null)
            {
                itemId += SelectedAffinity.GetIdOffset();
                aowId = SelectedAshOfWar.Id;
            }
        }

        if (SelectedItem is EventItem eventItem && eventItem.NeedsEvent)
        {
            _eventService.SetEvent(eventItem.EventId, true);
        }
        
        _itemService.SpawnItem(itemId, quantity, aowId, shouldQuantityAdjust, maxQuantity);
    }


    #endregion
}