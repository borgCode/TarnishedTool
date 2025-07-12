using System.Windows;
using System.Windows.Input;

namespace SilkyRing.Views
{
    public partial class LoggerWindow : Window
    {
        public LoggerWindow()
        {
            InitializeComponent();
        }

        private void ClearUniqueSetEvents_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ClearUniqueSpEffects_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ClearConsole_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void PauseAllLogging_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}