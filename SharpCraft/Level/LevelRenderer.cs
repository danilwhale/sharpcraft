using System.Collections.Concurrent;
using System.Numerics;
using SharpCraft.Level.Blocks;
using SharpCraft.Rendering;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public class LevelRenderer : IDisposable
{
    public readonly Level Level;
    private readonly BlockingCollection<Chunk> _rebuildStack = new();

    public LevelRenderer(Level level)
    {
        Level = level;
        level.OnAreaUpdate += (min, max) => { SetDirtyArea(min.X, min.Y, min.Z, max.X, max.Y, max.Z); };

        var rebuildThread = new Thread(RebuildLoop);
        rebuildThread.Start();
    }

    private void RebuildLoop()
    {
        while (!WindowShouldClose())
        {
            foreach (var chunk in Level.Chunks)
            {
                if (!chunk.TryBeginRebuild()) continue;
                _rebuildStack.Add(chunk);
            }
            
            WaitTime(GetFrameTime());
        }
    }

    public void Draw(BlockLayer layer)
    {
        while (_rebuildStack.TryTake(out var chunk))
        {
            chunk.EndRebuild();
        }

        var frustum = Frustum.Instance;

        foreach (var chunk in Level.Chunks)
        {
            if (!frustum.IsCubeVisible(chunk.BBox)) continue;
            chunk.Draw(layer);
        }
    }

    public void SetDirtyArea(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        minX = Math.Clamp(minX >> 4, 0, Level.ChunksX - 1);
        minY = Math.Clamp(minY >> 4, 0, Level.ChunksY - 1);
        minZ = Math.Clamp(minZ >> 4, 0, Level.ChunksZ - 1);

        maxX = Math.Clamp(maxX >> 4, 0, Level.ChunksX - 1);
        maxY = Math.Clamp(maxY >> 4, 0, Level.ChunksY - 1);
        maxZ = Math.Clamp(maxZ >> 4, 0, Level.ChunksZ - 1);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    Level.Chunks.GetUnsafeRef(y * Chunk.SizeSq + z * Chunk.Size + x).IsDirty = true;
                }
            }
        }
    }

    public void Dispose()
    {
        foreach (var chunk in Level.Chunks)
        {
            chunk.Dispose();
        }
    }
}