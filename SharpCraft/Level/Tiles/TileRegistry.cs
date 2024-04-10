using SharpCraft.Level.Tiles.Types;

namespace SharpCraft.Level.Tiles;

public static class TileRegistry
{
    public static readonly Tile?[] Tiles = new Tile?[255];

    public static readonly Tile Stone = new(1, 6);
    public static readonly Tile Grass = new GrassTile(2);
    public static readonly Tile Dirt = new(3, 2);
    public static readonly Tile Planks = new(4, 4);
    public static readonly Tile Sand = new(5, 5);
    public static readonly Tile Rock = new(6, 1);
    public static readonly Tile Wood = new WoodTile(7);
    public static readonly Tile Leaves = new LeavesTile(8, 9);
    public static readonly Tile WoodenPole = new PoleTile(9, 10, 4);
    public static readonly Tile StonePole = new PoleTile(10, 11, 1);

    public static readonly Tile Glass = new(11, 12,
        TileConfig.Default with { Layer = TileLayer.Translucent, IsLightBlocker = false });

    public static int GetNonNullTileCount()
    {
        return Tiles.Count(t => t != null);
    }
}