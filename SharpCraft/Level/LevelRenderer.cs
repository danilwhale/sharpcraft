using System.Numerics;
using SharpCraft.Rendering;

namespace SharpCraft.Level;

public class LevelRenderer : IDisposable
{
    public readonly int ChunksX;
    public readonly int ChunksY;
    public readonly int ChunksZ;

    private readonly Chunk[][][] _chunks;

    public readonly Level Level;

    public LevelRenderer(Level level)
    {
        Level = level;
        level.OnEverythingChanged += () =>
        {
            SetDirtyArea(new TilePosition(0, 0, 0), new TilePosition(level.Width, level.Height, level.Length));
        };
        level.OnLightLevelChanged += (x, z, minY, maxY) =>
        {
            SetDirtyArea(new TilePosition(x - 1, minY, z - 1), new TilePosition(x + 1, maxY, z + 1));
        };
        level.OnTileChanged += (x, y, z) =>
        {
            var position = new TilePosition(x, y, z);
            SetDirtyArea(position - TilePosition.One, position + TilePosition.One);
        };

        ChunksX = level.Width / Chunk.Size;
        ChunksY = level.Height / Chunk.Size;
        ChunksZ = level.Length / Chunk.Size;

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

    public void SetDirtyArea(TilePosition min, TilePosition max)
    {
        min = TilePosition.Clamp(
            TilePosition.ToChunkPosition(min),
            new TilePosition(0, 0, 0),
            new TilePosition(ChunksX, ChunksY, ChunksZ)
        );
        
        max = TilePosition.Clamp(
            TilePosition.ToChunkPosition(max),
            new TilePosition(0, 0, 0),
            new TilePosition(ChunksX, ChunksY, ChunksZ)
        );

        for (var x = min.X; x <= max.X; x++)
        {
            for (var y = min.Y; y <= max.Y; y++)
            {
                for (var z = min.Z; z <= max.Z; z++)
                {
                    _chunks[x][y][z].IsDirty = true;
                }
            }
        }
    }

    public void DrawHit(RayCollision hit)
    {
        var alpha = MathF.Sin((float)GetTime() * 1000.0f / 100.0f) * 0.2f + 0.4f;
        
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