// 

using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.GameIds.Emevd;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class EmevdService(IMemoryService memoryService) : IEmevdService
{
    public void ExecuteEmevdCommand(EmevdCommand command)
    {
        var args = CodeCaveOffsets.Base + CodeCaveOffsets.EmevdArgs;
        memoryService.WriteBytes(args, command.ParamData);
        
        var bytes = AsmLoader.GetAsmBytes(AsmScript.ExecuteEmevdCommand);
        AsmHelper.WriteAbsoluteAddresses(bytes, new []
        {
            (Functions.EmkEventInsCtor, 0x67 + 2),
            (args.ToInt64(), 0x95 + 2),
            (CSEmkSystem.Base.ToInt64(), 0xA9 + 2),
            (Functions.EmevdSwitch, 0xC0 + 2)
        });
        
        AsmHelper.WriteImmediateDwords(bytes, new []
        {
            (command.GroupId, 0x7A + 2),
            (command.CommandId, 0x80 + 3)
        });
        memoryService.AllocateAndExecute(bytes);
        
        memoryService.WriteBytes(args, new byte[command.ParamData.Length]);
    }
}