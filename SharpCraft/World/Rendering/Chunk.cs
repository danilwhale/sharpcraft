using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Rendering;

namespace SharpCraft.World.Rendering;

public sealed class Chunk : IDisposable
{
    private readonly Vector2 _center;
    
    private readonly BoundingBox _bbox;
    public readonly Chunklet[] Chunklets;

    private readonly WorldRenderer _worldRenderer;

    public Chunk(WorldRenderer worldRenderer, int x, int z)
    {
        _worldRenderer = worldRenderer;
        
        var height = worldRenderer.World.Height >> 4;
        
        _center = new Vector2((x << 4) + Chunklet.Size * 0.5f, (z << 4) + Chunklet.Size * 0.5f);
        
        _bbox = new BoundingBox(
            new Vector3(x << 4, 0.0f, z << 4), 
            new Vector3(x + 1 << 4, worldRenderer.World.Height, z + 1 << 4)
        );
        
        Chunklets = new Chunklet[height];

        for (var y = 0; y < height; y++)
        {
            Chunklets[y] = new Chunklet(worldRenderer.World, x, y, z);
        }
    }

    public float DistanceSquared(float x, float z)
    {
        var dx = x - _center.X;
        var dz = z - _center.Y;
        return dx * dx + dz * dz;
    }

    public void Draw(RenderLayer layer, PlayerEntity playerEntity, Frustum frustum)
    {
        if (frustum.IsBoxOutsideHorizontalPlane(_bbox)) return;

        var drawDistanceBlocks = _worldRenderer.DrawDistanceBlocks;
        var distanceSq = drawDistanceBlocks * drawDistanceBlocks;
        if (_worldRenderer.DrawDistance != 0 &&
            DistanceSquared(playerEntity.Position.X, playerEntity.Position.Z) >= distanceSq)
        {
            return;
        }
        
        foreach (var chunklet in Chunklets)
        {
            if (frustum.IsBoxOutsideVerticalPlane(chunklet.BBox)) continue;
            if (_worldRenderer.DrawDistance != 0 &&
                chunklet.DistanceSquared(playerEntity.Position) >= distanceSq)
            {
                continue;
            }
            
            chunklet.Draw(layer);
        }
    }

    public void Dispose()
    {
        foreach (var chunklet in Chunklets)
        {
            chunklet.Dispose();
        }
    }
}