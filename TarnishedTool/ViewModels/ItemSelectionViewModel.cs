using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TarnishedTool.Enums;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class ItemSelectionViewModel : BaseViewModel
{
    private readonly Dictionary<string, List<Item>> _itemsByCategory;
    private readonly List<Item> _allItems;
    private readonly List<AshOfWar> _allAshesOfWar;
    
    private static readonly CompareInfo _compareInfo = CultureInfo.InvariantCulture.CompareInfo;

    private bool _hasDlc;
    private string _preSearchCategory;
    private bool _isSearchActive;
    public bool IsSearchActive
    {
        get => _isSearchActive;
        private set => SetProperty(ref _isSearchActive, value);
    }

    public ItemSelectionViewModel(
        Dictionary<string, List<Item>> itemsByCategory,
        List<AshOfWar> ashesOfWar)
    {
        _itemsByCategory = itemsByCategory;
        _allItems = itemsByCategory.Values.SelectMany(x => x).ToList();
        _allAshesOfWar = ashesOfWar;

        _categories = new ObservableCollection<string>(itemsByCategory.Keys);
        SelectedCategory = _categories.FirstOrDefault();
    }

    public void SetDlcAvailable(bool hasDlc)
    {
        _hasDlc = hasDlc;
        UpdateItemsList();
    }

    #region Properties

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
                IsSearchActive = false;
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
                IsSearchActive = false;
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
                    IsSearchActive = true;
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

            MaxQuantity = value.MaxStorage > 0 
                ? value.MaxStorage + value.StackSize 
                : value.StackSize;
            SelectedQuantity = value.StackSize;
            QuantityEnabled = value.StackSize > 1;
            
            if (SelectedCategory == "Incantations" || SelectedCategory == "Sorceries")
            {
                SelectedQuantity = 1;
            }

            if (value is Weapon weapon)
            {
                ConfigureForWeapon(weapon);
            }
            else if (value is SpiritAsh spiritAsh)
            {
                ShowWeaponOptions = false;
                ShowAowOptions = false;
                ShowSpiritAshUpgradeOptions = spiritAsh.CanUpgrade;
                SelectedSpiritAshUpgrade = 0;
            }
            else
            {
                ShowWeaponOptions = false;
                ShowAowOptions = false;
                ShowSpiritAshUpgradeOptions = false;
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


    private bool _showSpiritAshUpgradeOptions;

    public bool ShowSpiritAshUpgradeOptions
    {
        get => _showSpiritAshUpgradeOptions;
        private set => SetProperty(ref _showSpiritAshUpgradeOptions, value);
    }
    
    private int _selectedSpiritAshUpgrade;

    public int SelectedSpiritAshUpgrade
    {
        get => _selectedSpiritAshUpgrade;
        set
        {
            int clampedValue = Math.Max(0, Math.Min(value, 10));
            SetProperty(ref _selectedSpiritAshUpgrade, clampedValue);
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
    
    private bool _showAffinityOptions;

    public bool ShowAffinityOptions
    {
        get => _showAffinityOptions;
        private set => SetProperty(ref _showAffinityOptions, value);
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

            if (value == AshOfWar.None)
            {
                ShowAffinityOptions = false;
                AvailableAffinities = new ObservableCollection<Affinity>();
                _selectedAffinity = 0;
            }
            else
            {
                ShowAffinityOptions = true;
                var affinities = value.GetAvailableAffinities().ToList();
                AvailableAffinities = new ObservableCollection<Affinity>(affinities);
                _selectedAffinity = affinities.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedAffinity));
            }
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
        var filtered = _allItems.Where(i => 
            _compareInfo.IndexOf(i.Name, _searchText, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
        
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

            AvailableAshesOfWar = new ObservableCollection<AshOfWar>(aows.Prepend(AshOfWar.None));
            SelectedAshOfWar = AvailableAshesOfWar.FirstOrDefault();
        }
        else
        {
            ShowAowOptions = false;
            ShowAffinityOptions = false;
            SelectedAshOfWar = null;
            AvailableAffinities = new ObservableCollection<Affinity>();
        }
    }

    #endregion
}