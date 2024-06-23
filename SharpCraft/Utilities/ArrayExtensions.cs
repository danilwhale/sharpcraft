using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpCraft.Utilities;

public static class ArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetUnsafeRef<T>(this T[] array, int index)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Get2DUnsafeRef<T>(this T[] array, int x, int y, int xSize)
    {
        return ref array.GetUnsafeRef(x + xSize * y);
    }

    public static ref T Get3DUnsafeRef<T>(this T[] array, int x, int y, int z, int zSizeSq, int zSize)
    {
        return ref array.GetUnsafeRef(y * zSizeSq + z * zSize + x);
    }
}