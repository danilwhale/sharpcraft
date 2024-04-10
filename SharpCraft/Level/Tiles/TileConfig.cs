namespace SharpCraft.Level.Tiles;

public record TileConfig(bool IsSolid = true, bool IsLightBlocker = true, TileLayer Layer = TileLayer.Solid)
{
    public static readonly TileConfig Default = new();
}