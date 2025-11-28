using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SilkyRing.Interfaces;
using SilkyRing.Models;
using SilkyRing.Services;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels
{
    public class TravelViewModel : BaseViewModel
    {
        private readonly TravelService _travelService;
        private readonly EventService _eventService;
        // private readonly HotkeyManager _hotkeyManager;

        private bool _areButtonsEnabled = true;
        
        private ObservableCollection<string> _mainAreas;
        private ObservableCollection<Grace> _areaLocations;
        
        private string _selectedMainArea;
        private Grace _selectedGrace;
        
        private Dictionary<string, List<Grace>> _locationDict;
        private List<Grace> _allLocations;
        
        private string _searchText = string.Empty;
        private bool _isSearchActive;
        private string _preSearchMainArea;
        private readonly ObservableCollection<Grace> _searchResultsCollection = new ObservableCollection<Grace>();
        
        public TravelViewModel(TravelService travelService, EventService eventService, IStateService stateService)
        {
            _travelService = travelService;
            // _hotkeyManager = hotkeyManager;
            
            _mainAreas = new ObservableCollection<string>();
            _areaLocations = new ObservableCollection<Grace>();

            LoadLocations();
            // RegisterHotkeys();
        }

        private void LoadLocations()
        {
            _locationDict = DataLoader.GetGraces();
            _allLocations = _locationDict.Values.SelectMany(x => x).ToList();

            foreach (var area in _locationDict.Keys)
            {
                _mainAreas.Add(area);
            }

            SelectedMainArea = _mainAreas.FirstOrDefault();
        }
        
        public bool AreButtonsEnabled
        {
            get => _areButtonsEnabled;
            set => SetProperty(ref _areButtonsEnabled, value);
        }
        
        public ObservableCollection<string> MainAreas
        {
            get => _mainAreas;
            private set => SetProperty(ref _mainAreas, value);
        }
        
        public ObservableCollection<Grace> AreaLocations
        {
            get => _areaLocations;
            set => SetProperty(ref _areaLocations, value);
        }
        
        public string SelectedMainArea
        {
            get => _selectedMainArea;
            set
            {
                if (!SetProperty(ref _selectedMainArea, value) || value == null) return;
                
                if (_isSearchActive)
                {
                    IsSearchActive = false;
                    _searchText = string.Empty;
                    OnPropertyChanged(nameof(SearchText));
                    _preSearchMainArea = null;
                }
                
                UpdateLocationsList();
            }
        }
        
        public Grace SelectedGrace
        {
            get => _selectedGrace;
            set => SetProperty(ref _selectedGrace, value);
        }
        
        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set => SetProperty(ref _isSearchActive, value);
        }
        
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!SetProperty(ref _searchText, value)) return;
                
                if (string.IsNullOrEmpty(value))
                {
                    _isSearchActive = false;
                    
                    if (_preSearchMainArea != null)
                    {
                        _selectedMainArea = _preSearchMainArea;
                        UpdateLocationsList();
                        _preSearchMainArea = null;
                    }
                }
                else
                {
                    if (!_isSearchActive)
                    {
                        _preSearchMainArea = SelectedMainArea;
                        _isSearchActive = true;
                    }
                    
                    ApplyFilter();
                }
            }
        }
        
        private void UpdateLocationsList()
        {
            if (string.IsNullOrEmpty(SelectedMainArea) || !_locationDict.ContainsKey(SelectedMainArea))
            {
                AreaLocations = new ObservableCollection<Grace>();
                return;
            }
            
            AreaLocations = new ObservableCollection<Grace>(_locationDict[SelectedMainArea]);
            SelectedGrace = AreaLocations.FirstOrDefault();
        }
        
        private void ApplyFilter()
        {
            _searchResultsCollection.Clear();
            var searchTextLower = SearchText.ToLower();
            
            foreach (var location in _allLocations)
            {
                if (location.Name.ToLower().Contains(searchTextLower) || 
                    location.MainArea.ToLower().Contains(searchTextLower))
                {
                    _searchResultsCollection.Add(location);
                }
            }
            
            AreaLocations = new ObservableCollection<Grace>(_searchResultsCollection);
            SelectedGrace = AreaLocations.FirstOrDefault();
        }

        public void Warp() => _travelService.Warp(SelectedGrace);

        public void UnlockMainGameGraces()
        {
            foreach (var grace in _allLocations)
            {
                if (grace.IsDlc) continue;
                _travelService.UnlockGrace(grace);
            }
        }

        public void UnlockDlcGraces()
        {
            foreach (var grace in _allLocations)
            {
                if (!grace.IsDlc) continue;
                _travelService.UnlockGrace(grace);
            }
        }

        public void Test()
        {
            _travelService.Test();
        }
    }
}