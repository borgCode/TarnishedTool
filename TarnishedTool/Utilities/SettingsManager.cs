// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace TarnishedTool.Utilities;

public class SettingsManager
{
    private static SettingsManager _default;
    public static SettingsManager Default => _default ?? (_default = Load());

    public double DefenseWindowLeft { get; set; }
    public double DefenseWindowTop { get; set; }
    public bool DefensesAlwaysOnTop { get; set; }
    public double AttackInfoWindowLeft { get; set; }
    public double AttackInfoWindowTop { get; set; }
    public bool AtkInfoAlwaysOnTop { get; set; }
    public double TargetSpEffectWindowLeft { get; set; }
    public double TargetSpEffectWindowTop { get; set; }
    public bool TargetSpEffectAlwaysOnTop { get; set; }
    public double EventLogWindowLeft { get; set; }
    public double EventLogWindowTop { get; set; }
    public bool EventLogWindowAlwaysOnTop { get; set; }
    public double WindowLeft { get; set; }
    public double WindowTop { get; set; }
    public bool AlwaysOnTop { get; set; }
    public bool StutterFix { get; set; }
    public bool DisableAchievements { get; set; }
    public bool NoLogo { get; set; }
    public bool MuteMusic { get; set; }
    [DefaultValue(1.0)] public double ResistancesWindowScaleX { get; set; }
    [DefaultValue(1.0)] public double ResistancesWindowScaleY { get; set; }
    public double ResistancesWindowOpacity { get; set; }
    public double ResistancesWindowWidth { get; set; }
    public double ResistancesWindowLeft { get; set; }
    public double ResistancesWindowTop { get; set; }
    public string HotkeyActionIds { get; set; } = "";
    public bool EnableHotkeys { get; set; }
    public bool RememberPlayerSpeed { get; set; }
    public float PlayerSpeed { get; set; }
    public bool RememberGameSpeed { get; set; }
    public float GameSpeed { get; set; }
    public bool IsNoClipKeyboardDisabled { get; set; }
    public bool BlockHotkeysFromGame { get; set; }
    public bool HotkeyReminder { get; set; }
    [DefaultValue(true)] public bool EnableUpdateChecks { get; set; }
    public string SaveCustomHp { get; set; } = "";
    public double GraceImportWindowLeft { get; set; }
    public double GraceImportWindowTop { get; set; }
    public double GracePresetWindowLeft { get; set; }
    public double GracePresetWindowTop { get; set; }
    public bool GracePresetWindowAlwaysOnTop { get; set; }
    public double CreateCustomWarpWindowLeft { get; set; }
    public double CreateCustomWarpWindowTop { get; set; }
    public bool CreateCustomWarpWindowAlwaysOnTop { get; set; }
    public double ParamEditorWindowLeft { get; set; }
    public double ParamEditorWindowTop { get; set; }
    public bool ParamEditorWindowAlwaysOnTop { get; set; }
    [DefaultValue("OffsetNameInternal")] public string ParamFieldDisplayMode { get; set; }
    public double AiWindowWindowLeft { get; set; }
    public double AiWindowWindowTop { get; set; }
    public bool AiWindowAlwaysOnTop { get; set; }
    public double AiOverlayToolbarLeft { get; set; }
    public double AiOverlayToolbarTop { get; set; }

    public double AiOverlayGoalsLeft { get; set; }
    public double AiOverlayGoalsTop { get; set; }
    public double AiOverlayGoalsOpacity { get; set; }
    
    public double AiOverlayCoolTimesLeft { get; set; }
    public double AiOverlayCoolTimesTop { get; set; }
    public double AiOverlayCoolTimesOpacity { get; set; }

    public double AiOverlayLuaTimersLeft { get; set; }
    public double AiOverlayLuaTimersTop { get; set; }
    public double AiOverlayLuaTimersOpacity { get; set; }

    public double AiOverlayLuaNumbersLeft { get; set; }
    public double AiOverlayLuaNumbersTop { get; set; }
    public double AiOverlayLuaNumbersOpacity { get; set; }

    public double AiOverlaySpEffectObservesLeft { get; set; }
    public double AiOverlaySpEffectObservesTop { get; set; }
    public double AiOverlaySpEffectObservesOpacity { get; set; }

    public double AiOverlayInterruptsLeft { get; set; }
    public double AiOverlayInterruptsTop { get; set; }
    public double AiOverlayInterruptsOpacity { get; set; }

    public double AiOverlaySpEffectsLeft { get; set; }
    public double AiOverlaySpEffectsTop { get; set; }
    public double AiOverlaySpEffectsOpacity { get; set; }

    public double ParamEditorWindowWidth { get; set; }
    public double ParamEditorWindowHeight { get; set; }


    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TarnishedTool",
        "settings.txt");

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var lines = new List<string>();

            foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(this);
                var stringValue = value switch
                {
                    double d => d.ToString(CultureInfo.InvariantCulture),
                    float f => f.ToString(CultureInfo.InvariantCulture),
                    _ => value?.ToString() ?? ""
                };
                lines.Add($"{prop.Name}={stringValue}");
            }

            File.WriteAllLines(SettingsPath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error saving settings: {ex.Message}");
        }
    }

    private static SettingsManager Load()
    {
        var settings = new SettingsManager();

        foreach (var prop in typeof(SettingsManager).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAttr != null)
                prop.SetValue(settings, defaultAttr.Value);
        }

        if (!File.Exists(SettingsPath))
            return settings;

        try
        {
            var props = new Dictionary<string, PropertyInfo>();
            foreach (var prop in typeof(SettingsManager).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                props[prop.Name] = prop;

            foreach (var line in File.ReadAllLines(SettingsPath))
            {
                var parts = line.Split(['='], 2);
                if (parts.Length != 2) continue;

                var key = parts[0];
                var value = parts[1];

                if (!props.TryGetValue(key, out var prop)) continue;

                object parsed = prop.PropertyType switch
                {
                    { } t when t == typeof(double) =>
                        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0.0,
                    { } t when t == typeof(float) =>
                        float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : 0f,
                    { } t when t == typeof(bool) =>
                        bool.TryParse(value, out var b) && b,
                    { } t when t == typeof(string) => value,
                    _ => null
                };

                if (parsed != null)
                    prop.SetValue(settings, parsed);
            }
        }
        catch
        {
            // Return default settings on error
        }

        return settings;
    }
}