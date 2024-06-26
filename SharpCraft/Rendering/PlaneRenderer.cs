namespace SharpCraft.Rendering;

public static class PlaneRenderer
{
    private static void VertexTexCoord(float vx, float vy, float vz, float tx, float ty)
    {
        Rlgl.TexCoord2f(tx, ty);
        Rlgl.Vertex3f(vx, vy, vz);
    }
    
    public static void DrawLeftPlane(float y0, float z0, float y1, float z1, float x)
    {
        var ySize = y1 - y0;
        var zSize = z1 - z0;
        
        VertexTexCoord(x, y0, z0, 0.0f, ySize);
        VertexTexCoord(x, y0, z1, zSize, ySize);
        VertexTexCoord(x, y1, z1, zSize, 0.0f);
        VertexTexCoord(x, y1, z0, 0.0f, 0.0f);
    }
    
    public static void DrawRightPlane(float y0, float z0, float y1, float z1, float x)
    {
        var ySize = y1 - y0;
        var zSize = z1 - z0;
        
        VertexTexCoord(x, y0, z0, zSize, ySize);
        VertexTexCoord(x, y1, z0, zSize, 0.0f);
        VertexTexCoord(x, y1, z1, 0.0f, 0.0f);
        VertexTexCoord(x, y0, z1, 0.0f, ySize);
    }

    public static void DrawFrontPlane(float x0, float y0, float x1, float y1, float z)
    {
        var ySize = y1 - y0;
        var xSize = x1 - x0;
        
        VertexTexCoord(x0, y0, z, 0.0f, ySize);
        VertexTexCoord(x1, y0, z, xSize, ySize);
        VertexTexCoord(x1, y1, z, xSize, 0.0f);
        VertexTexCoord(x0, y1, z, 0.0f, 0.0f);
    }

    public static void DrawBackPlane(float x0, float y0, float x1, float y1, float z)
    {
        var ySize = y1 - y0;
        var xSize = x1 - x0;
        
        VertexTexCoord(x0, y0, z, xSize, ySize);
        VertexTexCoord(x0, y1, z, xSize, 0.0f);
        VertexTexCoord(x1, y1, z, 0.0f, 0.0f);
        VertexTexCoord(x1, y0, z, 0.0f, ySize);
    }
    
    public static void DrawTopPlane(float x0, float z0, float x1, float z1, float y)
    {
        var xSize = x1 - x0;
        var zSize = z1 - z0;
        
        VertexTexCoord(x0, y, z0, 0.0f, 0.0f);
        VertexTexCoord(x0, y, z1, 0.0f, zSize);
        VertexTexCoord(x1, y, z1, xSize, zSize);
        VertexTexCoord(x1, y, z0, xSize, 0.0f);
    }
}