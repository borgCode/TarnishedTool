// 

using TarnishedTool.ViewModels;

namespace TarnishedTool.Interfaces;

public interface IAiWindowService
{
    void OpenAiWindow(ChrInsEntry entry);
    void UpdateAiWindow(nint oldChrIns, ChrInsEntry newEntry);
    void CloseAllAiWindows();
    void CloseSpecificWindow(nint chrIns);
}