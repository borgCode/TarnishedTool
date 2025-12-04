// 

using System;
using System.IO;

namespace SilkyRing.Utilities;

public class SettingsManager
{
    private static SettingsManager _default;
    public static SettingsManager Default => _default ?? (_default = Load());
    
    public double DefenseWindowLeft { get; set; }
    public double DefenseWindowTop { get; set; }
    
    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SilkyRing",
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
                    
                };

                File.WriteAllLines(SettingsPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
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