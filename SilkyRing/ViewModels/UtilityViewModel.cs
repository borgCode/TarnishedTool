using SilkyRing.Services;

namespace SilkyRing.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private const float DefaultNoclipMultiplier = 1f;
        private const uint BaseXSpeedHex = 0x3e4ccccd;
        private const uint BaseYSpeedHex = 0x3e19999a;
        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;
        private bool _isNoClipEnabled;
        
        
        private readonly UtilityService _utilityService;
        public UtilityViewModel(UtilityService utilityService)
        {
            _utilityService = utilityService;
            // RegisterHotkeys();
        }
        
        // private void RegisterHotkeys()
        // {
        //     _hotkeyManager.RegisterAction("ForceSave", () =>
        //     {
        //         if (!AreButtonsEnabled) return;
        //         _utilityService.ForceSave();
        //     });
        //     _hotkeyManager.RegisterAction("NoClip", () => { IsNoClipEnabled = !IsNoClipEnabled; });
        //     _hotkeyManager.RegisterAction("IncreaseNoClipSpeed", () =>
        //     {
        //         if (IsNoClipEnabled)
        //             NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
        //     });
        //
        //     _hotkeyManager.RegisterAction("DecreaseNoClipSpeed", () =>
        //     {
        //         if (IsNoClipEnabled)
        //             NoClipSpeed = Math.Max(0.05f, NoClipSpeed - 0.50f);
        //     });
        //     _hotkeyManager.RegisterAction("ToggleGameSpeed", ToggleSpeed);
        //     _hotkeyManager.RegisterAction("IncreaseGameSpeed", () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
        //     _hotkeyManager.RegisterAction("DecreaseGameSpeed", () => SetSpeed(Math.Max(0, GameSpeed - 0.50f)));
        // }


        // private bool _areButtonsEnabled;
        // public bool AreButtonsEnabled
        // {
        //     get => _areButtonsEnabled;
        //     set => SetProperty(ref _areButtonsEnabled, value);
        // }
        
        // public bool IsDrawHitboxEnabled
        // {
        //     get => _isDrawHitboxEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawHitboxEnabled, value)) return;
        //         _utilityService.ToggleDrawHitbox(_isDrawHitboxEnabled);
        //     }
        // }
        //
        // public bool IsDrawEventEnabled
        // {
        //     get => _isDrawEventEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventEnabled, value)) return;
        //         IsDrawEventGeneralEnabled = _isDrawEventEnabled;
        //         IsDrawEventSpawnEnabled = _isDrawEventEnabled;
        //         IsDrawEventInvasionEnabled = _isDrawEventEnabled;
        //         IsDrawEventLeashEnabled = _isDrawEventEnabled;
        //         if (!_isDrawEventEnabled) IsDrawEventOtherEnabled = false;
        //     }
        // }
        //
        // public bool IsDrawEventGeneralEnabled
        // {
        //     get => _isDrawEventGeneralEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventGeneralEnabled, value)) return;
        //         _utilityService.ToggleDrawEvent(DrawType.EventGeneral, _isDrawEventGeneralEnabled);
        //     }
        // }
        //
        // public bool IsDrawEventSpawnEnabled
        // {
        //     get => _isDrawEventSpawnEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventSpawnEnabled, value)) return;
        //         _utilityService.ToggleDrawEvent(DrawType.EventSpawn, _isDrawEventSpawnEnabled);
        //     }
        // }
        //
        // public bool IsDrawEventInvasionEnabled
        // {
        //     get => _isDrawEventInvasionEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventInvasionEnabled, value)) return;
        //         _utilityService.ToggleDrawEvent(DrawType.EventInvasion, _isDrawEventInvasionEnabled);
        //     }
        // }
        //
        // public bool IsDrawEventLeashEnabled
        // {
        //     get => _isDrawEventLeashEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventLeashEnabled, value)) return;
        //         _utilityService.ToggleDrawEvent(DrawType.EventLeash, _isDrawEventLeashEnabled);
        //     }
        // }
        //
        // public bool IsDrawEventOtherEnabled
        // {
        //     get => _isDrawEventOtherEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawEventOtherEnabled, value)) return;
        //         _utilityService.ToggleDrawEvent(DrawType.EventOther, _isDrawEventOtherEnabled);
        //     }
        // }
        //
        // public bool IsDrawSoundEnabled
        // {
        //     get => _isDrawSoundEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawSoundEnabled, value)) return;
        //         _utilityService.ToggleDrawSound(_isDrawSoundEnabled);
        //     }
        // }
        //
        // public bool IsTargetingViewEnabled
        // {
        //     get => _isTargetingViewEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
        //         _utilityService.ToggleTargetingView(_isTargetingViewEnabled);
        //     }
        // }
        //
        //
        // public bool IsDrawRagdollsEnabled
        // {
        //     get => _isDrawRagdollEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
        //         _utilityService.ToggleRagdoll(_isDrawRagdollEnabled);
        //         if (!_isDrawRagdollEnabled) IsSeeThroughWallsEnabled = false;
        //     }
        // }
        //
        // public bool IsSeeThroughWallsEnabled
        // {
        //     get => _isSeeThroughwallsEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isSeeThroughwallsEnabled, value)) return;
        //         _utilityService.ToggleRagdollEsp(_isSeeThroughwallsEnabled);
        //     }
        // }
        //
        // public bool IsColWireframeEnabled
        // {
        //     get => _isColWireframeEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isColWireframeEnabled, value)) return;
        //         _utilityService.ToggleColWireframe(_isColWireframeEnabled);
        //     }
        // }
        //
        // public bool IsDrawKillboxEnabled
        // {
        //     get => _isDrawKillboxEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawKillboxEnabled, value)) return;
        //         _utilityService.ToggleDrawKillbox(_isDrawKillboxEnabled);
        //     }
        // }
        //
        //
        // public bool IsDrawCollisionEnabled
        // {
        //     get => _isDrawCollisionEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawCollisionEnabled, value)) return;
        //         _utilityService.ToggleDrawCol(_isDrawCollisionEnabled);
        //         if (!_isDrawCollisionEnabled) IsColWireframeEnabled = false;
        //     }
        // }
        //
        //
        // public bool IsHideCharactersEnabled
        // {
        //     get => _isHideCharactersEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isHideCharactersEnabled, value)) return;
        //         _utilityService.ToggleHideChr(_isHideCharactersEnabled);
        //     }
        // }
        //
        // public bool IsHideMapEnabled
        // {
        //     get => _isHideMapEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isHideMapEnabled, value)) return;
        //         _utilityService.ToggleHideMap(_isHideMapEnabled);
        //     }
        // }
        
        public bool IsNoClipEnabled
        {
            get => _isNoClipEnabled;
            set
            {
                if (!SetProperty(ref _isNoClipEnabled, value)) return;
                _utilityService.ToggleNoClip(_isNoClipEnabled);
            }
        }

        public float NoClipSpeed
        {
            get => _noClipSpeedMultiplier;
            set
            {
                if (SetProperty(ref _noClipSpeedMultiplier, value))
                {
                    SetNoClipSpeed(value);
                }
            }
        }

        public void SetNoClipSpeed(float multiplier)
        {
            // if (!IsNoClipEnabled) return;
            // if (multiplier < 0.05f) multiplier = 0.05f;
            // else if (multiplier > 5.0f) multiplier = 5.0f;
            //
            // SetProperty(ref _noClipSpeedMultiplier, multiplier);
            //
            // float baseXFloat = BitConverter.ToSingle(BitConverter.GetBytes(BaseXSpeedHex), 0);
            // float baseYFloat = BitConverter.ToSingle(BitConverter.GetBytes(BaseYSpeedHex), 0);
            //
            // float newXFloat = baseXFloat * multiplier;
            // float newYFloat = baseYFloat * multiplier;
            //
            // byte[] xBytes = BitConverter.GetBytes(newXFloat);
            // byte[] yBytes = BitConverter.GetBytes(newYFloat);
            //
            // _utilityService.SetNoClipSpeed(xBytes, yBytes);
        }
        
    }
}