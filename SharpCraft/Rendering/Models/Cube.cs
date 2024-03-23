using System.Numerics;

namespace SharpCraft.Rendering.Models;

public readonly struct Cube(Vector3 position, Vector3 size, 
    Rectangle leftSource, Rectangle rightSource,
    Rectangle topSource, Rectangle bottomSource,
    Rectangle frontSource, Rectangle backSource)
{
    private readonly Polygon[] _polygons =
    [
        // left
        new Polygon(
            new Vertex(
                position.X, 
                position.Y, 
                position.Z, 
                leftSource.X, 
                leftSource.Y + leftSource.Height),
            new Vertex(
                position.X,
                position.Y, 
                position.Z + size.Z, 
                leftSource.X + leftSource.Width, 
                leftSource.Y + leftSource.Height),
            new Vertex(
                position.X, 
                position.Y + size.Y, 
                position.Z + size.Z, 
                leftSource.X + leftSource.Height, 
                leftSource.Y),
            new Vertex(
                position.X, 
                position.Y + size.Y,
                position.Z, 
                leftSource.X, 
                leftSource.Y)
            ),
        
        // right
        new Polygon(
            new Vertex(
                position.X + size.X,
                position.Y, 
                position.Z,
                rightSource.X + rightSource.Width, 
                rightSource.Y + rightSource.Height),
            new Vertex(
                position.X + size.X, 
                position.Y + size.Y, 
                position.Z, 
                rightSource.X + rightSource.Width, 
                rightSource.Y),
            new Vertex(
                position.X + size.X, 
                position.Y + size.Y, 
                position.Z + size.Z, 
                rightSource.X, 
                rightSource.Y),
            new Vertex(
                position.X + size.X, 
                position.Y, 
                position.Z + size.X, 
                rightSource.X, 
                rightSource.Y + rightSource.Height)
        ),
        
        // top
        new Polygon(
            new Vertex(
                position.X, 
                position.Y + size.Y,
                position.Z, 
                topSource.X, 
                topSource.Y),
            new Vertex(
                position.X, 
                position.Y + size.Y, 
                position.Z + size.Z, 
                topSource.X, 
                topSource.Y + topSource.Height),
            new Vertex(
                position.X + size.X, 
                position.Y + size.Y, 
                position.Z + size.Z, 
                topSource.X + topSource.Width, 
                topSource.Y + topSource.Height),
            new Vertex(
                position.X + size.X, 
                position.Y + size.Y, 
                position.Z, 
                topSource.X + topSource.Width, 
                topSource.Y)
        ),
        
        // bottom
        new Polygon(
            new Vertex(
                position.X, 
                position.Y, 
                position.Z, 
                bottomSource.X + backSource.Width, 
                bottomSource.Y),
            new Vertex(
                position.X + size.X,
                position.Y, 
                position.Z, 
                bottomSource.X, 
                bottomSource.Y),
            new Vertex(
                position.X + size.X,
                position.Y, 
                position.Z + size.Z, 
                bottomSource.X, 
                bottomSource.Y + bottomSource.Height),
            new Vertex(
                position.X,
                position.Y, 
                position.Z + size.Z, 
                bottomSource.X + backSource.Width, 
                bottomSource.Y + bottomSource.Height)
        ),
        
        // front
        new Polygon(
            new Vertex(
                position.X, 
                position.Y, 
                position.Z + size.Z, 
                frontSource.X, 
                frontSource.Y + frontSource.Height),
            new Vertex(
                position.X + size.X,
                position.Y, 
                position.Z + size.Z, 
                frontSource.X + frontSource.Width, 
                frontSource.Y + frontSource.Height),
            new Vertex(
                position.X + size.X, 
                position.Y + size.Y, 
                position.Z + size.Z, 
                frontSource.X + frontSource.Width, 
                frontSource.Y),
            new Vertex(
                position.X, 
                position.Y + size.Y, 
                position.Z + size.Z, 
                frontSource.X, 
                frontSource.Y)
        ),
        
        // back
        new Polygon(
            new Vertex(
                position.X, 
                position.Y, 
                position.Z, 
                backSource.X + backSource.Width, 
                backSource.Y + backSource.Height),
            new Vertex(
                position.X, 
                position.Y + size.Y,
                position.Z, 
                backSource.X + backSource.Width, 
                backSource.Y),
            new Vertex(
                position.X + size.X, 
                position.Y + size.Y,
                position.Z, backSource.X,
                backSource.Y),
            new Vertex(
                position.X + size.X,
                position.Y, 
                position.Z, 
                backSource.X, 
                backSource.Y + backSource.Height)
        ),
    ];

    public void Add(MeshBuilder builder)
    {
        foreach (var p in _polygons)
        {
            p.Add(builder);
        }
    }

    public static Cube MakeTextureCube(
        Vector3 position, Vector3 size,
        int textureWidth, int textureHeight,
        Rectangle leftSource, Rectangle rightSource,
        Rectangle topSource, Rectangle bottomSource,
        Rectangle frontSource, Rectangle backSource)
    {
        var textureSize = new Vector2(textureWidth, textureHeight);
        
        leftSource.Position /= textureSize;
        leftSource.Size /= textureSize;

        rightSource.Position /= textureSize;
        rightSource.Size /= textureSize;

        topSource.Position /= textureSize;
        topSource.Size /= textureSize;

        bottomSource.Position /= textureSize;
        bottomSource.Size /= textureSize;

        frontSource.Position /= textureSize;
        frontSource.Size /= textureSize;

        backSource.Position /= textureSize;
        backSource.Size /= textureSize;

        return new Cube(position, size, leftSource, rightSource, topSource, bottomSource, frontSource, backSource);
    }
}