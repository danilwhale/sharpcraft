using System.Numerics;

namespace SharpCraft.Rendering.Models;

public readonly struct Cube
{
    private readonly Polygon[] _polygons;

    public Cube(Polygon[] polygons)
    {
        _polygons = polygons;
    }

    public Cube(Vector3 position, Vector3 size,
        Rectangle leftSource, Rectangle rightSource,
        Rectangle topSource, Rectangle bottomSource,
        Rectangle frontSource, Rectangle backSource)
    {
        _polygons =
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
                    leftSource.X + leftSource.Width,
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
                    position.Z + size.Z,
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
    }

    public void Draw()
    {
        foreach (var p in _polygons)
        {
            p.Draw();
        }
    }

    public Cube Move(float x, float y, float z)
    {
        return new Cube(_polygons.Select(p => p.Move(x, y, z)).ToArray());
    }

    public static Cube MakeTextureCube(
        Vector3 position, Vector3 size,
        int textureWidth, int textureHeight,
        int sourceX, int sourceY)
    {
        var textureSize = new Vector2(textureWidth, textureHeight);

        var leftSource = new Rectangle(
            sourceX + size.Z + size.X,
            sourceY + size.Z,
            size.Z,
            size.Y
        );

        var rightSource = new Rectangle(
            sourceX,
            sourceY + size.Z,
            size.Z,
            size.Y
        );

        var topSource = new Rectangle(
            sourceX + size.Z,
            sourceY,
            size.X,
            size.Z
        );

        var bottomSource = new Rectangle(
            sourceX + size.Z + size.X,
            sourceY,
            size.X,
            size.Z
        );

        var frontSource = new Rectangle(
            sourceX + size.Z + size.X + size.Z,
            sourceY + size.Z,
            size.X,
            size.Y
        );

        var backSource = new Rectangle(
            sourceX + size.Z,
            sourceY + size.Z,
            size.X,
            size.Y
        );

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