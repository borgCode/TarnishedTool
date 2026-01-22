// 

using System.Collections.Generic;
using TarnishedTool.Enums;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IParamRepository
{
    IReadOnlyList<Param> AvailableParams { get; }
    LoadedParam GetParam(Param param);
    (int TableIndex, int SlotIndex) GetLocation(Param paramName);
    public Dictionary<Param, List<ParamEntry>> GetAllEntriesByParam();

}