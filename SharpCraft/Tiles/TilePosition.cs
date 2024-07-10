using System.Numerics;

namespace SharpCraft.Tiles;

public readonly struct TilePosition(int x, int y, int z) : IEquatable<TilePosition>
{
    public static readonly TilePosition One = new(1, 1, 1);
    public static readonly TilePosition Zero = new(0, 0, 0);
    
    public static readonly TilePosition UnitX = new(1, 0, 0);
    public static readonly TilePosition UnitY = new(0, 1, 0);
    public static readonly TilePosition UnitZ = new(0, 0, 1);

    public readonly int X = x;
    public readonly int Y = y;
    public readonly int Z = z;
    
    public override bool Equals(object? obj)
    {
        return obj is TilePosition position && 
               Equals(position);
    }
    
    public bool Equals(TilePosition other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
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