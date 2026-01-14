// 

using System;
using System.IO;

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
    public double WindowLeft { get; set; }
    public double WindowTop { get; set; }
    public bool AlwaysOnTop { get; set; }
    public bool StutterFix { get; set; }
    public bool DisableAchievements { get; set; }
    public bool NoLogo { get; set; }
    public bool MuteMusic { get; set; }
    public double ResistancesWindowScaleX { get; set; } = 1.0;
    public double ResistancesWindowScaleY { get; set; } = 1.0;
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
    public bool BlockHotkeysFromGame  { get; set; }
    public bool HotkeyReminder { get; set; }
    public bool EnableUpdateChecks { get; set; } = true;
    public string SaveCustomHp { get; set; } = "";
    

    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TarnishedTool",
        "settings.txt");

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));

            var lines = new[]
            {
                $"DefenseWindowLeft={DefenseWindowLeft}",
                $"DefenseWindowTop={DefenseWindowTop}",
                $"DefensesAlwaysOnTop={DefensesAlwaysOnTop}",
                $"AttackInfoWindowLeft={AttackInfoWindowLeft}",
                $"AttackInfoWindowTop={AttackInfoWindowTop}",
                $"AtkInfoAlwaysOnTop={AtkInfoAlwaysOnTop}",
                $"TargetSpEffectWindowLeft={TargetSpEffectWindowLeft}",
                $"TargetSpEffectWindowTop={TargetSpEffectWindowTop}",
                $"TargetSpEffectAlwaysOnTop={TargetSpEffectAlwaysOnTop}",
                $"WindowLeft={WindowLeft}",
                $"WindowTop={WindowTop}",
                $"AlwaysOnTop={AlwaysOnTop}",
                $"StutterFix={StutterFix}",
                $"DisableAchievements={DisableAchievements}",
                $"NoLogo={NoLogo}",
                $"MuteMusic={MuteMusic}",
                $"ResistancesWindowScaleX={ResistancesWindowScaleX}",
                $"ResistancesWindowScaleY={ResistancesWindowScaleY}",
                $"ResistancesWindowOpacity={ResistancesWindowOpacity}",
                $"ResistancesWindowWidth={ResistancesWindowWidth}",
                $"ResistancesWindowLeft={ResistancesWindowLeft}",
                $"ResistancesWindowTop={ResistancesWindowTop}",
                $"HotkeyActionIds={HotkeyActionIds}",
                $"EnableHotkeys={EnableHotkeys}",
                $"RememberPlayerSpeed={RememberPlayerSpeed}",
                $"PlayerSpeed={PlayerSpeed}",
                $"RememberGameSpeed={RememberGameSpeed}",
                $"GameSpeed={GameSpeed}",
                $"IsNoClipKeyboardDisabled={IsNoClipKeyboardDisabled}",
                $"BlockHotkeysFromGame={BlockHotkeysFromGame}",
                $"EnableUpdateChecks={EnableUpdateChecks}",
                $"HotkeyReminder={HotkeyReminder}",
                $"SaveCustomHp={SaveCustomHp}",
            };

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

        if (File.Exists(SettingsPath))
        {
            try
            {
                foreach (var line in File.ReadAllLines(SettingsPath))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0];
                        var value = parts[1];

                        switch (key)
                        {
                            case "DefenseWindowLeft":
                                double.TryParse(value, out double dwl);
                                settings.DefenseWindowLeft = dwl;
                                break;
                            case "DefenseWindowTop":
                                double.TryParse(value, out double dwt);
                                settings.DefenseWindowTop = dwt;
                                break;
                            case "DefensesAlwaysOnTop":
                                bool.TryParse(value, out bool daot);
                                settings.DefensesAlwaysOnTop = daot;
                                break;
                            case "AttackInfoWindowLeft":
                                double.TryParse(value, out double aiwl);
                                settings.AttackInfoWindowLeft = aiwl;
                                break;
                            case "AttackInfoWindowTop":
                                double.TryParse(value, out double aiwt);
                                settings.AttackInfoWindowTop = aiwt;
                                break;
                            case "AtkInfoAlwaysOnTop":
                                bool.TryParse(value, out bool aiaot);
                                settings.AtkInfoAlwaysOnTop = aiaot;
                                break;
                            case "TargetSpEffectWindowLeft":
                                double.TryParse(value, out double tsewl);
                                settings.TargetSpEffectWindowLeft = tsewl;
                                break;
                            case "TargetSpEffectWindowTop":
                                double.TryParse(value, out double tsewt);
                                settings.TargetSpEffectWindowTop = tsewt;
                                break;
                            case "TargetSpEffectAlwaysOnTop":
                                bool.TryParse(value, out bool tseaot);
                                settings.TargetSpEffectAlwaysOnTop = tseaot;
                                break;
                            case "WindowLeft":
                                double.TryParse(value, out double wl);
                                settings.WindowLeft = wl;
                                break;
                            case "WindowTop":
                                double.TryParse(value, out double wt);
                                settings.WindowTop = wt;
                                break;
                            case "AlwaysOnTop":
                                bool.TryParse(value, out bool aot);
                                settings.AlwaysOnTop = aot;
                                break;
                            case "StutterFix":
                                bool.TryParse(value, out bool sf);
                                settings.StutterFix = sf;
                                break;
                            case "DisableAchievements":
                                bool.TryParse(value, out bool da);
                                settings.DisableAchievements = da;
                                break;
                            case "NoLogo":
                                bool.TryParse(value, out bool nl);
                                settings.NoLogo = nl;
                                break;
                            case "MuteMusic":
                                bool.TryParse(value, out bool mm);
                                settings.MuteMusic = mm;
                                break;
                            case "ResistancesWindowScaleX":
                                double.TryParse(value, out double rwx);
                                settings.ResistancesWindowScaleX = rwx;
                                break;
                            case "ResistancesWindowScaleY":
                                double.TryParse(value, out double rwy);
                                settings.ResistancesWindowScaleY = rwy;
                                break;
                            case "ResistancesWindowOpacity":
                                double.TryParse(value, out double rwo);
                                settings.ResistancesWindowOpacity = rwo;
                                break;
                            case "ResistancesWindowLeft":
                                double.TryParse(value, out double rwl);
                                settings.ResistancesWindowLeft = rwl;
                                break;
                            case "ResistancesWindowTop":
                                double.TryParse(value, out double rwt);
                                settings.ResistancesWindowTop = rwt;
                                break;
                            case "ResistancesWindowWidth":
                                double.TryParse(value, out double rww);
                                settings.ResistancesWindowWidth = rww;
                                break;
                            case "HotkeyActionIds": settings.HotkeyActionIds = value; break;
                            case "EnableHotkeys":
                                bool.TryParse(value, out bool eh);
                                settings.EnableHotkeys = eh;
                                break;
                            case "RememberPlayerSpeed":
                                bool.TryParse(value, out bool rps);
                                settings.RememberPlayerSpeed = rps;
                                break;
                            case "PlayerSpeed":
                                float.TryParse(value, out float ps);
                                settings.PlayerSpeed = ps;
                                break;
                            case "RememberGameSpeed":
                                bool.TryParse(value, out bool rgs);
                                settings.RememberGameSpeed = rgs;
                                break;
                            case "GameSpeed":
                                float.TryParse(value, out float gs);
                                settings.GameSpeed = gs;
                                break;
                            case "IsNoClipKeyboardDisabled":
                                bool.TryParse(value, out bool inkd);
                                settings.IsNoClipKeyboardDisabled = inkd;
                                break;
                            case "BlockHotkeysFromGame":
                                bool.TryParse(value, out bool bhfg);
                                settings.BlockHotkeysFromGame = bhfg;
                                break;
                            case "EnableUpdateChecks":
                                bool.TryParse(value, out bool euc);
                                settings.EnableUpdateChecks = euc;
                                break;
                            case "HotkeyReminder":
                                bool.TryParse(value, out bool hr);
                                settings.HotkeyReminder = hr;
                                break;
                            case "SaveCustomHp":
                                settings.SaveCustomHp = value;
                                break;
                                
                        }
                    }
                }
            }
            catch
            {
                // Return default settings on error
            }
        }

        return settings;
    }
}