// 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TarnishedTool;

public static class TextBoxBehavior
{
    public static readonly DependencyProperty CommitOnEnterProperty =
        DependencyProperty.RegisterAttached(
            "CommitOnEnter",
            typeof(bool),
            typeof(TextBoxBehavior),
            new PropertyMetadata(false, OnCommitOnEnterChanged));

    public static bool GetCommitOnEnter(DependencyObject obj) => (bool)obj.GetValue(CommitOnEnterProperty);
    public static void SetCommitOnEnter(DependencyObject obj, bool value) => obj.SetValue(CommitOnEnterProperty, value);

    private static void OnCommitOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
                textBox.KeyDown += TextBox_KeyDown;
            else
                textBox.KeyDown -= TextBox_KeyDown;
        }
    }

    private static void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is TextBox textBox)
        {
            textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            Keyboard.ClearFocus();
        }
    }
}