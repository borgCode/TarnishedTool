// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.ViewModels.SearchableGroupedCollection<TarnishedTool.Enums.Param,
    TarnishedTool.Models.ParamEntry>;

namespace TarnishedTool.ViewModels;

public sealed class ParamEditorViewModel : BaseViewModel
{
    private readonly IParamRepository _paramRepository;
    private readonly IParamService _paramService;
    private readonly IReminderService _reminderService;
    private readonly Dictionary<string, Type> _enumTypes = new();
    private readonly Dictionary<(Param, uint), byte[]> _vanillaData = new();
    private readonly HashSet<(Param, uint)> _modifiedEntries = new();
    private LoadedParam _currentParam;
    private List<FieldValueViewModel> _fields;
    private IntPtr _currentRowPtr;
    private byte[] _currentRowData;

    private NavigationHistory<(Param, uint)> _history;
    private bool _isNavigatingHistory;

    private Dictionary<string, Dictionary<uint, string>> _customNames;

    public ParamEditorViewModel(IParamRepository paramRepository, IParamService paramService,
        IReminderService reminderService)
    {
        _paramRepository = paramRepository;
        _paramService = paramService;
        _reminderService = reminderService;


        _customNames = _paramRepository.LoadCustomNames();

        var entriesByParam = _paramRepository.GetAllEntriesByParam();

        foreach (var paramEntry in _customNames)
        {
            if (entriesByParam.TryGetValue((Param)Enum.Parse(typeof(Param), paramEntry.Key), out var entries))
            {
                var entryLookup = entries.ToDictionary(e => e.Id);
                foreach (var rowEntry in paramEntry.Value)
                {
                    if (entryLookup.TryGetValue(rowEntry.Key, out var entry))
                    {
                        entry.CustomName = rowEntry.Value;
                    }
                }
            }
        }

        ParamEntries = new SearchableGroupedCollection<Param, ParamEntry>(
            entriesByParam,
            (entry, search) =>
                entry.Id.ToString().Contains(search) ||
                entry.Parent.ToString().Contains(search) ||
                (entry.DisplayName?.ToLower().Contains(search) ?? false)
        );

        ParamEntries.PropertyChanged += OnParamEntriesPropertyChanged;

        RestoreSelectedEntryCommand = new DelegateCommand(RestoreSelectedEntry);
        RestoreAllEntriesCommand = new DelegateCommand(RestoreAllEntries);
        TogglePinCommand = new DelegateCommand<ParamEntry>(TogglePin);
        NavigateToPinnedCommand = new DelegateCommand<ParamEntry>(NavigateToEntry);
        NavigateBackCommand = new DelegateCommand(NavigateBack);
        NavigateForwardCommand = new DelegateCommand(NavigateForward);
        RenameRowCommand = new DelegateCommand<ParamEntry>(RenameRow);
        ToggleVanillaValuesCommand = new DelegateCommand(ToggleVanillaValues);
        CycleParamFieldDisplayModeCommand = new DelegateCommand(CycleParamFieldDisplayMode);
        PopulateEnumTypes();

        ParamEntries.SetSearchScope(SearchScopes.SelectedGroup);

        OnParamChanged();

        if (Enum.TryParse<ParamFieldDisplayMode>(SettingsManager.Default.ParamFieldDisplayMode, out var savedMode))
        {
            _paramFieldDisplayMode = savedMode;
        }
        else
        {
            _paramFieldDisplayMode = ParamFieldDisplayMode.OffsetNameInternal;
        }
    }

    #region Commands

    public ICommand RestoreSelectedEntryCommand { get; set; }
    public ICommand RestoreAllEntriesCommand { get; set; }
    public ICommand TogglePinCommand { get; set; }
    public ICommand NavigateToPinnedCommand { get; set; }
    public ICommand NavigateBackCommand { get; set; }
    public ICommand NavigateForwardCommand { get; set; }
    public ICommand RenameRowCommand { get; set; }
    public ICommand ToggleVanillaValuesCommand { get; set; }
    public ICommand CycleParamFieldDisplayModeCommand { get; set; }

    #endregion

    #region Properties

    public SearchableGroupedCollection<Param, ParamEntry> ParamEntries { get; }

    private ICollectionView _fieldsView;

    public ICollectionView FieldsView => _fieldsView;

    private string _fieldSearchText;

