// 

using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class SettingsService(IMemoryService memoryService) : ISettingsService
{
    

    public void ToggleStutterFix(bool isEnabled) =>
        memoryService.Write(memoryService.Read<nint>(UserInputManager.Base) + UserInputManager.SteamInputEnum,
            isEnabled);

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

    private byte? _originalMusicVolume = SettingsManager.Default.HasOriginalMusicVolume
        ? SettingsManager.Default.OriginalMusicVolume
        : null;

    public void ToggleMuteMusic(bool isMuteMusicEnabled)
    {
        var optionsPtr =
            memoryService.Read<nint>(memoryService.Read<nint>(GameDataMan.Base) + GameDataMan.Options);
        var musicPtr = optionsPtr + (int)GameDataMan.OptionsOffsets.Music;

        if (isMuteMusicEnabled)
        {
            if (!SettingsManager.Default.HasOriginalMusicVolume)
            {
                _originalMusicVolume = memoryService.Read<byte>(musicPtr);
                SettingsManager.Default.OriginalMusicVolume = _originalMusicVolume.Value;
                SettingsManager.Default.HasOriginalMusicVolume = true;
                SettingsManager.Default.Save();
            }

            memoryService.Write(musicPtr, (byte)0);
        }
        else
        {
            memoryService.Write(musicPtr, SettingsManager.Default.OriginalMusicVolume);
            SettingsManager.Default.HasOriginalMusicVolume = false;
            SettingsManager.Default.Save();
        }
    }
}