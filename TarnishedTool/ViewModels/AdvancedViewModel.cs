using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels;

public class AdvancedViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    
    private readonly ParamEditorViewModel _paramEditorViewModel;
    
    private readonly ISpEffectService _spEffectService;
    private readonly SpEffectViewModel _spEffectViewModel = new();
    private SpEffectsWindow _spEffectsWindow;
    
    private readonly HotkeyManager _hotkeyManager;
    private readonly IGameTickService _gameTickService;
    
    private readonly IUtilityService _utilityService;
    private readonly IChrInsService _chrInsService;
    private readonly ChrInsWindowViewModel _chrInsWindowViewModel;
    private ChrInsWindow _chrInsWindow;

    private ParamEditorWindow _paramEditorWindow;

    private readonly IPlayerService _playerService;
    
    private bool _hasNotifiedInitialOpen;

    public AdvancedViewModel(IItemService itemService, IStateService stateService, IEventService eventService,
        IParamService paramService, IParamRepository paramRepository, ISpEffectService spEffectService,
        IPlayerService playerService, HotkeyManager hotkeyManager, IGameTickService gameTickService,
        IReminderService reminderService, IAiService aiService, IUtilityService utilityService,
        IChrInsService chrInsService)
    {
        _itemService = itemService;
        _spEffectService = spEffectService;
        _playerService = playerService;
        _hotkeyManager = hotkeyManager;
        _gameTickService = gameTickService;
        _utilityService = utilityService;
        _chrInsService = chrInsService;

        RegisterHotkeys();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

        SpawnWithEquipIdCommand = new DelegateCommand(SpawnWithEquipId);
        OpenParamEditorCommand = new DelegateCommand(OpenParamEditor);
        ApplySpEffectCommand = new DelegateCommand(ApplySpEffect);
        RemoveSpEffectCommand = new DelegateCommand(RemoveSpEffect);
        AboutSpEffectsCommand = new DelegateCommand(ShowAboutSpEffects);
        OpenAiWindowCommand = new DelegateCommand(OpenAiWindow);

        SelectedEquipType = EquipTypes[0].Value;

        _paramEditorViewModel = new ParamEditorViewModel(paramRepository, paramService, reminderService);
        _chrInsWindowViewModel = new ChrInsWindowViewModel(aiService, stateService, gameTickService, playerService, chrInsService);
    }

    
    #region Commands

    public ICommand SpawnWithEquipIdCommand { get; set; }
    public ICommand OpenParamEditorCommand { get; set; }
    public ICommand ApplySpEffectCommand { get; set; }
    public ICommand RemoveSpEffectCommand { get; set; }
    public ICommand AboutSpEffectsCommand { get; set; }
    public ICommand OpenAiWindowCommand { get; set; }

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
    
    private string _applySpEffectId;

    public string ApplySpEffectId
    {
        get => _applySpEffectId;
        set => SetProperty(ref _applySpEffectId, value);
    }

    private string _removeSpEffectId;

    public string RemoveSpEffectId
    {
        get => _removeSpEffectId;
        set => SetProperty(ref _removeSpEffectId, value);
    }
    
    private bool _isSpEffectWindowOpen;
    
    public bool IsSpEffectWindowOpen
    {
        get => _isSpEffectWindowOpen;
        set
        {
            if (SetProperty(ref _isSpEffectWindowOpen, value))
            {
                if (_isSpEffectWindowOpen)
                {
                    OpenSpEffectsWindow();
                    _gameTickService.Subscribe(SpEffectsTick);
                }
                else
                {
                    _gameTickService.Unsubscribe(SpEffectsTick);
                }
            }
        }
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsSpEffectWindowOpen) _gameTickService.Subscribe(SpEffectsTick);
    }

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.ApplySpEffect, () => SafeExecute(ApplySpEffect));
        _hotkeyManager.RegisterAction(HotkeyActions.RemoveSpEffect, () => SafeExecute(RemoveSpEffect));
    }

    private void SafeExecute(Action action)
    {
        if (!AreOptionsEnabled) return;
        action();
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        if (IsSpEffectWindowOpen) _gameTickService.Unsubscribe(SpEffectsTick);
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
        if (!_hasNotifiedInitialOpen)
        {
            _paramEditorViewModel.NotifyInitialWindowOpened();
            _hasNotifiedInitialOpen = true;
        }
    }
    
    private void OpenSpEffectsWindow()
    {
        if (_spEffectsWindow != null && _spEffectsWindow.IsVisible)
        {
            _spEffectsWindow.Activate();
            return;
        }
        
        
        _spEffectsWindow = new SpEffectsWindow
        {
            DataContext = _spEffectViewModel,
            Title = "Player Active Special Effects"
        };
        _spEffectsWindow.Closed += (s, e) =>
        {
            _spEffectsWindow = null;
            IsSpEffectWindowOpen = false;
        };
        _spEffectsWindow.Show();
    }

    
    private void ShowAboutSpEffects()
    {
        MsgBox.Show(
            "To put it simply Special Effects are effects that get applied to every entity in the game in order to achieve a specific goal in mind, that goal can quite literally be anything the devs have in mind. For example you can lock the player in a certain area, activate the effect of a talisman after the player equips it, apply a buff to the player. You can can also force a boss to follow up a specific move after an attack or trigger an entire phase through it. spEffects also control the hp and damage scaling of enemies and many more things that it's hard to explain in a small info box. If you want to learn about this I would recommend you check out Smithbox by Vawser and slowly get a grasp on how things work as most things are annotated thanks to the community effort so it will be a little easier to navigate.",
            "About Special Effects");
    }

    private void SpEffectsTick()
    {
        var spEffects = _spEffectService.GetActiveSpEffectList(_playerService.GetPlayerIns());
        _spEffectViewModel.RefreshEffects(spEffects);
    }

    private void RemoveSpEffect()
    {
        if (!uint.TryParse(RemoveSpEffectId, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out uint spEffectId)) return;
        var playerIns = _playerService.GetPlayerIns();
        _spEffectService.RemoveSpEffect(playerIns, spEffectId);
    }

    private void ApplySpEffect()
    {
        if (!uint.TryParse(ApplySpEffectId, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out uint spEffectId)) return;
        var playerIns = _playerService.GetPlayerIns();
        _spEffectService.ApplySpEffect(playerIns, spEffectId);
    }
    
    private void OpenAiWindow()
    {
        if (_chrInsWindow != null && _chrInsWindow.IsVisible)
        {
            _chrInsWindow.Activate();
            return;
        }
        
        _chrInsWindow = new ChrInsWindow
        {
            DataContext = _chrInsWindowViewModel,
            Title = "AI"
        };
        _chrInsWindow.Closed += (s, e) =>
        {
            _chrInsWindow = null;
            _chrInsWindowViewModel.NotifyWindowClosed();
        };

        _utilityService.PatchDebugFont();
        _chrInsWindow.Show();
        _chrInsWindowViewModel.NotifyWindowOpen();
    }

    #endregion
}