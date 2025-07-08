using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SilkyRing.Memory;
using SilkyRing.Services;
using SilkyRing.ViewModels;
using UtilityTab = SilkyRing.Views.UtilityTab;

namespace SilkyRing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MemoryIo _memoryIo;
        private readonly AoBScanner _aobScanner;

        private readonly DispatcherTimer _gameLoadedTimer;
        private readonly HookManager _hookManager;

        // private readonly PlayerViewModel _playerViewModel;
        // private readonly TravelViewModel _travelViewModel;
        // private readonly EventViewModel _eventViewModel;
        private readonly UtilityViewModel _utilityViewModel;
        // private readonly EnemyViewModel _enemyViewModel;
        // private readonly ItemViewModel _itemViewModel;
        // private readonly SettingsViewModel _settingsViewModel;
        //
        // private readonly DebugDrawService _debugDrawService;

        public MainWindow()
        {
            _memoryIo = new MemoryIo();
            _memoryIo.StartAutoAttach();
            InitializeComponent();

            //
            // if (SettingsManager.Default.WindowLeft != 0 || SettingsManager.Default.WindowTop != 0)
            // {
            //     Left = SettingsManager.Default.WindowLeft;
            //     Top = SettingsManager.Default.WindowTop;
            // }
            // else WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //
            // GameLauncher.SetVersionOffsets();
            _hookManager = new HookManager(_memoryIo);
            _aobScanner = new AoBScanner(_memoryIo);

            // var hotkeyManager = new HotkeyManager(_memoryIo);
            //
            // var playerService = new PlayerService(_memoryIo);
            var utilityService = new UtilityService(_memoryIo, _hookManager);
            // var eventService = new EventService(_memoryIo, _hookManager);
            // var enemyService = new EnemyService(_memoryIo, _hookManager);
            // var travelService = new TravelService(_memoryIo, _hookManager);
            // var cinderService = new CinderService(_memoryIo, _hookManager);
            // var itemService = new ItemService(_memoryIo);
            // var settingsService = new SettingsService(_memoryIo);
            // _debugDrawService = new DebugDrawService(_memoryIo);

            // _playerViewModel = new PlayerViewModel(playerService, hotkeyManager);
            _utilityViewModel =
                new UtilityViewModel(utilityService);
            // _travelViewModel = new TravelViewModel(travelService, hotkeyManager);
            // _eventViewModel = new EventViewModel(eventService);
            // var eventTab = new EventTab(_eventViewModel);
            // _enemyViewModel = new EnemyViewModel(enemyService, cinderService, hotkeyManager, _debugDrawService);
            // _itemViewModel = new ItemViewModel(itemService);
            // _settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager);


            // var playerTab = new PlayerTab(_playerViewModel);
            var utilityTab = new UtilityTab(_utilityViewModel);
            // var travelTab = new TravelTab(_travelViewModel);
            // var enemyTab = new EnemyTab(_enemyViewModel);
            // var itemTab = new ItemTab(_itemViewModel);
            // var settingsTab = new SettingsTab(_settingsViewModel);
            //
            // MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
            // MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
            // MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
            MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
            // MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
            // MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
            // MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });
            //
            // _settingsViewModel.ApplyStartUpOptions();
            // Closing += MainWindow_Closing;

            _gameLoadedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(25)
            };
            _gameLoadedTimer.Tick += Timer_Tick;
            _gameLoadedTimer.Start();
        }

        // VersionChecker.UpdateVersionText(AppVersion);
        //
        // if (SettingsManager.Default.EnableUpdateChecks)
        // {
        //     VersionChecker.CheckForUpdates(this);
        // }
        private bool _loaded;
        private bool _hasAppliedDelayedFeatures;

        private bool _hasScanned;

        private bool _hasAllocatedMemory;

        private bool _hasAppliedNoLogo;

        private bool _appliedOneTimeFeatures;
        private bool _hasAppliedLaunchFeatures;
        private int _storedArea;
        private int _currentArea;
        private bool _hasShownVersionError;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryIo.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                // LaunchGameButton.IsEnabled = false;

                if (!_hasScanned)
                {
                    _aobScanner.Scan();
                    _hasScanned = true;
                    Offsets.Initialize();
                    Console.WriteLine($"Base: 0x{_memoryIo.BaseAddress.ToInt64():X}");
                }


                if (!_hasAppliedLaunchFeatures)
                {
                    ApplyLaunchFeatures();
                }


                // _currentArea = _memoryIo.ReadInt32(Offsets.MapId);
                // if (_currentArea != _storedArea)
                // {
                //     _eventViewModel.AreaChange(_currentArea);
                //     _storedArea = _currentArea;
                // }

                if (!_hasAllocatedMemory)
                {
                    _memoryIo.AllocCodeCave();
                    Console.WriteLine($"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
                    _hasAllocatedMemory = true;
                    // _damageControlService.WriteDamageControlCode();
                    // _dllManager.CreateDrawSharedMem();
                    // _dllManager.CreateSpeedSharedMem();
                }

                if (_memoryIo.IsGameLoaded())
                {
                    if (!_hasAppliedDelayedFeatures)
                    {
                        // if (!_memoryIo.IsLoadingScreen())
                        // {
                        //     ApplyDelayedFeatures();
                        //     _hasAppliedDelayedFeatures = true;
                        // }
                    }

                    if (_loaded) return;
                    _loaded = true;
                    TryEnableFeatures();
                    // _settingsViewModel.ApplyLoadedOptions();
                    if (_appliedOneTimeFeatures) return;
                    ApplyOneTimeFeatures();
                    _appliedOneTimeFeatures = true;
                }
                else if (_loaded)
                {
                    DisableFeatures();
                    _loaded = false;
                    _hasAppliedDelayedFeatures = false;
                }
            }
            else
            {
                _hookManager.ClearHooks();
                DisableFeatures();
                // _nopManager.ClearRegistry();
                ResetState();
                _hasScanned = false;
                _loaded = false;
                _hasAllocatedMemory = false;
                _appliedOneTimeFeatures = false;
                IsAttachedText.Text = "Not attached";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                // LaunchGameButton.IsEnabled = true;
            }
        }

        private void ApplyDelayedFeatures()
        {
            // _utilityViewModel.ApplyDelayedFeatures();
        }

        private void ResetState()
        {
            // _dllManager.ResetState();
            // _settingsViewModel.ResetAttached();
            // _itemService.Reset();
            // _utilityViewModel.Reset();
        }

        private void ApplyLaunchFeatures()
        {
            // _playerViewModel.ApplyLaunchFeatures();
            // _itemViewModel.ApplyLaunchFeatures();
            // _utilityViewModel.ApplyLaunchFeatures();
        }

        private void ApplyOneTimeFeatures()
        {
            // _playerViewModel.TryApplyOneTimeFeatures();
            // _utilityViewModel.TryApplyOneTimeFeatures();
            // _enemyViewModel.TryApplyOneTimeFeatures();
            // _eventViewModel.TryApplyOneTimeFeatures();
        }

        private void TryEnableFeatures()
        {
            // _playerViewModel.TryEnableFeatures();
            // _utilityViewModel.TryEnableFeatures();
            // _enemyViewModel.TryEnableFeatures();
            // _itemViewModel.TryEnableFeatures();
            // _travelViewModel.TryEnableFeatures();
            // _eventViewModel.TryEnableFeatures();
        }

        private void DisableFeatures()
        {
            // _utilityViewModel.DisableFeatures();
            // _playerViewModel.DisableFeatures();
            // _enemyViewModel.DisableFeatures();
            // _itemViewModel.DisableFeatures();
            // _travelViewModel.DisableFeatures();
            // _eventViewModel.DisableFeatures();
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

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // SettingsManager.Default.WindowLeft = Left;
            // SettingsManager.Default.WindowTop = Top;
            // SettingsManager.Default.Save();
            // _itemService.SignalClose();
            _hookManager.UninstallAllHooks();
            // _nopManager.RestoreAll();
        }

        // private void LaunchGame_Click(object sender, RoutedEventArgs e) => Task.Run(GameLauncher.LaunchDarkSouls2);
        // private void CheckUpdate_Click(object sender, RoutedEventArgs e) => VersionChecker.CheckForUpdates(this, true);
    }
}