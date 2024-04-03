namespace SharpCraft.Level.Tiles;

public static class TileRegistry
{
    public static readonly Tile[] Tiles = new Tile[255];
    
    public static readonly Tile Rock = new(1, 1);
    public static readonly Tile Grass = new(2, 0);
}