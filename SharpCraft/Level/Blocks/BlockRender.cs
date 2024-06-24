using SharpCraft.Rendering;

namespace SharpCraft.Level.Blocks;

public static class BlockRender
{
    private const float Darkest = 0.6f;
    private const float Darker = 0.8f;
    private const float Light = 1.0f;

    public static void BuildLeftFace(IVertexBuilder builder, float brightness, Block block, int x, int y, int z)
    {
        var textureIndex = block.GetTextureIndexForFace(Face.Left);
        var coords = block.GetTextureCoordinates(Face.Left, textureIndex);
        
        var x0 = x + block.Bounds.Min.X;
        var y0 = y + block.Bounds.Min.Y;
        var z0 = z + block.Bounds.Min.Z;
        var y1 = y + block.Bounds.Max.Y;
        var z1 = z + block.Bounds.Max.Z;
            
        var u0 = coords.Origin.X;
        var u1 = coords.Origin.X + coords.Size.X;
        var v0 = coords.Origin.Y;
        var v1 = coords.Origin.Y + coords.Size.Y;

        var b = brightness * Darkest;

        if (builder.SupportsIndices)
        {
            builder.Indices([ 0, 1, 2, 0, 2, 3 ], true);
        }
        
        builder.Color(b, b, b);

        builder.VertexWithTex(x0, y0, z0, u0, v1);
        builder.VertexWithTex(x0, y0, z1, u1, v1);
        builder.VertexWithTex(x0, y1, z1, u1, v0);
        builder.VertexWithTex(x0, y1, z0, u0, v0);
    }
    
    public static void BuildLeftFace(IVertexBuilder builder, Level level, Block block, int x, int y, int z)
    {
        BuildLeftFace(builder, level.GetBrightness(x - 1, y, z), block, x, y, z);
    }

    public static void BuildRightFace(IVertexBuilder builder, float brightness, Block block, int x, int y, int z)
    {
        var textureIndex = block.GetTextureIndexForFace(Face.Right);
        var coords = block.GetTextureCoordinates(Face.Right, textureIndex);
        
        var y0 = y + block.Bounds.Min.Y;
        var z0 = z + block.Bounds.Min.Z;
        var x1 = x + block.Bounds.Max.X;
        var y1 = y + block.Bounds.Max.Y;
        var z1 = z + block.Bounds.Max.Z;
            
        var u0 = coords.Origin.X;
        var u1 = coords.Origin.X + coords.Size.X;
        var v0 = coords.Origin.Y;
        var v1 = coords.Origin.Y + coords.Size.Y;

        var b = brightness * Darkest;

        if (builder.SupportsIndices)
        {
            builder.Indices([ 0, 1, 2, 0, 2, 3 ], true);
        }
        
        builder.Color(b, b, b);

        builder.VertexWithTex(x1, y0, z0, u1, v1);
        builder.VertexWithTex(x1, y1, z0, u1, v0);
        builder.VertexWithTex(x1, y1, z1, u0, v0);
        builder.VertexWithTex(x1, y0, z1, u0, v1);
    }

    public static void BuildRightFace(IVertexBuilder builder, Level level, Block block, int x, int y, int z)
    {
        BuildRightFace(builder, level.GetBrightness(x + 1, y, z), block, x, y, z);
    }

    public static void BuildTopFace(IVertexBuilder builder, float brightness, Block block, int x, int y, int z)
    {
        var textureIndex = block.GetTextureIndexForFace(Face.Top);
        var coords = block.GetTextureCoordinates(Face.Top, textureIndex);
        
        var x0 = x + block.Bounds.Min.X;
        var z0 = z + block.Bounds.Min.Z;
        var x1 = x + block.Bounds.Max.X;
        var y1 = y + block.Bounds.Max.Y;
        var z1 = z + block.Bounds.Max.Z;
            
        var u0 = coords.Origin.X;
        var u1 = coords.Origin.X + coords.Size.X;
        var v0 = coords.Origin.Y;
        var v1 = coords.Origin.Y + coords.Size.Y;

        var b = brightness * Light;
        
        if (builder.SupportsIndices)
        {
            builder.Indices([ 0, 1, 2, 0, 2, 3 ], true);
        }

        builder.Color(b, b, b);

        builder.VertexWithTex(x0, y1, z0, u0, v0);
        builder.VertexWithTex(x0, y1, z1, u0, v1);
        builder.VertexWithTex(x1, y1, z1, u1, v1);
        builder.VertexWithTex(x1, y1, z0, u1, v0);
    }

