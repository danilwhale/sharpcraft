using SharpCraft.Rendering;

namespace SharpCraft.Level;

public class Tile(int textureIndex)
{
    private const float Darkest = 0.6f;
    private const float Darker = 0.8f;
    private const float Light = 1.0f;

    public static readonly Tile Rock = new(0);
    public static readonly Tile Grass = new(1);

    public void Build(MeshBuilder builder, Level level, int x, int y, int z)
    {
        var u0 = textureIndex % 16.0f / 16.0f;
        var u1 = u0 + 1.0f / 16;
        var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
        var v1 = v0 + 1.0f / 16;

        var x0 = x;
        var y0 = y;
        var z0 = z;
        var x1 = x + 1;
        var y1 = y + 1;
        var z1 = z + 1;

        if (!level.IsSolidTile(x - 1, y, z))
        {
            var b = level.GetBrightness(x - 1, y, z) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z0, u0, v0);
        }

        if (!level.IsSolidTile(x + 1, y, z))
        {
            var b = level.GetBrightness(x + 1, y, z) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z1, u0, v0);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
        }

        if (!level.IsSolidTile(x, y + 1, z))
        {
            var b = level.GetBrightness(x, y + 1, z) * Light;
            
            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v1);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y1, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
        }

        if (!level.IsSolidTile(x, y - 1, z))
        {
            var b = level.GetBrightness(x, y - 1, z) * Light;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
        }

        if (!level.IsSolidTile(x, y, z + 1))
        {
            var b = level.GetBrightness(x, y, z + 1) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y0, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v0);
        }

        if (!level.IsSolidTile(x, y, z - 1))
        {
            var b = level.GetBrightness(x, y, z - 1) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v1);
            builder.VertexWithTex(x0, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z0, u0, v0);

            builder.VertexWithTex(x0, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v1);
        }
    }

    public int GetFaceCount(Level level, int x, int y, int z)
    {
        var count = 0;

        // check right side
        if (!level.IsSolidTile(x + 1, y, z)) count++;

        // check left side
        if (!level.IsSolidTile(x - 1, y, z)) count++;

        // check top side
        if (!level.IsSolidTile(x, y + 1, z)) count++;

        // check bottom side
        if (!level.IsSolidTile(x, y - 1, z)) count++;

        // check front side
        if (!level.IsSolidTile(x, y, z + 1)) count++;

        // check back side
        if (!level.IsSolidTile(x, y, z - 1)) count++;

        return count;
    }

    public void DrawRlGlFace(TilePosition position, Face face)
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