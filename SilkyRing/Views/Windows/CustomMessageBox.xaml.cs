// 

using System.Windows;
using System.Windows.Input;

namespace SilkyRing.Views.Windows;

public partial class CustomMessageBox : Window
{
    public bool Result { get; private set; }

    public CustomMessageBox(string message, bool showCancel)
    {
        InitializeComponent();
        MessageText.Text = message;
            
        if (showCancel)
        {
            CancelButton.Visibility = Visibility.Visible;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}