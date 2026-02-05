// 

using System.Collections.Generic;

namespace TarnishedTool.Models;

public class GoalInfo
{
    public string GoalName { get; set; }
    public List<GoalParamDef> ParamNames { get; set; }
}