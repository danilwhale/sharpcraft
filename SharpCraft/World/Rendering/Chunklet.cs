using System.Numerics;
using SharpCraft.Utilities;

namespace SharpCraft.World.Rendering;

public sealed class Chunklet : IDisposable
{
    public const int Size = 16;

    // cache amount of RenderLayer values
    private static readonly int Layers = Enum.GetValues<RenderLayer>().Length;

    public static int Updates;

    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public readonly int MaxX;
    public readonly int MaxY;
    public readonly int MaxZ;

    public readonly BoundingBox BBox;
    
    public bool IsDirty = true;

    private readonly World _world;
    private readonly ChunkBuilder[] _layers = new ChunkBuilder[Layers];

    public Chunklet(World world, int x, int y, int z)
    {
        _world = world;
        X = x << 4;
        Y = y << 4;
        Z = z << 4;
        MaxX = (x + 1) << 4;
        MaxY = (y + 1) << 4;
        MaxZ = (z + 1) << 4;
        BBox = new BoundingBox(new Vector3(X, Y, Z), new Vector3(MaxX, MaxY, MaxZ));

        for (var i = 0; i < _layers.Length; i++) _layers[i] = new ChunkBuilder();
    }

    private void Rebuild(RenderLayer layer)
    {
        var builder = _layers[(byte)layer];

        builder.Begin(DrawMode.Quads);

        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    var tile = _world.GetTile(x, y, z);
                    Registries.Tiles.Registry[tile]?.Build(builder, _world, x, y, z, layer);
                }
            }
        }

        IsDirty = false;
    }

    public void FinishRebuild()
    {
        for (var i = 0; i < Layers; i++)
        {
            _layers[i].End();
        }
        
        Updates++;
    }

    public void Rebuild()
    {
        for (var i = 0; i < Layers; i++)
        {
            Rebuild((RenderLayer)i);
        }
    }

    public void Draw(RenderLayer layer)
    {
        _layers[(byte)layer].Draw(Assets.GetTextureMaterial("terrain.png"));
    }

    public void Dispose()
    {
        foreach (var builder in _layers)
        {
            builder.Dispose();
        }
    }
}