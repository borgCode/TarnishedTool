// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public enum ConflictResolution
{
    [Description("Skip")]
    Skip,
    [Description("Overwrite")]
    Overwrite,
    [Description("Rename")]
    Rename
}


public class GraceImportPresetSelectionViewModel : BaseViewModel
{

    public ObservableCollection<GraceImportItem> Presets { get; }
    
    public GraceImportPresetSelectionViewModel(
        List<GracePresetTemplate> importedPresets,
        Dictionary<string, GracePresetTemplate> existingPresets)
    {

        Presets = new ObservableCollection<GraceImportItem>(
            importedPresets.Select(p => new GraceImportItem(p, existingPresets.ContainsKey(p.Name)))
        );

        foreach (var preset in Presets)
        {
            preset.PropertyChanged += OnPresetSelectionChanged;
        }
        
    }

    #region Properties

    private ConflictResolution _selectedConflictResolution = ConflictResolution.Skip;
    public ConflictResolution SelectedConflictResolution
    {
        get => _selectedConflictResolution;
        set => SetProperty(ref _selectedConflictResolution, value);
    }
    
    private int _selectedCount;
    public int SelectedCount
    {
        get => _selectedCount;
        private set
        {
            SetProperty(ref _selectedCount, value);
            OnPropertyChanged(nameof(CanImport));
        }
    }
    
    public bool CanImport => SelectedCount > 0;

    public bool HasConflicts => Presets.Any(p => p.IsSelected && p.AlreadyExists);

    #endregion



    #region Private Methods

    private void OnPresetSelectionChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GraceImportItem.IsSelected))
        {
            SelectedCount = Presets.Count(p => p.IsSelected);
            OnPropertyChanged(nameof(HasConflicts));
        }
    }


    #endregion
    
    
    #region Public Methods
    
    public List<GracePresetTemplate> GetSelectedPresets()
    {
        return Presets
            .Where(p => p.IsSelected)
            .Select(p => p.Preset)
            .ToList();
    }
    
    #endregion
    
    
    
}