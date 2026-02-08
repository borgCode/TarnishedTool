using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Primitives;

namespace TarnishedTool.Utilities;

public class UpDownHelper<T> where T : struct
{
    private readonly UpDownBase<T?> _upDown;
    private readonly Action? _pauseUpdates;
    private readonly Action? _resumeUpdates;
    private readonly Action<T> _setValue;
    
    
    public UpDownHelper(
        UpDownBase<T?> upDown,
        Action<T> setValue,
        Action? pauseUpdates = null,
        Action? resumeUpdates = null)
    {
        _upDown = upDown;
        _pauseUpdates = pauseUpdates;
        _resumeUpdates = resumeUpdates;
        _setValue = setValue;

        AttachHandlers();
    }
    
    private void AttachHandlers()
    {
        _upDown.Loaded += UpDown_Loaded;
        _upDown.LostFocus += UpDown_LostFocus;
        _upDown.PreviewKeyDown += UpDown_PreviewKeyDown;
    }
    
    private void UpDown_Loaded(object sender, RoutedEventArgs e)
    {
        if (_upDown.Template.FindName("PART_TextBox", _upDown) is TextBox textBox)
        {
            textBox.GotFocus += (s, args) => _pauseUpdates?.Invoke();
        }

        var spinner = _upDown.Template.FindName("PART_Spinner", _upDown);
        if (spinner == null) return;

        var type = spinner.GetType();
        var incField = type.GetField("_increaseButton", BindingFlags.Instance | BindingFlags.NonPublic);
        var decField = type.GetField("_decreaseButton", BindingFlags.Instance | BindingFlags.NonPublic);

        if (incField?.GetValue(spinner) is ButtonBase incBtn)
            incBtn.Click += HandleSpinnerClick;

        if (decField?.GetValue(spinner) is ButtonBase decBtn)
            decBtn.Click += HandleSpinnerClick;
    }

    private void HandleSpinnerClick(object sender, RoutedEventArgs e)
    {
        _pauseUpdates?.Invoke();
        if (_upDown.Value.HasValue)
        {
            _setValue(_upDown.Value.Value);
        }
        _resumeUpdates?.Invoke();
    }

    private void UpDown_LostFocus(object sender, RoutedEventArgs e)
    {
        if (_upDown.Value.HasValue)
        {
            _setValue(_upDown.Value.Value);
        }
        _resumeUpdates?.Invoke();
    }

    private void UpDown_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter && e.Key != Key.Return) return;
        if (_upDown.Value.HasValue)
        {
            _setValue(_upDown.Value.Value);
        }
        _upDown.Focus();
        _resumeUpdates?.Invoke();
        e.Handled = true;
    }
    
    
}