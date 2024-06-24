using System.Numerics;
using SharpCraft.Physics;
using SharpCraft.Utilities;
using Silk.NET.Maths;

namespace SharpCraft.Level.Blocks.Types;

public class PoleBlock : Block
{
    private readonly int _textureIndex;
    
    public PoleBlock(byte id, int viewTextureIndex, int textureIndex) : base(id, viewTextureIndex, new BlockConfig(false, false))
    {
        Bounds = new BoundingBox(new Vector3(0.3f, 0.0f, 0.3f), new Vector3(0.7f, 1.0f, 0.7f));
        _textureIndex = textureIndex;
    }

    public override int GetTextureIndexForFace(Face face)
    {
        return _textureIndex;
    }

    public override bool ShouldKeepFace(Level level, int x, int y, int z, Face face)
    {
        if (face is not (Face.Top or Face.Bottom)) return true;
        if (!level.IsInRange(x, y, z)) return true;

        var id = level.GetBlockUnchecked(x, y, z);
        var block = BlockRegistry.Blocks.GetUnsafeRef(id);
        
        if (block == null) return true;

        return block is not PoleBlock;
    }

    public override Rectangle<float> GetTextureCoordinates(Face face, int textureIndex)
    {
        var x = textureIndex % 16.0f / 16.0f;
        var y = MathF.Floor(textureIndex / 16.0f) / 16.0f;
        
        var offset = MathF.Min(Bounds.Min.X, Bounds.Min.Z) / 16.0f;
        var size =
            (MathF.Max(Bounds.Max.X, Bounds.Max.Z) - MathF.Min(Bounds.Min.X, Bounds.Min.Z))
            / 16.0f;

        if (face is Face.Bottom or Face.Top)
        {
            return new Rectangle<float>(x + offset, y + offset, size, size);
        }

        return new Rectangle<float>(
            x + offset,
            y,
            size,
            1.0f / 16
            );
    }
}