using System;
using System.Windows.Input;

namespace SilkyRing.Core;

internal class DelegateCommand(Action<object> execute, Predicate<object>? canExecute = null)
    : ICommand
{
    private readonly Action? _executeWithoutParam;

    public DelegateCommand(Action execute, Predicate<object>? canExecute = null) : this(default(Action<object>), canExecute)
    {
        _executeWithoutParam = execute;
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChange() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter ?? new()) ?? true;

    public void Execute(object? parameter)
    {
        execute?.Invoke(parameter ?? new());
        _executeWithoutParam?.Invoke();
    }
}

internal class DelegateCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    public DelegateCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChange() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter is T t ? t : default) ?? true;

    public void Execute(object? parameter)
    {
        if (parameter is T t)
            _execute(t);
        else
            _execute(default);
    }
}



