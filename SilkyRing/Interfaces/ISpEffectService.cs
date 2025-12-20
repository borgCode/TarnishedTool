// 

using System.Collections.Generic;
using SilkyRing.Models;

namespace SilkyRing.Interfaces;

public interface ISpEffectService
{
    void ApplySpEffect(long chrIns, uint spEffectId);
    void RemoveSpEffect(long chrIns, uint spEffectId);
    bool HasSpEffect(long chrIns, uint spEffectId);
    List<SpEffectEntry> GetActiveSpEffectList(long chrIns);
}