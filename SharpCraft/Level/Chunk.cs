using System.Numerics;
using SharpCraft.Level.Blocks;
using SharpCraft.Rendering;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public class Chunk : IDisposable, ILevelSerializable
{
    public const int Size = 16;
    public const int SizeSq = Size * Size;
    
    public static int Updates;
    private static readonly int LayerCount = Enum.GetValues<BlockLayer>().Length;

    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    
    public readonly int MaxX;
    public readonly int MaxY;
    public readonly int MaxZ;

    public readonly BoundingBox BBox;
    public bool IsDirty = true;

    private readonly Level _level;
    private readonly MeshBuilder[] _layers = new MeshBuilder[LayerCount];

    private readonly byte[] _blocks = new byte[Size * Size * Size];

    private bool _hasBeganRebuild;

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

        for (var i = 0; i < LayerCount; i++)
        {
            _layers[i] = new MeshBuilder();
        }
    }

    public byte this[int x, int y, int z]
    {
        get => _blocks[x + Size * (y + Size * z)];
        set => _blocks[x + Size * (y + Size * z)] = value;
    }

    public bool TryBeginRebuild()
    {
        if (!IsDirty) return false;
        if (_hasBeganRebuild) return false;
        
        _hasBeganRebuild = true;
        
        Updates++;

        for (var i = 0; i < LayerCount; i++)
        {
            BeginLayerRebuild((BlockLayer)i);
        }

        return true;
    }

    private void BeginLayerRebuild(BlockLayer layer)
    {
        var builder = _layers[(int)layer];

        var faceCount = GetFaceCount(layer);
        if (faceCount == 0) return;
            
        builder.Begin(faceCount * 2);
        
        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    BlockRegistry.Blocks[_level.GetBlock(x, y, z)]?.Build(builder, _level, x, y, z, layer);
                }
            }
        }
    }

    public void EndRebuild()
    {
        if (!_hasBeganRebuild) return;
        
        for (var i = 0; i < LayerCount; i++)
        {
            EndLayerRebuild((BlockLayer)i);
        }
    }

    private void EndLayerRebuild(BlockLayer layer)
    {
        _hasBeganRebuild = false;
        IsDirty = false;
        
        _layers[(int)layer].End();
    }

    public void Draw(BlockLayer layer)
    {
        _layers[(int)layer].Draw(Resources.DefaultTerrainMaterial);
    }

    private int GetFaceCount(BlockLayer layer)
    {
        var count = 0;

        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    count += BlockRegistry.Blocks[_level.GetBlock(x, y, z)]?.GetFaceCount(_level, x, y, z, layer) ?? 0;
                }
            }
        }

        return count;
    }

    public void Dispose()
    {
        foreach (var layer in _layers)
        {
            layer.Dispose();
        }
    }

    public void Read(Stream stream)
    {
        stream.ReadExactly(_blocks);
    }

    public void Write(Stream stream)
    {
        stream.Write(_blocks);
    }
}