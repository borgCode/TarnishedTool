namespace TarnishedTool.Interfaces;

public interface IDamageService
{
    void ToggleDamageMultiplier(bool isEnabled, float outgoing, float incoming);
    void SetOutgoingMultiplier(float value);
    void SetIncomingMultiplier(float value);
}
