using System.Windows.Input;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Tabs;

public partial class AdvancedTab
{
    public AdvancedTab(AdvancedViewModel advancedViewModel)
    {
        InitializeComponent();
        DataContext = advancedViewModel;
    }
}