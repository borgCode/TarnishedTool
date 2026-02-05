// 

using System;
using System.Windows;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows.AiOverlay;

public partial class GoalsOverlayWindow : AiOverlayWindowBase
{
    protected override string SettingsPrefix => "AiOverlayGoals";
    
    public GoalsOverlayWindow(AiWindowViewModel viewModel, Action onClosed = null) 
        : base(onClosed)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
    
    private void CycleOpacity_Click(object sender, RoutedEventArgs e) => CycleOpacity();
    private void Close_Click(object sender, RoutedEventArgs e) => Close();

}