using SharpCraft.Rendering;

namespace SharpCraft.Tiles;

public class Tile
{
    private readonly int _textureIndex;

    protected Tile(byte id)
    {
        TileRegistry.Registry[id] = this;
    }

    public Tile(byte id, int textureIndex)
        : this(id)
    {
        _textureIndex = textureIndex;
    }

    public void Build(MeshBuilder builder, Level.Level level, int x, int y, int z)
    {
        TileRender.Render(builder, level, this, GetFaces(level, x, y, z), x, y, z);
    }

    private bool ShouldKeepFace(Level.Level level, int x, int y, int z)
    {
        return !level.IsSolidTile(x, y, z);
    }

    public int GetFaceTextureIndex(Face face)
    {
        return _textureIndex;
    }

    public Face GetFaces(Level.Level level, int x, int y, int z)
    {
        var faces = Face.None;

        if (ShouldKeepFace(level, x + 1, y, z)) faces |= Face.Right;
        if (ShouldKeepFace(level, x - 1, y, z)) faces |= Face.Left;
        if (ShouldKeepFace(level, x, y + 1, z)) faces |= Face.Top;
        if (ShouldKeepFace(level, x, y - 1, z)) faces |= Face.Bottom;
        if (ShouldKeepFace(level, x, y, z + 1)) faces |= Face.Front;
        if (ShouldKeepFace(level, x, y, z - 1)) faces |= Face.Back;

        return faces;
    }

    public int GetFaceCount(Level.Level level, int x, int y, int z)
    {
        var count = 0;
        
        if (ShouldKeepFace(level, x + 1, y, z)) count++;
        if (ShouldKeepFace(level, x - 1, y, z)) count++;
        if (ShouldKeepFace(level, x, y + 1, z)) count++;
        if (ShouldKeepFace(level, x, y - 1, z)) count++;
        if (ShouldKeepFace(level, x, y, z + 1)) count++;
        if (ShouldKeepFace(level, x, y, z - 1)) count++;

        return count;
    }
}