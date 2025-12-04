// 

using System.Collections.Generic;
using SilkyRing.Models;

namespace SilkyRing.Interfaces;

public interface IAttackInfoService
{
    void ToggleAttackInfoHook(bool isEnabled);
    List<AttackInfo> PollAttackInfo();
}