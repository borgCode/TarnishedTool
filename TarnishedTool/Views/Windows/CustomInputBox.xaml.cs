// 

using System.Windows;

namespace TarnishedTool.Views.Windows;

public partial class CustomInputBox : Window
{
    public string InputText { get; private set; }
    public bool Result { get; private set; }

    public CustomInputBox(string title, string defaultText = "")
    {
        InitializeComponent();
        Title = title;
        InputTextBox.Text = defaultText;
        
        Loaded += (s, e) => 
        {
            InputTextBox.Focus();
            InputTextBox.SelectAll();
        };
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        InputText = InputTextBox.Text;
        Result = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}