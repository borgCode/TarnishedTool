// 

using System;
using System.Collections.Generic;

namespace TarnishedTool.Core;

public class NavigationHistory<T>(T initial)
{
    private Stack<T> _backStack = new();
    private Stack<T> _forwardStack = new();
    private T _current = initial;
    
    public void Navigate(T item)
    {
        _backStack.Push(_current);
        _current = item;
        _forwardStack.Clear(); 
    }
    
    public T GoBack()
    {
        if (_backStack.Count == 0) 
            throw new InvalidOperationException("Nothing to go back to");
        
        _forwardStack.Push(_current);
        _current = _backStack.Pop();
        return _current;
    }

    public T GoForward()
    {
        if (_forwardStack.Count == 0) 
            throw new InvalidOperationException("Nothing to go forward to");
        
        _backStack.Push(_current);
        _current = _forwardStack.Pop();
        return _current;
    }

    public bool CanGoBack => _backStack.Count > 0;
    public bool CanGoForward => _forwardStack.Count > 0;
    public T Current => _current;
}