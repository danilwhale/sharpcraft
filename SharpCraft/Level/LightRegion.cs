using System.Runtime.CompilerServices;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public class LightRegion
{
    private readonly ushort[] _levels = new ushort[Chunk.SizeSq];

    public ushort this[int x, int z]
    {
        get => _levels.GetUnsafeRef(x + Chunk.Size * z);
        set => _levels.GetUnsafeRef(x + Chunk.Size * z) = value;
    }
}