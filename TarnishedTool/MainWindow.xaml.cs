using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Services;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;
using TarnishedTool.Views.Tabs;
using static TarnishedTool.Memory.Offsets;
using UtilityTab = TarnishedTool.Views.Tabs.UtilityTab;

namespace TarnishedTool
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

            _aobScanner = new AoBScanner(_memoryService);
            _stateService = new StateService(_memoryService);

            var hookManager = new HookManager(_memoryService, _stateService);
            var hotkeyManager = new HotkeyManager(_memoryService);

            IReminderService reminderService = new ReminderService(_memoryService, hookManager, _stateService);
            ITravelService travelService = new TravelService(_memoryService, hookManager);
            IPlayerService playerService =
                new PlayerService(_memoryService, hookManager, travelService, reminderService);
            IUtilityService utilityService = new UtilityService(_memoryService, hookManager, playerService);
            IEventService eventService = new EventService(_memoryService, hookManager, reminderService);
            IAttackInfoService attackInfoService = new AttackInfoService(_memoryService, hookManager);
            ITargetService targetService =
                new TargetService(_memoryService, hookManager, playerService, reminderService);
            IEnemyService enemyService = new EnemyService(_memoryService, hookManager, reminderService);
            ISettingsService settingsService = new SettingsService(_memoryService, hookManager);
            IEzStateService ezStateService = new EzStateService(_memoryService);
            IItemService itemService = new ItemService(_memoryService);
            ISpEffectService spEffectService = new SpEffectService(_memoryService, reminderService);
            IEmevdService emevdService = new EmevdService(_memoryService);
            IFlaskService flaskService = new FlaskService(ezStateService, _memoryService);
            IParamService paramService = new ParamService(_memoryService);

            _dlcService = new DlcService(_memoryService);


            PlayerViewModel playerViewModel = new PlayerViewModel(playerService, _stateService, hotkeyManager,
                eventService, spEffectService, emevdService, _dlcService, ezStateService);
            TravelViewModel travelViewModel =
                new TravelViewModel(travelService, eventService, _stateService, _dlcService, emevdService);
            EnemyViewModel enemyViewModel = new EnemyViewModel(enemyService, _stateService, hotkeyManager, emevdService,
                _dlcService, spEffectService, paramService, playerService, eventService, reminderService,
                travelService);
            TargetViewModel targetViewModel = new TargetViewModel(targetService, _stateService, enemyService,
                attackInfoService, hotkeyManager, spEffectService, emevdService);
            EventViewModel eventViewModel =
                new EventViewModel(eventService, _stateService, itemService, _dlcService, ezStateService, emevdService,
                    hotkeyManager, utilityService);
            UtilityViewModel utilityViewModel = new UtilityViewModel(utilityService, _stateService, ezStateService,
                playerService, hotkeyManager, emevdService, playerViewModel, _dlcService, spEffectService,
                flaskService);
            ItemViewModel itemViewModel = new ItemViewModel(itemService, _dlcService, _stateService, eventService);
            SettingsViewModel settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager, _stateService);

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

            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;

            _stateService.Publish(State.AppStart);

            var versionInfo = FileVersionInfo.GetVersionInfo(ExeManager.GetExePath());
            var fileVersion = versionInfo.FileVersion;
            Console.WriteLine($"Version: {fileVersion}");

            Closing += MainWindow_Closing;

            _gameLoadedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(25)
            };
            _gameLoadedTimer.Tick += Timer_Tick;
            _gameLoadedTimer.Start();

            VersionChecker.UpdateVersionText(AppVersion);

            if (SettingsManager.Default.EnableUpdateChecks)
            {
                VersionChecker.CheckForUpdates(this);
            }
        }

        private bool _loaded;
        private bool _hasScanned;
        private bool _hasAllocatedMemory;
        private bool _appliedOneTimeFeatures;
        private bool _hasPublishedLoaded;
        private bool _hasPublishedFadedIn;
        private bool _hasCheckedPatch;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryService.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                LaunchGameButton.IsEnabled = false;

                if (!_hasCheckedPatch)
                {
                    if (!PatchManager.Initialize(_memoryService))
                    {
                        _aobScanner.Scan();
#if DEBUG
                        Console.WriteLine($@"Base: 0x{_memoryService.BaseAddress.ToInt64():X}");
#endif
                        _hasCheckedPatch = true;
                    }
                    
#if DEBUG
                    Console.WriteLine($@"Base: 0x{_memoryService.BaseAddress.ToInt64():X}");
#endif
                    
                    _hasCheckedPatch = true;
                }

                if (!_hasScanned)
                {
                    var sw = Stopwatch.StartNew();
                    _aobScanner.Scan();
                    sw.Stop();
                    Console.WriteLine($"AoBScanner.Scan (fallback): {sw.ElapsedMilliseconds}ms");
    
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
                    _stateService.Publish(State.Attached);
#endif
                    _hasAllocatedMemory = true;
                }

                if (_stateService.IsLoaded())
                {
                    if (!_hasPublishedFadedIn && _hasPublishedLoaded && IsFadedIn())
                    {
                        _stateService.Publish(State.FadedIn);
                        _hasPublishedFadedIn = true;
                    }

                    if (_loaded) return;
                    _loaded = true;
                    _dlcService.CheckDlc();
                    _stateService.Publish(State.Loaded);
                    _hasPublishedLoaded = true;
                    CheckIfGameStart();
                    if (_appliedOneTimeFeatures) return;
                    _stateService.Publish(State.FirstLoaded);
                    _appliedOneTimeFeatures = true;
                }
                else if (_loaded)
                {
                    _stateService.Publish(State.NotLoaded);
                    _loaded = false;
                    _hasPublishedLoaded = false;
                    _hasPublishedFadedIn = false;
                }
            }
            else
            {
                _hasScanned = false;
                _loaded = false;
                _hasAllocatedMemory = false;
                _appliedOneTimeFeatures = false;
                _hasPublishedLoaded = false;
                _hasPublishedFadedIn = false;
                _stateService.Publish(State.Detached);
                IsAttachedText.Text = "Not attached";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                LaunchGameButton.IsEnabled = true;
            }
        }

        private bool IsFadedIn() =>
            _memoryService.ReadUInt8((IntPtr)_memoryService.ReadInt64(MenuMan.Base) + MenuMan.IsFading) == 0;

        private void CheckIfGameStart()
        {
            var igt = _memoryService.ReadUInt32(
                (IntPtr)_memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.Igt);
            if (igt < 5000) _stateService.Publish(State.GameStart);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            SettingsManager.Default.WindowLeft = Left;
            SettingsManager.Default.WindowTop = Top;
            SettingsManager.Default.Save();
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e) => Task.Run(ExeManager.LaunchGame);

        private void CheckUpdate_Click(object sender, RoutedEventArgs e) =>
            VersionChecker.CheckForUpdates(this, true);

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && MainTabControl.SelectedItem is TabItem selectedTab)
            {
                if (selectedTab.Header.ToString() == "Event")
                {
                    _stateService.Publish(State.EventTabActivated);
                }
            }
        }
    }
}