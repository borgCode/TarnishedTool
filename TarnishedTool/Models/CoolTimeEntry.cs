// 

using TarnishedTool.ViewModels;

namespace TarnishedTool.Models;

public class CoolTimeEntry(int animationId, float timeSinceLastAttack, float cooldown) : BaseViewModel
{
    private int _animationId = animationId;
    public int AnimationId
    {
        get => _animationId;
        set => SetProperty(ref _animationId, value);
    }

    private float _timeSinceLastAttack = timeSinceLastAttack;
    public float TimeSinceLastAttack
    {
        get => _timeSinceLastAttack;
        set => SetProperty(ref _timeSinceLastAttack, value);
    }

    private float _cooldown = cooldown;
    public float Cooldown
    {
        get => _cooldown;
        set => SetProperty(ref _cooldown, value);
    }
    
}