// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TarnishedTool.ViewModels;

public class SearchableGroupedCollection<TGroup, TItem> : BaseViewModel
{
    private readonly Func<TItem, string, bool> _matchesSearch;
    private TGroup _preSearchGroup;
    private readonly Dictionary<TGroup, List<TItem>> _groupedItems;
    
    
    public SearchableGroupedCollection(
        Dictionary<TGroup, List<TItem>> data,
        Func<TItem, string, bool> matchesSearch)
    {
        _matchesSearch = matchesSearch;
        _groupedItems = data;
        _allItems = data.Values.SelectMany(x => x).ToList();
        _groups = new ObservableCollection<TGroup>(data.Keys);
        _items = new ObservableCollection<TItem>();
        
        SelectedGroup = _groups.FirstOrDefault();
    }

    private ObservableCollection<TGroup> _groups;
    public ObservableCollection<TGroup> Groups => _groups;
    private ObservableCollection<TItem> _items;
    public List<TItem> this[TGroup group] => _groupedItems[group];

    public ObservableCollection<TItem> Items
    {
        get => _items;
        private set => SetProperty(ref _items, value);
    }

    private List<TItem> _allItems;
    public IReadOnlyList<TItem> AllItems => _allItems;

    private TGroup _selectedGroup;

    public TGroup SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (!SetProperty(ref _selectedGroup, value) || value == null) return;

            if (_isSearchActive)
            {
                _isSearchActive = false;
                _searchText = string.Empty;
                OnPropertyChanged(nameof(SearchText));
                _preSearchGroup = default;
            }

            UpdateItemsList();
        }
    }

    private TItem _selectedItem;

    public TItem SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    private bool _isSearchActive;

    public bool IsSearchActive
    {
        get => _isSearchActive;
        private set => SetProperty(ref _isSearchActive, value);
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
                if (_preSearchGroup != null)
                {
                    _selectedGroup = _preSearchGroup;
                    OnPropertyChanged(nameof(SelectedGroup));
                    UpdateItemsList();
                    _preSearchGroup = default;
                }
            }
            else
            {
                if (!_isSearchActive)
                {
                    _preSearchGroup = SelectedGroup;
                    IsSearchActive = true;
                }

                ApplyFilter();
            }
        }
    }

    private void UpdateItemsList()
    {
        if (SelectedGroup == null || !_groupedItems.ContainsKey(SelectedGroup))
        {
            Items = new ObservableCollection<TItem>();
            return;
        }

        Items = new ObservableCollection<TItem>(_groupedItems[SelectedGroup]);
        SelectedItem = Items.FirstOrDefault();
    }

    private void ApplyFilter()
    {
        var searchLower = SearchText.ToLower();
        Items = new ObservableCollection<TItem>(
            _allItems.Where(item => _matchesSearch(item, searchLower)));
        SelectedItem = Items.FirstOrDefault();
    }
    
    public void Initialize()
    {
        SelectedGroup = _groups.FirstOrDefault();
    }
}