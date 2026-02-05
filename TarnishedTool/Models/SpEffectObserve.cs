// 

using TarnishedTool.ViewModels;

namespace TarnishedTool.Models;

public class SpEffectObserve(int target, int spEffectId) : BaseViewModel
{
    private int _target = target;
    public int Target
    {
        get => _target;
        set => SetProperty(ref _target, value);
    }

    private int _spEffectId = spEffectId;
    public int SpEffectId
    {
        get => _spEffectId;
        set => SetProperty(ref _spEffectId, value);
    }

    private string _targetName;
    public string TargetName
    {
        get => _targetName;
        set => SetProperty(ref _targetName, value);
    }
    
}