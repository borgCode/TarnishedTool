using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class ResistancesWindow : Window
{
    private double _scaleMultiplier = 1.0;
    private double _backgroundOpacity = 0.5;

    public ResistancesWindow()
    {
        InitializeComponent();

        MouseLeftButtonDown += (s, e) => DragMove();
        Background = new SolidColorBrush(
            Color.FromArgb(128, 0, 0, 0));

        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.ResistancesWindowLeft > 0)
                Left = SettingsManager.Default.ResistancesWindowLeft;

            if (SettingsManager.Default.ResistancesWindowTop > 0)
                Top = SettingsManager.Default.ResistancesWindowTop;

            if (SettingsManager.Default.ResistancesWindowScaleX > 0)
            {
                _scaleMultiplier = SettingsManager.Default.ResistancesWindowScaleX;
                ContentScale.ScaleX = _scaleMultiplier;
                ContentScale.ScaleY = _scaleMultiplier;
            }

            if (SettingsManager.Default.ResistancesWindowOpacity > 0)
                _backgroundOpacity = SettingsManager.Default.ResistancesWindowOpacity;

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
            if (SettingsManager.Default.ResistancesWindowWidth > 0)
            {
                Width = SettingsManager.Default.ResistancesWindowWidth;
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
            viewModel.IsResistancesWindowOpen = false;
        }

        Close();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        SettingsManager.Default.ResistancesWindowScaleX = _scaleMultiplier;
        SettingsManager.Default.ResistancesWindowOpacity = _backgroundOpacity;
        SettingsManager.Default.ResistancesWindowLeft = Left;
        SettingsManager.Default.ResistancesWindowTop = Top;
        SettingsManager.Default.ResistancesWindowWidth = Width;

        if (DataContext is TargetViewModel vm)
        {
            SettingsManager.Default.ResistancesShowCombatInfo = vm.ShowCombatInfo;
        }

        SettingsManager.Default.Save();
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

    private void ToggleCombatInfo_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TargetViewModel vm)
            vm.ShowCombatInfo = !vm.ShowCombatInfo;
    }
}