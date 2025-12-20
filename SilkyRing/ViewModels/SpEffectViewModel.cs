// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using SilkyRing.Models;

namespace SilkyRing.ViewModels;

public class SpEffectViewModel : BaseViewModel
{
    private ObservableCollection<SpEffectEntry> _activeEffects = new();
    public ObservableCollection<SpEffectEntry> ActiveEffects
    {
        get => _activeEffects;
        set => SetProperty(ref _activeEffects, value);
    }
    
    public void RefreshEffects(List<SpEffectEntry> effects)
    {
        ActiveEffects.Clear();
        foreach (var effect in effects)
            ActiveEffects.Add(effect);
    }
}