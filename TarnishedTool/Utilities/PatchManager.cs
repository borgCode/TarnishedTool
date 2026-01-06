// 

using System;
using System.Diagnostics;
using TarnishedTool.Memory;
using TarnishedTool.Services;

namespace TarnishedTool.Utilities;

public static class PatchManager
{
    public static bool Initialize(MemoryService memoryService)
    {
        if (memoryService.TargetProcess == null) return false;
        var process = memoryService.TargetProcess;
        var module = process.MainModule;
        var fileVersion = module?.FileVersionInfo.FileVersion;
        Console.WriteLine(fileVersion);

        return Offsets.Initialize(fileVersion);


        
    }
}