// 

using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class ItemService(IMemoryService memoryService) : IItemService
{
    public void SpawnItem(int itemId, int quantity, int aowId, bool isQuantityAdjustable, int maxQuantity)
    {
        var shouldAdjustQuantity = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldCheckQuantity;
        var maxQuantityLoc = CodeCaveOffsets.Base + CodeCaveOffsets.MaxQuantity;
        var itemStruct = CodeCaveOffsets.Base + CodeCaveOffsets.ItemSpawnStruct;
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ItemSpawnCode;
        
        memoryService.Write(shouldAdjustQuantity, isQuantityAdjustable);
        memoryService.Write(maxQuantityLoc, maxQuantity);
        memoryService.Write(itemStruct + 0x40, 1);
        memoryService.Write(itemStruct + 0x44, itemId);
        memoryService.Write(itemStruct + 0x48, quantity);
        memoryService.Write(itemStruct + 0x4C, -1);
        memoryService.Write(itemStruct + 0x50, aowId);

        var bytes = AsmLoader.GetAsmBytes(AsmScript.ItemSpawn);
        AsmHelper.WriteRelativeOffsets(bytes, new []
        {
            (code + 0x4, itemStruct, 7, 0x4 + 3),
            (code + 0xB, shouldAdjustQuantity, 7, 0xB + 2),
            (code + 0x1D, maxQuantityLoc, 6, 0x1D + 2),
            (code + 0x18, Functions.GetPlayerItemQuantityById, 5, 0x18 + 1),
            (code + 0x31, MapItemManImpl.Base, 7, 0x31 + 3),
            (code + 0x46, Functions.ItemSpawn, 5, 0x46 + 1)
        });
        
        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code);
    }
}