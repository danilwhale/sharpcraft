using System.Numerics;
using SharpCraft.Rendering;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public class Chunk : IDisposable
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
        X = x * Size;
        Y = y * Size;
        Z = z * Size;
        MaxX = (x + 1) * Size;
        MaxY = (y + 1) * Size;
        MaxZ = (z + 1) * Size;
        BBox = new BoundingBox(new Vector3(X, Y, Z), new Vector3(MaxX, MaxY, MaxZ));
    }

    private void Rebuild()
    {
        if (Rebuilds >= 2) return;
        
        Updates++;
        Rebuilds++;
        
        _builder.Begin(GetFaceCount() * 2);
        
        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    if (!_level.IsTile(new TilePosition(x, y, z))) continue;
                    
                    if (y == _level.Height * 2 / 3) Tile.Grass.Build(_builder, _level, new TilePosition(x, y, z));
                    else Tile.Rock.Build(_builder, _level, new TilePosition(x, y, z));
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
        
        _builder.Draw(Resources.DefaultTerrainMaterial);
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
                    if (!_level.IsTile(new TilePosition(x, y, z))) continue;

                    if (y == _level.Height * 2 / 3) count += Tile.Grass.GetFaceCount(_level, new TilePosition(x, y, z));
                    else count += Tile.Rock.GetFaceCount(_level, new TilePosition(x, y, z));
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