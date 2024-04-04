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
                    if (y == groundLevel)
                    {
                        level.SetTileUnchecked(x, y, z, TileRegistry.Grass.Id, false);
                    }
                    else if (y >= yLevel && y < groundLevel)
                    {
                        level.SetTileUnchecked(x, y, z, TileRegistry.Dirt.Id, false);
                    }
                    else if (y < yLevel)
                    {
                        level.SetTileUnchecked(x, y, z, TileRegistry.Rock.Id, false);
                    }
                }
            }
        }
    }
}