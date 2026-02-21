// 

using System;
using System.Windows.Input;
using TarnishedTool.Core;

namespace TarnishedTool.ViewModels;

public class ChrInsEntry(nint chrIns) : BaseViewModel
{
    public nint ChrIns { get; set; } = chrIns;
    public int ChrId { get; set; }
    public uint NpcParamId { get; set; }
    public int NpcThinkParamId { get; set; }
    public long Handle { get; set; }
    public uint EntityId { get; set; }
    public string Name { get; set; }
    public string InternalName { get; set; }
    public Action<ChrInsEntry, string, bool> OnOptionChanged { get; set; }
    public Action<ChrInsEntry, string> OnCommandExecuted { get; set; }
    public Action<ChrInsEntry> OnExpanded { get; set; }

    public string ChrInsHeader =>
        $@"{Name}    {InternalName}";

    public string ChrInsIds =>
        $"ChrId: {ChrId}\nNpcParamId: {NpcParamId}\nNpcThinkParamId: {NpcThinkParamId}\nEntityId: {EntityId}\nChrIns: {(long)ChrIns:X}";


    public string AiWindowDisplay =>
        $@"{Name}   ChrId: {ChrId} NpcParamId: {NpcParamId} NpcThinkParamId: {NpcThinkParamId}";

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value) && value)
            {
                OnExpanded?.Invoke(this);
            }
        }
    }


    private bool _isAiDisabled;
    public bool IsAiDisabled
    {
        get => _isAiDisabled;
        set
        {
            if (SetProperty(ref _isAiDisabled, value))
            {
                OnOptionChanged.Invoke(this, nameof(IsAiDisabled), value);
            }
        }
    }

    private bool _isTargetViewEnabled;
    public bool IsTargetViewEnabled
    {
        get => _isTargetViewEnabled;
        set
        {
            if (SetProperty(ref _isTargetViewEnabled, value))
            {
                OnOptionChanged.Invoke(this, nameof(IsTargetViewEnabled), value);
            }
        }
    }

    private bool _isNoAttackEnabled;
    public bool IsNoAttackEnabled
    {
        get => _isNoAttackEnabled;
        set
        {
            if (SetProperty(ref _isNoAttackEnabled, value))
            {
                OnOptionChanged.Invoke(this, nameof(IsNoAttackEnabled), value);
            }
        }
    }

    private bool _isNoMoveEnabled;
    public bool IsNoMoveEnabled
    {
        get => _isNoMoveEnabled;
        set
        {
            if (SetProperty(ref _isNoMoveEnabled, value))
            {
                OnOptionChanged.Invoke(this, nameof(IsNoMoveEnabled), value);
            }
        }
    }

    private bool _isNoDamageEnabled;
    public bool IsNoDamageEnabled
    {
        get => _isNoDamageEnabled;
        set
        {
            if (SetProperty(ref _isNoDamageEnabled, value))
            {
                OnOptionChanged.Invoke(this, nameof(IsNoDamageEnabled), value);
            }
        }
    }

    private float _distance;
    public float Distance
    {
        get => _distance;
        set => SetProperty(ref _distance, value);
    }

    private int _currentHp;

    public int CurrentHp
    {
        get => _currentHp;
        set => SetProperty(ref _currentHp, value);
    }

    private int _maxHp;

    public int MaxHp
    {
        get => _maxHp;
        set => SetProperty(ref _maxHp, value);
    }

    private ICommand _warpCommand;
    public ICommand WarpCommand => _warpCommand ??= new DelegateCommand(() =>
    {
        OnCommandExecuted?.Invoke(this, nameof(WarpCommand));
    });

    private ICommand _openGoalWindowCommand;
    public ICommand OpenAiWindowCommand => _openGoalWindowCommand ??= new DelegateCommand(() =>
    {
        OnCommandExecuted?.Invoke(this, nameof(OpenAiWindowCommand));
    });

    private ICommand _killChrCommand;
    public ICommand KillChrCommand => _killChrCommand ??= new DelegateCommand(() =>
    {
        OnCommandExecuted?.Invoke(this, nameof(KillChrCommand));
    });

    private ICommand _setHpCommand;
    public ICommand SetHpCommand => _setHpCommand ??= new DelegateCommand(() =>
    {
        OnCommandExecuted?.Invoke(this, nameof(SetHpCommand));
    });

}