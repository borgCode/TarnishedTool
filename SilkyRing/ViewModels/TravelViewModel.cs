using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.GameIds;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels
{
    public class TravelViewModel : BaseViewModel
    {
        private readonly ITravelService _travelService;
        private readonly IEventService _eventService;
        private readonly IDlcService _dlcService;
        private readonly IEmevdService _emevdService;

        public SearchableGroupedCollection<string, Grace> Graces { get; }
        public SearchableGroupedCollection<string, BossWarp> Bosses { get; }
        
        private readonly List<long> _baseGameMaps;
        private readonly List<long> _dlcMaps;

        public TravelViewModel(ITravelService travelService, IEventService eventService, IStateService stateService,
            IDlcService dlcService, IEmevdService emevdService)
        {
            _travelService = travelService;
            _eventService = eventService;
            _dlcService = dlcService;
            _emevdService = emevdService;


            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

            Graces = new SearchableGroupedCollection<string, Grace>(
                DataLoader.GetGraces(),
                (grace, search) => grace.Name.ToLower().Contains(search) ||
                                   grace.MainArea.ToLower().Contains(search));
            Bosses = new SearchableGroupedCollection<string, BossWarp>(
                DataLoader.GetBossWarps(),
                (bossWarp, search) => bossWarp.Name.ToLower().Contains(search) ||
                                      bossWarp.MainArea.ToLower().Contains(search));

            GraceWarpCommand = new DelegateCommand(GraceWarp);
            UnlockMainGameGracesCommand = new DelegateCommand(UnlockMainGameGraces);
            UnlockDlcGracesCommand = new DelegateCommand(UnlockDlcGraces);
            BossWarpCommand = new DelegateCommand(BossWarp);
            UnlockBaseGameMapsCommand = new DelegateCommand(UnlockBaseGameMaps);
            UnlockDlcMapsCommand = new DelegateCommand(UnlockDlcMaps);
            
            _baseGameMaps = DataLoader.GetSimpleList("BaseGameMaps", long.Parse);
            _dlcMaps = DataLoader.GetSimpleList("DLCMaps", long.Parse);
        }

        #region Commands

        public ICommand GraceWarpCommand { get; set; }
        public ICommand BossWarpCommand { get; set; }
        public ICommand UnlockMainGameGracesCommand { get; set; }
        public ICommand UnlockDlcGracesCommand { get; set; }
        public ICommand UnlockBaseGameMapsCommand { get; set; }
        public ICommand UnlockDlcMapsCommand { get; set; }

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

        private bool _isRestOnWarpEnabled;

        public bool IsRestOnWarpEnabled
        {
            get => _isRestOnWarpEnabled;
            set => SetProperty(ref _isRestOnWarpEnabled, value);
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

        private void GraceWarp()
        {
            if (Graces.SelectedItem.IsDlc && !IsDlcAvailable) return;
            _travelService.Warp(Graces.SelectedItem);
        }

        private void BossWarp()
        {
            if (Bosses.SelectedItem.IsDlc && !IsDlcAvailable) return;
            _ = Task.Run(() =>
            {
                _travelService.WarpToBlockId(Bosses.SelectedItem.Position);
                if (IsRestOnWarpEnabled) _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.Rest);
            });
        }

        private void UnlockMainGameGraces()
        {
            foreach (var grace in Graces.AllItems)
            {
                if (grace.IsDlc) continue;
                _eventService.SetEvent(grace.FlagId, true);
            }
        }

        private void UnlockDlcGraces()
        {
            foreach (var grace in Graces.AllItems)
            {
                if (!grace.IsDlc) continue;
                _eventService.SetEvent(grace.FlagId, true);
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