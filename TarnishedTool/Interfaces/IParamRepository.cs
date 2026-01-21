// 

using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IParamRepository
{
    IReadOnlyList<string> AvailableParams { get; }
    LoadedParam GetParam(string paramName);
}