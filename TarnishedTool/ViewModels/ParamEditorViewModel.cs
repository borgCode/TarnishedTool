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
using TarnishedTool.Enums.ParamEnums.AtkParam;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.ViewModels.SearchableGroupedCollection<TarnishedTool.Enums.Param,TarnishedTool.Models.ParamEntry>;

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

    public ParamEditorViewModel(IParamRepository paramRepository, IParamService paramService,
        IReminderService reminderService)
    {
        _paramRepository = paramRepository;
        _paramService = paramService;
        _reminderService = reminderService;

        ParamEntries = new SearchableGroupedCollection<Param, ParamEntry>(
            _paramRepository.GetAllEntriesByParam(),
            (entry, search) =>
                entry.Id.ToString().Contains(search) ||
                entry.Parent.ToString().Contains(search) ||
                (entry.Name?.ToLower().Contains(search) ?? false)
        );

        ParamEntries.PropertyChanged += OnParamEntriesPropertyChanged;

        RestoreSelectedEntryCommand = new DelegateCommand(RestoreSelectedEntry);
        RestoreAllEntriesCommand = new DelegateCommand(RestoreAllEntries);
        TogglePinCommand = new DelegateCommand<ParamEntry>(TogglePin);
        NavigateToPinnedCommand = new DelegateCommand<ParamEntry>(NavigateToEntry);
        PopulateEnumTypes();
            
     //   PrintEnums();
        
        ParamEntries.SetSearchScope(SearchScopes.SelectedGroup);

        OnParamChanged();
    }


    #region Commands

    public ICommand RestoreSelectedEntryCommand { get; set; }
    public ICommand RestoreAllEntriesCommand { get; set; }
    public ICommand TogglePinCommand { get; set; }
    public ICommand NavigateToPinnedCommand { get; set; }

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

/* Debugging
        private void PrintEnums()
        {
            Console.WriteLine("Enums");
            foreach (var enumType in _enumTypes)
            {
                Console.WriteLine($"{enumType.Key}:");

                foreach (var enumVal in Enum.GetValues(enumType.Value))
                {
                    Console.WriteLine($"{enumVal}");
                }
            }
            Console.WriteLine();
        }
            
*/

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

    #endregion
}