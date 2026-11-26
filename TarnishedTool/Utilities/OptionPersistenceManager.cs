using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Utilities;

public class OptionPersistenceManager
{
    private readonly List<(string Key, Func<bool> Get, Action<bool> Set)> _options;
    private readonly HashSet<string> _trackedKeys;
    private bool _isRestoring;

    public OptionPersistenceManager(
        PlayerViewModel player,
        EnemyViewModel enemy,
        UtilityViewModel utility,
        TravelViewModel travel,
        ItemViewModel item,
        IStateService stateService)
    {
        _options =
        [
            ("Player.NoDeath", () => player.IsNoDeathEnabled, v => player.IsNoDeathEnabled = v),
            ("Player.NoDamage", () => player.IsNoDamageEnabled, v => player.IsNoDamageEnabled = v),
            ("Player.NoHit", () => player.IsNoHitEnabled, v => player.IsNoHitEnabled = v),
            ("Player.InfStamina", () => player.IsInfiniteStaminaEnabled, v => player.IsInfiniteStaminaEnabled = v),
            ("Player.InfConsumables", () => player.IsInfiniteConsumablesEnabled, v => player.IsInfiniteConsumablesEnabled = v),
            ("Player.InfArrows", () => player.IsInfiniteArrowsEnabled, v => player.IsInfiniteArrowsEnabled = v),
            ("Player.InfFp", () => player.IsInfiniteFpEnabled, v => player.IsInfiniteFpEnabled = v),
            ("Player.OneShot", () => player.IsOneShotEnabled, v => player.IsOneShotEnabled = v),
            ("Player.InfPoise", () => player.IsInfinitePoiseEnabled, v => player.IsInfinitePoiseEnabled = v),
            ("Player.Silent", () => player.IsSilentEnabled, v => player.IsSilentEnabled = v),
            ("Player.Hidden", () => player.IsHiddenEnabled, v => player.IsHiddenEnabled = v),
            ("Player.FasterDeath", () => player.IsFasterDeathEnabled, v => player.IsFasterDeathEnabled = v),
            ("Player.TorrentNoDeath", () => player.IsTorrentNoDeathEnabled, v => player.IsTorrentNoDeathEnabled = v),
            ("Player.RfbsOnLoad", () => player.IsSetRfbsOnLoadEnabled, v => player.IsSetRfbsOnLoadEnabled = v),
            ("Player.NoRuneGain", () => player.IsNoRuneGainEnabled, v => player.IsNoRuneGainEnabled = v),
            ("Player.NoRuneLoss", () => player.IsNoRuneLossEnabled, v => player.IsNoRuneLossEnabled = v),
            ("Player.NoRuneArcLoss", () => player.IsNoRuneArcLossEnabled, v => player.IsNoRuneArcLossEnabled = v),
            ("Player.GgrFix", () => player.IsGgrFixEnabled, v => player.IsGgrFixEnabled = v),
            ("Player.NoTimePassOnDeath", () => player.IsNoTimePassOnDeathEnabled, v => player.IsNoTimePassOnDeathEnabled = v),
            ("Player.Hot", () => player.IsHotEnabled, v => player.IsHotEnabled = v),
            ("Player.FpRegen", () => player.IsFpRegenEnabled, v => player.IsFpRegenEnabled = v),
            ("Player.TorrentAnywhere", () => player.IsTorrentAnywhereEnabled, v => player.IsTorrentAnywhereEnabled = v),
            ("Player.ResetWorldIncluded", () => player.IsResetWorldIncluded, v => player.IsResetWorldIncluded = v),
            ("Player.SpeedBuff", () => player.IsSpeedBuffEnabled, v => player.IsSpeedBuffEnabled = v),
            ("Player.SpeedBuffInCombat", () => player.IsSpeedBuffInCombatEnabled, v => player.IsSpeedBuffInCombatEnabled = v),

            ("Travel.NoMapPopups", () => travel.IsNoMapAcquiredPopupsEnabled, v => travel.IsNoMapAcquiredPopupsEnabled = v),
            ("Travel.AutoUnlockPreset", () => travel.IsAutoUnlockPresetEnabled, v => travel.IsAutoUnlockPresetEnabled = v),
            ("Travel.RestOnWarp", () => travel.IsRestOnWarpEnabled, v => travel.IsRestOnWarpEnabled = v),
            ("Travel.ShowAllMaps", () => travel.IsShowAllMapsEnabled, v => travel.IsShowAllMapsEnabled = v),
            ("Travel.ShowAllGraces", () => travel.IsShowAllGracesEnabled, v => travel.IsShowAllGracesEnabled = v),

            ("Enemy.NoDeath", () => enemy.IsNoDeathEnabled, v => enemy.IsNoDeathEnabled = v),
            ("Enemy.NoDamage", () => enemy.IsNoDamageEnabled, v => enemy.IsNoDamageEnabled = v),
            ("Enemy.NoHit", () => enemy.IsNoHitEnabled, v => enemy.IsNoHitEnabled = v),
            ("Enemy.NoAttack", () => enemy.IsNoAttackEnabled, v => enemy.IsNoAttackEnabled = v),
            ("Enemy.NoMove", () => enemy.IsNoMoveEnabled, v => enemy.IsNoMoveEnabled = v),
            ("Enemy.DisableAi", () => enemy.IsDisableAiEnabled, v => enemy.IsDisableAiEnabled = v),
            ("Enemy.RestOnRevive", () => enemy.IsRestOnReviveEnabled, v => enemy.IsRestOnReviveEnabled = v),

            ("Utility.CombatMap", () => utility.IsCombatMapEnabled, v => utility.IsCombatMapEnabled = v),
            ("Utility.DungeonWarp", () => utility.IsDungeonWarpEnabled, v => utility.IsDungeonWarpEnabled = v),
            ("Utility.GuaranteedDrop", () => utility.IsGuaranteedDropEnabled, v => utility.IsGuaranteedDropEnabled = v),
            ("Utility.DisableCutscenes", () => utility.IsDisableCutscenesEnabled, v => utility.IsDisableCutscenesEnabled = v),
            ("Utility.FullShopLineup", () => utility.IsShowFullShopLineupEnabled, v => utility.IsShowFullShopLineupEnabled = v),
            ("Utility.DrawHitbox", () => utility.IsDrawHitboxEnabled, v => utility.IsDrawHitboxEnabled = v),
            ("Utility.DrawPoiseBars", () => utility.IsDrawPoiseBarsEnabled, v => utility.IsDrawPoiseBarsEnabled = v),
            ("Utility.DrawPlayerSound", () => utility.IsDrawPlayerSoundEnabled, v => utility.IsDrawPlayerSoundEnabled = v),
            ("Utility.DrawRagdolls", () => utility.IsDrawRagdollsEnabled, v => utility.IsDrawRagdollsEnabled = v),
            ("Utility.DrawLowHit", () => utility.IsDrawLowHitEnabled, v => utility.IsDrawLowHitEnabled = v),
            ("Utility.DrawHighHit", () => utility.IsDrawHighHitEnabled, v => utility.IsDrawHighHitEnabled = v),
            ("Utility.DrawMapTiles1", () => utility.IsDrawMapTiles1Enabled, v => utility.IsDrawMapTiles1Enabled = v),
            ("Utility.DrawMapTiles2", () => utility.IsDrawMapTiles2Enabled, v => utility.IsDrawMapTiles2Enabled = v),
            ("Utility.DrawMiniMap", () => utility.IsDrawMiniMapEnabled, v => utility.IsDrawMiniMapEnabled = v),
            ("Utility.DrawTilesOnMap", () => utility.IsDrawTilesOnMapEnabled, v => utility.IsDrawTilesOnMapEnabled = v),
            ("Utility.HideCharacters", () => utility.IsHideCharactersEnabled, v => utility.IsHideCharactersEnabled = v),
            ("Utility.HideMap", () => utility.IsHideMapEnabled, v => utility.IsHideMapEnabled = v),

            ("Item.AutoSpawn", () => item.AutoSpawnEnabled, v => item.AutoSpawnEnabled = v),
            ("Item.AutoLoadoutSpawn", () => item.AutoLoadoutSpawnEnabled, v => item.AutoLoadoutSpawnEnabled = v),
        ];

        _trackedKeys = new HashSet<string>(_options.Select(o => o.Key));

        INotifyPropertyChanged[] viewModels = [player, enemy, utility, travel, item];
        foreach (var vm in viewModels)
            vm.PropertyChanged += OnViewModelPropertyChanged;

        stateService.Subscribe(State.AppStart, RestoreSavedOptions);
    }

    public void SaveCurrentOptions()
    {
        if (!SettingsManager.Default.RememberSelectedOptions) return;

        var enabledKeys = _options.Where(o => o.Get()).Select(o => o.Key);
        SettingsManager.Default.SavedSelectedOptions = string.Join(",", enabledKeys);
        SettingsManager.Default.Save();
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (_isRestoring || !SettingsManager.Default.RememberSelectedOptions) return;
        SaveCurrentOptions();
    }

    private void RestoreSavedOptions()
    {
        if (!SettingsManager.Default.RememberSelectedOptions) return;

        var raw = SettingsManager.Default.SavedSelectedOptions;
        if (string.IsNullOrEmpty(raw)) return;

        var saved = new HashSet<string>(raw.Split([','], StringSplitOptions.RemoveEmptyEntries));
        _isRestoring = true;
        try
        {
            foreach (var (key, _, set) in _options)
            {
                if (saved.Contains(key) && _trackedKeys.Contains(key))
                    set(true);
            }
        }
        finally
        {
            _isRestoring = false;
        }
    }
}
