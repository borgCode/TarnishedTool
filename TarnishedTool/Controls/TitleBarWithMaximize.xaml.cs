using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TarnishedTool.Controls;

public partial class TitleBarWithMaximize : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(TitleBarWithMaximize),
            new PropertyMetadata("Window"));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public TitleBarWithMaximize()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window != null)
        {
            window.StateChanged += Window_StateChanged;
            UpdateMaximizeIcon(window.WindowState);
        }
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        var window = (Window)sender;
        UpdateMaximizeIcon(window.WindowState);
    }

    private void UpdateMaximizeIcon(WindowState state)
    {
        if (state == WindowState.Maximized)
        {
            MaximizeIcon.Visibility = Visibility.Collapsed;
            RestoreIcon.Visibility = Visibility.Visible;
            MaximizeButton.ToolTip = "Restore Down";
        }
        else
        {
            MaximizeIcon.Visibility = Visibility.Visible;
            RestoreIcon.Visibility = Visibility.Collapsed;
            MaximizeButton.ToolTip = "Maximize";
        }
    }


    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window == null) return;

        // Double-click to maximize/restore
        if (e.ClickCount == 2)
        {
            ToggleMaximize();
        }
        else
        {
            window.DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window != null) window.WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleMaximize();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) =>
        Window.GetWindow(this)?.Close();

    private void ToggleMaximize()
    {
        var window = Window.GetWindow(this);
        if (window == null) return;

        if (window.WindowState == WindowState.Maximized)
        {
            window.WindowState = WindowState.Normal;
        }
        else
        {
            window.WindowState = WindowState.Maximized;
        }
    }
}