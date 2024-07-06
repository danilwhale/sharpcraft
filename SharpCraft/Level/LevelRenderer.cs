using System.Numerics;
using SharpCraft.Rendering;

namespace SharpCraft.Level;

public sealed class LevelRenderer : IDisposable
{
    public readonly int ChunksX;
    public readonly int ChunksY;
    public readonly int ChunksZ;

    private readonly TilePosition _min;
    private readonly TilePosition _max;
    
    private readonly Chunk[] _chunks;

    public readonly Level Level;

    public LevelRenderer(Level level)
    {
        Level = level;
        level.OnAreaUpdate += SetDirtyArea;

        ChunksX = level.Width >> 4;
        ChunksY = level.Height >> 4;
        ChunksZ = level.Depth >> 4;

        _min = new TilePosition(0, 0, 0);
        _max = new TilePosition(level.Width, level.Height, level.Depth);

        _chunks = new Chunk[ChunksX * ChunksY * ChunksZ];
        for (var x = 0; x < ChunksX; x++)
        {
            for (var y = 0; y < ChunksY; y++)
            {
                for (var z = 0; z < ChunksZ; z++)
                {
                    _chunks[(y * ChunksZ + z) * ChunksX + x] = new Chunk(level, x, y, z);
                }
            }
        }
    }

    public void Draw()
    {
        Chunk.Rebuilds = 0;
        var frustum = Frustum.Instance;

        foreach (var chunk in _chunks)
        {
            if (!frustum.IsCubeVisible(chunk.BBox)) continue;
            chunk.Draw();
        }
    }

    public void SetDirtyArea(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        minX = Math.Clamp(minX >> 4, 0, ChunksX - 1);
        minY = Math.Clamp(minY >> 4, 0, ChunksY - 1);
        minZ = Math.Clamp(minZ >> 4, 0, ChunksZ - 1);

        maxX = Math.Clamp(maxX >> 4, 0, ChunksX - 1);
        maxY = Math.Clamp(maxY >> 4, 0, ChunksY - 1);
        maxZ = Math.Clamp(maxZ >> 4, 0, ChunksZ - 1);
        
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    _chunks[(y * ChunksZ + z) * ChunksX + x].IsDirty = true;
                }
            }
        }
    }

    public void DrawHit(RayCollision hit)
    {
        var alpha = MathF.Sin((float)GetTime() * 10.0f) * 0.2f + 0.4f;
        
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.Color4f(1.0f, 1.0f, 1.0f, alpha);

        var position = (TilePosition)(hit.Point - hit.Normal / 2.0f);
        
        if (hit.Normal == new Vector3(1.0f, 0.0f, 0.0f)) Tile.Rock.DrawRlGlFace(position, Face.Right);
        else if (hit.Normal == new Vector3(-1.0f, 0.0f, 0.0f)) Tile.Rock.DrawRlGlFace(position, Face.Left);
        else if (hit.Normal == new Vector3(0.0f, 1.0f, 0.0f)) Tile.Rock.DrawRlGlFace(position, Face.Top);
        else if (hit.Normal == new Vector3(0.0f, -1.0f, 0.0f)) Tile.Rock.DrawRlGlFace(position, Face.Bottom);
        else if (hit.Normal == new Vector3(0.0f, 0.0f, 1.0f)) Tile.Rock.DrawRlGlFace(position, Face.Front);
        else if (hit.Normal == new Vector3(0.0f, 0.0f, -1.0f)) Tile.Rock.DrawRlGlFace(position, Face.Back);
        
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