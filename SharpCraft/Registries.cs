using SharpCraft.Tiles;
using SharpCraft.Tiles.Types;

namespace SharpCraft;

public static class Registries
{
    public static readonly TileRegistry Tiles = new();
    public static readonly TileGroupRegistry TileGroups = new();
    
    public sealed class TileRegistry
    {
        public readonly Tile?[] Registry = new Tile?[byte.MaxValue];

        public readonly Tile
            Rock = new(1, 1),
            Grass = new GrassTile(2),
            Dirt = new(3, 2),
            Stone = new(4, 16),
            Wood = new(5, 4),
            Bush = new BushTile(6);
    }

    public sealed class TileGroupRegistry
    {
        public readonly TileGroup Growable = new([Tiles.Grass, Tiles.Dirt]);
    }
}