using System;
using System.ComponentModel;
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

            IParamService paramService = new ParamService(_memoryService);
            IReminderService reminderService = new ReminderService(_memoryService, hookManager, _stateService);
            IChrInsService chrInsService = new ChrInsService(_memoryService);
            ITravelService travelService = new TravelService(_memoryService, hookManager);
            IPlayerService playerService =
                new PlayerService(_memoryService, hookManager, travelService, reminderService, paramService, chrInsService);
            IUtilityService utilityService = new UtilityService(_memoryService, hookManager, playerService);
            IEventService eventService = new EventService(_memoryService, hookManager, reminderService);
            IAttackInfoService attackInfoService = new AttackInfoService(_memoryService, hookManager);
            ITargetService targetService =
                new TargetService(_memoryService, hookManager, playerService, reminderService, chrInsService);
            IEnemyService enemyService = new EnemyService(_memoryService, hookManager, reminderService);
            ISettingsService settingsService = new SettingsService(_memoryService, hookManager);
            IEzStateService ezStateService = new EzStateService(_memoryService);
            IItemService itemService = new ItemService(_memoryService);
            ISpEffectService spEffectService = new SpEffectService(_memoryService, reminderService);
            IEmevdService emevdService = new EmevdService(_memoryService);
            IFlaskService flaskService = new FlaskService(ezStateService, _memoryService);
            IEventLogReader eventLogReader = new EventLogReader(_memoryService);
            IParamRepository paramRepository = new ParamRepository();
            IGameTickService gameTickService = new GameTickService(_stateService);
            IAiService aiService = new AiService(_memoryService);

            _dlcService = new DlcService(_memoryService);

            PlayerViewModel playerViewModel = new PlayerViewModel(
                playerService, _stateService, hotkeyManager,
                eventService, spEffectService, emevdService,
                _dlcService, ezStateService, gameTickService, paramService
            );

            TravelViewModel travelViewModel = new TravelViewModel(
                travelService, eventService, _stateService,
                _dlcService, emevdService, playerService,
                gameTickService
            );

            EnemyViewModel enemyViewModel = new EnemyViewModel(
                enemyService, _stateService, hotkeyManager, emevdService,
                _dlcService, spEffectService, paramService, playerService,
                eventService, reminderService, travelService, chrInsService
            );

            TargetViewModel targetViewModel = new TargetViewModel(
                targetService, _stateService, enemyService,
                attackInfoService, hotkeyManager, spEffectService,
                emevdService, gameTickService
            );

            EventViewModel eventViewModel = new EventViewModel(
                eventService, _stateService, itemService, _dlcService,
                ezStateService, emevdService, hotkeyManager,
                utilityService, eventLogReader
            );

            UtilityViewModel utilityViewModel = new UtilityViewModel(
                utilityService, _stateService, ezStateService,
                playerService, hotkeyManager, playerViewModel,
                _dlcService, spEffectService, flaskService, paramService
            );

            ItemViewModel itemViewModel = new ItemViewModel(
                itemService, _dlcService, _stateService, eventService
            );

            AdvancedViewModel advancedViewModel = new AdvancedViewModel(
                itemService, _stateService, eventService,
                paramService, paramRepository, spEffectService, playerService,
                hotkeyManager, gameTickService, reminderService, aiService,
                utilityService, chrInsService
            );

            SettingsViewModel settingsViewModel = new SettingsViewModel(
                settingsService, hotkeyManager, _stateService
            );

            var playerTab = new PlayerTab(playerViewModel);
            var travelTab = new TravelTab(travelViewModel);
            var enemyTab = new EnemyTab(enemyViewModel);
            var targetTab = new TargetTab(targetViewModel);
            var utilityTab = new UtilityTab(utilityViewModel);
            var itemTab = new ItemTab(itemViewModel);
            var eventTab = new EventTab(eventViewModel);
            var advancedTab = new AdvancedTab(advancedViewModel);
            var settingsTab = new SettingsTab(settingsViewModel);


            MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
            MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
            MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
            MainTabControl.Items.Add(new TabItem { Header = "Target", Content = targetTab });
            MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
            MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
            MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
            MainTabControl.Items.Add(new TabItem { Header = "Advanced", Content = advancedTab });
            MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });

            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;

            _stateService.Publish(State.AppStart);

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
        private DateTime? _attachedTime;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryService.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                LaunchGameButton.IsEnabled = false;

                if (!_attachedTime.HasValue)
                {
                    _attachedTime = DateTime.Now;
                    return;
                }

                if ((DateTime.Now - _attachedTime.Value).TotalSeconds < 2)
                    return;

                if (!_hasCheckedPatch)
                {
                    if (!PatchManager.Initialize(_memoryService))
                    {
                        _aobScanner.DoFallbackScan();
                    }

#if DEBUG
                    Console.WriteLine($@"Base: 0x{_memoryService.BaseAddress.ToInt64():X}");
#endif
                    _hasCheckedPatch = true;
                }

                if (!_hasScanned)
                {
                    _aobScanner.Scan();
                    _hasScanned = true;
                }


                if (!_hasAllocatedMemory)
                {
                    _memoryService.AllocCodeCave();
#if DEBUG
                    Console.WriteLine($@"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
#endif
                    _stateService.Publish(State.Attached);
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
                _hasCheckedPatch = false;
                _loaded = false;
                _attachedTime = null;
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
            if (igt < 5000) _stateService.Publish(State.OnNewGameStart);
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