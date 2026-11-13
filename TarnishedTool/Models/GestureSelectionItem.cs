using TarnishedTool.ViewModels;

namespace TarnishedTool.Models;

public class GestureSelectionItem : BaseViewModel
{
    public int Id { get; }
    public string Name { get; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public GestureSelectionItem(int id, string name)
    {
        Id = id;
        Name = name;
    }
}