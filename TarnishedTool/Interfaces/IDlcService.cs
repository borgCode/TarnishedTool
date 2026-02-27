// 

namespace TarnishedTool.Interfaces;

public interface IDlcService
{
    void CheckDlc();
    public bool IsDlcAvailable { get; }
    public bool PreOrderBaseGesture { get; }
    public bool PreOrderDlcGesture { get; }
}