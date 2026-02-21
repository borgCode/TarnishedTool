using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly IEventService _eventService;
        private readonly IItemService _itemService;
        private readonly IDlcService _dlcService;
        private readonly IEzStateService _ezStateService;
        private readonly IEmevdService _emevdService;
        private readonly HotkeyManager _hotkeyManager;
        private readonly IUtilityService _utilityService;
        private readonly IEventLogReader _eventLogReader;
        public const int WhetstoneBladeId = 0x4000218E;

        private static readonly int[] StartingFlaskIds = [201, 203, 205, 207, 209, 211, 213, 215, 217, 219, 221, 223, 225, 227, 229];

        private readonly List<int> _baseGameGestureIds;
        private readonly List<int> _dlcGestureIds;

        private readonly EventLogViewModel _eventLogViewModel = new();
        private EventLogWindow _eventLogWindow;

        private static readonly Brush OnColor = Brushes.Chartreuse;
        private static readonly Brush OffColor = Brushes.Red;

        public EventViewModel(IEventService eventService, IStateService stateService, IItemService itemService,
            IDlcService dlcService, IEzStateService ezStateService, IEmevdService emevdService,
            HotkeyManager hotkeyManager, IUtilityService utilityService, IEventLogReader eventLogReader)
        {
            _eventService = eventService;
            _itemService = itemService;
            _dlcService = dlcService;
            _ezStateService = ezStateService;
            _emevdService = emevdService;
            _hotkeyManager = hotkeyManager;
            _utilityService = utilityService;
            _eventLogReader = eventLogReader;


            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);
            stateService.Subscribe(State.EventTabActivated, OnEventTabActivated);

            SetEventCommand = new DelegateCommand(SetEvent);
            GetEventCommand = new DelegateCommand(GetEvent);
            UnlockWhetbladesCommand = new DelegateCommand(UnlockWhetblades);
            UnlockMetyrCommand = new DelegateCommand(UnlockMetyr);
            FightFortissaxCommand = new DelegateCommand(FightFortissax);
            UnlockGesturesCommand = new DelegateCommand(UnlockGestures);
            FightEldenBeastCommand = new DelegateCommand(FightEldenBeast);
            ClearDlcCommand = new DelegateCommand(ToggleClearDlc);
            DeactivateMausoleumCommand = new DelegateCommand(ToggleSnowfieldMausoleum);
            SetMorningCommand = new DelegateCommand(SetMorning);
            SetNoonCommand = new DelegateCommand(SetNoon);
            SetNightCommand = new DelegateCommand(SetNight);
            SetWeatherCommand = new DelegateCommand(SetWeather);
            GiveStartingFlasksCommand = new DelegateCommand(GiveStartingFlasks);

            _baseGameGestureIds =
                DataLoader.GetSimpleList("BaseGestures", s => int.Parse(s, CultureInfo.InvariantCulture));
            _dlcGestureIds = DataLoader.GetSimpleList("DlcGestures", s => int.Parse(s, CultureInfo.InvariantCulture));

            SelectedWeatherType = WeatherTypes.FirstOrDefault();

            RegisterHotkeys();

            _eventLogReader = eventLogReader;
            _eventLogReader.EntriesReceived += OnLogEntriesReceived;
        }

        #region Commands

        public ICommand SetEventCommand { get; set; }
        public ICommand GetEventCommand { get; set; }
        public ICommand UnlockWhetbladesCommand { get; set; }
        public ICommand UnlockMetyrCommand { get; set; }
        public ICommand FightFortissaxCommand { get; set; }
        public ICommand UnlockGesturesCommand { get; set; }
        public ICommand FightEldenBeastCommand { get; set; }
        public ICommand ClearDlcCommand { get; set; }
        public ICommand DeactivateMausoleumCommand { get; set; }
        public ICommand SetMorningCommand { get; set; }
        public ICommand SetNoonCommand { get; set; }
        public ICommand SetNightCommand { get; set; }
        public ICommand SetWeatherCommand { get; set; }
        public ICommand GiveStartingFlasksCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isDlcAvailable;

        public bool IsDlcAvailable
        {
            get => _isDlcAvailable;
            set => SetProperty(ref _isDlcAvailable, value);
        }

        private string _setFlagId;

        public string SetFlagId
        {
            get => _setFlagId;
            set => SetProperty(ref _setFlagId, value);
        }

        private int _flagStateIndex;

        public int FlagStateIndex
        {
            get => _flagStateIndex;
            set => SetProperty(ref _flagStateIndex, value);
        }

        private string _getFlagId;

        public string GetFlagId
        {
            get => _getFlagId;
            set => SetProperty(ref _getFlagId, value);
        }

        private string _eventStatusText;

        public string EventStatusText
        {
            get => _eventStatusText;
            set => SetProperty(ref _eventStatusText, value);
        }

        private Brush _eventStatusColor;

        public Brush EventStatusColor
        {
            get => _eventStatusColor;
            set => SetProperty(ref _eventStatusColor, value);
        }

        private string _dlcClearStatusText;

        public string DlcClearStatusText
        {
            get => _dlcClearStatusText;
            set => SetProperty(ref _dlcClearStatusText, value);
        }

        private Brush _dlcClearStatusColor;

        public Brush DlcClearStatusColor
        {
            get => _dlcClearStatusColor;
            set => SetProperty(ref _dlcClearStatusColor, value);
        }

        private string _deactivateMausoleumStatusText;

        public string DeactivateMausoleumStatusText
        {
            get => _deactivateMausoleumStatusText;
            set => SetProperty(ref _deactivateMausoleumStatusText, value);
        }

        private Brush _deactivateMausoleumStatusColor;

        public Brush DeactivateMausoleumStatusColor
        {
            get => _deactivateMausoleumStatusColor;
            set => SetProperty(ref _deactivateMausoleumStatusColor, value);
        }

        private bool _isDrawEventsEnabled;

        public bool IsDrawEventsEnabled
        {
            get => _isDrawEventsEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventsEnabled, value)) return;
                if (_isDrawEventsEnabled)
                {
                    _utilityService.PatchDebugFont();
                    _eventService.PatchEventEnable();
                }

                _eventService.ToggleDrawEvents(_isDrawEventsEnabled);
            }
        }

        private bool _isDisableEventsEnabled;

        public bool IsDisableEventsEnabled
        {
            get => _isDisableEventsEnabled;
            set
            {
                if (!SetProperty(ref _isDisableEventsEnabled, value)) return;
                _eventService.ToggleDisableEvents(_isDisableEventsEnabled);
            }
        }

        private bool _isEventLoggerEnabled;

        public bool IsEventLoggerEnabled
        {
            get => _isEventLoggerEnabled;
            set
            {
                if (!SetProperty(ref _isEventLoggerEnabled, value)) return;

                _eventService.ToggleEventLogger(value);

                if (value)
                {
                    OpenEventLogWindow();
                    _eventLogViewModel.Reset();
                    _eventLogReader.Start();
                }
                else
                {
                    _eventLogReader.Stop();
                    _eventLogWindow?.Close();
                }
            }
        }

        public IReadOnlyList<Weather> WeatherTypes { get; } = DataLoader.GetWeatherTypes().ToList();

        private Weather _selectedWeatherType;

        public Weather SelectedWeatherType
        {
            get => _selectedWeatherType;
            set => SetProperty(ref _selectedWeatherType, value);
        }

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            IsDlcAvailable = _dlcService.IsDlcAvailable;
            UpdateEventStatus();
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
        }

        private void OnGameFirstLoaded()
        {
            if (IsDrawEventsEnabled)
            {
                _utilityService.PatchDebugFont();
                _eventService.PatchEventEnable();
                _eventService.ToggleDrawEvents(true);
            }

            if (IsDisableEventsEnabled) _eventService.ToggleDisableEvents(false);
        }

        private void OnEventTabActivated()
        {
            if (!AreOptionsEnabled) return;
            UpdateEventStatus();
        }

        private void UpdateEventStatus()
        {
            (DlcClearStatusText, DlcClearStatusColor) = GetStatus(_eventService.GetEvent(Event.ClearDlc));
            (DeactivateMausoleumStatusText, DeactivateMausoleumStatusColor) =
                GetStatus(_eventService.GetEvent(Event.SnowfieldMausoleum));
        }

        private (string, Brush) GetStatus(bool isOn) => isOn ? ("ON", OnColor) : ("OFF", OffColor);

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.DrawEvents, () => IsDrawEventsEnabled = !IsDrawEventsEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.GetEvent, () => SafeExecute(GetEvent));
            _hotkeyManager.RegisterAction(HotkeyActions.SetEvent, () => SafeExecute(SetEvent));
            _hotkeyManager.RegisterAction(HotkeyActions.DisableEvents,
                () => IsDisableEventsEnabled = !IsDisableEventsEnabled);
            _hotkeyManager.RegisterAction(HotkeyActions.OpenEventLogger, () => OpenEventLogWindow());
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockAffinites, () => SafeExecute(UnlockWhetblades));
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockGestures, () => SafeExecute(UnlockGestures));
            _hotkeyManager.RegisterAction(HotkeyActions.FightEldenBeast, () => SafeExecute(FightEldenBeast));
            _hotkeyManager.RegisterAction(HotkeyActions.FightFortissax, () => SafeExecute(FightFortissax));
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockMetyr, () => SafeExecute(UnlockMetyr));
            _hotkeyManager.RegisterAction(HotkeyActions.SetMorning, () => SafeExecute(SetMorning));
            _hotkeyManager.RegisterAction(HotkeyActions.SetNoon, () => SafeExecute(SetNoon));
            _hotkeyManager.RegisterAction(HotkeyActions.SetNight, () => SafeExecute(SetNight));
            _hotkeyManager.RegisterAction(HotkeyActions.DefaultWeather,
                () => SafeExecute(() => SetWeatherByType(0)));
            _hotkeyManager.RegisterAction(HotkeyActions.RainyWeather, () => SafeExecute(() => SetWeatherByType(1)));
            _hotkeyManager.RegisterAction(HotkeyActions.SnowyWeather, () => SafeExecute(() => SetWeatherByType(2)));
            _hotkeyManager.RegisterAction(HotkeyActions.WindyRainWeather,
                () => SafeExecute(() => SetWeatherByType(3)));
            _hotkeyManager.RegisterAction(HotkeyActions.FoggyWeather, () => SafeExecute(() => SetWeatherByType(4)));
            _hotkeyManager.RegisterAction(HotkeyActions.FlatCloudsWeather,
                () => SafeExecute(() => SetWeatherByType(6)));
            _hotkeyManager.RegisterAction(HotkeyActions.WindyPuffyClouds,
                () => SafeExecute(() => SetWeatherByType(12)));
            _hotkeyManager.RegisterAction(HotkeyActions.RainyHeavyFog,
                () => SafeExecute(() => SetWeatherByType(15)));
            _hotkeyManager.RegisterAction(HotkeyActions.ScatteredRain,
                () => SafeExecute(() => SetWeatherByType(17)));
        }

        private void SafeExecute(Action action)
        {
            if (!AreOptionsEnabled) return;
            action();
        }

        private void SetEvent()
        {
            if (string.IsNullOrWhiteSpace(SetFlagId))
                return;

            string trimmedFlagId = SetFlagId.Trim();

            if (!long.TryParse(trimmedFlagId, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out long flagIdValue) || flagIdValue <= 0)
                return;

            _eventService.SetEvent(flagIdValue, FlagStateIndex == 0);
        }

        private void GetEvent()
        {
            if (string.IsNullOrWhiteSpace(GetFlagId)) return;
            if (!long.TryParse(GetFlagId.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out long flagIdValue) || flagIdValue <= 0) return;

            var isOn = _eventService.GetEvent(flagIdValue);
            (EventStatusText, EventStatusColor) = isOn ? ("True", OnColor) : ("False", OffColor);
        }

        private void UnlockWhetblades()
        {
            _itemService.SpawnItem(WhetstoneBladeId, 1, -1, false, 1);
            foreach (var whetBlade in Event.WhetBlades)
            {
                _eventService.SetEvent(whetBlade, true);
            }
        }

        private void UnlockMetyr()
        {
            foreach (var eventId in Event.UnlockMetyr)
            {
                _eventService.SetEvent(eventId, true);
            }
        }

        private void FightFortissax() => _eventService.SetEvent(Event.FightFortissax, true);
        private void FightEldenBeast() => Array.ForEach(Event.FightEldenBeast, e => _eventService.SetEvent(e, true));

        private void UnlockGestures()
        {
            foreach (var baseGameGestureId in _baseGameGestureIds)
            {
                _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture(baseGameGestureId));
            }

            if (!IsDlcAvailable) return;

            foreach (var dlcGestureId in _dlcGestureIds)
            {
                _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture(dlcGestureId));
            }
        }

        private void ToggleClearDlc()
        {
            _eventService.ToggleEvent(Event.ClearDlc);
            UpdateEventStatus();
        }

        private void ToggleSnowfieldMausoleum()
        {
            _eventService.ToggleEvent(Event.SnowfieldMausoleum);
            UpdateEventStatus();
        }

        private void SetMorning() => _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetMorning);
        private void SetNoon() => _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetNoon);
        private void SetNight() => _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetNight);

        private void SetWeather() =>
            _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetWeather(SelectedWeatherType.Type));

        private void SetWeatherByType(sbyte type) =>
            _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetWeather(type));

        private void OpenEventLogWindow()
        {
            _eventLogWindow = new EventLogWindow()
            {
                DataContext = _eventLogViewModel,
                Title = "Event Log"
            };
            _eventLogWindow.Closed += (s, e) =>
            {
                _eventLogWindow = null;
                if (_isEventLoggerEnabled)
                {
                    _isEventLoggerEnabled = false;
                    OnPropertyChanged(nameof(IsEventLoggerEnabled));
                    _eventLogReader.Stop();
                    _eventService.ToggleEventLogger(false);
                }
            };
            _eventLogWindow.Show();
        }

        private void OnLogEntriesReceived(List<EventLogEntry> events) =>
            _eventLogViewModel.RefreshEventLogs(events);

        private void GiveStartingFlasks() => _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.AwardItemsIncludingClients(2000));
     


        #endregion
    }
}