// 

using System.Numerics;

namespace TarnishedTool.Utilities;

public static class PositionUtils
{
    public const int BaseGameOverworldId = 0x3C;
    public const int DlcOverworldId = 0x3D;
    public const int GridSize = 256;
    
    public static Vector3 ToAbsolute(Vector3 mapCoords, uint blockId)
    {
        byte area = (byte)((blockId >> 24) & 0xFF);

        if (IsOverworld(area))
        {
            byte gridX = (byte)((blockId >> 16) & 0xFF);
            byte gridZ = (byte)((blockId >> 8) & 0xFF);

            return new Vector3(
                mapCoords.X + GridSize * gridX,
                mapCoords.Y,
                mapCoords.Z + GridSize * gridZ
            );
        }

        return mapCoords;
    }

    public static bool IsOverworld(byte area) => area == BaseGameOverworldId || area == DlcOverworldId;
}