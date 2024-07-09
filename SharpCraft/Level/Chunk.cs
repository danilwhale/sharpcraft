using System.Numerics;
using SharpCraft.Rendering;
using SharpCraft.Tiles;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public sealed class Chunk : IDisposable
{
    public const int Size = 16;

    // cache amount of ChunkLayer values
    private static readonly int Layers = Enum.GetValues<RenderLayer>().Length;

    public static int Updates;
    public static int Rebuilds;

    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public readonly Vector3 Center;

    public readonly int MaxX;
    public readonly int MaxY;
    public readonly int MaxZ;

    public readonly BoundingBox BBox;
    private bool _isDirty = true;

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            _isDirty = value;
            DirtyTime = GetTime();
        }
    }

    public double DirtyTime;

    private readonly Level _level;
    private readonly ChunkBuilder[] _layers = new ChunkBuilder[Layers];

    public Chunk(Level level, int x, int y, int z)
    {
        _level = level;
        X = x << 4;
        Y = y << 4;
        Z = z << 4;
        MaxX = (x + 1) << 4;
        MaxY = (y + 1) << 4;
        MaxZ = (z + 1) << 4;
        Center = new Vector3(X + MaxX, Y + MaxY, Z + MaxZ) * 0.5f;
        BBox = new BoundingBox(new Vector3(X, Y, Z), new Vector3(MaxX, MaxY, MaxZ));

        for (var i = 0; i < _layers.Length; i++) _layers[i] = new ChunkBuilder();
    }

    private void Rebuild(RenderLayer layer)
    {
        Updates++;
        Rebuilds++;

        var builder = _layers[(byte)layer];

        builder.Begin();

        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    var tile = _level.GetTile(x, y, z);
                    TileRegistry.Registry[tile]?.Build(builder, _level, x, y, z, layer);
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