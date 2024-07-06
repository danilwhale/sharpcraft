using System.Numerics;
using SharpCraft.Rendering;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public sealed class Chunk : IDisposable
{
    public const int Size = 16;

    public static int Updates;
    public static int Rebuilds;

    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    
    public readonly int MaxX;
    public readonly int MaxY;
    public readonly int MaxZ;

    public readonly BoundingBox BBox;
    public bool IsDirty = true;

    private readonly Level _level;
    private readonly MeshBuilder _builder = new();

    public Chunk(Level level, int x, int y, int z)
    {
        _level = level;
        X = x << 4;
        Y = y << 4;
        Z = z << 4;
        MaxX = (x + 1) << 4;
        MaxY = (y + 1) << 4;
        MaxZ = (z + 1) << 4;
        BBox = new BoundingBox(new Vector3(X, Y, Z), new Vector3(MaxX, MaxY, MaxZ));
    }

    private void Rebuild()
    {
        if (Rebuilds >= 2) return;
        
        Updates++;
        Rebuilds++;

        var faces = GetFaceCount();
        _builder.Begin(faces * 4, faces * 6);
        
        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    if (!_level.IsTile(x, y, z)) continue;
                    
                    if (y == _level.Height * 2 / 3) Tile.Grass.Build(_builder, _level, x, y, z);
                    else Tile.Rock.Build(_builder, _level, x, y, z);
                }
            }
        }
        
        _builder.End();
        
        IsDirty = false;
    }

    public void Draw()
    {
        if (IsDirty)
        {
            Rebuild();
        }
        
        _builder.Draw(Assets.GetTextureMaterial("terrain.png"));
    }

    private int GetFaceCount()
    {
        var count = 0;

        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    if (!_level.IsTile(x, y, z)) continue;

                    if (y == _level.Height * 2 / 3) count += Tile.Grass.GetFaceCount(_level, x, y, z);
                    else count += Tile.Rock.GetFaceCount(_level, x, y, z);
                }
            }
        }

        return count;
    }

    public void Dispose()
    {
        _builder.Dispose();
    }
}