// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels;

public class ItemViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly IDlcService _dlcService;
    private readonly IEventService _eventService;

    private readonly Dictionary<string, List<Item>> _itemsByCategory;
    private readonly List<AshOfWar> _allAshesOfWar;

    private readonly List<Item> _allItems;

    private readonly Dictionary<string, Type> enumsType;

    private readonly Dictionary<string, LoadoutTemplate> _customLoadoutTemplates;

    public ItemSelectionViewModel ItemSelection { get; }

    public ItemViewModel(IItemService itemService, IDlcService dlcService, IStateService stateService,
        IEventService eventService)
    {
        _itemService = itemService;
        _dlcService = dlcService;
        _eventService = eventService;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);
        stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);

        _itemsByCategory = LoadItemData();
        _allAshesOfWar = DataLoader.GetAshOfWars();

        _customLoadoutTemplates = DataLoader.LoadCustomLoadouts();
        _loadouts = new ObservableCollection<string>(_customLoadoutTemplates.Keys);
        SelectedLoadoutName = _loadouts.FirstOrDefault();

        _allItems = _itemsByCategory.Values.SelectMany(x => x).ToList();

        ItemSelection = new ItemSelectionViewModel(_itemsByCategory, _allAshesOfWar);

        ItemSelection.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ItemSelection.SelectedCategory))
            {
                SelectedMassSpawnCategory = ItemSelection.SelectedCategory;
            }
        };
        SelectedMassSpawnCategory = ItemSelection.SelectedCategory;
        _weaponList = _itemsByCategory["Weapons"];
        SelectedAutoSpawnWeapon = _weaponList.FirstOrDefault();

        SpawnItemCommand = new DelegateCommand(SpawnItem);
        MassSpawnCommand = new DelegateCommand(MassSpawn);
        OpenCreateLoadoutCommand = new DelegateCommand(OpenCreateLoadoutWindow);
        SpawnLoadoutCommand = new DelegateCommand(SpawnLoadout);
    }

    #region Commands

    public ICommand SpawnItemCommand { get; set; }
    public ICommand MassSpawnCommand { get; set; }
    public ICommand OpenCreateLoadoutCommand { get; set; }
    public ICommand SpawnLoadoutCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private string _selectedMassSpawnCategory;

    public string SelectedMassSpawnCategory
    {
        get => _selectedMassSpawnCategory;
        set => SetProperty(ref _selectedMassSpawnCategory, value);
    }

    private ObservableCollection<string> _loadouts;

    public ObservableCollection<string> Loadouts
    {
        get => _loadouts;
        private set => SetProperty(ref _loadouts, value);
    }

    private string _selectedLoadoutName;

    public string SelectedLoadoutName
    {
        get => _selectedLoadoutName;
        set => SetProperty(ref _selectedLoadoutName, value);
    }

    private bool _autoSpawnEnabled;

    public bool AutoSpawnEnabled
    {
        get => _autoSpawnEnabled;
        set => SetProperty(ref _autoSpawnEnabled, value);
    }

    private Item _selectedAutoSpawnWeapon;

    public Item SelectedAutoSpawnWeapon
    {
        get => _selectedAutoSpawnWeapon;
        set => SetProperty(ref _selectedAutoSpawnWeapon, value);
    }

    private bool _autoLoadoutSpawnEnabled;

    public bool AutoLoadoutSpawnEnabled
    {
        get => _autoLoadoutSpawnEnabled;
        set => SetProperty(ref _autoLoadoutSpawnEnabled, value);
    }

    private List<Item> _weaponList;

    public List<Item> WeaponList
    {
        get => _weaponList;
        private set => SetProperty(ref _weaponList, value);
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void OnGameFirstLoaded()
    {
        var hasDlc = _dlcService.IsDlcAvailable;
        ItemSelection.SetDlcAvailable(hasDlc);
    }

    private void OnNewGameStart()
    {
        if (AutoSpawnEnabled && SelectedAutoSpawnWeapon != null)
        {
            _itemService.SpawnItem(SelectedAutoSpawnWeapon.Id, 1, -1, false, 1);
        }

        if (!AutoLoadoutSpawnEnabled || string.IsNullOrEmpty(SelectedLoadoutName)) return;
        SpawnLoadout();
    }

    private Dictionary<string, List<Item>> LoadItemData()
    {
        var items = new Dictionary<string, List<Item>>
        {
            ["Armor"] = DataLoader.GetItems("Armor", "Armor"),
            ["Arrows"] = DataLoader.GetItems("Arrows", "Arrows"),
            ["Ash of War"] = DataLoader.GetItems("AshOfWarSpawn", "Ash of War"),
            ["Bell Bearings"] = DataLoader.GetItems("BellBearings", "Bell Bearings"),
            ["Consumables"] = DataLoader.GetItems("Consumables", "Consumables"),
            ["Cookbooks"] = DataLoader.GetEventItems("Cookbooks", "Cookbooks").Cast<Item>().ToList(),
            ["Crafting Materials"] = DataLoader.GetItems("CraftingMaterials", "Crafting Materials"),
            ["Crystal Tears"] = DataLoader.GetItems("CrystalTears", "Crystal Tears"),
            ["Incantations"] = DataLoader.GetItems("Incantations", "Incantations"),
            ["Key Items"] = DataLoader.GetEventItems("KeyItems", "Key Items").Cast<Item>().ToList(),
            ["Pots and Perfumes"] = DataLoader.GetItems("PotsAndPerfumes", "Pots and Perfumes"),
            ["Prattling Pate"] = DataLoader.GetItems("PrattlingPate", "Prattling Pate"),
            ["Sorceries"] = DataLoader.GetItems("Sorceries", "Sorceries"),
            ["Spirit Ashes"] = DataLoader.GetItems("SpiritAshes", "Spirit Ashes"),
            ["Talismans"] = DataLoader.GetItems("Talismans", "Talismans"),
            ["Upgrade Materials"] = DataLoader.GetItems("UpgradeMaterials", "Upgrade Materials"),
            ["Weapons"] = DataLoader.GetWeapons().Cast<Item>().ToList()
        };

        return items;
    }

    private void SpawnItem()
    {
        if (ItemSelection.SelectedItem == null) return;

        int itemId = ItemSelection.SelectedItem.Id;
        int quantity = ItemSelection.SelectedQuantity;
        int aowId = -1;
        int maxQuantity = ItemSelection.SelectedItem.MaxStorage + ItemSelection.SelectedItem.StackSize;
        bool shouldQuantityAdjust = ItemSelection.SelectedItem.StackSize > 1;

        if (ItemSelection.SelectedItem is Weapon weapon)
        {
            if (ItemSelection.CanUpgrade) itemId += ItemSelection.SelectedUpgrade;

            if (weapon.CanApplyAow && ItemSelection.SelectedAshOfWar != null
                                   && ItemSelection.SelectedAshOfWar != AshOfWar.None)
            {
                itemId += ItemSelection.SelectedAffinity.GetIdOffset();
                aowId = ItemSelection.SelectedAshOfWar.Id;
            }
        }

        if (ItemSelection.SelectedItem is EventItem eventItem && eventItem.NeedsEvent)
        {
            _eventService.SetEvent(eventItem.EventId, true);
        }

        _itemService.SpawnItem(itemId, quantity, aowId, shouldQuantityAdjust, maxQuantity);
    }

    private async void MassSpawn()
    {
        if (string.IsNullOrEmpty(SelectedMassSpawnCategory) ||
            !_itemsByCategory.ContainsKey(SelectedMassSpawnCategory))
            return;

        
        var items = _itemsByCategory[SelectedMassSpawnCategory];
        var hasDlc = _dlcService.IsDlcAvailable;

        if (!hasDlc) items = items.Where(i => !i.IsDlc).ToList();

        bool needsDelay = SelectedMassSpawnCategory is "Cookbooks" or "Crystal Tears";

        await Task.Run(async () =>
        {
            foreach (var item in items)
            {
                if (needsDelay) await Task.Delay(50);

                if (item is EventItem eventItem && eventItem.NeedsEvent)
                {
                    _eventService.SetEvent(eventItem.EventId, true);
                }

                int itemId = item.Id;
                int quantity = item.StackSize;
                int aowId = -1;
                int maxQuantity = item.MaxStorage + item.StackSize;
                bool shouldQuantityAdjust = item.StackSize > 1;

                _itemService.SpawnItem(itemId, quantity, aowId, shouldQuantityAdjust, maxQuantity);
            }
        });
    }

    private void OpenCreateLoadoutWindow()
    {
        var window = new CreateLoadoutWindow(
            _itemsByCategory,
            _allAshesOfWar,
            _customLoadoutTemplates,
            _dlcService.IsDlcAvailable);

        if (window.ShowDialog() == true)
            RefreshLoadouts();
    }

    private void RefreshLoadouts()
    {
        _loadouts.Clear();
        foreach (var name in _customLoadoutTemplates.Keys)
        {
            _loadouts.Add(name);
        }

        if (string.IsNullOrEmpty(SelectedLoadoutName) || !_customLoadoutTemplates.ContainsKey(SelectedLoadoutName))
        {
            SelectedLoadoutName = _loadouts.FirstOrDefault();
        }

        DataLoader.SaveCustomLoadouts(_customLoadoutTemplates);
    }

    public void SpawnLoadout()
    {
        if (string.IsNullOrEmpty(SelectedLoadoutName) || !_customLoadoutTemplates.ContainsKey(SelectedLoadoutName))
            return;

        var loadout = _customLoadoutTemplates[SelectedLoadoutName];
        foreach (var template in loadout.Items)
        {
            var item = _allItems.FirstOrDefault(i => i.Name == template.ItemName);
            if (item == null) continue;

            int itemId = item.Id;
            int quantity = template.Quantity > 0 ? template.Quantity : item.StackSize;
            int aowId = -1;
            int maxQuantity = item.MaxStorage + item.StackSize;
            bool shouldQuantityAdjust = item.StackSize > 1;

            if (item is Weapon weapon)
            {
                if (weapon.UpgradeType < 2)
                {
                    itemId += template.Upgrade;
                }

                if (weapon.CanApplyAow && !string.IsNullOrEmpty(template.AshOfWarName)
                                       && template.AshOfWarName != "None")
                {
                    var aow = _allAshesOfWar.FirstOrDefault(a => a.Name == template.AshOfWarName);
                    if (aow != null)
                    {
                        aowId = aow.Id;

                        if (!string.IsNullOrEmpty(template.AffinityName) &&
                            Enum.TryParse<Affinity>(template.AffinityName, out var affinity))
                        {
                            itemId += affinity.GetIdOffset();
                        }
                    }
                }
            }

            if (item is EventItem eventItem && eventItem.NeedsEvent)
            {
                _eventService.SetEvent(eventItem.EventId, true);
            }

            _itemService.SpawnItem(itemId, quantity, aowId, shouldQuantityAdjust, maxQuantity);
        }
    }

    #endregion
}