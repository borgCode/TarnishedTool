// 

namespace TarnishedTool.Models;

public abstract record CustomWarpChange;

public record WarpAdded(BlockWarp Warp) : CustomWarpChange
{
    public BlockWarp Warp { get; } = Warp;
}

public record WarpDeleted(string Category, BlockWarp Warp) : CustomWarpChange
{
    public string Category { get; } = Category;
    public BlockWarp Warp { get; } = Warp;
}


public record CategoryDeleted(string Category) : CustomWarpChange
{
    public string Category { get; } = Category;
}