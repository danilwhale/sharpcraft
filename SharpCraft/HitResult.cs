namespace SharpCraft;

public record HitResult(int X, int Y, int Z, Face Face)
{
    public (int X, int Y, int Z) GetAligned()
    {
        return Face switch
        {
            Face.Top => (X, Y + 1, Z),
            Face.Bottom => (X, Y - 1, Z),
            Face.Right => (X + 1, Y, Z),
            Face.Left => (X - 1, Y, Z),
            Face.Front => (X, Y, Z + 1),
            Face.Back => (X, Y, Z - 1),
            _ => (X, Y, Z)
        };
    }
}