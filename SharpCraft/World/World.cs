using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpCraft.Registries;
using SharpCraft.Tiles;

namespace SharpCraft.World;

public sealed class World
{
    public delegate void OnAreaUpdateEvent(int x0, int y0, int z0, int x1, int y1, int z1);

    public const float LightValue = 1.0f;
    public const float DarkValue = 0.6f;

    private readonly byte[] _data;
    private readonly byte[] _lightLevels;

    public readonly int Width;
    public readonly byte Height;
    public readonly int Depth;

    private readonly Random _random;

    public event OnAreaUpdateEvent? OnAreaUpdate;

    public World(int width, byte height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;

        _data = new byte[width * height * depth];
        _lightLevels = new byte[width * depth];

        _random = new Random();

        if (!TryLoad())
        {
            WorldGen.Generate(this);
        }

        UpdateLightLevels(0, 0, width, depth);
    }

    private bool TryLoad(string path = "level.dat")
    {
        if (!File.Exists(path)) return false;

        try
        {
            using var fileStream = File.OpenRead(path);
            using var stream = new GZipStream(fileStream, CompressionMode.Decompress);
            stream.ReadExactly(_data);

            OnAreaUpdate?.Invoke(0, 0, 0, Width, Height, Depth);

            return true;
        }
        catch (Exception e)
        {
            TraceLog(TraceLogLevel.Warning, e.ToString());
        }

        return false;
    }

    public void Save(string path = "level.dat")
    {
        try
        {
            using var fileStream = File.OpenWrite(path);
            using var stream = new GZipStream(fileStream, CompressionMode.Compress);
            stream.Write(_data);
        }
        catch (Exception e)
        {
            TraceLog(TraceLogLevel.Warning, e.ToString());
        }
    }

    private void UpdateLightLevels(int x, int z, int width, int depth)
    {
        for (var i = x; i < x + width; i++)
        {
            for (var j = z; j < z + depth; j++)
            {
                var oldY = _lightLevels[i + Width * j];

                var y = Height;
                for (; y > 0 && !IsLightBlocker(i, y, j); y--)
                {
                }

                if (y == oldY) continue;

                var minY = Math.Min(y, oldY);
                var maxY = Math.Max(y, oldY);

                OnAreaUpdate?.Invoke(i - 1, minY, j - 1, i + 1, maxY, j + 1);
                _lightLevels[i + Width * j] = y;
            }
        }
    }

    public byte GetTile(int x, int y, int z) => !IsInRange(x, y, z) ? (byte)0 : _data[GetDataIndex(x, y, z)];

    public byte GetTile(TilePosition position) => GetTile(position.X, position.Y, position.Z);

    public bool IsSolidTile(int x, int y, int z) =>
        Registries.Tiles.Registry[GetTile(x, y, z)]?.Capabilities.IsSolid ?? false;

    public bool IsSolidTile(TilePosition position) => IsSolidTile(position.X, position.Y, position.Z);

    public bool IsLightBlocker(int x, int y, int z) =>
        Registries.Tiles.Registry[GetTile(x, y, z)]?.Capabilities.CanBlockLight ?? false;

    public bool IsLightBlocker(TilePosition position) => IsLightBlocker(position.X, position.Y, position.Z);

