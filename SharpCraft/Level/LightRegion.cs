using System.Runtime.CompilerServices;

namespace SharpCraft.Level;

public class LightRegion
{
    public const int Size = Chunk.Size;

    private readonly ushort[] _levels = new ushort[Size * Size];

    public ushort this[int x, int z]
    {
        get => _levels[x + Size * z];
        set => _levels[x + Size * z] = value;
    }
}