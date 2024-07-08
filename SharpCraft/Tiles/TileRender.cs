using SharpCraft.Rendering;

namespace SharpCraft.Tiles;

public static class TileRender
{
    public const float TexFactor = 1.0f / 16;

    private const float Darkest = 0.6f;
    private const float Darker = 0.8f;
    private const float Light = 1.0f;

    // use OR to combine faces: Face.Left | Face.Right | Face.Top | Face.Bottom, etc.
    public static void Render(
        MeshBuilder b,
        Level.Level level,
        Tile tile,
        Face faces,
        int x, int y, int z)
    {
        var x1 = x + 1;
        var y1 = y + 1;
        var z1 = z + 1;

        if ((faces & Face.Left) == Face.Left)
        {
            var texIndex = tile.GetFaceTextureIndex(Face.Left);

            var u0 = (texIndex & 15) * TexFactor;
            var u1 = u0 + TexFactor;
            var v0 = (texIndex >> 4) * TexFactor;
            var v1 = v0 + TexFactor;

            SetBrightness(b, level, x - 1, y, z, Darkest);

            b.Quad();

            b.VertexTex(x, y, z, u0, v1);
            b.VertexTex(x, y, z1, u1, v1);
            b.VertexTex(x, y1, z1, u1, v0);
            b.VertexTex(x, y1, z, u0, v0);
        }

        if ((faces & Face.Right) == Face.Right)
        {
            var texIndex = tile.GetFaceTextureIndex(Face.Right);

            var u0 = (texIndex & 15) * TexFactor;
            var u1 = u0 + TexFactor;
            var v0 = (texIndex >> 4) * TexFactor;
            var v1 = v0 + TexFactor;

            SetBrightness(b, level, x + 1, y, z, Darkest);

            b.Quad();

            b.VertexTex(x1, y, z, u1, v1);
            b.VertexTex(x1, y1, z, u1, v0);
            b.VertexTex(x1, y1, z1, u0, v0);
            b.VertexTex(x1, y, z1, u0, v1);
        }

        if ((faces & Face.Top) == Face.Top)
        {
            var texIndex = tile.GetFaceTextureIndex(Face.Top);

            var u0 = (texIndex & 15) * TexFactor;
            var u1 = u0 + TexFactor;
            var v0 = (texIndex >> 4) * TexFactor;
            var v1 = v0 + TexFactor;

            SetBrightness(b, level, x, y + 1, z, Light);

            b.Quad();

            b.VertexTex(x, y1, z, u0, v0);
            b.VertexTex(x, y1, z1, u0, v1);
            b.VertexTex(x1, y1, z1, u1, v1);
            b.VertexTex(x1, y1, z, u1, v0);
        }

        if ((faces & Face.Bottom) == Face.Bottom)
        {
            var texIndex = tile.GetFaceTextureIndex(Face.Bottom);

            var u0 = (texIndex & 15) * TexFactor;
            var u1 = u0 + TexFactor;
            var v0 = (texIndex >> 4) * TexFactor;
            var v1 = v0 + TexFactor;

            SetBrightness(b, level, x, y - 1, z, Light);

            b.Quad();

            b.VertexTex(x, y, z, u1, v0);
            b.VertexTex(x1, y, z, u0, v0);
            b.VertexTex(x1, y, z1, u0, v1);
            b.VertexTex(x, y, z1, u1, v1);
        }

        if ((faces & Face.Front) == Face.Front)
        {
            var texIndex = tile.GetFaceTextureIndex(Face.Back);

            var u0 = (texIndex & 15) * TexFactor;
            var u1 = u0 + TexFactor;
            var v0 = (texIndex >> 4) * TexFactor;
            var v1 = v0 + TexFactor;

            SetBrightness(b, level, x, y, z + 1, Darker);

            b.Quad();

            b.VertexTex(x, y, z1, u0, v1);
            b.VertexTex(x1, y, z1, u1, v1);
            b.VertexTex(x1, y1, z1, u1, v0);
            b.VertexTex(x, y1, z1, u0, v0);
        }

        if ((faces & Face.Back) == Face.Back)
        {
            var texIndex = tile.GetFaceTextureIndex(Face.Back);

            var u0 = (texIndex & 15) * TexFactor;
            var u1 = u0 + TexFactor;
            var v0 = (texIndex >> 4) * TexFactor;
            var v1 = v0 + TexFactor;

            SetBrightness(b, level, x, y, z - 1, Darker);

            b.Quad();

            b.VertexTex(x, y, z, u1, v1);
            b.VertexTex(x, y1, z, u1, v0);
            b.VertexTex(x1, y1, z, u0, v0);
            b.VertexTex(x1, y, z, u0, v1);
        }
    }

    private static void SetBrightness(MeshBuilder b, Level.Level level, int x, int y, int z, float factor)
    {
        var brightness = level.GetBrightness(x, y, z) * factor;
        b.Color(brightness, brightness, brightness);
    }
    
    public static void RenderFace(TilePosition position, Face face)
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