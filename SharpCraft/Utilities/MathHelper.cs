using System.Runtime.CompilerServices;

namespace SharpCraft.Utilities;

public static class MathHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FFloor(float value) => (int)value - (value < (int)value ? 1 : 0);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FCell(float value) => (int)value + (value > (int)value ? 1 : 0);
}