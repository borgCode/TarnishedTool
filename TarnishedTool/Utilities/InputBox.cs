// 

using TarnishedTool.Views.Windows;

namespace TarnishedTool.Utilities;

/// <summary>
/// Static helper class to show input dialogs from anywhere in the application.
/// </summary>
public static class InputBox
{
    /// <summary>
    /// Shows an input dialog to get text from the user.
    /// </summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="defaultText">The default text to show in the input box.</param>
    /// <returns>The entered text if OK was clicked, null if Cancel was clicked.</returns>
    public static string Show(string title, string defaultText = "")
    {
        var box = new CustomInputBox(title, defaultText);
        box.ShowDialog();
        return box.Result ? box.InputText : null;
    }
}