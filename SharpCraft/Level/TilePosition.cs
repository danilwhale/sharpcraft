using System.Numerics;

namespace SharpCraft.Level;

public readonly struct TilePosition(int x, int y, int z)
{
    public static readonly TilePosition One = new(1, 1, 1);
    public static readonly TilePosition Zero = new(0, 0, 0);
    
    public static readonly TilePosition UnitX = new(1, 0, 0);
    public static readonly TilePosition UnitY = new(0, 1, 0);
    public static readonly TilePosition UnitZ = new(0, 0, 1);

    public readonly int X = x;
    public readonly int Y = y;
    public readonly int Z = z;

    public static bool IsInRange(TilePosition value, TilePosition min, TilePosition max)
    {
        return value >= min && value < max;
    }

    public static TilePosition ToChunkPosition(TilePosition worldPosition)
    {
        return new TilePosition(worldPosition.X / Chunk.Size, worldPosition.Y / Chunk.Size, worldPosition.Z / Chunk.Size);
    }

    public static TilePosition Clamp(TilePosition value, TilePosition min, TilePosition max)
    {
        return Max(min, Min(value, max));
    }

    public static TilePosition Min(TilePosition a, TilePosition b)
    {
        return a <= b ? a : b;
    }
    
    public static TilePosition Max(TilePosition a, TilePosition b)
    {
        return a >= b ? a : b;
    }

    public override bool Equals(object? obj)
    {
        return obj is TilePosition position && 
               position.X == X && position.Y == Y && position.Z == Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(TilePosition left, TilePosition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TilePosition left, TilePosition right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(TilePosition left, TilePosition right)
    {
        return left.X < right.X && left.Y < right.Y && left.Z < right.Z;
    }

    public static bool operator >(TilePosition left, TilePosition right)
    {
        return left.X > right.X && left.Y > right.Y && left.Z > right.Z;
    }

    public static bool operator <=(TilePosition left, TilePosition right)
    {
        return left < right || left == right;
    }

    public static bool operator >=(TilePosition left, TilePosition right)
    {
        return left > right || left == right;
    }

    public static TilePosition operator +(TilePosition left, TilePosition right)
    {
        return new TilePosition(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static TilePosition operator -(TilePosition left, TilePosition right)
    {
        return left + new TilePosition(-right.X, -right.Y, -right.Z);
    }

    public static TilePosition operator -(TilePosition value)
    {
        return Zero - value;
    }

    public static implicit operator Vector3(TilePosition position) => new(position.X, position.Y, position.Z);
    public static implicit operator TilePosition(Vector3 vector) => new((int)vector.X, (int)vector.Y, (int)vector.Z);
}