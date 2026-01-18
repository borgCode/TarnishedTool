// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using TarnishedTool.Core;
using TarnishedTool.Models;
using TarnishedTool.Utilities;

namespace TarnishedTool.ViewModels;

public class EventLogViewModel : BaseViewModel
{
    
    
    private HashSet<uint> _uniqueEventIds = new();
    private bool _isPaused;

    public EventLogViewModel()
    {
        AddToExcludeCommand = new DelegateCommand(AddToExclude);
        RemoveFromExcludedCommand = new DelegateCommand(RemoveFromExcluded);
        
        ClearCommand = new DelegateCommand(ClearLog);
        ClearUniqueCommand = new DelegateCommand(ClearUnique);
        ClearExcludedCommand = new DelegateCommand(ClearExcluded);
        
        PauseResumeCommand = new DelegateCommand(TogglePauseResume);
        
        ImportExcludedCommand = new DelegateCommand(ImportExcluded);
        ExportExcludedCommand = new DelegateCommand(ExportExcluded);
    }

    
    #region Commands

    public ICommand AddToExcludeCommand { get; }
    public ICommand RemoveFromExcludedCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ClearUniqueCommand { get; }
    public ICommand ClearExcludedCommand { get; }
    public ICommand PauseResumeCommand { get; }
    public ICommand ImportExcludedCommand { get; }
    public ICommand ExportExcludedCommand { get; }
    
    

    #endregion
    
    #region Properties
    
    
    private ObservableCollection<EventLogEntry> _logEntries = new();
    public ObservableCollection<EventLogEntry> LogEntries => _logEntries;
    
    private ObservableCollection<uint> _excludedEventIds = new();
    public ObservableCollection<uint> ExcludedEventIds => _excludedEventIds;
    
    private bool _isLogUniqueIdsEnabled;

    public bool IsLogUniqueIdsEnabled
    {
        get => _isLogUniqueIdsEnabled;
        set
        {
            if (!SetProperty(ref _isLogUniqueIdsEnabled, value)) return;
            if (!value) _uniqueEventIds.Clear(); 
        }
    }
    
    private int _logLimit = 4000;
    
    public int LogLimit
    {
        get => _logLimit;
        set => SetProperty(ref _logLimit, value);
    }
    
    private string _pauseResumeText = "Pause";
        
    public string PauseResumeText 
    {
        get => _pauseResumeText;
        set => SetProperty(ref _pauseResumeText, value);
    }

    private string _eventIdToExclude;

    public string EventIdToExclude
    {
        get => _eventIdToExclude;
        set => SetProperty(ref _eventIdToExclude, value);
    }
    
    private uint _selectedExcludedId;

    public uint SelectedExcludedId
    {
        get => _selectedExcludedId;
        set => SetProperty(ref _selectedExcludedId, value);
    }
    
    
    #endregion
    
    #region Public Methods
    
    public void RefreshEventLogs(List<EventLogEntry> entries)
    {
        if (_isPaused) return;
        
        foreach (var eventLogEntry in entries)
        {
            
            if (_excludedEventIds.Contains(eventLogEntry.EventId)) continue;
            
            if (IsLogUniqueIdsEnabled && _uniqueEventIds.Contains(eventLogEntry.EventId)) continue;
            
            _logEntries.Add(eventLogEntry);
            
            if (IsLogUniqueIdsEnabled) _uniqueEventIds.Add(eventLogEntry.EventId);
        }

        while (_logEntries.Count > LogLimit)
            _logEntries.RemoveAt(0);
    }

    public void Reset()
    {
        _logEntries.Clear();
        _uniqueEventIds.Clear();
    }
    
    public void RemoveFromExcluded(uint eventId) => _excludedEventIds.Remove(eventId);
    public void AddToExcluded(uint eventId) => _excludedEventIds.Add(eventId);
    
    public void AddMultipleToExcluded(IEnumerable<EventLogEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (!_excludedEventIds.Contains(entry.EventId))
                _excludedEventIds.Add(entry.EventId);
        }
    }
    
    #endregion
    
    #region Private Methods
    
    private void AddToExclude()
    {
        if (!uint.TryParse(EventIdToExclude, out uint eventId))
        {
            MsgBox.Show("Not a valid format for an event ID, should be a number.");
            return; 
        }
    
        if (!_excludedEventIds.Contains(eventId))
            _excludedEventIds.Add(eventId);
        
        EventIdToExclude = string.Empty;
    }

    private void RemoveFromExcluded() =>
        _excludedEventIds.Remove(SelectedExcludedId);
    
    private void ClearLog() => _logEntries.Clear();
    private void ClearUnique() => _uniqueEventIds.Clear();
    private void ClearExcluded() => _excludedEventIds.Clear();
    
    private void TogglePauseResume()
    {
        if (_isPaused)
        {
            PauseResumeText = "Pause";
            _isPaused = false;
        }
        else
        {
            PauseResumeText = "Resume";
            _isPaused = true;
        }
    }
    
    private void ImportExcluded()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            Title = "Import Excluded Event IDs"
        };
    
        if (dialog.ShowDialog() == true)
        {
            var lines = File.ReadAllLines(dialog.FileName);
            foreach (var line in lines)
            {
                if (uint.TryParse(line.Trim(), out uint eventId) && !_excludedEventIds.Contains(eventId))
                    _excludedEventIds.Add(eventId);
            }
        }
    }

    private void ExportExcluded()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            Title = "Export Excluded Event IDs",
            FileName = "excluded_events.txt"
        };
    
        if (dialog.ShowDialog() == true)
        {
            File.WriteAllLines(dialog.FileName, _excludedEventIds.Select(id => id.ToString()));
        }
    }
    
    #endregion

    
}