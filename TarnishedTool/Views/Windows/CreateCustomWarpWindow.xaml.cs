// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
        IPlayerService playerService, IGameTickService gameTickService, Action<CustomWarpChange> onChange)
    {
        InitializeComponent();

        var viewModel = new CreateCustomWarpViewModel(
            customWarps,
            areOptionsEnabled,
            stateService,
            playerService,
            gameTickService,
            onChange
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

    private void DeleteCustomWarpListView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            var vm = (CreateCustomWarpViewModel)DataContext;
            var itemsToDelete = DeleteCustomWarpListView.SelectedItems.Cast<BlockWarp>().ToList();
            vm.DeleteWarps(itemsToDelete);
        }
    }
}