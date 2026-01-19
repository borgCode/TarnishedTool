// 

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            customPresetTemplates,
            ShowInputDialog);

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

    private string ShowInputDialog(string prompt, string defaultValue)
    {
        var dialog = new Window
        {
            Title = "Input",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = (Brush)Application.Current.Resources["BackgroundBrush"],
            Foreground = (Brush)Application.Current.Resources["TextBrush"]
        };

        var panel = new StackPanel { Margin = new Thickness(10) };
        panel.Children.Add(new TextBlock { Text = prompt, Margin = new Thickness(0, 0, 0, 10) });

        var textBox = new TextBox { Text = defaultValue, Margin = new Thickness(0, 0, 0, 10) };
        panel.Children.Add(textBox);

        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

        var okButton = new Button { Content = "OK", Width = 60, IsDefault = true, Margin = new Thickness(0, 0, 5, 0) };
        okButton.Click += (_, _) => dialog.DialogResult = true;
        buttonPanel.Children.Add(okButton);

        var cancelButton = new Button { Content = "Cancel", Width = 60, IsCancel = true };
        cancelButton.Click += (_, _) => dialog.DialogResult = false;
        buttonPanel.Children.Add(cancelButton);

        panel.Children.Add(buttonPanel);
        dialog.Content = panel;

        return dialog.ShowDialog() == true ? textBox.Text : string.Empty;
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