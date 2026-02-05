// 

using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface ISpEffectService
{
    void ApplySpEffect(nint chrIns, uint spEffectId);
    void RemoveSpEffect(nint chrIns, uint spEffectId);
    bool HasSpEffect(nint chrIns, uint spEffectId);
    List<SpEffectEntry> GetActiveSpEffectList(nint chrIns);
}