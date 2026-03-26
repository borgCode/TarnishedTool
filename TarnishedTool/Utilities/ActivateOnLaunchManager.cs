using System;
using System.Collections.Generic;
using System.Linq;

namespace TarnishedTool.Utilities;

public class ActivateOnLaunchManager
{
    private readonly Dictionary<string, string> _values = new();

    public ActivateOnLaunchManager()
    {
        Load();
    }

    public bool GetBool(string actionId)
    {
        return _values.TryGetValue(actionId, out var v) && bool.TryParse(v, out var b) && b;
    }

    public void SetBool(string actionId, bool value)
    {
        _values[actionId] = value.ToString();
        Save();
    }

    public int GetInt(string actionId, int defaultValue = 0)
    {
        return _values.TryGetValue(actionId, out var v) && int.TryParse(v, out var i) ? i : defaultValue;
    }

    public void SetInt(string actionId, int value)
    {
        _values[actionId] = value.ToString();
        Save();
    }

    public void Save()
    {
        try
        {
            SettingsManager.Default.ActivateOnLaunchActionIds = string.Join(",", _values.Select(kv => $"{kv.Key}:{kv.Value}"));
            SettingsManager.Default.Save();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error saving activate on launch settings: {ex.Message}");
        }
    }

    public void Load()
    {
        try
        {
            _values.Clear();
            var raw = SettingsManager.Default.ActivateOnLaunchActionIds;
            if (string.IsNullOrEmpty(raw)) return;

            foreach (var token in raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var separatorIndex = token.IndexOf(':');
                if (separatorIndex <= 0) continue;
                _values[token.Substring(0, separatorIndex)] = token.Substring(separatorIndex + 1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error loading activate on launch settings: {ex.Message}");
        }
    }
}