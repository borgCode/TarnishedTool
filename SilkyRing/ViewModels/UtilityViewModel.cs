using System;
using System.Windows.Input;
using SilkyRing.Core;
using SilkyRing.Enums;
using SilkyRing.GameIds;
using SilkyRing.Interfaces;
using SilkyRing.Services;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.ViewModels
{
    public class UtilityViewModel : BaseViewModel
    {
        private const float DefaultNoclipMultiplier = 1f;
        private const uint BaseSpeed = 0x3e4ccccd;

        private bool _isDrawHitboxEnabled;
        private bool _isDrawLowHitEnabled;
        private bool _isDrawHighHitEnabled;
        private int _colDrawMode = 1;
        private bool _isDrawRagdollEnabled;

        private readonly IUtilityService _utilityService;
        private readonly IEzStateService _ezStateService;
        private readonly IPlayerService _playerService;
        private readonly HotkeyManager _hotkeyManager;

        public UtilityViewModel(IUtilityService utilityService, IStateService stateService,
            IEzStateService ezStateService, IPlayerService playerService, HotkeyManager hotkeyManager)
        {
            _utilityService = utilityService;
            _ezStateService = ezStateService;
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
            stateService.Subscribe(State.FirstLoaded, OnGameFirstLoaded);


            RegisterHotkeys();
        }

        #region Commands

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isNoClipEnabled;

        public bool IsNoClipEnabled
        {
            get => _isNoClipEnabled;
            set
            {
                if (!SetProperty(ref _isNoClipEnabled, value)) return;
                _utilityService.ToggleNoClip(_isNoClipEnabled);
            }
        }

        private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;

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
            if (!IsNoClipEnabled) return;
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

        private bool _isCombatMapEnabled;

        public bool IsCombatMapEnabled
        {
            get => _isCombatMapEnabled;
            set
            {
                if (!SetProperty(ref _isCombatMapEnabled, value)) return;
                _utilityService.ToggleCombatMap(_isCombatMapEnabled);
            }
        }

        private bool _isDungeonWarpEnabled;

        public bool IsDungeonWarpEnabled
        {
            get => _isDungeonWarpEnabled;
            set
            {
                if (!SetProperty(ref _isDungeonWarpEnabled, value)) return;
                _utilityService.ToggleDungeonWarp(_isDungeonWarpEnabled);
            }
        }

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
        }

        private void OnGameFirstLoaded()
        {
            if (IsCombatMapEnabled) _utilityService.ToggleCombatMap(true);
            if (IsDungeonWarpEnabled) _utilityService.ToggleDungeonWarp(true);
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.Noclip, () => { IsNoClipEnabled = !IsNoClipEnabled; });
            // _hotkeyManager.RegisterAction("IncreaseNoClipSpeed", () =>
            // {
            //     if (IsNoClipEnabled)
            //         NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
            // });
            //
            // _hotkeyManager.RegisterAction("DecreaseNoClipSpeed", () =>
            // {
            //     if (IsNoClipEnabled)
            //         NoClipSpeed = Math.Max(0.05f, NoClipSpeed - 0.50f);
            // });
            // _hotkeyManager.RegisterAction("ToggleGameSpeed", ToggleSpeed);
            // _hotkeyManager.RegisterAction("IncreaseGameSpeed", () => SetSpeed(Math.Min(10, GameSpeed + 0.50f)));
            // _hotkeyManager.RegisterAction("DecreaseGameSpeed", () => SetSpeed(Math.Max(0, GameSpeed - 0.50f)));
        }

        #endregion

        // public bool IsDrawHitboxEnabled
        // {
        //     get => _isDrawHitboxEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawHitboxEnabled, value)) return;
        //         _utilityService.ToggleDrawHitbox(_isDrawHitboxEnabled);
        //         _utilityService.SetColDrawMode(ColDrawMode);
        //     }
        // }
        //
        // public bool IsDrawLowHitEnabled
        // {
        //     get => _isDrawLowHitEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawLowHitEnabled, value)) return;
        //         // _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.LowHit, _isDrawLowHitEnabled);
        //         _utilityService.SetColDrawMode(ColDrawMode);
        //     }
        // }
        //
        // public bool IsDrawHighHitEnabled
        // {
        //     get => _isDrawHighHitEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawHighHitEnabled, value)) return;
        //         // _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.HighHit, _isDrawHighHitEnabled);
        //     }
        // }
        //
        // public int ColDrawMode
        // {
        //     get => _colDrawMode;
        //     set
        //     {
        //         if (!SetProperty(ref _colDrawMode, value)) return;
        //         if (!IsDrawHighHitEnabled && !IsDrawLowHitEnabled) return;
        //         _utilityService.SetColDrawMode(_colDrawMode);
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

        // private bool _isTargetingViewEnabled;
        //
        // public bool IsTargetingViewEnabled
        // {
        //     get => _isTargetingViewEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isTargetingViewEnabled, value)) return;
        //         _utilityService.ToggleTargetingView(_isTargetingViewEnabled);
        //         if (!_isTargetingViewEnabled)
        //         {
        //             IsDrawReducedTargetViewEnabled = false;
        //         }
        //     }
        // }
        //
        // private bool _isDrawReducedTargetViewEnabled;
        //
        // public bool IsDrawReducedTargetViewEnabled
        // {
        //     get => _isDrawReducedTargetViewEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawReducedTargetViewEnabled, value)) return;
        //         _utilityService.ToggleReducedTargetingView(_isDrawReducedTargetViewEnabled);
        //         _utilityService.SetTargetViewMaxDist(ReducedTargetViewDistance);
        //     }
        // }
        //
        // private float _reducedTargetViewDistance = 100;
        //
        // public float ReducedTargetViewDistance
        // {
        //     get => _reducedTargetViewDistance;
        //     set
        //     {
        //         if (!SetProperty(ref _reducedTargetViewDistance, value)) return;
        //         if (!IsDrawReducedTargetViewEnabled) return;
        //         _utilityService.SetTargetViewMaxDist(_reducedTargetViewDistance);
        //     }
        // }
        //
        // //
        //
        // public bool IsDrawRagdollsEnabled
        // {
        //     get => _isDrawRagdollEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isDrawRagdollEnabled, value)) return;
        //         // _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.Ragdoll, _isDrawRagdollEnabled);
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

        public void TryApplyOneTimeFeatures()
        {
            //
            // if (IsDrawHitboxEnabled) _utilityService.ToggleDrawHitbox(true);
            //
            // if (IsTargetingViewEnabled) _utilityService.ToggleTargetingView(true);
            // if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled)
            //     _utilityService.ToggleReducedTargetingView(true);
            // if (IsDrawReducedTargetViewEnabled && IsTargetingViewEnabled)
            //     _utilityService.SetTargetViewMaxDist(ReducedTargetViewDistance);
            //
            // if (IsDrawLowHitEnabled) _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.LowHit, true);
            // if (IsDrawHighHitEnabled) _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.HighHit, true);
            //
            // if (IsDrawRagdollsEnabled) _utilityService.ToggleWorldHitDraw(WorldHitMan.Offsets.Ragdoll, true);
            //
        }
    }
}