    public List<BoundingBox> GetBoxes(BoundingBox area)
    {
        var boxes = new List<BoundingBox>();

        int minX = (int)area.Min.X, minY = (int)area.Min.Y, minZ = (int)area.Min.Z;
        int maxX = (int)area.Max.X, maxY = (int)area.Max.Y, maxZ = (int)area.Max.Z;

        minX = Math.Clamp(minX, 0, Width);
        minY = Math.Clamp(minY, 0, Height);
        minZ = Math.Clamp(minZ, 0, Depth);

        maxX = Math.Clamp(maxX, 0, Width);
        maxY = Math.Clamp(maxY, 0, Height);
        maxZ = Math.Clamp(maxZ, 0, Depth);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    if (!IsSolidTile(x, y, z)) continue;

                    boxes.Add(new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1)));
                }
            }
        }

        return boxes;
    }

    public bool IsLit(int x, int y, int z) => !IsInRange(x, y, z) || y >= _lightLevels[x + Width * z];

    public bool IsLit(TilePosition position) => IsLit(position.X, position.Y, position.Z);

    public float GetBrightness(int x, int y, int z) => IsLit(x, y, z) ? LightValue : DarkValue;

    public float GetBrightness(TilePosition position) => GetBrightness(position.X, position.Y, position.Z);

    public bool TrySetTile(int x, int y, int z, byte value)
    {
        if (!IsInRange(x, y, z)) return false;
        if (_data[GetDataIndex(x, y, z)] == value) return false;

        _data[GetDataIndex(x, y, z)] = value;
        UpdateLightLevels(x, z, 1, 1);
        OnAreaUpdate?.Invoke(x - 1, y - 1, z - 1, x + 1, y + 1, z + 1);

        return true;
    }

    public bool TrySetTile(TilePosition position, byte value) => TrySetTile(position.X, position.Y, position.Z, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DirectSetTile(int x, int y, int z, byte value)
    {
        _data[GetDataIndex(x, y, z)] = value;
    }

    public void Tick()
    {
        var ticks = Width * Height * Depth * 0.0025f; // 0.0025 = 1 / 400
        for (var t = 0; t < ticks; t++)
        {
            var x = _random.Next(Width);
            var y = _random.Next(Height);
            var z = _random.Next(Depth);

            Registries.Tiles.Registry[GetTile(x, y, z)]?.Tick(this, x, y, z, _random);
        }
    }

    public RayCollision DoRayCast(Ray ray, float maxDistance)
    {
        var col = new RayCollision();

        var t = 0.0f;

        var ix = Floor(ray.Position.X) | 0;
        var iy = Floor(ray.Position.Y) | 0;
        var iz = Floor(ray.Position.Z) | 0;

        var stepX = ray.Direction.X > 0 ? 1 : -1;
        var stepY = ray.Direction.Y > 0 ? 1 : -1;
        var stepZ = ray.Direction.Z > 0 ? 1 : -1;

        var txDelta = MathF.Abs(1 / ray.Direction.X);
        var tyDelta = MathF.Abs(1 / ray.Direction.Y);
        var tzDelta = MathF.Abs(1 / ray.Direction.Z);

        var xDist = stepX > 0 ? ix + 1 - ray.Position.X : ray.Position.X - ix;
        var yDist = stepY > 0 ? iy + 1 - ray.Position.Y : ray.Position.Y - iy;
        var zDist = stepZ > 0 ? iz + 1 - ray.Position.Z : ray.Position.Z - iz;

        var txMax = txDelta < float.PositiveInfinity ? txDelta * xDist : float.PositiveInfinity;
        var tyMax = tyDelta < float.PositiveInfinity ? tyDelta * yDist : float.PositiveInfinity;
        var tzMax = tzDelta < float.PositiveInfinity ? tzDelta * zDist : float.PositiveInfinity;

        var steppedIndex = -1;

        while (t <= maxDistance)
        {
            if (Registries.Tiles.Registry[GetTile(ix, iy, iz)] != null)
            {
                col.Point = ray.Position + t * ray.Direction;

                switch (steppedIndex)
                {
                    case 0:
                        col.Normal.X = -stepX;
                        break;
                    case 1:
                        col.Normal.Y = -stepY;
                        break;
                    case 2:
                        col.Normal.Z = -stepZ;
                        break;
                }

                col.Hit = true;
                col.Distance = t;

                return col;
            }

            if (txMax < tyMax)
            {
                if (txMax < tzMax)
                {
                    ix += stepX;
                    t = txMax;
                    txMax += txDelta;
                    steppedIndex = 0;
                }
                else
                {
                    iz += stepZ;
                    t = tzMax;
                    tzMax += tzDelta;
                    steppedIndex = 2;
                }
            }
            else
            {
                if (tyMax < tzMax)
                {
                    iy += stepY;
                    t = tyMax;
                    tyMax += tyDelta;
                    steppedIndex = 1;
                }
                else
                {
                    iz += stepZ;
                    t = tzMax;
                    tzMax += tzDelta;
                    steppedIndex = 2;
                }
            }
        }

        col.Point = ray.Position + t * ray.Direction;
        col.Hit = false;
        col.Distance = t;

        return col;

        int Floor(float f)
        {
            return (int)MathF.Floor(f);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsInRange(int x, int y, int z) => x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Depth;

    // use original indexing to have compatibility with original levels
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDataIndex(int x, int y, int z) => (y * Depth + z) * Width + x;
}