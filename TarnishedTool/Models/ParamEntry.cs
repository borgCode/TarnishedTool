// 

using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Models;

public class ParamEntry : BaseViewModel
{
    public ParamEntry(uint id, string name)
    {
        Id = id;
        Name = name;
    }
    public bool HasName => !string.IsNullOrEmpty(DisplayName);
    public string Name { get; }
    public uint Id { get; }
    public Param Parent { get; set; }
    public string ParentName => Parent.ToString();
    private string _customName;
    public string CustomName
    {
        get => _customName;
        set
        {
            if (SetProperty(ref _customName, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(HasName));
            }
        }
    }
    public string DisplayName => CustomName ?? Name;
}