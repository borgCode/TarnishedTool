using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class PhaseTransitionViewModel : BaseViewModel
{
    private readonly ITargetService _targetService;
    private readonly IEmevdService _emevdService;
    private readonly IEventService _eventService;
    private readonly IChrInsService _chrInsService;
    private readonly ISpEffectService _spEffectService;
    private readonly IAiService _aiService;
    private PhaseTransition? _currentTransition;

    public PhaseTransitionViewModel(ITargetService targetService, IEmevdService emevdService, IEventService eventService, IChrInsService chrInsService, ISpEffectService spEffectService, IAiService aiService)
    {
        _targetService = targetService;
        _emevdService = emevdService;
        _eventService = eventService;
        _chrInsService = chrInsService;
        _spEffectService = spEffectService;
        _aiService = aiService;
        PhaseTransitionRegistry.Initialize(chrInsService, eventService, spEffectService, aiService);
        TriggerPhaseCommand = new DelegateCommand(TriggerPhase);
    }

    public ICommand TriggerPhaseCommand { get; }
    
    private ObservableCollection<PhaseButtonViewModel> _phaseButtons = new();
    public ObservableCollection<PhaseButtonViewModel> PhaseButtons
    {
        get => _phaseButtons;
        set => SetProperty(ref _phaseButtons, value);
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    // Called by TargetViewModel when the locked-on target changes
    public void OnTargetChanged(uint npcParamId)
    {
        var transitions = PhaseTransitionRegistry.Get(npcParamId);
        PhaseButtons = new ObservableCollection<PhaseButtonViewModel>(
            transitions?.Select(t => new PhaseButtonViewModel(t, _targetService, _emevdService, _eventService, _spEffectService))
            ?? Enumerable.Empty<PhaseButtonViewModel>()
        );
        IsVisible = PhaseButtons.Count > 0;
    }

    // Called by TargetViewModel every tick to keep the button state fresh
    public void OnTick()
    {
        foreach (var button in PhaseButtons)
            button.Refresh();
    }

    public void TriggerPhase()
    {
        if (_currentTransition?.CanActivate(_targetService) == true)
            _currentTransition.Execute(_targetService, _emevdService);
    }
}