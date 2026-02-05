// 

using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class SettingsService(IMemoryService memoryService, HookManager hookManager) : ISettingsService
{
    public void Quitout() =>
        memoryService.WriteUInt8((IntPtr)memoryService.ReadInt64(GameMan.Base) + GameMan.ShouldQuitout, 1);

    public void ToggleStutterFix(bool isEnabled) =>
        memoryService.WriteUInt8(
            (IntPtr)memoryService.ReadInt64(UserInputManager.Base) + UserInputManager.SteamInputEnum,
            isEnabled ? 1 : 0);

    public void ToggleDisableAchievements(bool isEnabled)
    {
        var isAwardAchievementsEnabledFlag = memoryService.FollowPointers(CSTrophy.Base, [
            CSTrophy.CSTrophyPlatformImp_forSteam,
            CSTrophy.IsAwardAchievementEnabled
        ], false);
        memoryService.WriteUInt8(isAwardAchievementsEnabledFlag, isEnabled ? 0 : 1);
    }

    public void ToggleNoLogo(bool isEnabled) =>
        memoryService.WriteBytes(Patches.NoLogo, isEnabled ? [0x90, 0x90] : [0x74, 0x53]);

    public void ToggleMuteMusic(bool isMuteMusicEnabled)
    {
        var optionsPtr = memoryService.ReadInt64((IntPtr)memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.Options);
        memoryService.WriteUInt8((IntPtr) optionsPtr + (int)GameDataMan.OptionsOffsets.Music, 0);
    }
}