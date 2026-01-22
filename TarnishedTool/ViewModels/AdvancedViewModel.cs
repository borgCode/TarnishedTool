using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels;

public class AdvancedViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly ParamEditorViewModel _paramEditorViewModel;
    
    private ParamEditorWindow _paramEditorWindow;

    public AdvancedViewModel(IItemService itemService, IStateService stateService, IEventService eventService,
        IParamService paramService, IParamRepository paramRepository)
    {
        _itemService = itemService;


        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        
        SpawnWithEquipIdCommand = new DelegateCommand(SpawnWithEquipId);
        OpenParamEditorCommand = new DelegateCommand(OpenParamEditor);
        
        SelectedEquipType = EquipTypes[0].Value;
        
        _paramEditorViewModel  = new ParamEditorViewModel(paramRepository, paramService);
    }

    
    #region Commands

    public ICommand SpawnWithEquipIdCommand { get; set; }
    public ICommand OpenParamEditorCommand { get; set; }

    #endregion

    #region Properties

    public ObservableCollection<KeyValuePair<string, uint>> EquipTypes { get; } = new()
    {
        new("Accessory", 0x20000000),
        new("Gem", 0x80000000),
        new("Goods", 0x40000000),
        new("Protector", 0x10000000),
        new("Weapon", 0x00000000)
    };

    private uint _selectedEquipType;
    public uint SelectedEquipType
    {
        get => _selectedEquipType;
        set => SetProperty(ref _selectedEquipType, value);
    }

    private string _equipId;
    public string EquipId
    {
        get => _equipId;
        set => SetProperty(ref _equipId, value);
    }

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        

        // Parallel.ForEach(param.Entries, entry =>
        // {
        //     IntPtr row = _paramService.GetParamRow(param.TableIndex, param.SlotIndex, entry.Id);
        //     if (row == IntPtr.Zero) return;
        //
        //     byte[] data = _paramService.ReadRow(row, param.RowSize);
        //
        //     foreach (var field in param.Fields)
        //     {
        //         _ = _paramService.ReadFieldFromBytes(data, field);
        //     }
        // });
    }
    

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void SpawnWithEquipId()
    {
        if (!uint.TryParse(EquipId.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out uint equipId))
        {
            MsgBox.Show("Invalid Equip ID");
            return;
        }

        uint itemId = equipId + SelectedEquipType;
        _itemService.SpawnItem((int)itemId, 1, -1, false, 1);
    }
    
    private void OpenParamEditor()
    {
        if (_paramEditorWindow != null && _paramEditorWindow.IsVisible)
        {
            _paramEditorWindow.Activate();
            return;
        }

        _paramEditorWindow = new ParamEditorWindow
        {
            DataContext = _paramEditorViewModel
        };
        
        _paramEditorWindow.Closed += (_, _) => _paramEditorWindow = null;
        _paramEditorWindow.Show();
    }

    #endregion
}