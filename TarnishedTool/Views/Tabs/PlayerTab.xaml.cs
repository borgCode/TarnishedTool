using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Tabs
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
            
            _ = new UpDownHelper<int>(
                FocusUpDown,
                _playerViewModel.SetFp,
                _playerViewModel.PauseUpdates,
                _playerViewModel.ResumeUpdates
            );
            
            _ = new UpDownHelper<int>(
                ScaduUpDown,
                _playerViewModel.SetScadu,
                _playerViewModel.PauseUpdates,
                _playerViewModel.ResumeUpdates
            );
            
            _ = new UpDownHelper<int>(
                SpiritAshUpDown,
                _playerViewModel.SetSpiritAsh,
                _playerViewModel.PauseUpdates,
                _playerViewModel.ResumeUpdates
            );
            
            
            
            var statControls = new[] 
            { 
                VigorUpDown, MindUpDown, EnduranceUpDown, 
                StrengthUpDown, DexterityUpDown, IntelligenceUpDown, 
                FaithUpDown, ArcaneUpDown 
            };
    
            foreach (var control in statControls)
            {
                var statName = control.Tag?.ToString();
                if (string.IsNullOrEmpty(statName)) continue;
        
                _ = new UpDownHelper<int>(
                    control,
                    value => _playerViewModel.SetStat(statName, value),
                    _playerViewModel.PauseUpdates,
                    _playerViewModel.ResumeUpdates
                );
            }
        }
    }
}