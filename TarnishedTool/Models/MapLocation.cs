// 

using System.Numerics;

namespace TarnishedTool.Models;

public class MapLocation(uint blockId, Vector3 localCoords, Vector3 mapCoords)
{
    public const int GridSize = 256;
    
    public uint BlockId { get; } = blockId;
    public Vector3 LocalCoords { get; } = localCoords;
    public Vector3 MapCoords { get; } = mapCoords;

    public byte Area => (byte)((BlockId >> 24) & 0xFF);
    public byte Block => (byte)((BlockId >> 16) & 0xFF);
    public byte Region => (byte)((BlockId >> 8) & 0xFF);
    
    private byte ExtraByte => (byte)(BlockId & 0xFF);
    
    public bool IsOverworld => Area >= 50 && Area < 89;
    
    public int Layer => IsOverworld ? (ExtraByte >> 4) & 0xF : 0;
    public int AltNo => IsOverworld ? 0 : ExtraByte;
    
    public Vector3 AbsoluteCoords => IsOverworld
        ? new Vector3(
            MapCoords.X + GridSize * Block,
            MapCoords.Y,
            MapCoords.Z + GridSize * Region)
        : MapCoords;
    
    public string FormattedId => IsOverworld
        ? $"{Area}_{Block}_{Region}_{Layer}"
        : $"{Area}_{Block}_{Region}_{AltNo}";
}