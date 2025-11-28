using System.Collections.ObjectModel;
using System.Windows.Media;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Services;
using SilkyRing.Utilities;

namespace SilkyRing.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly EventService _eventService;
        //
        // private NpcInfo _selectedNpc;
        // private ObservableCollection<NpcInfo> _npcList;
        // private bool _canMoveToMajula;
        // private string _setFlagId;
        // private int _flagStateIndex;
        // private string _getFlagId;
        //
        // private string _eventStatusText;
        // private Brush _eventStatusColor;
        //
        // private bool _areButtonsEnabled;
        
        public EventViewModel(EventService eventService, IStateService stateService)
        {
            LoadNpcs();
        }

        private void LoadNpcs()
        {
            // NpcList = new ObservableCollection<NpcInfo>(
            //     DataLoader.GetNpcs()
            // );
            //
            // if (NpcList.Count > 0)
            //     SelectedNpc = NpcList[0];
        }

        // public ObservableCollection<NpcInfo> NpcList
        // {
        //     get => _npcList;
        //     set => SetProperty(ref _npcList, value);
        // }
        //
        // public NpcInfo SelectedNpc
        // {
        //     get => _selectedNpc;
        //     set
        //     {
        //         SetProperty(ref _selectedNpc, value);
        //   
        //         CanMoveToMajula = value != null && 
        //                           (value.MoveToMajulaFlagIds != null && 
        //                            value.MoveToMajulaFlagIds.Length > 0 && 
        //                            value.MoveToMajulaFlagIds[0] != 0);
        //     }
        // }
        //
        // public bool CanMoveToMajula
        // {
        //     get => _canMoveToMajula;
        //     private set => SetProperty(ref _canMoveToMajula, value);
        // }
        // public void SetNpcAlive()
        // {
        //     if (SelectedNpc == null) return;
        //     _utilityService.SetEventOff(SelectedNpc.DeathFlagId);
        // }
        //
        // public void SetNpcDead()
        // {
        //     if (SelectedNpc == null) return;
        //     _utilityService.SetEventOn(SelectedNpc.DeathFlagId);
        // }
        //
        // public void SetNpcFriendly()
        // {
        //     if (SelectedNpc == null) return;
        //     _utilityService.SetEventOff(SelectedNpc.HostileFlagId);
        // }
        //
        // public void SetNpcHostile()
        // {
        //     if (SelectedNpc == null) return;
        //     _utilityService.SetEventOn(SelectedNpc.HostileFlagId);
        // }
        //
        // public void MoveNpcToMajula()
        // {
        //     if (SelectedNpc == null || !SelectedNpc.HasMajulaFlags) return;
        //     foreach (int flagId in SelectedNpc.MoveToMajulaFlagIds)
        //     {
        //         _utilityService.SetEventOn(flagId);
        //     }
        // }
        //
        // public void UnlockDarklurker() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.DarklurkerDungeonsLit);
        // public void UnlockNash() => _utilityService.SetEventOn(GameIds.EventFlags.GiantLordDefeated);
        // public void UnlockAldia() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.UnlockAldia);
        // public void VisibleAava() => _utilityService.SetEventOn(GameIds.EventFlags.VisibleAava);
        // public void BreakIce() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.Dlc3Ice);
        // public void RescueKnights() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.Dlc3Knights);
        // public void KingsRingAcquired() => _utilityService.SetEventOn(GameIds.EventFlags.KingsRingAcquired);
        // public void ActivateBrume() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.Scepter);

        //
        // public string SetFlagId
        // {
        //     get => _setFlagId;
        //     set => SetProperty(ref _setFlagId, value);
        // }
        //
        // public int FlagStateIndex
        // {
        //     get => _flagStateIndex;
        //     set => SetProperty(ref _flagStateIndex, value);
        // }
        //
        // public void SetFlag()
        // {
        //     if (string.IsNullOrWhiteSpace(SetFlagId))
        //         return;
        //     
        //     string trimmedFlagId = SetFlagId.Trim();
        //
        //     if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
        //         return;
        //     
        //     if (FlagStateIndex == 0) _utilityService.SetEventOn(flagIdValue);
        //     else _utilityService.SetEventOff(flagIdValue);
        // }
        //
        // public string GetFlagId
        // {
        //     get => _getFlagId;
        //     set => SetProperty(ref _getFlagId, value);
        // }
        //
        // public string EventStatusText
        // {
        //     get => _eventStatusText;
        //     set => SetProperty(ref _eventStatusText, value);
        // }
        //
        // public Brush EventStatusColor
        // {
        //     get => _eventStatusColor;
        //     set => SetProperty(ref _eventStatusColor, value);
        // }
        //
        // public void GetEvent()
        // {
        //     if (string.IsNullOrWhiteSpace(GetFlagId))
        //         return;
        //     
        //     string trimmedFlagId = GetFlagId.Trim();
        //     
        //     if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
        //         return;
        //
        //     if (_utilityService.GetEvent(flagIdValue))
        //     {
        //         EventStatusText = "True";
        //         EventStatusColor = Brushes.Chartreuse;
        //     }
        //     else
        //     {
        //         EventStatusText = "False";
        //         EventStatusColor = Brushes.Red;
        //     }
        // }
        //
        //
        // public bool AreButtonsEnabled
        // {
        //     get => _areButtonsEnabled;
        //     set => SetProperty(ref _areButtonsEnabled, value);
        // }
        //
        // private bool _isAreaBastille;
        //
        // public bool IsAreaBastille
        // {
        //     get => _isAreaBastille;
        //     set => SetProperty(ref _isAreaBastille, value);
        // }
        //
        // private bool _isSnowstormDisabled;
        //
        // public bool IsSnowstormDisabled
        // {
        //     get => _isSnowstormDisabled;
        //     set
        //     {
        //         if (SetProperty(ref _isSnowstormDisabled, value))
        //         {
        //             _utilityService.ToggleSnowstormHook(_isSnowstormDisabled);
        //             if (AreButtonsEnabled) _utilityService.SetEventOff(GameIds.EventFlags.FrigidSnowstorm);
        //         }
        //     }
        // }
        //
        // private bool _isMemoryTimerDisabled;
        //
        // public bool IsMemoryTimerDisabled
        // {
        //     get => _isMemoryTimerDisabled;
        //     set
        //     {
        //         if (SetProperty(ref _isMemoryTimerDisabled, value))
        //         {
        //             _utilityService.ToggleMemoryTimer(_isMemoryTimerDisabled);
        //          
        //         }
        //     }
        // }
        //
        // public void TryEnableFeatures()
        // {
        //     AreButtonsEnabled = true;
        // }
        //
        // public void DisableFeatures()
        // {
        //     AreButtonsEnabled = false;
        // }
        //
        // public void TryApplyOneTimeFeatures()
        // {
        //     if (IsSnowstormDisabled)
        //     {
        //         _utilityService.ToggleSnowstormHook(true);
        //         _utilityService.SetEventOff(GameIds.EventFlags.FrigidSnowstorm);
        //     }
        //     if (IsMemoryTimerDisabled) _utilityService.ToggleMemoryTimer(true);
        //     if (IsIvorySkipEnabled) _utilityService.ToggleIvorySkip(true);
        // }
        //
        // private bool _isIvorySkipEnabled;
        //
        // public bool IsIvorySkipEnabled
        // {
        //     get => _isIvorySkipEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isIvorySkipEnabled, value)) return;
        //         _utilityService.ToggleIvorySkip(_isIvorySkipEnabled);
        //     }
        // }
        //
        // public void Test()
        // {
        //    
        // }
        //
        // public void OpenGargsDoor()
        // {
        //     _utilityService.SetObjState(GameIds.Area.Bastille, GameIds.Obj.GargoylesDoor);
        //     _utilityService.DisableNavimesh(GameIds.Area.Bastille, GameIds.Navimesh.GargoylesDoor);
        //     _utilityService.DisableWhiteDoor(GameIds.Area.Bastille, GameIds.WhiteDoor.GargoylesDoor);
        // }
        //
        // public void LightSinner()
        // {
        //     _utilityService.SetObjState(GameIds.Area.Bastille, GameIds.Obj.SinnerLighting1);
        //     _utilityService.SetObjState(GameIds.Area.Bastille, GameIds.Obj.SinnerLighting2);
        //     _utilityService.SetObjState(GameIds.Area.Bastille, GameIds.Obj.SinnerLighting3);
        //     _utilityService.SetObjState(GameIds.Area.Bastille, GameIds.Obj.SinnerLighting4);
        // }
        //
        // public void AreaChange(int currentArea)
        // {
        //     IsAreaBastille = currentArea == GameIds.Area.Bastille;
        // }
    }
}