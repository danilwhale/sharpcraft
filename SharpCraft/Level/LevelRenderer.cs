using System.Numerics;
using SharpCraft.Level.Tiles;
using SharpCraft.Rendering;

namespace SharpCraft.Level;

public class LevelRenderer : IDisposable
{
    public readonly int ChunksX;
    public readonly int ChunksY;
    public readonly int ChunksZ;

    private readonly TilePosition _min;
    private readonly TilePosition _max;
    
    private readonly Chunk[][][] _chunks;

    public readonly Level Level;

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
        level.OnTileChanged += (x, y, z) =>
        {
            SetDirtyArea(x - 1, y - 1, z - 1, x + 1, y + 1, z + 1);
        };

        ChunksX = level.Width / Chunk.Size;
        ChunksY = level.Height / Chunk.Size;
        ChunksZ = level.Length / Chunk.Size;

        _min = new TilePosition(0, 0, 0);
        _max = new TilePosition(level.Width, level.Height, level.Length);

        _chunks = new Chunk[ChunksX][][];
        for (var x = 0; x < ChunksX; x++)
        {
            _chunks[x] = new Chunk[ChunksY][];
            for (var y = 0; y < ChunksY; y++)
            {
                _chunks[x][y] = new Chunk[ChunksZ];
                for (var z = 0; z < ChunksZ; z++)
                {
                    _chunks[x][y][z] = new Chunk(level, x, y, z);
                }
            }
        }
    }

    public void Draw()
    {
        Chunk.Rebuilds = 0;
        var frustum = Frustum.Instance;

        for (var x = 0; x < ChunksX; x++)
        {
            for (var y = 0; y < ChunksY; y++)
            {
                for (var z = 0; z < ChunksZ; z++)
                {
                    var chunk = _chunks[x][y][z];
                    if (!frustum.IsCubeVisible(chunk.BBox)) continue;
                    chunk.Draw();
                }
            }
        }
    }

    public void SetDirtyArea(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        minX = Math.Clamp(minX / Chunk.Size, 0, ChunksX - 1);
        minY = Math.Clamp(minY / Chunk.Size, 0, ChunksY - 1);
        minZ = Math.Clamp(minZ / Chunk.Size, 0, ChunksZ - 1);

        maxX = Math.Clamp(maxX / Chunk.Size, 0, ChunksX - 1);
        maxY = Math.Clamp(maxY / Chunk.Size, 0, ChunksY - 1);
        maxZ = Math.Clamp(maxZ / Chunk.Size, 0, ChunksZ - 1);
        
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    _chunks[x][y][z].IsDirty = true;
                }
            }
        }
    }

    public void DrawHit(RayCollision hit)
    {
        if (!hit.Hit) return;
        
        var position = (TilePosition)(hit.Point - hit.Normal / 2.0f);

        var id = Level.GetTile(position);
        var tile = TileRegistry.Tiles[id];
        if (tile == null) return;

        Rlgl.DisableDepthTest();

        var collision = tile.GetCollision(position.X, position.Y, position.Z);
        var size = collision.Max - collision.Min;
        DrawCubeWiresV(collision.Min + size / 2, size, Color.Black);
        
        Rlgl.EnableDepthTest();
    }

    public void Dispose()
    {
        for (var x = 0; x < ChunksX; x++)
        {
            for (var y = 0; y < ChunksY; y++)
            {
                for (var z = 0; z < ChunksZ; z++)
                {
                    _chunks[x][y][z].Dispose();
                }
            }
        }
    }
}