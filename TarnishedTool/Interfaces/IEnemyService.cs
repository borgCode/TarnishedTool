// 

namespace TarnishedTool.Interfaces;

public interface IEnemyService
{
    nint GetChrInsByEntityId(uint entityId);
    void ToggleNoDeath(bool isEnabled);
    void ToggleNoDamage(bool isEnabled);
    void ToggleNoHit(bool isEnabled);
    void ToggleNoAttack(bool isEnabled);
    void ToggleNoMove(bool isEnabled);
    void ToggleDisableAi(bool isEnabled);
    void ToggleTargetingView(bool isEnabled);
    void ToggleReducedTargetingView(bool isTargetingViewEnabled);
    void SetTargetViewMaxDist(float reducedTargetViewDistance);
    void ToggleRykardMega(bool isRykardNoMegaEnabled);
    void ForceActSequence(int[] actSequence, int npcThinkParamId);
    void UnhookForceAct();
    void ToggleLionCooldownHook(bool isEnabled, int lionMainBossNpcParamId);
    void ToggleDrawNavigationRoute(bool isDrawNavigationRouteEnabled);
}