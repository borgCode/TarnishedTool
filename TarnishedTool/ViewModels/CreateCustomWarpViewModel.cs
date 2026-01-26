// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.ViewModels;

public class CreateCustomWarpViewModel : BaseViewModel
{
    private readonly IPlayerService _playerService;
    private readonly IGameTickService _gameTickService;

    private const byte DlcOverworld = 61;
    private const byte DlcDungeonStart = 20;
    private const byte DlcDungeonEnd = 28;
    private const byte DlcCatacombsStart = 40;
    private const byte DlcCatacombsEnd = 43;

    private Action<CustomWarpChange> _onChange;

    public CreateCustomWarpViewModel(
        Dictionary<string, List<BlockWarp>> customWarps,
        bool areOptionsEnabled,
        IStateService stateService,
        IPlayerService playerService,
        IGameTickService gameTickService, Action<CustomWarpChange> onChange)
    {
        _playerService = playerService;
        _gameTickService = gameTickService;
        _onChange = onChange;

        CustomWarps = new SearchableGroupedCollection<string, BlockWarp>(
            customWarps,
            (customWarp, search) => customWarp.Name.ToLower().Contains(search) ||
                                    customWarp.MainArea.ToLower().Contains(search));


        AreOptionsEnabled = areOptionsEnabled;
        if (AreOptionsEnabled) _gameTickService.Subscribe(LocationTick);

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

        SavePositionCommand = new DelegateCommand(SavePosition);
        ImportWarpsCommand = new DelegateCommand(ImportWarps);
        DeleteCategoryCommand = new DelegateCommand(DeleteCategory);
    }

    #region Commands

    public ICommand SavePositionCommand { get; }
    public ICommand ImportWarpsCommand { get; }
    public ICommand DeleteCategoryCommand { get; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private MapLocation _mapLocation;

    public MapLocation MapLocation
    {
        get => _mapLocation;
        set => SetProperty(ref _mapLocation, value);
    }

    public SearchableGroupedCollection<string, BlockWarp> CustomWarps { get; }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        _gameTickService.Subscribe(LocationTick);
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        _gameTickService.Unsubscribe(LocationTick);
    }

    private void LocationTick() => MapLocation = _playerService.GetMapLocation();

    private void SavePosition()
    {
        var results = MsgBox.ShowInputs(new[]
        {
            new InputField("category", "Category", CustomWarps.SelectedGroup),
            new InputField("name", "Warp Name"),
        }, "New Custom Warp");


        if (results == null) return;

        if (string.IsNullOrWhiteSpace(results["category"]) || string.IsNullOrWhiteSpace(results["name"]))
        {
            MsgBox.Show("Category and name are required.", "Custom Warp");
            return;
        }

        CreateCustomWarp(results["category"], results["name"]);
    }

    private void CreateCustomWarp(string category, string name)
    {
        var warp = new BlockWarp
        {
            IsDlc = IsCurrentBlockDlc(),
            MainArea = category,
            Name = name,
            Position = new Position(
                MapLocation.BlockId,
                MapLocation.MapCoords,
                MapLocation.Angle
            )
        };

        CustomWarps.Add(category, warp);
        _onChange?.Invoke(new WarpAdded(warp));
    }

    private bool IsCurrentBlockDlc()
    {
        if (MapLocation.Area == DlcOverworld) return true;
        if (MapLocation.Area >= DlcDungeonStart && MapLocation.Area <= DlcDungeonEnd) return true;
        if (MapLocation.Area >= DlcCatacombsStart && MapLocation.Area <= DlcCatacombsEnd) return true;
        return false;
    }

    private void ImportWarps()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Import Custom Warps"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            string json = File.ReadAllText(dialog.FileName);
            var importedWarps = JsonSerializer.Deserialize<Dictionary<string, List<BlockWarp>>>(json);

            if (importedWarps == null || importedWarps.Count == 0)
            {
                MsgBox.Show("No warps found in file.");
                return;
            }

            var existingWarps = CustomWarps.GroupedItems;

            var selectionWindow = new CustomWarpImportSelectionWindow(importedWarps, existingWarps);
            if (selectionWindow.ShowDialog() != true) return;

            var selectedCategories = selectionWindow.ViewModel.GetSelectedCategories();
            var conflictResolution = selectionWindow.ViewModel.SelectedConflictResolution;

            int imported = 0;
            int skipped = 0;

            foreach (var kvp in selectedCategories)
            {
                var category = kvp.Key;
                var warps = kvp.Value;

                if (CustomWarps.GroupedItems.ContainsKey(category))
                {
                    switch (conflictResolution)
                    {
                        case ConflictResolution.Skip:
                            skipped++;
                            continue;

                        case ConflictResolution.Overwrite:
                            CustomWarps.RemoveGroup(category);
                            CustomWarps.AddRange(category, warps);
                            foreach (var warp in warps)
                            {
                                _onChange?.Invoke(new WarpAdded(warp));
                            }

                            imported++;
                            break;

                        case ConflictResolution.Rename:
                            string newName = GenerateUniqueCategoryName(category);
                            foreach (var warp in warps)
                            {
                                warp.MainArea = newName;
                            }

                            CustomWarps.AddRange(newName, warps);
                            foreach (var warp in warps)
                            {
                                _onChange?.Invoke(new WarpAdded(warp));
                            }

                            imported++;
                            break;
                    }
                }
                else
                {
                    CustomWarps.AddRange(category, warps);
                    foreach (var warp in warps)
                    {
                        _onChange?.Invoke(new WarpAdded(warp));
                    }

                    imported++;
                }
            }

            string message = $"Imported {imported} category{(imported != 1 ? "s" : "")}";
            if (skipped > 0)
                message += $" ({skipped} skipped)";

            MsgBox.Show(message);
        }
        catch (Exception ex)
        {
            MsgBox.Show($"Failed to import warps: {ex.Message}");
        }
    }

    private string GenerateUniqueCategoryName(string baseName)
    {
        string newName = baseName;
        int counter = 2;

        while (CustomWarps.GroupedItems.ContainsKey(newName))
        {
            newName = $"{baseName} ({counter})";
            counter++;
        }

        return newName;
    }

    private void DeleteCategory()
    {
        var category = CustomWarps.SelectedGroup;
        CustomWarps.RemoveGroup(category);
        _onChange?.Invoke(new CategoryDeleted(category));
    }
    

    #endregion

    #region Public Methods

    public void DeleteWarps(IEnumerable<BlockWarp> warps)
    {
        foreach (var warp in warps)
        {
            CustomWarps.Remove(warp.MainArea, warp);
            _onChange?.Invoke(new WarpDeleted(warp.MainArea, warp));
        }
        
        CustomWarps.SelectedItem = null;
    }

    #endregion
}