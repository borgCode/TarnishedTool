using System.Windows;
using System.Windows.Controls;
using SilkyRing.ViewModels;

namespace SilkyRing.Views
{
    public partial class TravelTab : UserControl
    {
        private readonly TravelViewModel _travelViewModel;

        public TravelTab(TravelViewModel travelViewModel)
        {
            InitializeComponent();
            _travelViewModel = travelViewModel;
            DataContext = _travelViewModel;
        }

        private void WarpButton_Click(object sender, RoutedEventArgs e)
        {
            _travelViewModel.Warp();
        }

        private void UnlockAllMainGameGraces_Click(object sender, RoutedEventArgs e) => _travelViewModel.UnlockMainGameGraces();

        private void UnlockAllDlcGraces_Click(object sender, RoutedEventArgs e)=> _travelViewModel.UnlockDlcGraces();
    }
}