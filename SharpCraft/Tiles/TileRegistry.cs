namespace SharpCraft.Tiles;

public static class TileRegistry
{
    public static readonly Tile?[] Registry = new Tile?[byte.MaxValue];

    public static readonly Tile Rock = new(1, 1);
    public static readonly Tile Grass = new(2, 0);
    public static readonly Tile Dirt = new(3, 2);
    public static readonly Tile Stone = new(4, 16);
    public static readonly Tile Wood = new(5, 4);
    public static readonly Tile Bush = new(6, 15);
}