    public string FieldSearchText
    {
        get => _fieldSearchText;
        set
        {
            if (SetProperty(ref _fieldSearchText, value))
                _fieldsView?.Refresh();
        }
    }

    private bool _isSelectedEntryModified;

    public bool IsSelectedEntryModified
    {
        get => _isSelectedEntryModified;
        set => SetProperty(ref _isSelectedEntryModified, value);
    }

    public bool HasAnyModified => _modifiedEntries.Any();

    private bool _showModifiedEntriesOnly;

    public bool ShowModifiedEntriesOnly
    {
        get => _showModifiedEntriesOnly;
        set
        {
            if (SetProperty(ref _showModifiedEntriesOnly, value))
            {
                if (value)
                    ParamEntries.SetAdditionalFilter(entry => IsEntryModified(entry.Parent, entry.Id));
                else
                    ParamEntries.ClearAdditionalFilter();
            }
        }
    }

    private bool _showVanillaValues = true;

    public bool ShowVanillaValues
    {
        get => _showVanillaValues;
        set => SetProperty(ref _showVanillaValues, value);
    }

    private bool _isSearchAllParamsEnabled;

    public bool IsSearchAllParamsEnabled
    {
        get => _isSearchAllParamsEnabled;
        set
        {
            if (SetProperty(ref _isSearchAllParamsEnabled, value))
            {
                if (value) ParamEntries.SetSearchScope(SearchScopes.AllGroups);
                else ParamEntries.SetSearchScope(SearchScopes.SelectedGroup);
            }
        }
    }

    private readonly ObservableCollection<ParamEntry> _pinnedEntries = new();
    public ObservableCollection<ParamEntry> PinnedEntries => _pinnedEntries;

    public bool HasPinnedEntries => _pinnedEntries.Count > 0;

    public bool CanGoBack => _history?.CanGoBack ?? false;
    public bool CanGoForward => _history?.CanGoForward ?? false;

    private ParamFieldDisplayMode _paramFieldDisplayMode;

    public ParamFieldDisplayMode ParamFieldDisplayMode
    {
        get => _paramFieldDisplayMode;
        set
        {
            if (SetProperty(ref _paramFieldDisplayMode, value))
            {
                SettingsManager.Default.ParamFieldDisplayMode = value.ToString();
                SettingsManager.Default.Save();
                _fieldsView?.Refresh();
            }
        }
    }

    private ParamEntry _entryToRename;

    public ParamEntry EntryToRename
    {
        get => _entryToRename;
        set => SetProperty(ref _entryToRename, value);
    }

    #endregion

    #region Private Methods

