using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Services;
using SilkyRing.Utilities;
using SilkyRing.ViewModels;
using SilkyRing.Views;
using SilkyRing.Views.Tabs;
using static SilkyRing.Memory.Offsets;
using UtilityTab = SilkyRing.Views.Tabs.UtilityTab;

namespace SilkyRing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MemoryService _memoryService;
        private readonly IStateService _stateService;
        private readonly IDlcService _dlcService;
        private readonly AoBScanner _aobScanner;

        private readonly DispatcherTimer _gameLoadedTimer;

        public MainWindow()
        {
            _memoryService = new MemoryService();
            _memoryService.StartAutoAttach();
            InitializeComponent();


            if (SettingsManager.Default.WindowLeft != 0 || SettingsManager.Default.WindowTop != 0)
            {
                Left = SettingsManager.Default.WindowLeft;
                Top = SettingsManager.Default.WindowTop;
            }
            else WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //
            // GameLauncher.SetVersionOffsets();
            _aobScanner = new AoBScanner(_memoryService);
            _stateService = new StateService(_memoryService);

            var hookManager = new HookManager(_memoryService, _stateService);
            var hotkeyManager = new HotkeyManager(_memoryService);

            ITravelService travelService = new TravelService(_memoryService, hookManager);
            IPlayerService playerService = new PlayerService(_memoryService, hookManager, travelService);
            IUtilityService utilityService = new UtilityService(_memoryService, hookManager, playerService);
            IEventService eventService = new EventService(_memoryService, hookManager);
            IAttackInfoService attackInfoService = new AttackInfoService(_memoryService, hookManager);
            ITargetService targetService = new TargetService(_memoryService, hookManager, playerService);
            IEnemyService enemyService = new EnemyService(_memoryService, hookManager);
            ISettingsService settingsService = new SettingsService(_memoryService, hookManager);
            IEzStateService ezStateService = new EzStateService(_memoryService);
            IItemService itemService = new ItemService(_memoryService);
            ISpEffectService spEffectService = new SpEffectService(_memoryService);
            IEmevdService emevdService = new EmevdService(_memoryService);

            _dlcService = new DlcService(_memoryService);


            PlayerViewModel playerViewModel = new PlayerViewModel(playerService, _stateService, hotkeyManager,
                eventService, spEffectService, emevdService, _dlcService);
            TravelViewModel travelViewModel =
                new TravelViewModel(travelService, eventService, _stateService, _dlcService);
            EnemyViewModel enemyViewModel = new EnemyViewModel(enemyService, _stateService, hotkeyManager);
            TargetViewModel targetViewModel = new TargetViewModel(targetService, _stateService, enemyService,
                attackInfoService, hotkeyManager);
            EventViewModel eventViewModel =
                new EventViewModel(eventService, _stateService, itemService, _dlcService, ezStateService, emevdService);
            UtilityViewModel utilityViewModel = new UtilityViewModel(utilityService, _stateService, ezStateService,
                playerService, hotkeyManager, emevdService);
            ItemViewModel itemViewModel = new ItemViewModel(itemService, _dlcService, _stateService, eventService);
            SettingsViewModel settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager);

            var playerTab = new PlayerTab(playerViewModel);
            var travelTab = new TravelTab(travelViewModel);
            var enemyTab = new EnemyTab(enemyViewModel);
            var targetTab = new TargetTab(targetViewModel);
            var utilityTab = new UtilityTab(utilityViewModel);
            var itemTab = new ItemTab(itemViewModel);
            var eventTab = new EventTab(eventViewModel);
            var settingsTab = new SettingsTab(settingsViewModel);


            MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
            MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
            MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
            MainTabControl.Items.Add(new TabItem { Header = "Target", Content = targetTab });
            MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
            MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
            MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
            MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });

            Closing += MainWindow_Closing;

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
        private bool _hasScanned;
        private bool _hasAllocatedMemory;
        private bool _appliedOneTimeFeatures;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryService.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                // LaunchGameButton.IsEnabled = false;

                if (!_hasScanned)
                {
                    _aobScanner.Scan();
                    _hasScanned = true;
                    _stateService.Publish(State.Attached);
#if DEBUG
                    Console.WriteLine($@"Base: 0x{_memoryService.BaseAddress.ToInt64():X}");
#endif
                }


                if (!_hasAllocatedMemory)
                {
                    _memoryService.AllocCodeCave();
#if DEBUG
                    Console.WriteLine($@"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
#endif
                    _hasAllocatedMemory = true;
                }

                if (_stateService.IsLoaded())
                {
                    if (_loaded) return;
                    _loaded = true;
                    _dlcService.CheckDlc();
                    _stateService.Publish(State.Loaded);
                    CheckIfGameStart();
                    if (_appliedOneTimeFeatures) return;
                    _stateService.Publish(State.FirstLoaded);
                    _appliedOneTimeFeatures = true;
                }
                else if (_loaded)
                {
                    _stateService.Publish(State.NotLoaded);
                    _loaded = false;
                }
            }
            else
            {
                // _hookManager.ClearHooks();
                _hasScanned = false;
                _loaded = false;
                _hasAllocatedMemory = false;
                _appliedOneTimeFeatures = false;
                IsAttachedText.Text = "Not attached";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                // LaunchGameButton.IsEnabled = true;
            }
        }

        private void CheckIfGameStart()
        {
            var igt = _memoryService.ReadUInt32((IntPtr)_memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.Igt);
            if (igt < 5000) _stateService.Publish(State.GameStart);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            SettingsManager.Default.WindowLeft = Left;
            SettingsManager.Default.WindowTop = Top;
            SettingsManager.Default.Save();
            // _itemService.SignalClose();
            // _hookManager.UninstallAllHooks();
            // _nopManager.RestoreAll();
        }

        // private void LaunchGame_Click(object sender, RoutedEventArgs e) => Task.Run(GameLauncher.LaunchDarkSouls2);
        // private void CheckUpdate_Click(object sender, RoutedEventArgs e) => VersionChecker.CheckForUpdates(this, true);
    }
}