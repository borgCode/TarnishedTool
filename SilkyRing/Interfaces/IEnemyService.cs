// 

namespace SilkyRing.Interfaces;

public interface IEnemyService
{
    void ToggleRykardMega(bool isRykardNoMegaEnabled);
    void ForceActSequence(int[] actSequence, int npcThinkParamId);
    void UnhookForceAct();
}