namespace SharpCraft.Tiles;

public readonly struct TileCapabilities
{
    public static TileCapabilities Default => new()
    {
        IsSolid = true,
        CanBlockLight = true
    };
    
    public bool IsSolid { get; init; }
    public bool CanBlockLight { get; init; }
}   