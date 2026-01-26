// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TarnishedTool.ViewModels;

public class SearchableGroupedCollection<TGroup, TItem> : BaseViewModel
{
    public enum SearchScopes
    {
        SelectedGroup,
        AllGroups
    }

    private readonly Func<TItem, string, bool> _matchesSearch;
    private TGroup _preSearchGroup;
    private readonly Dictionary<TGroup, List<TItem>> _groupedItems;

    private Func<TItem, bool> _additionalFilter;

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
        UpdateItemsList();
    }

    private ObservableCollection<TGroup> _groups;
    public ObservableCollection<TGroup> Groups => _groups;
    private ObservableCollection<TItem> _items;
    public List<TItem> this[TGroup group] => _groupedItems[group];

    public Dictionary<TGroup, List<TItem>> GroupedItems => _groupedItems;

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

    private SearchScopes _searchScope = SearchScopes.AllGroups;

    public SearchScopes SearchScope
    {
        get => _searchScope;
        set
        {
            if (!SetProperty(ref _searchScope, value)) return;
            if (_isSearchActive) ApplyFilter();
        }
    }

    #region Private Methods

    public void UpdateItemsList()
    {
        if (SelectedGroup == null || !_groupedItems.ContainsKey(SelectedGroup))
        {
            Items = new ObservableCollection<TItem>();
            return;
        }

        var items = _groupedItems[SelectedGroup].AsEnumerable();

        if (_additionalFilter != null)
            items = items.Where(_additionalFilter);

        Items = new ObservableCollection<TItem>(items);
        SelectedItem = Items.FirstOrDefault();
    }

    private void ApplyFilter()
    {
        var searchLower = SearchText.ToLower();

        var source = _searchScope == SearchScopes.AllGroups ? _allItems : _groupedItems[SelectedGroup];

        var items = source.Where(item => _matchesSearch(item, searchLower));

        if (_additionalFilter != null)
            items = items.Where(_additionalFilter);

        Items = new ObservableCollection<TItem>(items);
        SelectedItem = Items.FirstOrDefault();
    }

    #endregion

    #region Public Methods

    public void SetAdditionalFilter(Func<TItem, bool> filter)
    {
        _additionalFilter = filter;
        if (_isSearchActive)
            ApplyFilter();
        else
            UpdateItemsList();
    }

    public void ClearAdditionalFilter()
    {
        _additionalFilter = null;
        if (_isSearchActive)
            ApplyFilter();
        else
            UpdateItemsList();
    }

    public void SetSearchScope(SearchScopes scope) => SearchScope = scope;

    public void Add(TGroup group, TItem item)
    {
        if (!_groupedItems.ContainsKey(group))
        {
            _groupedItems[group] = new List<TItem>();
            _groups.Add(group);
        }

        _groupedItems[group].Add(item);
        _allItems.Add(item);

        if (EqualityComparer<TGroup>.Default.Equals(SelectedGroup, group))
        {
            Items.Add(item);
        }
    }

    public void AddRange(TGroup group, IEnumerable<TItem> items)
    {
        var itemList = items.ToList();

        if (!_groupedItems.ContainsKey(group))
        {
            _groupedItems[group] = new List<TItem>();
            _groups.Add(group);
        }

        _groupedItems[group].AddRange(itemList);
        _allItems.AddRange(itemList);

        if (EqualityComparer<TGroup>.Default.Equals(SelectedGroup, group))
        {
            foreach (var item in itemList)
            {
                Items.Add(item);
            }
        }
    }

    public void RemoveGroup(TGroup group)
    {
        if (!_groupedItems.ContainsKey(group)) return;

        var items = _groupedItems[group];
        foreach (var item in items)
        {
            _allItems.Remove(item);
        }

        _groupedItems.Remove(group);
        _groups.Remove(group);

        if (EqualityComparer<TGroup>.Default.Equals(SelectedGroup, group))
        {
            SelectedGroup = _groups.FirstOrDefault();
        }
    }
    
    public void Remove(TGroup group, TItem item)
    {
        if (!_groupedItems.ContainsKey(group)) return;

        _groupedItems[group].Remove(item);
        _allItems.Remove(item);
        
        Items.Remove(item);
    
        if (_groupedItems[group].Count == 0)
        {
            _groupedItems.Remove(group);
            _groups.Remove(group);

            if (EqualityComparer<TGroup>.Default.Equals(SelectedGroup, group))
            {
                SelectedGroup = _groups.FirstOrDefault();
            }
        }
    
        if (EqualityComparer<TItem>.Default.Equals(SelectedItem, item))
        {
            SelectedItem = Items.FirstOrDefault();
        }
    }

    #endregion
}