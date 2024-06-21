using System.Numerics;

namespace SharpCraft.Level.Blocks;

public readonly struct BlockPosition(int x, int y, int z)
{
    public static readonly BlockPosition One = new(1, 1, 1);
    public static readonly BlockPosition Zero = new(0, 0, 0);
    
    public static readonly BlockPosition UnitX = new(1, 0, 0);
    public static readonly BlockPosition UnitY = new(0, 1, 0);
    public static readonly BlockPosition UnitZ = new(0, 0, 1);

    public readonly int X = x;
    public readonly int Y = y;
    public readonly int Z = z;

    public static bool IsInRange(BlockPosition value, BlockPosition min, BlockPosition max)
    {
        return value >= min && value < max;
    }

    public static BlockPosition ToChunkPosition(BlockPosition worldPosition)
    {
        return new BlockPosition(worldPosition.X >> 4, worldPosition.Y >> 4, worldPosition.Z >> 4);
    }

    public static BlockPosition Clamp(BlockPosition value, BlockPosition min, BlockPosition max)
    {
        return Max(min, Min(value, max));
    }

    public static BlockPosition Min(BlockPosition a, BlockPosition b)
    {
        return a <= b ? a : b;
    }
    
    public static BlockPosition Max(BlockPosition a, BlockPosition b)
    {
        return a >= b ? a : b;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlockPosition position && 
               position.X == X && position.Y == Y && position.Z == Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(BlockPosition left, BlockPosition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlockPosition left, BlockPosition right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(BlockPosition left, BlockPosition right)
    {
        return left.X < right.X && left.Y < right.Y && left.Z < right.Z;
    }

    public static bool operator >(BlockPosition left, BlockPosition right)
    {
        return left.X > right.X && left.Y > right.Y && left.Z > right.Z;
    }

    public static bool operator <=(BlockPosition left, BlockPosition right)
    {
        return left < right || left == right;
    }

    public static bool operator >=(BlockPosition left, BlockPosition right)
    {
        return left > right || left == right;
    }

    public static BlockPosition operator +(BlockPosition left, BlockPosition right)
    {
        return new BlockPosition(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static BlockPosition operator -(BlockPosition left, BlockPosition right)
    {
        return left + new BlockPosition(-right.X, -right.Y, -right.Z);
    }

    public static BlockPosition operator -(BlockPosition value)
    {
        return Zero - value;
    }

    public static implicit operator Vector3(BlockPosition position) => new(position.X, position.Y, position.Z);
    public static implicit operator BlockPosition(Vector3 vector) => new((int)vector.X, (int)vector.Y, (int)vector.Z);
}