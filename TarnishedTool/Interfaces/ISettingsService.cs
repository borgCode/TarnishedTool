// 

namespace TarnishedTool.Interfaces;

public interface ISettingsService
{
    
    void ToggleStutterFix(bool isStutterFixEnabled);
    void ToggleDisableAchievements(bool isEnabled);
    void ToggleNoLogo(bool isEnabled);
    void ToggleMuteMusic(bool isMuteMusicEnabled);
}