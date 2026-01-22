// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class ParamEditorViewModel : BaseViewModel
{
    private readonly IParamRepository _paramRepository;
    private readonly IParamService _paramService;
    
    private LoadedParam _currentParam;
    private List<FieldValueViewModel> _fields;
    private IntPtr _currentRowPtr;
    private byte[] _currentRowData;

    public ParamEditorViewModel(IParamRepository paramRepository, IParamService paramService)
    {
        _paramRepository = paramRepository;
        _paramService = paramService;

        ParamEntries = new SearchableGroupedCollection<Param, ParamEntry>(
            _paramRepository.GetAllEntriesByParam(),
            (entry, search) =>
                entry.Id.ToString().Contains(search) ||
                (entry.Name?.ToLower().Contains(search) ?? false)
        );

        ParamEntries.PropertyChanged += OnParamEntriesPropertyChanged;
    }

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
            .Select(f => new FieldValueViewModel(f, this))
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

        _currentRowPtr = _paramService.GetParamRow(
            _currentParam.TableIndex,
            _currentParam.SlotIndex,
            ParamEntries.SelectedItem.Id
        );

        if (_currentRowPtr != IntPtr.Zero)
        {
            _currentRowData = _paramService.ReadRow(_currentRowPtr, _currentParam.RowSize);
        }
        
        foreach (var field in _fields)
        {
            field.RefreshValue();
        }
    }

    private bool FilterField(object obj)
    {
        if (string.IsNullOrEmpty(FieldSearchText)) return true;
        
        var field = (FieldValueViewModel)obj;
        return field.DisplayName.ToLower().Contains(FieldSearchText) ||
               field.InternalName.ToLower().Contains(FieldSearchText);
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
        
        _paramService.WriteField(_currentRowPtr, field, value);
        _currentRowData = _paramService.ReadRow(_currentRowPtr, _currentParam.RowSize);
    }

    #endregion

}