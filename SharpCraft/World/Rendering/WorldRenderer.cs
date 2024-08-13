using System.Collections.Concurrent;
using SharpCraft.Rendering;
using SharpCraft.Tiles;
using SharpCraft.Utilities;

namespace SharpCraft.World.Rendering;

public sealed class WorldRenderer : IDisposable
{
    private const int MaxUpdatesPerFrame = 8;

    private readonly int _chunksX;
    private readonly int _chunksY;
    private readonly int _chunksZ;

    private readonly Chunk[] _chunks;

    private readonly ConcurrentStack<Chunklet> _finishStack = new();

    public WorldRenderer(World world)
    {
        world.OnAreaUpdate += SetDirtyArea;

        _chunksX = world.Width >> 4;
        _chunksY = world.Height >> 4;
        _chunksZ = world.Depth >> 4;

        _chunks = new Chunk[_chunksX * _chunksZ];
        for (var x = 0; x < _chunksX; x++)
        {
            for (var z = 0; z < _chunksZ; z++)
            {
                _chunks[z * _chunksX + x] = new Chunk(world, x, z);
            }
        }
    }

    public void UpdateDirtyChunks()
    {
        Parallel.Invoke(RebuildDirtyChunks);

        var uploads = 0;
        while (uploads++ < MaxUpdatesPerFrame && _finishStack.TryPop(out var chunklet))
        {
            chunklet.FinishRebuild();
        }
    }

    private void RebuildDirtyChunks()
    {
        var updates = 0;

        foreach (var chunk in _chunks)
        {
            foreach (var chunklet in chunk.Chunklets)
            {
                if (updates >= MaxUpdatesPerFrame) return;
                    
                if (!chunklet.IsDirty) continue;
                    
                chunklet.Rebuild();
                _finishStack.Push(chunklet);
                    
                updates++;
            }
        }
    }

    public void Draw(RenderLayer layer)
    {
        var frustum = Frustum.Instance;

        foreach (var chunk in _chunks)
        {
            chunk.Draw(layer, frustum);
        }
    }

    public void SetDirtyArea(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        minX = Math.Clamp(minX >> 4, 0, _chunksX - 1);
        minY = Math.Clamp(minY >> 4, 0, _chunksY - 1);
        minZ = Math.Clamp(minZ >> 4, 0, _chunksZ - 1);

        maxX = Math.Clamp(maxX >> 4, 0, _chunksX - 1);
        maxY = Math.Clamp(maxY >> 4, 0, _chunksY - 1);
        maxZ = Math.Clamp(maxZ >> 4, 0, _chunksZ - 1);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    _chunks[z * _chunksX + x].Chunklets[y].IsDirty = true;
                }
            }
        }
    }

    public void DrawHit(RayCollision hit, EditMode mode, int tileId)
    {
        if (!hit.Hit) return;
        
        // reset to default texture, in case one of previous function didn't reset texture
        Rlgl.SetTexture(Rlgl.GetTextureIdDefault());
        Rlgl.Begin(DrawMode.Quads);

        var time = (float)GetTime() * 1000.0f;

        TilePosition position;
        float alpha;
        
        switch (mode)
        {
            case EditMode.Remove:
                position = hit.Point - hit.Normal / 2.0f;
                
                alpha = (MathF.Sin(time / 100.0f) * 0.2f + 0.4f) * 0.5f;
                Rlgl.Color4f(1.0f, 1.0f, 1.0f, alpha);
                
                TileRender.RenderFace(position, Face.Top);
                TileRender.RenderFace(position, Face.Bottom);
                TileRender.RenderFace(position, Face.Right);
                TileRender.RenderFace(position, Face.Left);
                TileRender.RenderFace(position, Face.Front);
                TileRender.RenderFace(position, Face.Back);
                
                break;
            
            case EditMode.Place:
                position = hit.Point + hit.Normal / 2.0f;
                
                alpha = MathF.Sin(time / 200.0f) * 0.2f + 0.5f;
                Rlgl.Color4f(1.0f, 1.0f, 1.0f, alpha);
                
                Rlgl.SetTexture(Assets.GetTexture("terrain.png").Id);
                RlglVertexBuilder.Instance.EnableColor = false;
                
                var tile = Registries.Tiles.Registry[tileId];
                tile?.Build(RlglVertexBuilder.Instance, null, position.X, position.Y, position.Z, RenderLayer.Lit);

                RlglVertexBuilder.Instance.EnableColor = true;
                Rlgl.SetTexture(Rlgl.GetTextureIdDefault());
                
                break;
        }

        Rlgl.End();
    }

    public void Dispose()
    {
        foreach (var chunk in _chunks)
        {
            chunk.Dispose();
        }
    }
}