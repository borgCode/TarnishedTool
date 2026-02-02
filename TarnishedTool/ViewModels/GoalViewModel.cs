// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TarnishedTool.ViewModels;

public class GoalViewModel : BaseViewModel
{
    public int GoalId { get; set; }
    private string _name;
    private float _life;
    private float _turnTime;
    private ObservableCollection<GoalViewModel> _children;
    private int _indentLevel;

    #region Properties

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public float Life
    {
        get => _life;
        set => SetProperty(ref _life, value);
    }

    public float TurnTime
    {
        get => _turnTime;
        set => SetProperty(ref _turnTime, value);
    }

    public bool HasTurnTime => TurnTime > 0;

    public List<GoalParamViewModel> Params { get; set; }

    public bool HasParams => Params?.Count > 0;

    public ObservableCollection<GoalViewModel> Children
    {
        get => _children;
        set => SetProperty(ref _children, value);
    }

    public int IndentLevel
    {
        get => _indentLevel;
        set => SetProperty(ref _indentLevel, value);
    }
    
    public string FormattedParams
    {
        get
        {
            if (Params == null)
                return string.Empty;
        
            const int paramsPerRow = 4;
        
            var chunks = Params
                .Select((p, i) => new { Formatted = $"  {p.Label}:{p.Value:F1}", Index = i })
                .GroupBy(x => x.Index / paramsPerRow)
                .Select(g => string.Join(", ", g.Select(x => x.Formatted)));
        
            return string.Join("\n", chunks);
        }
    }
    

    #endregion
}