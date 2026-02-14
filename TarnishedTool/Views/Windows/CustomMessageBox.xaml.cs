// 

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using TarnishedTool.Utilities;

namespace TarnishedTool.Views.Windows;

public partial class CustomMessageBox : Window
{
    public bool Result { get; private set; }


    public CustomMessageBox(string message, bool showCancel, string title = "Message")
    {
        InitializeComponent();
        MessageText.Text = message;
        TitleText.Text = title;
            
        if (showCancel)
        {
            CancelButton.Visibility = Visibility.Visible;
        }
        
        SetupWindow();
    }

    // Yes/No buttons
    public CustomMessageBox(string message, string title, bool showYesNo)
    {
        InitializeComponent();
        MessageText.Text = message;
        TitleText.Text = title;

        if (showYesNo)
        {
            OkButton.Visibility = Visibility.Collapsed;
            YesButton.Visibility = Visibility.Visible;
            NoButton.Visibility = Visibility.Visible;
        }

        SetupWindow();
    }

    private void SetupWindow()
    {
        Loaded += (s, e) =>
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            User32.SetTopmost(hwnd);

            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
            }
        };
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

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}