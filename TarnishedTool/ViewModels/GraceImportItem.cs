// 

using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class GraceImportItem(GracePresetTemplate preset, bool alreadyExists) : BaseViewModel
{
    public GracePresetTemplate Preset { get; } = preset;
    public string Name => Preset.Name;
    public int GraceCount => Preset.Graces?.Count ?? 0;
    public bool AlreadyExists { get; } = alreadyExists;

    private bool _isSelected = !alreadyExists;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}