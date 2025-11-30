// 

using SilkyRing.Views.Windows;

namespace SilkyRing.Utilities;

/// <summary>
/// Static helper class to show custom message boxes from anywhere in the application.
/// </summary>
public static class MsgBox
{
    /// <summary>
    /// Shows a message box with only an OK button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void Show(string message)
    {
        var box = new CustomMessageBox(message, showCancel: false);
        box.ShowDialog();
    }

    /// <summary>
    /// Shows a message box with OK and Cancel buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <returns>True if OK was clicked, false if Cancel was clicked.</returns>
    public static bool ShowOkCancel(string message)
    {
        var box = new CustomMessageBox(message, showCancel: true);
        box.ShowDialog();
        return box.Result;
    }
}