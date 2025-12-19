using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.GameIds;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly IEventService _eventService;
        private readonly IItemService _itemService;
        private readonly IDlcService _dlcService;
        private readonly IEzStateService _ezStateService;
        public const int WhetstoneBladeId = 0x4000218E;
        
        private List<BossRevive> _bossReviveList;

        private List<long> _baseGameMaps;
        private List<long> _dlcMaps;

        public EventViewModel(IEventService eventService, IStateService stateService, IItemService itemService,
            IDlcService dlcService, IEzStateService ezStateService)
        {
            _eventService = eventService;
            _itemService = itemService;
            _dlcService = dlcService;
            _ezStateService = ezStateService;


            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

            SetEventCommand = new DelegateCommand(SetEvent);
            GetEventCommand = new DelegateCommand(GetEvent);
            UnlockWhetbladesCommand = new DelegateCommand(UnlockWhetblades);
            UnlockBaseGameMapsCommand = new DelegateCommand(UnlockBaseGameMaps);
            UnlockDlcMapsCommand = new DelegateCommand(UnlockDlcMaps);
            TestCommand = new DelegateCommand(Test);
            
            _bossReviveList = DataLoader.GetBossRevives();
            _baseGameMaps = DataLoader.GetSimpleList("BaseGameMaps", long.Parse);
            _dlcMaps = DataLoader.GetSimpleList("DLCMaps", long.Parse);
        }

        private void Test()
        {
            _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture);
        }

        #region Commands
        
        public ICommand SetEventCommand { get; set; }
        public ICommand GetEventCommand { get; set; }
        public ICommand UnlockWhetbladesCommand { get; set; }
        public ICommand UnlockBaseGameMapsCommand { get; set; }
        public ICommand UnlockDlcMapsCommand { get; set; }
        public ICommand TestCommand { get; set; }

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
        
        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            IsDlcAvailable = _dlcService.IsDlcAvailable;
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
        }
        
        private void SetEvent()
        {
            if (string.IsNullOrWhiteSpace(SetFlagId))
                return;

            string trimmedFlagId = SetFlagId.Trim();

            if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
                return;

            _eventService.SetEvent(flagIdValue, FlagStateIndex == 0);
        }
        
        private void GetEvent()
        {
            if (string.IsNullOrWhiteSpace(GetFlagId))
                return;

            string trimmedFlagId = GetFlagId.Trim();

            if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
                return;

            if (_eventService.GetEvent(flagIdValue))
            {
                EventStatusText = "True";
                EventStatusColor = Brushes.Chartreuse;
            }
            else
            {
                EventStatusText = "False";
                EventStatusColor = Brushes.Red;
            }
        }
        
        private void UnlockWhetblades()
        {
            _itemService.SpawnItem(WhetstoneBladeId, 1, -1, false, 1);
            foreach (var whetBlade in Event.WhetBlades)
            {
                _eventService.SetEvent(whetBlade, true);
            }
        }
        
        private void TestRevive()
        {
            var boss = _bossReviveList[1];

            foreach (var bossFlag in boss.BossFlags)
            {
                _eventService.SetEvent(bossFlag.EventId, bossFlag.SetValue);
            }
        }
        
        private void UnlockBaseGameMaps()
        {
            foreach (var baseGameMap in _baseGameMaps)
            {
                _eventService.SetEvent(baseGameMap, true);
            }
        }

        private void UnlockDlcMaps()
        {
            foreach (var dlcMap in _dlcMaps)
            {
                _eventService.SetEvent(dlcMap, true);
            }
        }

        #endregion
    }
}