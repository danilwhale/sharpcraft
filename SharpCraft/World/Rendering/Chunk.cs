using System.Numerics;
using SharpCraft.Rendering;

namespace SharpCraft.World.Rendering;

public sealed class Chunk : IDisposable
{
    private readonly BoundingBox _bbox;
    public readonly Chunklet[] Chunklets;

    public Chunk(World world, int x, int z)
    {
        var height = world.Height >> 4;
        
        _bbox = new BoundingBox(
            new Vector3(x * Chunklet.Size, 0.0f, z * Chunklet.Size), 
            new Vector3((x + 1) * Chunklet.Size, world.Height, (z + 1) * Chunklet.Size)
        );
        
        Chunklets = new Chunklet[height];

        for (var y = 0; y < height; y++)
        {
            Chunklets[y] = new Chunklet(world, x, y, z);
        }
    }

    public void Draw(RenderLayer layer, Frustum frustum)
    {
        if (frustum.IsBoxOutsideHorizontalPlane(_bbox)) return;
        
        foreach (var chunklet in Chunklets)
        {
            if (frustum.IsBoxOutsideVerticalPlane(chunklet.BBox)) continue;
            
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