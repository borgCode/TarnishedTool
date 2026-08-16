using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels
{
    public class GestureSelectionViewModel : BaseViewModel
    {
        private readonly IEzStateService _ezStateService;

        public ObservableCollection<GestureSelectionItem> BaseGestures { get; }
        public ObservableCollection<GestureSelectionItem> DlcGestures { get; }
        public bool IsDlcAvailable { get; }

        public ICommand SelectAllCommand { get; }
        public ICommand SelectNoneCommand { get; }
        public ICommand UnlockSelectedCommand { get; }
        public ICommand UnlockAllCommand { get; }

        public GestureSelectionViewModel(
            IEzStateService ezStateService,
            IEnumerable<int> baseGestureIds,
            IEnumerable<int> dlcGestureIds,
            bool isDlcAvailable,
            bool preOrderBaseGesture,
            bool preOrderDlcGesture)
        {
            _ezStateService = ezStateService;
            IsDlcAvailable = isDlcAvailable;

            BaseGestures = new ObservableCollection<GestureSelectionItem>(
                baseGestureIds.Where(id => !GestureNameLookup.IsPreOrderVariant(id))
                    .Select(id => new GestureSelectionItem(id, GestureNameLookup.GetName(id))));

            int baseRingId = !preOrderBaseGesture ? 109 : 108;
            BaseGestures.Add(new GestureSelectionItem(baseRingId, GestureNameLookup.GetName(baseRingId)));

            DlcGestures = new ObservableCollection<GestureSelectionItem>(
                dlcGestureIds.Where(id => !GestureNameLookup.IsPreOrderVariant(id))
                    .Select(id => new GestureSelectionItem(id, GestureNameLookup.GetName(id))));

            if (isDlcAvailable)
            {
                int dlcRingId = !preOrderDlcGesture ? 113 : 116;
                DlcGestures.Add(new GestureSelectionItem(dlcRingId, GestureNameLookup.GetName(dlcRingId)));
            }

            SelectAllCommand = new DelegateCommand(() => SetAll(true));
            SelectNoneCommand = new DelegateCommand(() => SetAll(false));
            UnlockSelectedCommand = new DelegateCommand(UnlockSelected);
            UnlockAllCommand = new DelegateCommand(UnlockAll);
        }

        private void SetAll(bool value)
        {
            foreach (var g in BaseGestures) g.IsSelected = value;
            if (IsDlcAvailable) foreach (var g in DlcGestures) g.IsSelected = value;
        }

        private void UnlockSelected()
        {
            foreach (var g in BaseGestures.Where(g => g.IsSelected))
                _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture(g.Id));

            if (!IsDlcAvailable) return;

            foreach (var g in DlcGestures.Where(g => g.IsSelected))
                _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture(g.Id));
        }

        private void UnlockAll()
        {
            foreach (var g in BaseGestures)
                _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture(g.Id));

            if (!IsDlcAvailable) return;

            foreach (var g in DlcGestures)
                _ezStateService.ExecuteTalkCommand(EzState.TalkCommands.AcquireGesture(g.Id));
        }
    }
}