    public static void BuildTopFace(IVertexBuilder builder, Level level, Block block, int x, int y, int z)
    {
        BuildTopFace(builder, level.GetBrightness(x, y + 1, z), block, x, y, z);
    }

    public static void BuildBottomFace(IVertexBuilder builder, float brightness, Block block, int x, int y, int z)
    {
        var textureIndex = block.GetTextureIndexForFace(Face.Bottom);
        var coords = block.GetTextureCoordinates(Face.Bottom, textureIndex);
        
        var x0 = x + block.Bounds.Min.X;
        var y0 = y + block.Bounds.Min.Y;
        var z0 = z + block.Bounds.Min.Z;
        var x1 = x + block.Bounds.Max.X;
        var z1 = z + block.Bounds.Max.Z;
            
        var u0 = coords.Origin.X;
        var u1 = coords.Origin.X + coords.Size.X;
        var v0 = coords.Origin.Y;
        var v1 = coords.Origin.Y + coords.Size.Y;

        var b = brightness * Light;
        
        if (builder.SupportsIndices)
        {
            builder.Indices([ 0, 1, 2, 0, 2, 3 ], true);
        }

        builder.Color(b, b, b);

        builder.VertexWithTex(x0, y0, z0, u1, v0);
        builder.VertexWithTex(x1, y0, z0, u0, v0);
        builder.VertexWithTex(x1, y0, z1, u0, v1);
        builder.VertexWithTex(x0, y0, z1, u1, v1);
    }

    public static void BuildBottomFace(IVertexBuilder builder, Level level, Block block, int x, int y, int z)
    {
        BuildBottomFace(builder, level.GetBrightness(x, y - 1, z), block, x, y, z);
    }

    public static void BuildFrontFace(IVertexBuilder builder, float brightness, Block block, int x, int y, int z)
    {
        var textureIndex = block.GetTextureIndexForFace(Face.Front);
        var coords = block.GetTextureCoordinates(Face.Front, textureIndex);
            
        var x0 = x + block.Bounds.Min.X;
        var y0 = y + block.Bounds.Min.Y;
        var x1 = x + block.Bounds.Max.X;
        var y1 = y + block.Bounds.Max.Y;
        var z1 = z + block.Bounds.Max.Z;
        
        var u0 = coords.Origin.X;
        var u1 = coords.Origin.X + coords.Size.X;
        var v0 = coords.Origin.Y;
        var v1 = coords.Origin.Y + coords.Size.Y;
            
        var b = brightness * Darker;
        
        if (builder.SupportsIndices)
        {
            builder.Indices([ 0, 1, 2, 0, 2, 3 ], true);
        }

        builder.Color(b, b, b);

        builder.VertexWithTex(x0, y0, z1, u0, v1);
        builder.VertexWithTex(x1, y0, z1, u1, v1);
        builder.VertexWithTex(x1, y1, z1, u1, v0);
        builder.VertexWithTex(x0, y1, z1, u0, v0);
    }

    public static void BuildFrontFace(IVertexBuilder builder, Level level, Block block, int x, int y, int z)
    {
        BuildFrontFace(builder, level.GetBrightness(x, y, z + 1), block, x, y, z);
    }

    public static void BuildBackFace(IVertexBuilder builder, float brightness, Block block, int x, int y, int z)
    {
        var textureIndex = block.GetTextureIndexForFace(Face.Back);
        var coords = block.GetTextureCoordinates(Face.Back, textureIndex);
        
        var x0 = x + block.Bounds.Min.X;
        var y0 = y + block.Bounds.Min.Y;
        var z0 = z + block.Bounds.Min.Z;
        var x1 = x + block.Bounds.Max.X;
        var y1 = y + block.Bounds.Max.Y;
            
        var u0 = coords.Origin.X;
        var u1 = coords.Origin.X + coords.Size.X;
        var v0 = coords.Origin.Y;
        var v1 = coords.Origin.Y + coords.Size.Y;

        var b = brightness * Darker;
        
        if (builder.SupportsIndices)
        {
            builder.Indices([ 0, 1, 2, 0, 2, 3 ], true);
        }

        builder.Color(b, b, b);

        builder.VertexWithTex(x0, y0, z0, u1, v1);
        builder.VertexWithTex(x0, y1, z0, u1, v0);
        builder.VertexWithTex(x1, y1, z0, u0, v0);
        builder.VertexWithTex(x1, y0, z0, u0, v1);
    }

    public static void BuildBackFace(IVertexBuilder builder, Level level, Block block, int x, int y, int z)
    {
        BuildBackFace(builder, level.GetBrightness(x, y, z - 1), block, x, y, z);
    }
}