// 

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using TarnishedTool.Utilities;

namespace TarnishedTool.Views.Windows.AiOverlay;

public abstract class AiOverlayWindowBase : Window
{
    private readonly Action _onClosed;
    
    protected double BackgroundOpacity = 0.7;
    protected abstract string SettingsPrefix { get; }
    
    protected AiOverlayWindowBase(Action onClosed = null)
    {
        _onClosed = onClosed;
        
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        ShowInTaskbar = false;
        Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
        
        MouseLeftButtonDown += (s, e) => DragMove();
        
        Loaded += OnLoaded;
        Closing += OnClosing;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadSettings();
        
        IntPtr hwnd = new WindowInteropHelper(this).Handle;
        User32.SetTopmost(hwnd);

        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (s, args) => Close();
        }
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        SaveSettings();
        _onClosed?.Invoke();
    }
    
    private void LoadSettings()
    {
        var left = GetSetting("Left");
        var top = GetSetting("Top");
        var opacity = GetSetting("Opacity");

        if (left > 0) Left = left;
        if (top > 0) Top = top;
        if (opacity > 0) BackgroundOpacity = opacity;
        
        UpdateBackground();
    }
    
    private void SaveSettings()
    {
        SetSetting("Left", Left);
        SetSetting("Top", Top);
        SetSetting("Opacity", BackgroundOpacity);
        SettingsManager.Default.Save();
    }
    
    private double GetSetting(string name)
    {
        var prop = typeof(SettingsManager).GetProperty($"{SettingsPrefix}{name}");
        if (prop != null)
            return (double)prop.GetValue(SettingsManager.Default);
        return 0;
    }

    private void SetSetting(string name, double value)
    {
        var prop = typeof(SettingsManager).GetProperty($"{SettingsPrefix}{name}");
        prop?.SetValue(SettingsManager.Default, value);
    }
    
    protected void UpdateBackground()
    {
        Background = new SolidColorBrush(
            Color.FromArgb((byte)(BackgroundOpacity * 255), 0, 0, 0));
    }
    
    public void CycleOpacity()
    {
        BackgroundOpacity = BackgroundOpacity >= 0.9 ? 0.3 : BackgroundOpacity + 0.2;
        UpdateBackground();
    }
    
}