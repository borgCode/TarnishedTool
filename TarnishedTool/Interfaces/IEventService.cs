// 

using System.Collections.Generic;

namespace TarnishedTool.Interfaces;

public interface IEventService
{
    void SetEvent(long eventId, bool flagValue);
    bool GetEvent(long eventId);
    void PatchEventEnable();
    void ToggleDrawEvents(bool isEnabled);
    void ToggleDisableEvents(bool isEnabled);
    bool AreAllEventsTrue(long[] eventToCheck);
    void ToggleEvent(long clearDlc);
    void ToggleEventLogger(bool isEnabled);
    void ToggleEvents(IEnumerable<long> eventIds);
    void SetEvents(IEnumerable<long> eventIds, bool flagValue);
}