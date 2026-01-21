using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels;

public class AdvancedViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly IParamService _paramService;
    private readonly IParamRepository _paramRepository;

    public AdvancedViewModel(IItemService itemService, IStateService stateService, IEventService eventService,
        IParamService paramService, IParamRepository paramRepository)
    {
        _itemService = itemService;
        _paramService = paramService;
        _paramRepository = paramRepository;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        
        SpawnWithEquipIdCommand = new DelegateCommand(SpawnWithEquipId);
        
        SelectedEquipType = EquipTypes[0].Value;


 
        
    }

    #region Commands

    public ICommand SpawnWithEquipIdCommand { get; set; }

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
        
        
        var sw = System.Diagnostics.Stopwatch.StartNew();
        LoadedParam param = _paramRepository.GetParam("SpEffect");
        Console.WriteLine($"Load: {sw.ElapsedMilliseconds}ms");
    
        sw.Restart();
    
        int rowsRead = 0;
    
        Parallel.ForEach(param.Entries, entry =>
        {
            IntPtr row = _paramService.GetParamRow(param.TableIndex, param.SlotIndex, entry.Id);
            if (row == IntPtr.Zero) return;
        
            byte[] data = _paramService.ReadRow(row, param.RowSize);
        
            foreach (var field in param.Fields)
            {
                _ = _paramService.ReadFieldFromBytes(data, field);
            }
        
            Interlocked.Increment(ref rowsRead);
        });
    
        sw.Stop();
    
        Console.WriteLine($"Rows: {rowsRead}");
        Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
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

    #endregion
}