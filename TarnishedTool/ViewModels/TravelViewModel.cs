using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels
{
    public class TravelViewModel : BaseViewModel
    {
        private readonly HotkeyManager _hotkeyManager;
        private readonly ITravelService _travelService;
        private readonly IEventService _eventService;
        private readonly IStateService _stateService;
        private readonly IDlcService _dlcService;
        private readonly IEmevdService _emevdService;
        private readonly IPlayerService _playerService;
        private readonly IGameTickService _gameTickService;

        public SearchableGroupedCollection<string, Grace> Graces { get; }
        public SearchableGroupedCollection<string, BlockWarp> Bosses { get; }
        public SearchableGroupedCollection<string, BlockWarp> CustomWarps { get; }

        private SearchableGroupedCollection<string, Grace> _gracesForPresetWindow;
        private Dictionary<string, GracePresetTemplate> _customGracePresets;

        CreateCustomWarpWindow _createCustomWarpWindow;

        private readonly List<long> _baseGameMaps;
        private readonly List<long> _dlcMaps;
        private readonly List<long> _baseArGraces;
        private readonly List<long> _dlcArGraces;

        public TravelViewModel(ITravelService travelService, IEventService eventService, IStateService stateService,
            IDlcService dlcService, IEmevdService emevdService, IPlayerService playerService,
            IGameTickService gameTickService,HotkeyManager hotkeyManager)
        {
            _travelService = travelService;
            _eventService = eventService;
            _stateService = stateService;
            _dlcService = dlcService;
            _emevdService = emevdService;
            _playerService = playerService;
            _gameTickService = gameTickService;
            _hotkeyManager = hotkeyManager;
            
            RegisterHotkeys();


            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);

            Graces = new SearchableGroupedCollection<string, Grace>(
                DataLoader.GetGraces(),
                (grace, search) => grace.Name.ToLower().Contains(search) ||
                                   grace.MainArea.ToLower().Contains(search));
            Bosses = new SearchableGroupedCollection<string, BlockWarp>(
                DataLoader.GetBossWarps(),
                (bossWarp, search) => bossWarp.Name.ToLower().Contains(search) ||
                                      bossWarp.MainArea.ToLower().Contains(search));

            CustomWarps = new SearchableGroupedCollection<string, BlockWarp>(
                DataLoader.LoadCustomWarps(),
                (customWarp, search) => customWarp.Name.ToLower().Contains(search) ||
                                        customWarp.MainArea.ToLower().Contains(search));

            _gracesForPresetWindow = new SearchableGroupedCollection<string, Grace>(
                Graces.AllItems
                    .Select(g => g.Clone())
                    .GroupBy(g => g.MainArea)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                (grace, search) => grace.Name.ToLower().Contains(search) ||
                                   grace.MainArea.ToLower().Contains(search));

            GraceWarpCommand = new DelegateCommand(GraceWarp);
            UnlockMainGameGracesCommand = new DelegateCommand(UnlockMainGameGraces);
            UnlockDlcGracesCommand = new DelegateCommand(UnlockDlcGraces);
            BossWarpCommand = new DelegateCommand(BossWarp);
            UnlockBaseGameMapsCommand = new DelegateCommand(UnlockBaseGameMaps);
            UnlockDlcMapsCommand = new DelegateCommand(UnlockDlcMaps);
            UnlockBaseArGracesCommand = new DelegateCommand(UnlockBaseArGraces);
            UnlockDlcArGracesCommand = new DelegateCommand(UnlockDlcArGraces);
            OpenGracePresetWindowCommand = new DelegateCommand(OpenGracePresetWindow);
            UnlockPresetGracesCommand = new DelegateCommand(UnlockGracePreset);
            CustomWarpCommand = new DelegateCommand(CustomWarp);
            OpenCreateCustomWarpCommand = new DelegateCommand(OpenCreateCustomWarp);


            _customGracePresets = DataLoader.LoadGracePresets();
            _gracePresets = new ObservableCollection<string>(_customGracePresets.Keys);
            SelectedGracePreset = _gracePresets.FirstOrDefault();


            _baseGameMaps = DataLoader.GetSimpleList("BaseGameMaps", s => long.Parse(s, CultureInfo.InvariantCulture));
            _dlcMaps = DataLoader.GetSimpleList("DLCMaps", s => long.Parse(s, CultureInfo.InvariantCulture));
            _baseArGraces = DataLoader.GetSimpleList("ArBaseGraces", s => long.Parse(s, CultureInfo.InvariantCulture));
            _dlcArGraces = DataLoader.GetSimpleList("ArDlcGraces", s => long.Parse(s, CultureInfo.InvariantCulture));
        }

        #region Commands

        public ICommand GraceWarpCommand { get; set; }
        public ICommand BossWarpCommand { get; set; }
        public ICommand UnlockMainGameGracesCommand { get; set; }
        public ICommand UnlockDlcGracesCommand { get; set; }
        public ICommand UnlockBaseGameMapsCommand { get; set; }
        public ICommand UnlockDlcMapsCommand { get; set; }
        public ICommand UnlockBaseArGracesCommand { get; set; }
        public ICommand UnlockDlcArGracesCommand { get; set; }
        public ICommand OpenGracePresetWindowCommand { get; set; }
        public ICommand UnlockPresetGracesCommand { get; set; }
        public ICommand CustomWarpCommand { get; set; }
        public ICommand OpenCreateCustomWarpCommand { get; set; }

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

        private bool _isRestOnCustomWarpEnabled;

        public bool IsRestOnCustomWarpEnabled
        {
            get => _isRestOnCustomWarpEnabled;
            set => SetProperty(ref _isRestOnCustomWarpEnabled, value);
        }

        private bool _isShowAllGracesEnabled;

        public bool IsShowAllGracesEnabled
        {
            get => _isShowAllGracesEnabled;
            set
            {
                SetProperty(ref _isShowAllGracesEnabled, value);
                _travelService.ToggleShowAllGraces(_isShowAllGracesEnabled);
            }
        }

        private bool _isShowAllMapsEnabled;

        public bool IsShowAllMapsEnabled
        {
            get => _isShowAllMapsEnabled;
            set
            {
                SetProperty(ref _isShowAllMapsEnabled, value);
                _travelService.ToggleShowAllMaps(_isShowAllMapsEnabled);
            }
        }

        private bool _isNoMapAcquiredPopupsEnabled;

        public bool IsNoMapAcquiredPopupsEnabled
        {
            get => _isNoMapAcquiredPopupsEnabled;
            set
            {
                SetProperty(ref _isNoMapAcquiredPopupsEnabled, value);
                _travelService.ToggleNoMapAcquiredPopups(_isNoMapAcquiredPopupsEnabled);
            }
        }

        private ObservableCollection<string> _gracePresets;

        public ObservableCollection<string> GracePresets
        {
            get => _gracePresets;
            private set => SetProperty(ref _gracePresets, value);
        }

        private string _selectedGracePreset;

        public string SelectedGracePreset
        {
            get => _selectedGracePreset;
            set => SetProperty(ref _selectedGracePreset, value);
        }

        private bool _isAutoUnlockPresetEnabled;

        public bool IsAutoUnlockPresetEnabled
        {
            get => _isAutoUnlockPresetEnabled;
            set => SetProperty(ref _isAutoUnlockPresetEnabled, value);
        }

        #endregion

        #region Private Methods

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockMainGameMaps, UnlockBaseGameMaps);
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockDlcMaps, UnlockDlcMaps);
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockAllMainGameGraces, UnlockMainGameGraces);
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockAllDlcGraces, UnlockDlcGraces);
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockAllMainRemembrancesGraces, UnlockBaseArGraces);
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockAllDlcRemembrancesGraces,UnlockDlcArGraces);
            _hotkeyManager.RegisterAction(HotkeyActions.UnlockPresetGraces,UnlockGracePreset);
            _hotkeyManager.RegisterAction(HotkeyActions.ShowAllGraces, () => IsShowAllGracesEnabled = !IsShowAllGracesEnabled );
            _hotkeyManager.RegisterAction(HotkeyActions.ShowAllMaps, ()  => IsShowAllMapsEnabled = !IsShowAllMapsEnabled );
            _hotkeyManager.RegisterAction(HotkeyActions.NoMapAcquiredPopup, () => IsNoMapAcquiredPopupsEnabled = !IsNoMapAcquiredPopupsEnabled );
            _hotkeyManager.RegisterAction(HotkeyActions.WarpToGrace, () => GraceWarp());
            _hotkeyManager.RegisterAction(HotkeyActions.WarpToBoss, () =>  BossWarp());
            _hotkeyManager.RegisterAction(HotkeyActions.WarpToCustomLocation,  () => CustomWarp());
            _hotkeyManager.RegisterAction(HotkeyActions.RestOnWarp,  () => IsRestOnCustomWarpEnabled = !IsRestOnCustomWarpEnabled);
        }

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            IsDlcAvailable = _dlcService.IsDlcAvailable;
            if (IsShowAllGracesEnabled) _travelService.ToggleShowAllGraces(true);
            if (IsShowAllMapsEnabled) _travelService.ToggleShowAllMaps(true);
            if (IsNoMapAcquiredPopupsEnabled) _travelService.ToggleNoMapAcquiredPopups(true);
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

        private void CustomWarp()
        {
            if (CustomWarps.SelectedItem.IsDlc && !IsDlcAvailable) return;
            _ = Task.Run(() =>
            {
                _travelService.WarpToBlockId(CustomWarps.SelectedItem.Position);
                if (IsRestOnCustomWarpEnabled) _emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.Rest);
            });
        }

        private void UnlockMainGameGraces()
        {
            _eventService.SetEvent(Event.SeeUndergroundGraces, true);
            foreach (var grace in Graces.AllItems)
            {
                if (grace.IsDlc) continue;
                _eventService.SetEvent(grace.FlagId, true);
            }
        }

        private void UnlockDlcGraces()
        {
            _eventService.SetEvent(Event.SeeDlcGraces, true);
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

        private void UnlockBaseArGraces()
        {
            _eventService.SetEvent(Event.SeeUndergroundGraces, true);
            foreach (var baseArGrace in _baseArGraces)
            {
                _eventService.SetEvent(baseArGrace, true);
            }
        }

        private void UnlockDlcArGraces()
        {
            _eventService.SetEvent(Event.SeeDlcGraces, true);
            foreach (var dlcArGrace in _dlcArGraces)
            {
                _eventService.SetEvent(dlcArGrace, true);
            }
        }

        private void OpenGracePresetWindow()
        {
            var window = new GracePresetWindow(
                _gracesForPresetWindow,
                _customGracePresets);

            if (window.ShowDialog() == true)
            {
                RefreshGracePresets();
            }
        }

        private void RefreshGracePresets()
        {
            _gracePresets.Clear();
            foreach (var name in _customGracePresets.Keys)
            {
                _gracePresets.Add(name);
            }

            if (string.IsNullOrEmpty(SelectedGracePreset) || !_customGracePresets.ContainsKey(SelectedGracePreset))
            {
                SelectedGracePreset = _gracePresets.FirstOrDefault();
            }

            DataLoader.SaveGracePresets(_customGracePresets);
        }

        private void UnlockGracePreset()
        {
            var preset = _customGracePresets[SelectedGracePreset];

            foreach (var gracePresetEntry in preset.Graces)
            {
                if (gracePresetEntry.IsDlc && !IsDlcAvailable) continue;
                _eventService.SetEvent(gracePresetEntry.FlagId, true);
            }

            _eventService.SetEvent(Event.SeeUndergroundGraces, true);
            if (IsDlcAvailable) _eventService.SetEvent(Event.SeeDlcGraces, true);
        }

        private void OnNewGameStart()
        {
            if (!IsAutoUnlockPresetEnabled || SelectedGracePreset == null) return;
            UnlockGracePreset();
        }

        private void OpenCreateCustomWarp()
        {
            if (_createCustomWarpWindow != null && _createCustomWarpWindow.IsVisible)
            {
                _createCustomWarpWindow.Activate();
                return;
            }

            var clonedWarps = CustomWarps.GroupedItems.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(w => new BlockWarp
                {
                    IsDlc = w.IsDlc,
                    MainArea = w.MainArea,
                    Name = w.Name,
                    Position = new Position(w.Position.BlockId, w.Position.Coords, w.Position.Angle)
                }).ToList()
            );

            _createCustomWarpWindow = new CreateCustomWarpWindow(
                clonedWarps,
                AreOptionsEnabled,
                _stateService,
                _playerService,
                _gameTickService,
                OnCustomWarpChanged
            );

            _createCustomWarpWindow.Closed += (_, _) => _createCustomWarpWindow = null;
            _createCustomWarpWindow.Show();
        }

        private void OnCustomWarpChanged(CustomWarpChange change)
        {
            switch (change)
            {
                case WarpAdded(var warp):
                    CustomWarps.Add(warp.MainArea, warp);
                    break;
                case WarpDeleted(var category, var warp):
                    var original = CustomWarps.GroupedItems[category]
                        .FirstOrDefault(w => w.Name == warp.Name && w.MainArea == warp.MainArea);
                    if (original != null)
                        CustomWarps.Remove(category, original);
                    break;
                case CategoryDeleted(var category):
                    CustomWarps.RemoveGroup(category);
                    break;
            }

            DataLoader.SaveCustomWarps(CustomWarps.GroupedItems);
        }

        #endregion
    }
}