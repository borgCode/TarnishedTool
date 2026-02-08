using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TarnishedTool.Models;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class CreateLoadoutWindow : Window
{
    public CreateLoadoutWindow(
        Dictionary<string, List<Item>> itemsByCategory,
        List<AshOfWar> ashesOfWar,
        Dictionary<string, LoadoutTemplate> customLoadoutTemplates,
        bool hasDlc)
    {
        InitializeComponent();
        
        var viewModel = new CreateLoadoutViewModel(
            itemsByCategory,
            ashesOfWar,
            customLoadoutTemplates,
            hasDlc,
            ShowInputDialog);
        
        DataContext = viewModel;
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (_, _) => Close();
        }
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

    
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is CreateLoadoutViewModel vm && vm.AddItemCommand.CanExecute(null))
        {
            vm.AddItemCommand.Execute(null);
        }
    }

    private void AddToLoadout_Click(object sender, RoutedEventArgs e)
    {
        var vm = (CreateLoadoutViewModel)DataContext;
        var listView = FindItemsListView(this);
        if (listView != null)
        {
            var selected = listView.SelectedItems.Cast<Item>().ToList();
            foreach (var item in selected)
            {
                vm.ItemSelection.SelectedItem = item;
                vm.AddItemCommand.Execute(null);
            }
        }
    }

    private void RemoveFromLoadout_Click(object sender, RoutedEventArgs e)
    {
        var vm = (CreateLoadoutViewModel)DataContext;
        var listView = FindLoadoutItemsListView(this);
        if (listView != null)
        {
            var selected = listView.SelectedItems.Cast<ItemTemplate>().ToList();
            foreach (var item in selected)
            {
                vm.SelectedLoadoutItem = item;
                vm.RemoveItemCommand.Execute(null);
            }
        }
    }

    private ListView FindItemsListView(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is ListView listView && listView.ItemsSource == ((CreateLoadoutViewModel)DataContext).ItemSelection.Items)
            {
                return listView;
            }

            var result = FindItemsListView(child);
            if (result != null) return result;
        }
        return null;
    }

    private ListView FindLoadoutItemsListView(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is ListView listView && listView.ItemsSource == ((CreateLoadoutViewModel)DataContext).CurrentLoadoutItems)
            {
                return listView;
            }

            var result = FindLoadoutItemsListView(child);
            if (result != null) return result;
        }
        return null;
    }
}