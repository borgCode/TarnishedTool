// 

using System;
using TarnishedTool.Interfaces;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class DlcService(IMemoryService memoryService) : IDlcService
{
    
    public void CheckDlc()
    {
        var flags = memoryService.Read<nint>(CsDlcImp.Base) + CsDlcImp.ByteFlags;
        IsDlcAvailable = memoryService.Read<byte>((IntPtr)flags + (int)CsDlcImp.Flags.DlcCheck) == 1;
        PreOrderBaseGesture = memoryService.Read<byte>((IntPtr)flags + (int)CsDlcImp.Flags.DlcCheck) == 0;
        PreOrderDlcGesture = memoryService.Read<byte>((IntPtr)flags + (int)CsDlcImp.Flags.DlcCheck) == 2;
    }

    public bool IsDlcAvailable { get; private set; }
    public bool PreOrderBaseGesture { get; private set; }
    public bool PreOrderDlcGesture { get; private set; }
}