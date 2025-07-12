using SilkyRing.Memory.DLLShared;
using SilkyRing.Services;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private const float DefaultNoclipMultiplier = 1f;
        private const uint BaseXSpeedHex = 0x3e4ccccd;
        private const uint BaseYSpeedHex = 0x3e19999a;
        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;
        private bool _isNoClipEnabled;
        
        
        private bool _isDrawHitboxEnabled;
        private bool _isDrawLowHitEnabled;
        private bool _isDrawHighHitEnabled;
        private int _colDrawMode = 1;
        private bool _isDrawRagdollEnabled;


        private readonly UtilityService _utilityService;
        private readonly DllManager _dllManager;
        
        
        public UtilityViewModel(UtilityService utilityService, DllManager dllManager)
        {
            _utilityService = utilityService;
            _dllManager = dllManager;
            
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


        private bool _areOptionsEnabled;
        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }
        
        
        
        public bool IsDrawHitboxEnabled
        {
            get => _isDrawHitboxEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHitboxEnabled, value)) return;
                _utilityService.ToggleDrawHitbox(_isDrawHitboxEnabled);
                _utilityService.SetColDrawMode(ColDrawMode);
            }
        }
        
        public bool IsDrawLowHitEnabled
        {
            get => _isDrawLowHitEnabled;
            set
            {
                if (!SetProperty(ref _isDrawLowHitEnabled, value)) return;
                _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.LowHit, _isDrawLowHitEnabled);
                _utilityService.SetColDrawMode(ColDrawMode);
            }
        }
        
        public bool IsDrawHighHitEnabled
        {
            get => _isDrawHighHitEnabled;
            set
            {
                if (!SetProperty(ref _isDrawHighHitEnabled, value)) return;
                _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.HighHit, _isDrawHighHitEnabled);
            }
        }
        
        public int ColDrawMode
        {
            get => _colDrawMode;
            set
            {
                if (!SetProperty(ref _colDrawMode, value)) return;
                if (!IsDrawHighHitEnabled && !IsDrawLowHitEnabled) return;
                _utilityService.SetColDrawMode(_colDrawMode);
            }
        }
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
        
        private bool _isTargetingViewEnabled;
        public bool IsTargetingViewEnabled
        {
            get => _isTargetingViewEnabled;
            set
            {
                if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
                _utilityService.ToggleTargetingView(_isTargetingViewEnabled);
                if (!_isTargetingViewEnabled)
                {
                    IsDrawReducedTargetViewEnabled = false;
                }
            }
        }
        
        private bool _isDrawReducedTargetViewEnabled;
        public bool IsDrawReducedTargetViewEnabled
        {
            get => _isDrawReducedTargetViewEnabled;
            set
            {
                if (!SetProperty(ref _isDrawReducedTargetViewEnabled, value)) return;
                _utilityService.ToggleReducedTargetingView(_isDrawReducedTargetViewEnabled);
                _utilityService.SetTargetViewMaxDist(ReducedTargetViewDistance);
            }
        }
        
        private float _reducedTargetViewDistance = 100;
        public float ReducedTargetViewDistance
        {
            get => _reducedTargetViewDistance;
            set
            {
                if (!SetProperty(ref _reducedTargetViewDistance, value)) return;
                if (!IsDrawReducedTargetViewEnabled) return;
                _utilityService.SetTargetViewMaxDist(_reducedTargetViewDistance);
            }
        }
        
        //
        
        public bool IsDrawRagdollsEnabled
        {
            get => _isDrawRagdollEnabled;
            set
            {
                if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
                _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.Ragdoll, _isDrawRagdollEnabled);
         
            }
        }
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
        
        public void TryEnableFeatures()
        {
            AreOptionsEnabled = true;
        }
        public void TryApplyOneTimeFeatures()
        {
            // if (Is100DropEnabled) _utilityService.Toggle100Drop(true);
            // if (IsCreditSkipEnabled) _utilityService.ToggleCreditSkip(true);
            if (IsDrawHitboxEnabled) _utilityService.ToggleDrawHitbox(true);
            //
            // if (IsDrawEventGeneralEnabled) _utilityService.ToggleDrawEvent(DrawType.EventGeneral, true);
            // if (IsDrawEventSpawnEnabled) _utilityService.ToggleDrawEvent(DrawType.EventSpawn, true);
            // if (IsDrawEventInvasionEnabled) _utilityService.ToggleDrawEvent(DrawType.EventInvasion, true);
            // if (IsDrawEventLeashEnabled) _utilityService.ToggleDrawEvent(DrawType.EventLeash, true);
            // if (IsDrawEventOtherEnabled) _utilityService.ToggleDrawEvent(DrawType.EventOther, true);
            //
            // if (IsDrawSoundEnabled) _utilityService.ToggleDrawSound(true);
            if (IsTargetingViewEnabled) _utilityService.ToggleTargetingView(true);
            if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled) _utilityService.ToggleReducedTargetingView(true);
            if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled) _utilityService.SetTargetViewMaxDist(ReducedTargetViewDistance);
            
            if (IsDrawLowHitEnabled) _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.LowHit, true);
            if (IsDrawHighHitEnabled) _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.HighHit, true);
            // if (IsHideMapEnabled) _utilityService.ToggleHideMap(true);
            // if (IsHideCharactersEnabled) _utilityService.ToggleHideChr(true);
            // if (IsLightGutterEnabled) _utilityService.ToggleLightGutter(true);
            // if (IsDrawCollisionEnabled) _utilityService.ToggleDrawCol(true);
            // if (IsNoFogEnabled) _utilityService.ToggleShadedFog(true);
            // if (IsColWireframeEnabled) _utilityService.ToggleColWireframe(true);
            // if (IsDrawKillboxEnabled) _utilityService.ToggleDrawKillbox(true);
            if (IsDrawRagdollsEnabled) _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.Ragdoll, true);
            // if (IsSeeThroughWallsEnabled) _utilityService.ToggleRagdollEsp(true);
        }

        public void ForceSave() => _utilityService.ForceSave();

        public void Inject()
        {
            _dllManager.InjectDll();
        }
    }
}