using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class PhaseButtonViewModel : BaseViewModel
{
    private readonly PhaseTransition _transition;
    private readonly ITargetService _targetService;
    private readonly IEmevdService _emevdService;
    private readonly IEventService _eventService;
    private readonly ISpEffectService _spEffectService;

    public PhaseButtonViewModel(PhaseTransition transition, ITargetService targetService,
        IEmevdService emevdService, IEventService eventService,  ISpEffectService spEffectService)
    {
        _transition = transition;
        _targetService = targetService;
        _emevdService = emevdService;
        _eventService = eventService;
        _spEffectService = spEffectService;
        TriggerCommand = new DelegateCommand(Trigger);
    }

    public ICommand TriggerCommand { get; }
    public string Label => _transition.Label;

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public void Refresh()
    {
        bool isPhase2 = _transition.IsPhase2(_spEffectService,  _targetService);
        IsEnabled = !isPhase2 && _transition.CanActivate(_targetService);
    }

    private void Trigger()
    {
        if (_transition.CanActivate(_targetService))
        {
            IsEnabled = false;
            Task.Run(() => _transition.Execute(_targetService, _emevdService))
                .ContinueWith(_ => Refresh());
        }
    }
}