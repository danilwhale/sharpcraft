using SharpCraft.Level.Generation.Structures;
using SharpCraft.Level.Tiles;

namespace SharpCraft.Level.Generation;

public static class LevelGeneration
{
    public static void Generate(Level level, int seed)
    {
        var random = new Random(seed);

        var groundLevel = level.Height * 2 / 3;

        for (var x = 0; x < level.Width; x++)
        {
            for (var z = 0; z < level.Length; z++)
            {
                var yLevel = level.Height * 2 / 3 - random.Next(2, 5);

                for (var y = 0; y < level.Height; y++)
                {
                    byte id = 0;
                    
                    if (y == groundLevel)
                    {
                        id = TileRegistry.Grass.Id;
                    }
                    else if (y == groundLevel + 1 && random.NextDouble() < 0.01)
                    {
                        id = TileRegistry.Rock.Id;
                    }
                    else if (y >= yLevel && y < groundLevel)
                    {
                        id = TileRegistry.Dirt.Id;
                    }
                    else if (y < yLevel && random.NextDouble() > y / (float)level.Height)
                    {
                        id = TileRegistry.Rock.Id;
                    }
                    else if (y < yLevel)
                    {
                        id = TileRegistry.Stone.Id;
                    }
                    
                    if (id == 0) continue;
                    
                    level.SetTileUnchecked(x, y, z, id, false);
                }
            }
        }

        var tree = new TreeStructure();

        for (var i = 1; i < random.Next(60, 80); i++)
        {
            tree.Place(level, random.Next(0, level.Width), groundLevel, random.Next(0, level.Length));
        }
    }
}