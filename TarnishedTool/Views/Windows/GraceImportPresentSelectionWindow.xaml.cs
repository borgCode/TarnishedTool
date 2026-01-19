// 

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class GraceImportPresentSelectionWindow : TopmostWindow
{
    public GraceImportPresetSelectionViewModel ViewModel { get; }

    public GraceImportPresentSelectionWindow(
        List<GracePresetTemplate> importedPresets,
        Dictionary<string, GracePresetTemplate> existingPresets)
    {
        InitializeComponent();
        ViewModel = new GraceImportPresetSelectionViewModel(importedPresets, existingPresets);
        DataContext = ViewModel;
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }
        
        if (SettingsManager.Default.GraceImportWindowLeft > 0)
            Left = SettingsManager.Default.GraceImportWindowLeft;
        
        if (SettingsManager.Default.GraceImportWindowTop > 0)
            Top = SettingsManager.Default.GraceImportWindowTop;
    }

    private void PresetItem_Selected(object sender, RoutedEventArgs e)
    {
        if (((ListViewItem)sender).DataContext is GraceImportItem item)
        {
            item.IsSelected = true;
        }
    }

    private void PresetItem_Unselected(object sender, RoutedEventArgs e)
    {
        if (((ListViewItem)sender).DataContext is GraceImportItem item)
        {
            item.IsSelected = false;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);


        SettingsManager.Default.GraceImportWindowLeft = Left;
        SettingsManager.Default.GraceImportWindowTop = Top;
        SettingsManager.Default.Save();
    }
}