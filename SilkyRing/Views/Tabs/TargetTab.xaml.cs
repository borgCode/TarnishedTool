using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilkyRing.ViewModels;
using Xceed.Wpf.Toolkit;

namespace SilkyRing.Views
{
    public partial class TargetTab
    {
        private readonly TargetViewModel _targetViewModel;

        public TargetTab(TargetViewModel targetViewModel)
        {
            InitializeComponent();
            _targetViewModel = targetViewModel;
            DataContext = _targetViewModel;
        }
        
        private void OnHealthButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string parameter = button.CommandParameter.ToString();
            int healthPercentage = int.Parse(parameter);
            _targetViewModel.SetTargetHealth(healthPercentage);
        }
        
        private void SpeedUpDown_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key != Key.Enter && e.Key != Key.Return) return;
            var upDown = sender as DoubleUpDown;
            if (upDown?.Value.HasValue == true)
            {
                // _enemyViewModel.SetSpeed((float)upDown.Value);
            }
            Focus();
            e.Handled = true;
        }

        private void SpeedUpDown_LostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var upDown = sender as DoubleUpDown;
            if (upDown?.Value.HasValue == true)
            {
                // _enemyViewModel.SetSpeed((float)upDown.Value);
            }
        }

        private void OpenDefenseWindow(object sender, RoutedEventArgs e) => _targetViewModel.OpenDefenseWindow();
    }
}