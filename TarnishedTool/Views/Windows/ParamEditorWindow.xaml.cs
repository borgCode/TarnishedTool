// 

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class ParamEditorWindow : TopmostWindow
{
    public ParamEditorWindow()
    {
        InitializeComponent();

        CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll,
            (s, e) => { EntriesListView.SelectAll(); }));

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s, e) =>
        {
            var selectedItems = EntriesListView.SelectedItems
                .Cast<ParamEntry>()
                .Select(x => $"{x.Id}: {x.DisplayName}");

            Clipboard.SetText(string.Join(Environment.NewLine, selectedItems));
        }));

        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }

        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.ParamEditorWindowLeft > 0)
                Left = SettingsManager.Default.ParamEditorWindowLeft;

            if (SettingsManager.Default.ParamEditorWindowTop > 0)
                Top = SettingsManager.Default.ParamEditorWindowTop;

            if (SettingsManager.Default.ParamEditorWindowWidth > 0)
                Width = SettingsManager.Default.ParamEditorWindowWidth;

            if (SettingsManager.Default.ParamEditorWindowHeight > 0)
                Height = SettingsManager.Default.ParamEditorWindowHeight;

            AlwaysOnTopCheckBox.IsChecked = SettingsManager.Default.ParamEditorWindowAlwaysOnTop;
        };
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        // Save window position and size
        SettingsManager.Default.ParamEditorWindowLeft = Left;
        SettingsManager.Default.ParamEditorWindowTop = Top;
        SettingsManager.Default.ParamEditorWindowWidth = Width;
        SettingsManager.Default.ParamEditorWindowHeight = Height;
        SettingsManager.Default.ParamEditorWindowAlwaysOnTop = AlwaysOnTopCheckBox.IsChecked ?? false;
        SettingsManager.Default.Save();
    }

    private void PinnedItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem item && item.DataContext is ParamEntry entry)
        {
            ((ParamEditorViewModel)DataContext).NavigateToPinnedCommand.Execute(entry);
        }
    }

    private void UnpinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (PinnedListView.SelectedItem is ParamEntry entry)
        {
            ((ParamEditorViewModel)DataContext).TogglePinCommand.Execute(entry);
        }
    }

    private void EnumMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem) return;

        var value = menuItem.Tag;

        // Navigate up to find the ContextMenu and then the TextBox
        var contextMenu = menuItem.Parent as ContextMenu;
        if (contextMenu?.PlacementTarget is TextBox textBox)
        {
            if (textBox.DataContext is FieldValueViewModel fieldVm)
            {
                fieldVm.ValueText = value?.ToString() ?? "";
            }
        }
    }

    private void EntriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (EntriesListView.SelectedItem != null)
        {
            EntriesListView.ScrollIntoView(EntriesListView.SelectedItem);
        }
    }
}