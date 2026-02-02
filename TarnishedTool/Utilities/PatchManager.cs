// 

using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Services;

namespace TarnishedTool.Utilities;

public static class PatchManager
{
    public static bool Initialize(IMemoryService memoryService)
    {
        if (memoryService.TargetProcess == null) return false;
        var module = memoryService.TargetProcess.MainModule;
        var fileVersion = module?.FileVersionInfo.FileVersion;
        var moduleBase = memoryService.BaseAddress;
        
        Console.WriteLine($@"Patch: {fileVersion}");

        return Offsets.Initialize(fileVersion, moduleBase);
    }
}