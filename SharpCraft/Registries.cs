using SharpCraft.Tiles;
using SharpCraft.Tiles.Types;

namespace SharpCraft;

public static class Registries
{
    public static class Tiles
    {
        public static readonly Tile?[] Registry = new Tile?[byte.MaxValue];

        public static readonly Tile
            Rock = new(1, 1),
            Grass = new GrassTile(2),
            Dirt = new(3, 2),
            Stone = new(4, 16),
            Wood = new(5, 4),
            Bush = new BushTile(6),
            Bedrock = new Tile(7, 17);
    }

    public static class TileGroups
    {
        public static readonly TileGroup Growable = new([Tiles.Grass, Tiles.Dirt]);
    }
}