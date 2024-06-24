using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using Serilog;
using SharpCraft.Level.Blocks;
using SharpCraft.Level.Generation;
using SharpCraft.Physics;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public class Level
{
    public delegate void OnAreaUpdateEvent(BlockPosition min, BlockPosition max);

    private const float LightValue = 1.0f;
    private const float DarkValue = 0.4f;
    
    private const int Format = 2;

    public readonly int Width;
    public readonly int Height;
    public readonly int Length;

    public readonly int ChunksX;
    public readonly int ChunksY;
    public readonly int ChunksZ;

    public readonly Chunk[] Chunks;
    public readonly LevelColumn<byte>[] LightRegions;
    public readonly LevelColumn<byte>[] HeightMaps;

    public event OnAreaUpdateEvent? OnAreaUpdate;

    public Level(int width, int height, int length)
    {
        Width = width;
        Height = height;
        Length = length;

        ChunksX = width >> 4;
        ChunksY = height >> 4;
        ChunksZ = length >> 4;

        Chunks = new Chunk[ChunksX * ChunksY * ChunksZ];
        LightRegions = new LevelColumn<byte>[ChunksX * ChunksZ];
        HeightMaps = new LevelColumn<byte>[ChunksX * ChunksZ];

        for (var x = 0; x < ChunksX; x++)
        {
            for (var y = 0; y < ChunksY; y++)
            {
                for (var z = 0; z < ChunksZ; z++)
                {
                    Chunks.Get3DUnsafeRef(x, y, z, Chunk.SizeSq, Chunk.Size) = new Chunk(this, x, y, z);
                }
            }
        }

        for (var x = 0; x < ChunksX; x++)
        {
            for (var z = 0; z < ChunksZ; z++)
            {
                LightRegions.Get2DUnsafeRef(x, z, ChunksX) = new LevelColumn<byte>();
                HeightMaps.Get2DUnsafeRef(x, z, ChunksX) = new LevelColumn<byte>();
            }
        }

        if (!TryLoad())
        {
            LevelGeneration.Generate(this, Random.Shared.Next());
        }

        UpdateLightLevels(0, 0, width, length, false);
    }

    private bool TryLoad(string path = "level.dat")
    {
        if (!File.Exists(path)) return false;
        
        try
        {
            using var fileStream = File.OpenRead(path);
            using var stream = new GZipStream(fileStream, CompressionMode.Decompress);

            if (stream.ReadByte() != Format)
            {
                Log.Warning("Unknown level format detected! Big chance that this is old level format, generating new level");
                return false;
            }

            foreach (var chunk in Chunks)
            {
                chunk.Read(stream);
            }
        
            OnAreaUpdate?.Invoke(new BlockPosition(0, 0, 0), new BlockPosition(Width, Height, Length));
            
            Log.Information("Loaded level from {0}!", path);
        
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return false;
    }

    public void Save(string path = "level.dat")
    {
        try
        {
            using var fileStream = File.OpenWrite(path);
            using var stream = new GZipStream(fileStream, CompressionMode.Compress);
            
            stream.WriteByte(Format);
            
            foreach (var chunk in Chunks)
            {
                chunk.Write(stream);
            }
            
            Log.Information("Saved level to {0}!", path);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    public void UpdateLightLevels(int x, int z, int width, int length, bool markDirty = true)
    {
        for (var i = x; i < x + width; i++)
        {
            if (i < 0 || i >= Width) continue;
            
            for (var j = z; j < z + length; j++)
            {
                if (j < 0 || j >= Length) continue;
                
                var region = LightRegions.Get2DUnsafeRef(i >> 4, j >> 4, ChunksX);
                var y = Height;

                for (; y > 0 && !IsLightBlocker(i, y, j); y--)
                {
                }

                var originalY = region[i % Chunk.Size, j % Chunk.Size];
                var minY = Math.Min(y, originalY);
                var maxY = Math.Max(y, originalY);

                if (markDirty)
                {
                    OnAreaUpdate?.Invoke(new BlockPosition(i, minY, j), new BlockPosition(i + 1, maxY, j + 1));
                }
                
                region[i % Chunk.Size, j % Chunk.Size] = (byte)y;
            }
        }
    }

    public bool IsBlock(int x, int y, int z)
    {
        return GetBlock(x, y, z) > 0;
    }

    public bool IsBlock(BlockPosition position) => IsBlock(position.X, position.Y, position.Z);

    public bool IsSolidBlock(int x, int y, int z)
    {
        var id = GetBlock(x, y, z);
        var block = BlockRegistry.Blocks.GetUnsafeRef(id);

        return block?.Config.IsSolid ?? false;
    }

    public bool IsSolidBlock(BlockPosition position) => IsSolidBlock(position.X, position.Y, position.Z);

    public bool IsLightBlocker(int x, int y, int z)
    {
        var id = GetBlock(x, y, z);
        var block = BlockRegistry.Blocks.GetUnsafeRef(id);

        return block?.Config.IsLightBlocker ?? false;
    }

    public bool IsLightBlocker(BlockPosition position) => IsLightBlocker(position.X, position.Y, position.Z);

    public List<BoundingBox> GetBoxes(BoundingBox area)
    {
        var boxes = new List<BoundingBox>();

        int minX = (int)area.Min.X, minY = (int)area.Min.Y, minZ = (int)area.Min.Z;
        int maxX = (int)area.Max.X, maxY = (int)area.Max.Y, maxZ = (int)area.Max.Z;

        minX = Math.Clamp(minX, 0, Width);
        minY = Math.Clamp(minY, 0, Height);
        minZ = Math.Clamp(minZ, 0, Length);

        maxX = Math.Clamp(maxX, 0, Width);
        maxY = Math.Clamp(maxY, 0, Height);
        maxZ = Math.Clamp(maxZ, 0, Length);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    var id = GetBlock(x, y, z);
                    var block = BlockRegistry.Blocks.GetUnsafeRef(id);
                    if (block == null) continue;

                    boxes.Add(block.GetCollision(x, y, z));
                }
            }
        }

        return boxes;
    }

    public float GetBrightness(int x, int y, int z)
    {
        if (!IsInRange(x, y, z)) return LightValue;

        var cx = x >> 4;
        var cz = z >> 4;
        return LightRegions.Get2DUnsafeRef(cx, cz, ChunksX)[x - (cx << 4), z - (cz << 4)] > y ? DarkValue : LightValue;
    }

    public float GetBrightness(BlockPosition position) => GetBrightness(position.X, position.Y, position.Z);

    public void SetBlock(int x, int y, int z, byte value, bool updateLighting = true)
    {
        if (!IsInRange(x, y, z)) return;

        SetBlockUnchecked(x, y, z, value, updateLighting);
    }

    public void SetBlock(BlockPosition position, byte value, bool updateLighting = true) =>
        SetBlock(position.X, position.Y, position.Z, value, updateLighting);

    public byte GetBlock(int x, int y, int z) => !IsInRange(x, y, z) ? (byte)0 : GetBlockUnchecked(x, y, z);
    public byte GetBlock(BlockPosition position) => GetBlock(position.X, position.Y, position.Z);

    public void SetBlockUnchecked(int x, int y, int z, byte value, bool updateLighting)
    {
        var cx = x >> 4;
        var cy = y >> 4;
        var cz = z >> 4;

        var chunk = Chunks.Get3DUnsafeRef(cx, cy, cz, Chunk.SizeSq, Chunk.Size);

        chunk[x - (cx << 4), y - (cy << 4), z - (cz << 4)] = value;

        if (updateLighting) UpdateLightLevels(x, z, 1, 1);
        OnAreaUpdate?.Invoke(new BlockPosition(x - 1, y - 1, z - 1), new BlockPosition(x + 1, y + 1, z + 1));
    }

    public void SetBlockUnchecked(BlockPosition position, byte value, bool updateLighting) =>
        SetBlockUnchecked(position.X, position.Y, position.Z, value, updateLighting);

    public byte GetBlockUnchecked(int x, int y, int z)
    {
        var cx = x >> 4;
        var cy = y >> 4;
        var cz = z >> 4;

        var chunk = Chunks.Get3DUnsafeRef(cx, cy, cz, Chunk.SizeSq, Chunk.Size);

        return chunk[x - (cx << 4), y - (cy << 4), z - (cz << 4)];
    }
    public byte GetBlockUnchecked(BlockPosition position) => GetBlockUnchecked(position.X, position.Y, position.Z);

    public byte GetHeightUnchecked(int x, int z)
    {
        var map = HeightMaps.Get2DUnsafeRef(x >> 4, z >> 4, ChunksX);
        return map[x % Chunk.Size, z % Chunk.Size];
    }

    // public RayCollision DoRayCast(Ray ray, float maxDistance)
    // {
    //     var col = new RayCollision();
    //
    //     var t = 0.0f;
    //
    //     var ix = Floor(ray.Position.X) | 0;
    //     var iy = Floor(ray.Position.Y) | 0;
    //     var iz = Floor(ray.Position.Z) | 0;
    //
    //     var stepX = ray.Direction.X > 0 ? 1 : -1;
    //     var stepY = ray.Direction.Y > 0 ? 1 : -1;
    //     var stepZ = ray.Direction.Z > 0 ? 1 : -1;
    //
    //     var txDelta = MathF.Abs(1 / ray.Direction.X);
    //     var tyDelta = MathF.Abs(1 / ray.Direction.Y);
    //     var tzDelta = MathF.Abs(1 / ray.Direction.Z);
    //
    //     var xDist = stepX > 0 ? ix + 1 - ray.Position.X : ray.Position.X - ix;
    //     var yDist = stepY > 0 ? iy + 1 - ray.Position.Y : ray.Position.Y - iy;
    //     var zDist = stepZ > 0 ? iz + 1 - ray.Position.Z : ray.Position.Z - iz;
    //
    //     var txMax = txDelta < float.PositiveInfinity ? txDelta * xDist : float.PositiveInfinity;
    //     var tyMax = tyDelta < float.PositiveInfinity ? tyDelta * yDist : float.PositiveInfinity;
    //     var tzMax = tzDelta < float.PositiveInfinity ? tzDelta * zDist : float.PositiveInfinity;
    //
    //     var steppedIndex = -1;
    //
    //     while (t <= maxDistance)
    //     {
    //         if (IsBlock(ix, iy, iz))
    //         {
    //             col.Point = ray.Position + t * ray.Direction;
    //
    //             switch (steppedIndex)
    //             {
    //                 case 0:
    //                     col.Normal.X = -stepX;
    //                     break;
    //                 case 1:
    //                     col.Normal.Y = -stepY;
    //                     break;
    //                 case 2:
    //                     col.Normal.Z = -stepZ;
    //                     break;
    //             }
    //
    //             col.Hit = true;
    //             col.Distance = t;
    //
    //             return col;
    //         }
    //
    //         if (txMax < tyMax)
    //         {
    //             if (txMax < tzMax)
    //             {
    //                 ix += stepX;
    //                 t = txMax;
    //                 txMax += txDelta;
    //                 steppedIndex = 0;
    //             }
    //             else
    //             {
    //                 iz += stepZ;
    //                 t = tzMax;
    //                 tzMax += tzDelta;
    //                 steppedIndex = 2;
    //             }
    //         }
    //         else
    //         {
    //             if (tyMax < tzMax)
    //             {
    //                 iy += stepY;
    //                 t = tyMax;
    //                 tyMax += tyDelta;
    //                 steppedIndex = 1;
    //             }
    //             else
    //             {
    //                 iz += stepZ;
    //                 t = tzMax;
    //                 tzMax += tzDelta;
    //                 steppedIndex = 2;
    //             }
    //         }
    //     }
    //
    //     col.Point = ray.Position + t * ray.Direction;
    //     col.Hit = false;
    //     col.Distance = t;
    //
    //     return col;
    //
    //     int Floor(float f)
    //     {
    //         return (int)MathF.Floor(f);
    //     }
    // }

    public bool IsInRange(int x, int y, int z) => x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
}