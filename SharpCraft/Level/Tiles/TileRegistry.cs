using SharpCraft.Level.Tiles.Types;

namespace SharpCraft.Level.Tiles;

public static class TileRegistry
{
    public static readonly Tile[] Tiles = new Tile[255];
    
    public static readonly Tile Rock = new(1, 1);
    public static readonly Tile Grass = new GrassTile(2);
    public static readonly Tile Dirt = new(3, 2);

    public static int GetNonNullTileCount()
    {
        return Tiles.Count(t => t != null);
    }
}