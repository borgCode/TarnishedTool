// 

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows.AiOverlay;

public partial class AiOverlayToolbar : Window
{
    private readonly AiWindowViewModel _viewModel;
    private readonly Action _onExit;

    private readonly Dictionary<CheckBox, Window> _openOverlays = new();
    private readonly Dictionary<CheckBox, Func<Window>> _overlayFactories;
    private readonly Dictionary<CheckBox, Action<bool>> _viewModelSetters;

    public AiOverlayToolbar(AiWindowViewModel viewModel, string entityName, Action onExit)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _onExit = onExit;
        EntityNameText.Text = entityName;

        MouseLeftButtonDown += (s, e) => DragMove();


        _overlayFactories = new()
        {
            [GoalsCheckBox] = () => new GoalsOverlayWindow(_viewModel, () => OnOverlayClosed(GoalsCheckBox)),
            [CoolTimesCheckBox] = () => new CoolTimesOverlayWindow(_viewModel, () => OnOverlayClosed(CoolTimesCheckBox)),
            [LuaTimersCheckBox] = () => new LuaTimersOverlayWindow(_viewModel, () => OnOverlayClosed(LuaTimersCheckBox)),
            [LuaNumbersCheckBox] = () => new LuaNumbersOverlayWindow(_viewModel, () => OnOverlayClosed(LuaNumbersCheckBox)),
            [SpEffectObservesCheckBox] = () => new SpEffectObserversOverlayWindow(_viewModel, () => OnOverlayClosed(SpEffectObservesCheckBox)),
            [InterruptsCheckBox] = () => new InterruptsOverlayWindow(_viewModel, () => OnOverlayClosed(InterruptsCheckBox)),
            [SpEffectsCheckBox] = () => new SpEffectsOverlayWindow(_viewModel, () => OnOverlayClosed(SpEffectsCheckBox)),
        };
        _viewModelSetters = new()
        {
            [GoalsCheckBox] = v => _viewModel.IsShowGoalsEnabled = v,
            [CoolTimesCheckBox] = v => _viewModel.IsShowCoolTimesEnabled = v,
            [LuaTimersCheckBox] = v => _viewModel.IsShowLuaTimersEnabled = v,
            [LuaNumbersCheckBox] = v => _viewModel.IsShowLuaNumbersEnabled = v,
            [SpEffectObservesCheckBox] = v => _viewModel.IsShowSpEffectObservesEnabled = v,
            [InterruptsCheckBox] = v => _viewModel.IsShowInterruptsEnabled = v,
            [SpEffectsCheckBox] = v => _viewModel.IsShowSpEffectsEnabled = v,
        };

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadSettings();
        
        
        IntPtr hwnd = new WindowInteropHelper(this).Handle;
        User32.SetTopmost(hwnd);

        GoalsCheckBox.IsChecked = _viewModel.IsShowGoalsEnabled;
        CoolTimesCheckBox.IsChecked = _viewModel.IsShowCoolTimesEnabled;
        LuaTimersCheckBox.IsChecked = _viewModel.IsShowLuaTimersEnabled;
        LuaNumbersCheckBox.IsChecked = _viewModel.IsShowLuaNumbersEnabled;
        SpEffectObservesCheckBox.IsChecked = _viewModel.IsShowSpEffectObservesEnabled;
        InterruptsCheckBox.IsChecked = _viewModel.IsShowInterruptsEnabled;
        SpEffectsCheckBox.IsChecked = _viewModel.IsShowSpEffectsEnabled;

        foreach (var cb in _overlayFactories.Keys)
            if (cb.IsChecked == true)
                OpenOverlay(cb);
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }
        
    }

    private void LoadSettings()
    {
        if (SettingsManager.Default.AiOverlayToolbarLeft > 0)
            Left = SettingsManager.Default.AiOverlayToolbarLeft;
        
        if (SettingsManager.Default.AiOverlayToolbarTop > 0)
            Top = SettingsManager.Default.AiOverlayToolbarTop;
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        
        SettingsManager.Default.AiOverlayToolbarLeft = Left;
        SettingsManager.Default.AiOverlayToolbarTop = Top;
        SettingsManager.Default.Save();
    }
    
    private void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox cb) return;

        bool isChecked = cb.IsChecked == true;
        _viewModelSetters[cb](isChecked);

        if (isChecked) OpenOverlay(cb);
        else CloseOverlay(cb);
    }

    private void OpenOverlay(CheckBox cb)
    {
        if (_openOverlays.ContainsKey(cb)) return;
        if (!_overlayFactories.TryGetValue(cb, out var factory)) return;

        var overlay = factory();
        _openOverlays[cb] = overlay;
        overlay.Show();
    }

    private void CloseOverlay(CheckBox cb)
    {
        if (!_openOverlays.TryGetValue(cb, out var overlay)) return;
        _openOverlays.Remove(cb);
        overlay.Close();
    }

    private void OnOverlayClosed(CheckBox cb)
    {
        _openOverlays.Remove(cb);
        cb.IsChecked = false;
        _viewModelSetters[cb](false);
    }

    private void AllOn_Click(object sender, RoutedEventArgs e)
    {
        foreach (var cb in _viewModelSetters.Keys) cb.IsChecked = true;
    }

    private void AllOff_Click(object sender, RoutedEventArgs e)
    {
        foreach (var cb in _viewModelSetters.Keys) cb.IsChecked = false;
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        foreach (var overlay in _openOverlays.Values) overlay.Close();
        _openOverlays.Clear();
        _onExit?.Invoke();
        Close();
    }
}