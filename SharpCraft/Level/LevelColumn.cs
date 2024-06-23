using System.Runtime.CompilerServices;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public class LevelColumn<T>
{
    private readonly T[] _rows = new T[Chunk.SizeSq];

    public T this[int x, int z]
    {
        get => _rows.GetUnsafeRef(x + Chunk.Size * z);
        set => _rows.GetUnsafeRef(x + Chunk.Size * z) = value;
    }
}