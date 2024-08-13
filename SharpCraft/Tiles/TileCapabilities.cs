using System.Numerics;

namespace SharpCraft.Tiles;

public readonly struct TileCapabilities
{
    public static TileCapabilities Default => new()
    {
        IsSolid = true,
        CanBlockLight = true,
        CollisionBox = new BoundingBox(Vector3.Zero, Vector3.One),
        SelectionBox = new BoundingBox(Vector3.Zero, Vector3.One)
    };
    
    public bool IsSolid { get; init; }
    public bool CanBlockLight { get; init; }
    public BoundingBox? CollisionBox { get; init; }
    public BoundingBox SelectionBox { get; init; }
}