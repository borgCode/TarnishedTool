// 

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class DefensesWindow : Window
{
    private double _scaleMultiplier = 1.0;
    private double _backgroundOpacity = 0.5;
    
    public DefensesWindow()
    {
        
        InitializeComponent();

        MouseLeftButtonDown += (s, e) => DragMove();
        Background = new SolidColorBrush(
            Color.FromArgb(128, 0, 0, 0));

        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.DefensesWindowLeft > 0)
                Left = SettingsManager.Default.DefensesWindowLeft;

            if (SettingsManager.Default.DefensesWindowTop > 0)
                Top = SettingsManager.Default.DefensesWindowTop;

            if (SettingsManager.Default.DefensesWindowScaleX > 0)
            {
                _scaleMultiplier = SettingsManager.Default.DefensesWindowScaleX;
                ContentScale.ScaleX = _scaleMultiplier;
                ContentScale.ScaleY = _scaleMultiplier;
            }

            if (SettingsManager.Default.DefensesWindowOpacity > 0)
                _backgroundOpacity = SettingsManager.Default.DefensesWindowOpacity;

            UpdateBackground();

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            User32.SetTopmost(hwnd);

            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
            }
        };
        ContentRendered += (s, e) =>
        {
            if (SettingsManager.Default.DefensesWindowWidth > 0)
            {
                Width = SettingsManager.Default.DefensesWindowWidth;
            }
        };
    }
    
    private void UpdateBackground()
    {
        Background = new SolidColorBrush(
            Color.FromArgb((byte)(_backgroundOpacity * 255), 0, 0, 0));

        MainBorder.Background = new SolidColorBrush(
            Color.FromArgb((byte)(_backgroundOpacity * 255), 0, 0, 0));
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TargetViewModel viewModel)
        {
            viewModel.IsShowDefensesEnabled = false;
        }

        Close();
    }
    
    private void CycleOpacity_Click(object sender, RoutedEventArgs e)
    {
        _backgroundOpacity = _backgroundOpacity >= 0.9 ? 0.3 : _backgroundOpacity + 0.2;
        UpdateBackground();
    }


    private void DecreaseSize_Click(object sender, RoutedEventArgs e)
    {
        if (_scaleMultiplier > 0.6)
        {
            Width *= _scaleMultiplier / (_scaleMultiplier + 0.2);
            _scaleMultiplier -= 0.2;
            ContentScale.ScaleX = _scaleMultiplier;
            ContentScale.ScaleY = _scaleMultiplier;
        }
    }

    private void IncreaseSize_Click(object sender, RoutedEventArgs e)
    {
        if (_scaleMultiplier < 3.0)
        {
            Width *= (_scaleMultiplier + 0.2) / _scaleMultiplier;
            _scaleMultiplier += 0.2;
            ContentScale.ScaleX = _scaleMultiplier;
            ContentScale.ScaleY = _scaleMultiplier;
        }
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        SettingsManager.Default.DefensesWindowScaleX = _scaleMultiplier;
        SettingsManager.Default.DefensesWindowOpacity = _backgroundOpacity;
        SettingsManager.Default.DefensesWindowLeft = Left;
        SettingsManager.Default.DefensesWindowTop = Top;
        SettingsManager.Default.DefensesWindowWidth = Width;

        SettingsManager.Default.Save();
    }
}