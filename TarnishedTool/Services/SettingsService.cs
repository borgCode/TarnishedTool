// 

using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class SettingsService(IMemoryService memoryService, HookManager hookManager) : ISettingsService
{
    public void Quitout() =>
        memoryService.Write(memoryService.Read<nint>(GameMan.Base) + GameMan.ShouldQuitout, (byte)1);

    public void ToggleStutterFix(bool isEnabled) =>
        memoryService.Write(memoryService.Read<nint>(UserInputManager.Base) + UserInputManager.SteamInputEnum, isEnabled);

    public void ToggleDisableAchievements(bool isEnabled)
    {
        var isAwardAchievementsEnabledFlag = memoryService.FollowPointers(CSTrophy.Base, [
            CSTrophy.CSTrophyPlatformImp_forSteam,
            CSTrophy.IsAwardAchievementEnabled
        ], false);
        memoryService.Write(isAwardAchievementsEnabledFlag, isEnabled);
    }

    public void ToggleNoLogo(bool isEnabled) =>
        memoryService.WriteBytes(Patches.NoLogo, isEnabled ? [0x90, 0x90] : [0x74, 0x53]);

    public void ToggleMuteMusic(bool isMuteMusicEnabled)
    {
        var optionsPtr =
            memoryService.Read<nint>(memoryService.Read<nint>(GameDataMan.Base) + GameDataMan.Options);
        memoryService.Write(optionsPtr + (int)GameDataMan.OptionsOffsets.Music, (byte)0);
    }
}