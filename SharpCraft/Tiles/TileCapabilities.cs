using SharpCraft.World.Rendering;

namespace SharpCraft.Tiles;

public readonly struct TileCapabilities
{
    public static TileCapabilities Default => new()
    {
        IsSolid = true,
        CanBlockLight = true,
        Layer = RenderLayer.Solid
    };
    
    public bool IsSolid { get; init; }
    public bool CanBlockLight { get; init; }
    public RenderLayer Layer { get; init; }
}   