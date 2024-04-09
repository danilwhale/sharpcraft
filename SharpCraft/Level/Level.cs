﻿using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpCraft.Level.Generation;
using SharpCraft.Level.Tiles;

namespace SharpCraft.Level;

public class Level
{
    public delegate void OnTileChangedEvent(int x, int y, int z);

    public delegate void OnLightLevelChangedEvent(int x, int z, int minY, int maxY);

    public delegate void OnEverythingChangedEvent();

    private const float LightValue = 1.0f;
    private const float DarkValue = 0.4f;

    // I use 1d array only because it's easier to load/save level
    private readonly byte[] _data;
    private readonly int[] _lightLevels;

    public readonly int Width;
    public readonly int Height;
    public readonly int Length;

    public event OnTileChangedEvent? OnTileChanged;
    public event OnLightLevelChangedEvent? OnLightLevelChanged;
    public event OnEverythingChangedEvent? OnEverythingChanged;

    public Level(int width, int height, int length)
    {
        Width = width;
        Height = height;
        Length = length;

        _data = new byte[width * height * length];
        _lightLevels = new int[width * length];

        if (!TryLoad())
        {
            LevelGeneration.Generate(this, Random.Shared.Next());
        }

        UpdateLightLevels(0, 0, width, length);
    }

    private bool TryLoad(string path = "level.dat")
    {
        if (!File.Exists(path)) return false;

        try
        {
            using var fileStream = File.OpenRead(path);
            using var stream = new GZipStream(fileStream, CompressionMode.Decompress);
            stream.ReadExactly(_data);

            OnEverythingChanged?.Invoke();

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

    public void UpdateLightLevels(int x, int z, int width, int length)
    {
        for (var i = x; i < x + width; i++)
        {
            for (var j = z; j < z + length; j++)
            {
                var oldY = _lightLevels[i + Width * j];

                var y = Height;
                for (; y > 0 && !IsLightBlocker(i, y, j); y--)
                {
                }

                if (y == oldY) continue;

                var minY = Math.Min(y, oldY);
                var maxY = Math.Max(y, oldY);

                OnLightLevelChanged?.Invoke(i, j, minY, maxY);
                _lightLevels[i + Width * j] = y;
            }
        }
    }

    public bool IsTile(int x, int y, int z)
    {
        return GetTile(x, y, z) > 0;
    }

    public bool IsTile(TilePosition position) => IsTile(position.X, position.Y, position.Z);

    public bool IsSolidTile(int x, int y, int z)
    {
        var id = GetTile(x, y, z);
        var tile = TileRegistry.Tiles[id];

        return tile?.IsSolid ?? false;
    }

    public bool IsSolidTile(TilePosition position) => IsSolidTile(position.X, position.Y, position.Z);

    public bool IsLightBlocker(int x, int y, int z)
    {
        var id = GetTile(x, y, z);
        var tile = TileRegistry.Tiles[id];

        return tile?.IsLightBlocker ?? false;
    }

    public bool IsLightBlocker(TilePosition position) => IsLightBlocker(position.X, position.Y, position.Z);

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
                    var id = GetTile(x, y, z);
                    var tile = TileRegistry.Tiles[id];
                    if (tile == null) continue;

                    boxes.Add(tile.GetCollision(x, y, z));
                }
            }
        }

        return boxes;
    }

    public float GetBrightness(int x, int y, int z)
    {
        if (!IsInRange(x, y, z)) return LightValue;
        return _lightLevels[x + Width * z] >= y ? DarkValue : LightValue;
    }

    public float GetBrightness(TilePosition position) => GetBrightness(position.X, position.Y, position.Z);

    public void SetTile(int x, int y, int z, byte value, bool updateLighting = true)
    {
        if (!IsInRange(x, y, z)) return;

        SetTileUnchecked(x, y, z, value, updateLighting);
    }

    public void SetTile(TilePosition position, byte value, bool updateLighting = true) =>
        SetTile(position.X, position.Y, position.Z, value, updateLighting);

    public byte GetTile(int x, int y, int z) => !IsInRange(x, y, z) ? (byte)0 : GetTileUnchecked(x, y, z);
    public byte GetTile(TilePosition position) => GetTile(position.X, position.Y, position.Z);

    public void SetTileUnchecked(int x, int y, int z, byte value, bool updateLighting)
    {
        var index = GetDataIndex(x, y, z);
        if (_data[index] == value) return;
        
        _data[index] = value;

        if (updateLighting) UpdateLightLevels(x, z, 1, 1);
        OnTileChanged?.Invoke(x, y, z);
    }

    public void SetTileUnchecked(TilePosition position, byte value, bool updateLighting) =>
        SetTileUnchecked(position.X, position.Y, position.Z, value, updateLighting);

    public byte GetTileUnchecked(int x, int y, int z) => _data[GetDataIndex(x, y, z)];
    public byte GetTileUnchecked(TilePosition position) => GetTileUnchecked(position.X, position.Y, position.Z);

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
            if (IsTile(ix, iy, iz))
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
    public bool IsInRange(int x, int y, int z) => x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;

    // use original indexing to have compatibility with original levels
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDataIndex(int x, int y, int z) => (y * Length + z) * Width + x;
}