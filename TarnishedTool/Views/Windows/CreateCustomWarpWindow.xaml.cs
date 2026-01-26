// 

using System;
using System.Collections.Generic;
using System.Windows;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class CreateCustomWarpWindow : TopmostWindow
{

    public CreateCustomWarpWindow(Dictionary<string, List<BlockWarp>> customWarps,
        bool areOptionsEnabled,
        IStateService stateService,
        IPlayerService playerService, IGameTickService gameTickService, Action<BlockWarp> onWarpCreated)
    {
        InitializeComponent();

        var viewModel = new CreateCustomWarpViewModel(
            customWarps,
            areOptionsEnabled,
            stateService,
            playerService,
            gameTickService,
            onWarpCreated
        );
        
        DataContext = viewModel;
        
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (_, _) => Close();
        }
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.CreateCustomWarpWindowLeft > 0)
                Left = SettingsManager.Default.CreateCustomWarpWindowLeft;
        
            if (SettingsManager.Default.CreateCustomWarpWindowTop > 0)
                Top = SettingsManager.Default.CreateCustomWarpWindowTop;
            
            AlwaysOnTopCheckBox.IsChecked = SettingsManager.Default.CreateCustomWarpWindowAlwaysOnTop;
        };
    }
}