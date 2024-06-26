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
        while (Program.IsRunning)
        {
            foreach (var chunk in Level.Chunks)
            {
                if (!chunk.TryBeginRebuild()) continue;
                _rebuildStack.Add(chunk);
            }
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
        
        BeginShaderMode(ChunkShader.Shader);

        DrawGrassPlanes();
        DrawGrassInside();
        DrawDirtInside();
        
        EndShaderMode();
    }

    private void DrawGrassPlanes()
    {
        Rlgl.SetTexture(ResourceManager.GetTexture("Grass.png").Id);
        
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.Color4ub(255, 255, 255, 255);

        var y = Level.Height * 0.5f;
        
        PlaneRenderer.DrawTopPlane(-1000.0f, -1000.0f, Level.Width + 1000.0f, 0, y);
        PlaneRenderer.DrawTopPlane(-1000.0f, Level.Length, Level.Width + 1000.0f, Level.Length + 1000.0f, y);
        PlaneRenderer.DrawTopPlane(-1000.0f, 0, 0, Level.Length, y);
        PlaneRenderer.DrawTopPlane(Level.Width, 0, Level.Width + 1000.0f, Level.Length, y);
        
        Rlgl.End();
        
        Rlgl.SetTexture(0);
    }

    private void DrawGrassInside()
    {
        Rlgl.SetTexture(ResourceManager.GetTexture("GrassSide.png").Id);

        Rlgl.Begin(DrawMode.Quads);
        
        var y = Level.Height * 0.5f;

        Rlgl.Color4f(BlockRender.Darkest, BlockRender.Darkest, BlockRender.Darkest, 1.0f);
        PlaneRenderer.DrawLeftPlane(y - 1.0f, 0.0f, y, Level.Length, Level.Width);
        
        Rlgl.Color4f(BlockRender.Darkest, BlockRender.Darkest, BlockRender.Darkest, 1.0f);
        PlaneRenderer.DrawRightPlane(y - 1.0f, 0.0f, y, Level.Length, 0.0f);
        
        Rlgl.Color4f(BlockRender.Darker, BlockRender.Darker, BlockRender.Darker, 1.0f);
        PlaneRenderer.DrawFrontPlane(0.0f, y - 1.0f, Level.Width, y, 0.0f);
        
        Rlgl.Color4f(BlockRender.Darker, BlockRender.Darker, BlockRender.Darker, 1.0f);
        PlaneRenderer.DrawBackPlane(0.0f, y - 1.0f, Level.Width, y, Level.Length);
        
        Rlgl.End();
        
        Rlgl.SetTexture(0);
    }

    private void DrawDirtInside()
    {
        Rlgl.SetTexture(ResourceManager.GetTexture("Dirt.png").Id);

        Rlgl.Begin(DrawMode.Quads);
        
        var y = Level.Height * 0.5f;

        Rlgl.Color4f(BlockRender.Darkest, BlockRender.Darkest, BlockRender.Darkest, 1.0f);
        PlaneRenderer.DrawLeftPlane(0.0f, 0.0f, y - 1.0f, Level.Length, Level.Width);
        
        Rlgl.Color4f(BlockRender.Darkest, BlockRender.Darkest, BlockRender.Darkest, 1.0f);
        PlaneRenderer.DrawRightPlane(0.0f, 0.0f, y - 1.0f, Level.Length, 0.0f);
        
        Rlgl.Color4f(BlockRender.Darker, BlockRender.Darker, BlockRender.Darker, 1.0f);
        PlaneRenderer.DrawFrontPlane(0.0f, 0.0f, Level.Width, y - 1.0f, 0.0f);
        
        Rlgl.Color4f(BlockRender.Darker, BlockRender.Darker, BlockRender.Darker, 1.0f);
        PlaneRenderer.DrawBackPlane(0.0f, 0.0f, Level.Width, y - 1.0f, Level.Length);
        
        Rlgl.End();
        
        Rlgl.SetTexture(0);
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