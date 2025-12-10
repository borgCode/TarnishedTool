using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SilkyRing.Utilities;
using SilkyRing.ViewModels;
using Xceed.Wpf.Toolkit;

namespace SilkyRing.Views
{
    public partial class PlayerTab
    {
        private readonly PlayerViewModel _playerViewModel;

        public PlayerTab(PlayerViewModel playerViewModel)
        {
            InitializeComponent();
            _playerViewModel = playerViewModel;
            DataContext = _playerViewModel;
            
            InitializeUpDownHelpers();
        }

        private void InitializeUpDownHelpers()
        {
            _ = new UpDownHelper<int>(
                HealthUpDown,
                _playerViewModel.SetHp,
                _playerViewModel.PauseUpdates,
                _playerViewModel.ResumeUpdates
            );
        }
    }
}