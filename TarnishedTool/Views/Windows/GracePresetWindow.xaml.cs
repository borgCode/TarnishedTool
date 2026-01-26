// 

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class GracePresetWindow : TopmostWindow
{
    public GracePresetWindow(
        SearchableGroupedCollection<string, Grace> graces,
        Dictionary<string, GracePresetTemplate> customPresetTemplates)
    {
        InitializeComponent();

        var viewModel = new GracePresetViewModel(
            graces,
            customPresetTemplates);

        DataContext = viewModel;

        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (_, _) => Close();
        }
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.GracePresetWindowLeft > 0)
                Left = SettingsManager.Default.GracePresetWindowLeft;
        
            if (SettingsManager.Default.GracePresetWindowTop > 0)
                Top = SettingsManager.Default.GracePresetWindowTop;
            
            AlwaysOnTopCheckBox.IsChecked = SettingsManager.Default.GracePresetWindowAlwaysOnTop;
        };
    }

    
    private void Grace_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is GracePresetViewModel vm && vm.AddGraceCommand.CanExecute(null))
        {
            vm.AddGraceCommand.Execute(null);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void AddedGrace_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is GracePresetViewModel vm && vm.RemoveGraceCommand.CanExecute(null))
        {
            vm.RemoveGraceCommand.Execute(null);
        }
    }
    
    private void AddToPreset_Click(object sender, RoutedEventArgs e)
    {
        var vm = (GracePresetViewModel)DataContext;
        var selected = AddGraceListView.SelectedItems.Cast<Grace>().ToList();
        vm.AddGraces(selected);
    }
}