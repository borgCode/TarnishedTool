// 

using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.GameIds.EzState;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class EzStateService(IMemoryService memoryService) : IEzStateService
{
    private bool _npcTalkCreated;
    
    public void ExecuteTalkCommand(TalkCommand command) => ExecuteTalkCommand(command, 0);

    public void ExecuteTalkCommand(TalkCommand command, long chrHandle)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateTalkCode;
        var paramsLoc = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateTalkParams;
        
        for (int i = 0; i < command.Params.Length; i++)
        {
            memoryService.WriteInt32(paramsLoc + i * 4, command.Params[i]);
        }
        
        var bytes = AsmLoader.GetAsmBytes("ExecuteTalkCommand");
        AsmHelper.WriteRelativeOffsets(bytes, new []
        {
            (code.ToInt64() + 0x16, Functions.ExternalEventTempCtor, 5, 0x16 + 1),
            (code.ToInt64() + 0x5A, paramsLoc.ToInt64(), 7, 0x5A + 3),
            (code.ToInt64() + 0x9A, Functions.ExecuteTalkCommand, 5, 0x9A + 1),
        });

        AsmHelper.WriteImmediateDwords(bytes, new[]
        {
            (command.CommandId, 0x11 + 1),
            (command.Params.Length, 0x4D + 1)
        });
        
        AsmHelper.WriteAbsoluteAddress(bytes, chrHandle, 0x3F + 2);
        
        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code);
    }

    public EnvQueryResult EnvQuery(int commandId, params EnvQueryParam[] args)
    {
        EnsureNpcTalkCreated();
        
        var paramCount = 1 + args.Length;

        var stackBase = CodeCaveOffsets.Base + CodeCaveOffsets.EnvParams;
        BuildStack(stackBase, commandId, args);
        
        var result = CodeCaveOffsets.Base + CodeCaveOffsets.EnvQueryResult;
        
        ExecuteEnvQuery(result, paramCount);
        
        return ReadResult(result);
        
    }

    
    public void RequestNewNpcTalk() => _npcTalkCreated = false;
    
    private void EnsureNpcTalkCreated()
    {
        if (_npcTalkCreated) return;
        CreateNpcEzStateTalk(1000); 
        _npcTalkCreated = true;
    }

    private void CreateNpcEzStateTalk(uint talkScriptId, uint blockId = 0, long chrHandle = 0)
    {
        var savedNpcTalk = CodeCaveOffsets.Base + CodeCaveOffsets.SavedNpcTalk;
        var bytes = AsmLoader.GetAsmBytes("CreateNpcTalk");
        AsmHelper.WriteAbsoluteAddresses(bytes, new []
        {
            (savedNpcTalk.ToInt64(), 0x4 + 2),
            (chrHandle, 0x22 + 2),
            (Functions.NpcEzStateTalkCtor, 0x2C + 2),
        });
        
        AsmHelper.WriteImmediateDwords(bytes, new []
        {
            ((int)blockId, 0x11 + 3),
            ((int)talkScriptId, 0x1C + 2),
        });
        
        memoryService.AllocateAndExecute(bytes);
        
    }
    
    private void BuildStack(nint stackBase, int commandId, EnvQueryParam[] args)
    {
        memoryService.WriteInt32(stackBase, commandId);
        memoryService.WriteInt32(stackBase + 0x08, TypeInt);
        memoryService.WriteInt32(stackBase + 0x0C, 0);
        
        for (int i = 0; i < args.Length; i++)
        {
            var offset = (i + 1) * 0x10;
            var arg = args[i];

            switch (arg.TypeTag)
            {
                case TypeInt:
                    memoryService.Write(stackBase + offset, (int)arg.Value);
                    break;
                case TypeFloat:
                    memoryService.Write(stackBase + offset, (float)arg.Value);
                    break;
                case TypeObject:
                    memoryService.Write(stackBase + offset, (long)arg.Value);
                    break;
            }

            memoryService.WriteInt32(stackBase + offset + 0x08, arg.TypeTag);
            memoryService.WriteInt32(stackBase + offset + 0x0C, 0);
        }
    }
    
    private EnvQueryResult ReadResult(nint resultAddr)
    {
        var typeTag = memoryService.ReadInt32(resultAddr + 0x08);

        return new EnvQueryResult
        {
            TypeTag = typeTag,
            IntValue = memoryService.ReadInt32(resultAddr),
            FloatValue = memoryService.ReadFloat(resultAddr),
            PtrValue = memoryService.ReadInt64(resultAddr)
        };
    }
    
    private void ExecuteEnvQuery(IntPtr result, int paramCount)
    {
        var bytes = AsmLoader.GetAsmBytes("ExecuteEnvQuery");
        var envParams = CodeCaveOffsets.Base + CodeCaveOffsets.EnvParams;
        var savedNpcTalk = CodeCaveOffsets.Base + CodeCaveOffsets.SavedNpcTalk;
        
        AsmHelper.WriteImmediateDwords(bytes, new[]
        {
            (paramCount, 0x7 + 2)
        });
    
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (envParams.ToInt64(), 0x10 + 2),
            (savedNpcTalk.ToInt64(), 0x2A + 2),
            (result.ToInt64(), 0x37 + 2),
            (Functions.EzStateEnvQueryImplCtor, 0x45 + 2)
        });
    
        memoryService.AllocateAndExecute(bytes);
    }
}