using System.Numerics;
using SharpCraft.Physics;
using SharpCraft.Rendering;
using SharpCraft.Utilities;
using Silk.NET.Maths;

namespace SharpCraft.Level.Blocks;

public class Block
{
    public readonly BlockConfig Config;

    public readonly byte Id;
    public readonly int TextureIndex;
    public BoundingBox Bounds = new(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));

    public Block(byte id, int textureIndex)
        : this(id, textureIndex, BlockConfig.Default)
    { }
    
    public Block(byte id, int textureIndex, BlockConfig config)
    {
        Id = id;
        BlockRegistry.Blocks.GetUnsafeRef(id) = this;

        TextureIndex = textureIndex;

        Config = config;
    }

    public void Build(IVertexBuilder builder, Level level, int x, int y, int z, BlockLayer layer)
    {
        if (layer != Config.Layer) return;

        if (ShouldKeepFace(level, x - 1, y, z, Face.Left))
        {
            BlockRender.BuildLeftFace(builder, level, this, x, y, z);
        }

        if (ShouldKeepFace(level, x + 1, y, z, Face.Right))
        {
            BlockRender.BuildRightFace(builder, level, this, x, y, z);
        }

        if (ShouldKeepFace(level, x, y + 1, z, Face.Top))
        {
            BlockRender.BuildTopFace(builder, level, this, x, y, z);
        }

        if (ShouldKeepFace(level, x, y - 1, z, Face.Bottom))
        {
            BlockRender.BuildBottomFace(builder, level, this, x, y, z);
        }

        if (ShouldKeepFace(level, x, y, z + 1, Face.Front))
        {
            BlockRender.BuildFrontFace(builder, level, this, x, y, z);
        }

        if (ShouldKeepFace(level, x, y, z - 1, Face.Back))
        {
            BlockRender.BuildBackFace(builder, level, this, x, y, z);
        }
    }

    public void Build(IVertexBuilder builder, int x, int y, int z)
    {
        BlockRender.BuildLeftFace(builder, 1.0f, this, x, y, z);
        BlockRender.BuildRightFace(builder, 1.0f, this, x, y, z);
        BlockRender.BuildTopFace(builder, 1.0f, this, x, y, z);
        BlockRender.BuildBottomFace(builder, 1.0f, this, x, y, z);
        BlockRender.BuildFrontFace(builder, 1.0f, this, x, y, z);
        BlockRender.BuildBackFace(builder, 1.0f, this, x, y, z);
    }

    public int GetFaceCount(Level level, int x, int y, int z, BlockLayer layer)
    {
        if (layer != Config.Layer) return 0;
        
        var count = 0;

        // check right side
        if (ShouldKeepFace(level, x + 1, y, z, Face.Right)) count++;

        // check left side
        if (ShouldKeepFace(level, x - 1, y, z, Face.Left)) count++;

        // check top side
        if (ShouldKeepFace(level, x, y + 1, z, Face.Top)) count++;

        // check bottom side
        if (ShouldKeepFace(level, x, y - 1, z, Face.Bottom)) count++;

        // check front side
        if (ShouldKeepFace(level, x, y, z + 1, Face.Front)) count++;

        // check back side
        if (ShouldKeepFace(level, x, y, z - 1, Face.Back)) count++;

        return count;
    }

    public virtual int GetTextureIndexForFace(Face face)
    {
        return TextureIndex;
    }

    public virtual bool ShouldKeepFace(Level level, int x, int y, int z, Face face)
    {
        var id = level.GetBlock(x, y, z);
        var block = BlockRegistry.Blocks.GetUnsafeRef(id);

        return (!block?.Config.IsSolid ?? true) || block.Config.Layer != Config.Layer;
    }

    public virtual Rectangle<float> GetTextureCoordinates(Face face, int textureIndex)
    {
        return new Rectangle<float>(
                textureIndex % 16.0f / 16.0f,
                MathF.Floor(textureIndex / 16.0f) / 16.0f,
                1.0f / 16,
                1.0f / 16
        );
    }

    public BoundingBox GetCollision(int x, int y, int z) =>
        new(Bounds.Min + new Vector3(x, y, z), Bounds.Max + new Vector3(x, y, z));
}