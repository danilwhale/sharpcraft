using System.Numerics;
using SharpCraft.Level.Blocks;
using SharpCraft.Level.Generation.Structures;
using SharpCraft.Utilities;

namespace SharpCraft.Level.Generation;

public static class LevelGeneration
{
    public static void Generate(Level level, int seed)
    {
        var random = new Random(seed);

        GenerateHeightMap(level, random, seed);
        GenerateTerrain(level, random, seed);
        DoTreePass(level, random, seed);
    }
    
    private static void SetupNoise(FastNoiseLite noise)
    {
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(7);
        noise.SetFractalLacunarity(1.27f);
        noise.SetFractalGain(0.58f);
        noise.SetFractalWeightedStrength(0.15f);
        
        noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
        noise.SetDomainWarpAmp(26.5f);
    }

    private static float GetNoise(FastNoiseLite noise, float x, float y)
    {
        noise.DomainWarp(ref x, ref y);
        return noise.GetNoise(x, y);
    }

    private static float GetGroundLevel(Level level)
    {
        return level.Height / 2.0f;
    }
    
    private static void GenerateHeightMap(Level level, Random random, int seed)
    {
        var noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Value);
        noise.SetFrequency(0.03f);
        SetupNoise(noise);

        var noise2 = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFrequency(0.06f);
        SetupNoise(noise2);

        var groundLevel = GetGroundLevel(level);

        for (var x = 0; x < level.Width; x++)
        {
            for (var z = 0; z < level.Length; z++)
            {
                var maxNoise = Math.Max(GetNoise(noise, x, z), GetNoise(noise2, x, z) * 1.3f) * 1.5f;

                var y = groundLevel + Math.Abs(maxNoise * 8.0f);

                var map = level.HeightMaps.Get2DUnsafeRef(x >> 4, z >> 4, level.ChunksX);
                map[x % Chunk.Size, z % Chunk.Size] = (byte)y;
            }
        }
    }

    private static void GenerateTerrain(Level level, Random random, int seed)
    {
        for (var x = 0; x < level.Width; x++)
        {
            for (var z = 0; z < level.Length; z++)
            {
                var height = level.GetHeightUnchecked(x, z);
                level.SetBlockUnchecked(x, height, z, BlockRegistry.Grass.Id, false);
                for (var y = height - 1; y > 0; y--)
                {
                    byte blockId;
                    
                    if (y < height - random.Next(2, 4))
                    {
                        blockId = random.NextSingle() > 0.75f ? BlockRegistry.Stone.Id : BlockRegistry.Rock.Id;
                    }
                    else if (y < height)
                    {
                        blockId = BlockRegistry.Dirt.Id;
                    }
                    else
                    {
                        continue;
                    }
                    
                    level.SetBlockUnchecked(x, y, z, blockId, false);
                }
            }
        }
    }

    private static void DoTreePass(Level level, Random random, int seed)
    {
        var tree = new TreeStructure();
        
        for (var i = 0; i < random.Next(40, 60); i++)
        {
            var x = random.Next(level.Width);
            var z = random.Next(level.Length);
            var y = level.GetHeightUnchecked(x, z) + 1;
            tree.Place(level, x, y, z);
        }
    }
}