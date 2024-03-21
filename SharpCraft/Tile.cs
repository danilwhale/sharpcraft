namespace SharpCraft;

public class Tile(int textureIndex)
{
    private const float Darkest = 0.6f;
    private const float Darker = 0.8f;
    private const float Light = 1.0f;

    public static Tile Rock = new(0);
    public static Tile Grass = new(1);

    private readonly int _textureIndex = textureIndex;

    public void Build(MeshBuilder builder, Level level, BlockPosition position)
    {
        var u0 = textureIndex % 16.0f / 16.0f;
        var u1 = u0 + 1.0f / 16;
        var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
        var v1 = v0 + 1.0f / 16;

        var x0 = position.X;
        var y0 = position.Y;
        var z0 = position.Z;
        var x1 = position.X + 1;
        var y1 = position.Y + 1;
        var z1 = position.Z + 1;

        if (!level.IsSolidTile(position - BlockPosition.UnitX))
        {
            var b = level.GetBrightness(position - BlockPosition.UnitX) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z0, u0, v0);
        }

        if (!level.IsSolidTile(position + BlockPosition.UnitX))
        {
            var b = level.GetBrightness(position + BlockPosition.UnitX) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z1, u0, v0);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
        }

        if (!level.IsSolidTile(position + BlockPosition.UnitY))
        {
            var b = level.GetBrightness(position + BlockPosition.UnitY) * Light;
            
            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v1);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y1, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
        }

        if (!level.IsSolidTile(position - BlockPosition.UnitY))
        {
            var b = level.GetBrightness(position - BlockPosition.UnitY) * Light;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
        }

        if (!level.IsSolidTile(position + BlockPosition.UnitZ))
        {
            var b = level.GetBrightness(position + BlockPosition.UnitZ) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y0, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v0);
        }

        if (!level.IsSolidTile(position - BlockPosition.UnitZ))
        {
            var b = level.GetBrightness(position - BlockPosition.UnitZ) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v1);
            builder.VertexWithTex(x0, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z0, u0, v0);

            builder.VertexWithTex(x0, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v1);
        }
    }

    public int GetFaceCount(Level level, BlockPosition position)
    {
        var count = 0;

        // check right side
        if (!level.IsSolidTile(position + BlockPosition.UnitX)) count++;

        // check left side
        if (!level.IsSolidTile(position - BlockPosition.UnitX)) count++;

        // check top side
        if (!level.IsSolidTile(position + BlockPosition.UnitY)) count++;

        // check bottom side
        if (!level.IsSolidTile(position - BlockPosition.UnitY)) count++;

        // check front side
        if (!level.IsSolidTile(position + BlockPosition.UnitZ)) count++;

        // check back side
        if (!level.IsSolidTile(position - BlockPosition.UnitZ)) count++;

        return count;
    }

    public void DrawRlGlFace(BlockPosition position, Face face)
    {
        var x0 = position.X;
        var y0 = position.Y;
        var z0 = position.Z;
        var x1 = position.X + 1;
        var y1 = position.Y + 1;
        var z1 = position.Z + 1;
        
        switch (face)
        {
            case Face.Left:
                Rlgl.Vertex3f(x0, y0, z0);
                Rlgl.Vertex3f(x0, y0, z1);
                Rlgl.Vertex3f(x0, y1, z1);
                Rlgl.Vertex3f(x0, y1, z0);
                break;
            
            case Face.Right:
                Rlgl.Vertex3f(x1, y0, z0);
                Rlgl.Vertex3f(x1, y1, z0);
                Rlgl.Vertex3f(x1, y1, z1);
                Rlgl.Vertex3f(x1, y0, z1);
                break;
            
            case Face.Top:
                Rlgl.Vertex3f(x0, y1, z0);
                Rlgl.Vertex3f(x0, y1, z1);
                Rlgl.Vertex3f(x1, y1, z1);
                Rlgl.Vertex3f(x1, y1, z0);
                break;
            
            case Face.Bottom:
                Rlgl.Vertex3f(x0, y0, z0);
                Rlgl.Vertex3f(x1, y0, z0);
                Rlgl.Vertex3f(x1, y0, z1);
                Rlgl.Vertex3f(x0, y0, z1);
                break;
            
            case Face.Front:
                Rlgl.Vertex3f(x0, y0, z1);
                Rlgl.Vertex3f(x1, y0, z1);
                Rlgl.Vertex3f(x1, y1, z1);
                Rlgl.Vertex3f(x0, y1, z1);
                break;
            
            case Face.Back:
                Rlgl.Vertex3f(x0, y0, z0);
                Rlgl.Vertex3f(x0, y1, z0);
                Rlgl.Vertex3f(x1, y1, z0);
                Rlgl.Vertex3f(x1, y0, z0);
                break;
        }
    }
}