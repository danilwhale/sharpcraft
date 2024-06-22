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
}