using SharpCraft.Level;

namespace SharpCraft.Tiles;

public readonly struct TileCapabilities
{
    public static TileCapabilities Default => new()
    {
        IsSolid = true,
        CanBlockLight = true,
        Layer = ChunkLayer.Solid
    };
    
    public bool IsSolid { get; init; }
    public bool CanBlockLight { get; init; }
    public ChunkLayer Layer { get; init; }
}   