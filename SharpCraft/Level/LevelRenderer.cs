using System.Numerics;
using SharpCraft.Level.Blocks;
using SharpCraft.Rendering;

namespace SharpCraft.Level;

public class LevelRenderer : IDisposable
{
    private readonly BlockPosition _min;
    private readonly BlockPosition _max;

    public readonly Level Level;

    private Stack<Chunk> _rebuildStack = new();
    private Mutex _rebuildStackMutex = new();

    public LevelRenderer(Level level)
    {
        Level = level;
        level.OnEverythingChanged += () =>
        {
            SetDirtyArea(0, 0, 0, level.Width, level.Height, level.Length);
        };
        level.OnLightLevelChanged += (x, z, minY, maxY) =>
        {
            SetDirtyArea(x - 1, minY, z - 1, x + 1, maxY, z + 1);
        };
        level.OnBlockChanged += (x, y, z) =>
        {
            SetDirtyArea(x - 1, y - 1, z - 1, x + 1, y + 1, z + 1);
        };

        _min = new BlockPosition(0, 0, 0);
        _max = new BlockPosition(level.Width, level.Height, level.Length);

        var rebuildThread = new Thread(RebuildLoop);
        rebuildThread.Start();
    }

    private void RebuildLoop()
    {
        while (!WindowShouldClose())
        {
            for (var x = 0; x < Level.ChunksX; x++)
            {
                for (var y = 0; y < Level.ChunksY; y++)
                {
                    for (var z = 0; z < Level.ChunksZ; z++)
                    {
                        var chunk = Level.Chunks[x + Level.ChunksX * (y + Level.ChunksY * z)];
                        if (!chunk.TryBeginRebuild()) continue;
                        
                        _rebuildStackMutex.WaitOne();
                        
                        _rebuildStack.Push(chunk);
                        
                        _rebuildStackMutex.ReleaseMutex();
                    }
                }
            }
        }
    }

    public void Draw(BlockLayer layer)
    {
        _rebuildStackMutex.WaitOne();

        while (_rebuildStack.TryPop(out var chunk))
        {
            chunk.EndRebuild();
        }
        
        _rebuildStackMutex.ReleaseMutex();
        
        var frustum = Frustum.Instance;

        for (var x = 0; x < Level.ChunksX; x++)
        {
            for (var y = 0; y < Level.ChunksY; y++)
            {
                for (var z = 0; z < Level.ChunksZ; z++)
                {
                    var chunk = Level.Chunks[y * Chunk.SizeSq + z * Chunk.Size + x];
                    if (!frustum.IsCubeVisible(chunk.BBox)) continue;
                    chunk.Draw(layer);
                }
            }
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
                    Level.Chunks[y * Chunk.SizeSq + z * Chunk.Size + x].IsDirty = true;
                }
            }
        }
    }

    public void Dispose()
    {
        for (var x = 0; x < Level.ChunksX; x++)
        {
            for (var y = 0; y < Level.ChunksY; y++)
            {
                for (var z = 0; z < Level.ChunksZ; z++)
                {
                    Level.Chunks[y * Chunk.SizeSq + z * Chunk.Size + x].Dispose();
                }
            }
        }
    }
}