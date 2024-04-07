using System.Numerics;

namespace SharpCraft.Level.Tiles.Types;

public class PoleTile : Tile
{
    private readonly int _textureIndex;
    
    public PoleTile(byte id, int viewTextureIndex, int textureIndex) : base(id, viewTextureIndex, false, false)
    {
        Bounds = new BoundingBox(new Vector3(0.3f, 0.0f, 0.3f), new Vector3(0.7f, 1.0f, 0.7f));
        _textureIndex = textureIndex;
    }

    protected override int GetTextureIndexForFace(Face face)
    {
        return _textureIndex;
    }

    protected override bool ShouldKeepFace(Level level, int x, int y, int z, Face face)
    {
        if (face is not (Face.Top or Face.Bottom)) return true;
        if (!level.IsInRange(x, y, z)) return true;

        var id = level.GetTileUnchecked(x, y, z);
        var tile = TileRegistry.Tiles[id];
        
        if (tile == null) return true;

        return tile is not PoleTile;
    }

    protected override Rectangle GetTextureCoordinates(Face face, int textureIndex)
    {
        var x = textureIndex % 16.0f / 16.0f;
        var y = MathF.Floor(textureIndex / 16.0f) / 16.0f;
        
        var offset = MathF.Min(Bounds.Min.X, Bounds.Min.Z) / 16.0f;
        var size =
            (MathF.Max(Bounds.Max.X, Bounds.Max.Z) - MathF.Min(Bounds.Min.X, Bounds.Min.Z))
            / 16.0f;

        if (face is Face.Bottom or Face.Top)
        {
            return new Rectangle(x + offset, y + offset, size, size);
        }

        return new Rectangle(
            x + offset,
            y,
            size,
            1.0f / 16
            );
    }
}