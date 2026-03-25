using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class ActivateOnLaunchWindow
{
    public ActivateOnLaunchWindow(ActivateOnLaunchViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}