    private void OnParamEntriesPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ParamEntries.SelectedGroup):
                OnParamChanged();
                break;
            case nameof(ParamEntries.SelectedItem):
                OnEntryChanged();
                break;
        }
    }

    private void OnParamChanged()
    {
        _currentParam = _paramRepository.GetParam(ParamEntries.SelectedGroup);

        _fields = _currentParam.Fields
            .Where(field => !field.InternalName.ToLower().Contains("pad"))
            .Select(f =>
            {
                var vm = new FieldValueViewModel(f, this);

                if (!string.IsNullOrEmpty(f.EnumType) && _enumTypes.TryGetValue(f.EnumType, out var enumType))
                {
                    var enumValues = new List<EnumValueItem>();
                    foreach (var enumValue in Enum.GetValues(enumType))
                    {
                        enumValues.Add(new EnumValueItem
                        {
                            Name = enumValue.ToString(),
                            Value = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumType))
                        });
                    }

                    vm.SetEnumValues(enumValues);
                }

                return vm;
            })
            .ToList();

        _fieldsView = CollectionViewSource.GetDefaultView(_fields);
        _fieldsView.Filter = FilterField;
        OnPropertyChanged(nameof(FieldsView));

        _currentRowPtr = IntPtr.Zero;
        _currentRowData = null;
    }

    private void OnEntryChanged()
    {
        if (ParamEntries.SelectedItem == null || _currentParam == null)
        {
            _currentRowPtr = IntPtr.Zero;
            _currentRowData = null;
            return;
        }

        var key = (ParamEntries.SelectedGroup, ParamEntries.SelectedItem.Id);

        _currentRowPtr = _paramService.GetParamRow(
            _currentParam.TableIndex,
            _currentParam.SlotIndex,
            ParamEntries.SelectedItem.Id
        );

        if (_currentRowPtr != IntPtr.Zero)
        {
            _currentRowData = _paramService.ReadRow(_currentRowPtr, _currentParam.RowSize);

            if (!_vanillaData.ContainsKey(key))
            {
                _vanillaData[key] = (byte[])_currentRowData.Clone();
            }
        }

        foreach (var field in _fields)
        {
            field.RefreshValue();
        }

        IsSelectedEntryModified = IsEntryModified(ParamEntries.SelectedGroup, ParamEntries.SelectedItem.Id);


        if (!_isNavigatingHistory)
        {
            if (_history == null)
                _history = new NavigationHistory<(Param, uint)>(key);
            else
                _history.Navigate(key);

            NotifyNavigationChanged();
        }
    }

    private void NotifyNavigationChanged()
    {
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    private bool FilterField(object obj)
    {
        if (string.IsNullOrEmpty(FieldSearchText)) return true;

        var field = (FieldValueViewModel)obj;
        return field.DisplayName.ToLower().Contains(FieldSearchText) ||
               field.InternalName.ToLower().Contains(FieldSearchText);
    }

    private void RestoreSelectedEntry()
    {
        if (_currentRowPtr == IntPtr.Zero || ParamEntries.SelectedItem == null)
            return;

        var key = (ParamEntries.SelectedGroup, ParamEntries.SelectedItem.Id);

        if (!_vanillaData.TryGetValue(key, out var vanilla))
            return;

        _paramService.WriteRow(_currentRowPtr, vanilla);
        _currentRowData = (byte[])vanilla.Clone();

        _modifiedEntries.Remove(key);
        IsSelectedEntryModified = false;
        OnPropertyChanged(nameof(HasAnyModified));

        foreach (var field in _fields)
        {
            field.RefreshValue();
        }
    }

    private void RestoreAllEntries()
    {
        foreach (var key in _modifiedEntries.ToList())
        {
            if (!_vanillaData.TryGetValue(key, out var vanilla))
                continue;

            var rowPtr = _paramService.GetParamRow(
                _paramRepository.GetParam(key.Item1).TableIndex,
                _paramRepository.GetParam(key.Item1).SlotIndex,
                key.Item2
            );

            if (rowPtr != IntPtr.Zero)
                _paramService.WriteRow(rowPtr, vanilla);
        }

        _modifiedEntries.Clear();
        IsSelectedEntryModified = false;
        OnPropertyChanged(nameof(HasAnyModified));

        // Make Restore All Params Refresh Field view too
        if (_currentRowPtr != IntPtr.Zero && ParamEntries.SelectedItem != null)
        {
            _currentRowData = _paramService.ReadRow(_currentRowPtr, _currentParam.RowSize);
        }

        foreach (var field in _fields)
        {
            field.RefreshValue();
        }
    }

    private void TogglePin(ParamEntry entry)
    {
        if (entry == null) return;

        var existing = _pinnedEntries.FirstOrDefault(e => e.Parent == entry.Parent && e.Id == entry.Id);
        if (existing != null)
            _pinnedEntries.Remove(existing);
        else
            _pinnedEntries.Add(entry);

        OnPropertyChanged(nameof(HasPinnedEntries));
    }

    private void NavigateToEntry(ParamEntry entry)
    {
        if (entry == null) return;

        ParamEntries.SelectedGroup = entry.Parent;
        ParamEntries.SelectedItem = ParamEntries.Items.FirstOrDefault(e => e.Id == entry.Id);
    }

    private void PopulateEnumTypes()
    {
        var enumTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsEnum && t.Namespace?.StartsWith("TarnishedTool.Enums.ParamEnums") == true);


        foreach (var type in enumTypes)
            _enumTypes.Add(type.Name, type);
    }

    private void NavigateBack()
    {
        if (_history == null || !_history.CanGoBack) return;

        _isNavigatingHistory = true;
        var (param, id) = _history.GoBack();
        NavigateToParamEntry(param, id);
        _isNavigatingHistory = false;

        NotifyNavigationChanged();
    }

    private void NavigateForward()
    {
        if (_history == null || !_history.CanGoForward) return;

        _isNavigatingHistory = true;
        var (param, id) = _history.GoForward();
        NavigateToParamEntry(param, id);
        _isNavigatingHistory = false;

        NotifyNavigationChanged();
    }

    private void NavigateToParamEntry(Param param, uint id)
    {
        ParamEntries.SelectedGroup = param;
        ParamEntries.SelectedItem = ParamEntries.Items.FirstOrDefault(e => e.Id == id);
    }

    private void ToggleVanillaValues()
    {
        ShowVanillaValues = !ShowVanillaValues;
    }

    private void CycleParamFieldDisplayMode()
    {
        ParamFieldDisplayMode = ParamFieldDisplayMode switch
        {
            ParamFieldDisplayMode.OffsetNameInternal => ParamFieldDisplayMode.NameInternal,
            ParamFieldDisplayMode.NameInternal => ParamFieldDisplayMode.NameOnly,
            ParamFieldDisplayMode.NameOnly => ParamFieldDisplayMode.OffsetInternal,
            ParamFieldDisplayMode.OffsetInternal => ParamFieldDisplayMode.OffsetNameInternal,
            _ => ParamFieldDisplayMode.OffsetNameInternal
        };
    }

    private void RenameRow(ParamEntry entry)
    {
        if (entry == null) return;
        var newName = MsgBox.ShowInput(
            $"New Name",
            entry.CustomName ?? entry.DisplayName,
            $"Rename Row {entry.Id}"
        );
        if (newName != null)
        {
            ApplyCustomName(entry, newName);
        }
    }

    #endregion

    #region Public Methods

    public object ReadFieldValue(ParamFieldDef field)
    {
        if (_currentRowData == null) return null;
        return _paramService.ReadFieldFromBytes(_currentRowData, field);
    }

    public void WriteFieldValue(ParamFieldDef field, object value)
    {
        if (_currentRowPtr == IntPtr.Zero) return;
        _reminderService.TrySetReminder();
        _paramService.WriteField(_currentRowPtr, field, value);
        _currentRowData = _paramService.ReadRow(_currentRowPtr, _currentParam.RowSize);

        var key = (ParamEntries.SelectedGroup, ParamEntries.SelectedItem.Id);

        if (_vanillaData.TryGetValue(key, out var vanilla))
        {
            if (_currentRowData.SequenceEqual(vanilla))
                _modifiedEntries.Remove(key);
            else
                _modifiedEntries.Add(key);

            IsSelectedEntryModified = _modifiedEntries.Contains(key);
            OnPropertyChanged(nameof(HasAnyModified));
        }
    }

    public object ReadVanillaFieldValue(ParamFieldDef field)
    {
        var key = (ParamEntries.SelectedGroup, ParamEntries.SelectedItem.Id);
        if (_vanillaData.TryGetValue(key, out var vanillaBytes))
        {
            return _paramService.ReadFieldFromBytes(vanillaBytes, field);
        }

        return null;
    }

    public bool IsEntryModified(Param param, uint entryId)
    {
        return _modifiedEntries.Contains((param, entryId));
    }

    public bool IsPinned(ParamEntry entry)
    {
        return _pinnedEntries.Any(e => e.Parent == entry.Parent && e.Id == entry.Id);
    }

    public void NotifyInitialWindowOpened()
    {
        OnParamChanged();
        OnEntryChanged();
    }

    public void ApplyCustomName(ParamEntry entry, string newName)
    {
        if (entry == null) return;

        var paramName = entry.Parent.ToString();

        if (string.IsNullOrWhiteSpace(newName))
        {
            if (_customNames.TryGetValue(paramName, out var dict))
            {
                dict.Remove(entry.Id);
                if (dict.Count == 0)
                    _customNames.Remove(paramName);
            }

            entry.CustomName = null;
        }
        else
        {
            if (!_customNames.TryGetValue(paramName, out var dict))
            {
                dict = new Dictionary<uint, string>();
                _customNames[paramName] = dict;
            }

            dict[entry.Id] = newName;
            entry.CustomName = newName;
        }

        _paramRepository.SaveCustomNames(_customNames);

        // "refresh" pinned stuff after a rename
        var pinnedEntry = _pinnedEntries.FirstOrDefault(p => p.Id == entry.Id && p.Parent == entry.Parent);
        if (pinnedEntry != null)
        {
            var index = _pinnedEntries.IndexOf(pinnedEntry);
            _pinnedEntries.RemoveAt(index);
            _pinnedEntries.Insert(index, pinnedEntry);
        }
    }

    #endregion
}