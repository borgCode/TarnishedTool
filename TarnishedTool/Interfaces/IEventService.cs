// 

namespace TarnishedTool.Interfaces;

public interface IEventService
{
    void SetEvent(long eventId, bool flagValue);
    bool GetEvent(long eventId);
    void PatchEventEnable();
    void ToggleDrawEvents(bool isEnabled);
    void ToggleDisableEvents(bool isEnabled);
    bool AreAllEventsTrue(long[] unlockMetyr);
    void ToggleEvent(long clearDlc);
    void ToggleEventLogger(bool isEnabled);
}