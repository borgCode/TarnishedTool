// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SilkyRing.Utilities;

public static class GameLauncher
{
    public static void LaunchGame()
    {
        try
        {
            string exePath = GetExePath();
            if (exePath == null) return;

            var psi = new ProcessStartInfo(exePath)
            {
                EnvironmentVariables = { ["SteamAppId"] = "1245620" },
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };
        
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            MsgBox.Show($"Failed to launch Elden Ring: {ex.Message}");
        }
    }

    private static string GetExePath()
    {
        string steamPath =
            Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath",
                null) as string;
        if (string.IsNullOrEmpty(steamPath))
            throw new FileNotFoundException("Steam installation path not found in registry.");
        
        string configPath = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");
        if (!File.Exists(configPath))
            throw new FileNotFoundException($"Steam library configuration not found at {configPath}");
        
        var paths = new List<string> { steamPath };
        var regex = new Regex(@"""path""\s*""(.+?)""");

        foreach (var line in File.ReadLines(configPath))
        {
            var match = regex.Match(line);
            if (match.Success) paths.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
        }

        foreach (var path in paths)
        {
            string fullPath = Path.Combine(path, @"steamapps\common\Elden Ring\Game\eldenring.exe");
            if (File.Exists(fullPath)) return fullPath;
        }

        return null;

    }
}