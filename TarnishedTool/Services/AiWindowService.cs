// 

using System.Collections.Generic;
using System.Linq;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;
using TarnishedTool.Views.Windows;

namespace TarnishedTool.Services;

public class AiWindowService : IAiWindowService
{
    private readonly IAiService _aiService;
    private readonly IGameTickService _gameTickService;
    private readonly Dictionary<int, GoalInfo> _goalInfos;
    private readonly Dictionary<string, Dictionary<int, string>> _enumDicts;
    private readonly Dictionary<int, string> _aiInterruptEnums;
    private readonly ISpEffectService _spEffectService;
    
    private readonly Dictionary<nint, AiWindow> _openAiWindows = new();
    
    private const int MaxAiWindows = 3;

    public AiWindowService(IAiService aiService, IGameTickService gameTickService, ISpEffectService spEffectService)
    {
        _aiService = aiService;
        _gameTickService = gameTickService;
        _spEffectService = spEffectService;
        
        _goalInfos = DataLoader.LoadGoalInfo();
        var aiTargetEnums = DataLoader.GetSimpleDict("AiTargetEnum", int.Parse, s => s);
        _aiInterruptEnums = DataLoader.GetSimpleDict("AiInterruptEnum", int.Parse, s => s);
        var aiGoalResulEnums = DataLoader.GetSimpleDict("AiGoalResultEnum", int.Parse, s => s);
        var aiGuardGoalEnums = DataLoader.GetSimpleDict("AiGuardGoalEnum", int.Parse, s => s);
        var aiDirTypeEnums = DataLoader.GetSimpleDict("AiDirTypeEnum", int.Parse, s => s);

        _enumDicts = new Dictionary<string, Dictionary<int, string>>
        {
            ["target"] = aiTargetEnums,
            ["dirtype"] = aiDirTypeEnums,
            ["goalresult"] = aiGoalResulEnums,
            ["guardresult"] = aiGuardGoalEnums
        };
 
    }

    
    public void OpenAiWindow(ChrInsEntry entry)
    {
        if (entry == null) return;
        var chrIns = entry.ChrIns;

        if (_openAiWindows.TryGetValue(chrIns, out var existing))
        {
            existing.Activate();
            return;
        }

        if (_openAiWindows.Count >= MaxAiWindows)
        {
            MsgBox.Show("Only 3 AI windows can be open at once, close one to open another", "Too many AI windows");
            return;
        }

        var window = new AiWindow();
        var vm = new AiWindowViewModel(_aiService, _gameTickService, _goalInfos, entry, _enumDicts,
            _aiInterruptEnums, _aiService.GetAiThinkPtr(chrIns), _spEffectService, window);
        window.DataContext = vm;
        window.Closed += (_, _) => _openAiWindows.Remove(chrIns);
        _openAiWindows[chrIns] = window;
        window.Show();
    }

    public void UpdateAiWindow(nint oldChrIns, ChrInsEntry newEntry)
    {
        if (!_openAiWindows.TryGetValue(oldChrIns, out var window)) return;
    
        _openAiWindows.Remove(oldChrIns);
    
        var vm = window.DataContext as AiWindowViewModel;
        vm?.UpdateTarget(newEntry);
    
        var newChrIns = newEntry.ChrIns;
        window.Closed += (_, _) => _openAiWindows.Remove(newChrIns);
        _openAiWindows[newChrIns] = window;
    }

    
    public void CloseAllAiWindows()
    {
        foreach (var window in _openAiWindows.Values.ToList())
        {
            window.Close();
        }
        
        _openAiWindows.Clear();
    }

    public void CloseSpecificWindow(nint chrIns)
    {
        if (_openAiWindows.TryGetValue(chrIns, out var window))
        {
            window.Close();
            _openAiWindows.Remove(chrIns);
        }
    }
}