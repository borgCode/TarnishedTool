using System.Windows.Controls;
using SilkyRing.ViewModels;

namespace SilkyRing.Views
{
    public partial class UtilityTab : UserControl
    {
        private readonly UtilityViewModel _utilityViewModel;
        public UtilityTab(UtilityViewModel utilityViewModel)
        {
            InitializeComponent();
            _utilityViewModel = utilityViewModel;
            DataContext = utilityViewModel;
        }
    }
}