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
    }

    public bool IsDlcAvailable { get; private set; }
}