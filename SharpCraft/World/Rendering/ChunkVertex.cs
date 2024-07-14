namespace SharpCraft.World.Rendering;

public readonly struct ChunkVertex(float x, float y, float z, float u, float v, float light)
{
    public readonly float X = x, Y = y, Z = z;
    public readonly float U = u, V = v;
    public readonly float Light = light;
}