using SharpCraft.Level.Tiles;

namespace SharpCraft.Level.Generation.Structures;

public class Structure
{
    public readonly int Width;
    public readonly int Height;
    public readonly int Length;

    protected readonly byte[] Tiles;

    protected Structure(int width, int height, int length)
    {
        Tiles = new byte[width * height * length];
        Width = width;
        Height = height;
        Length = length;
    }

    protected void WriteTiles(params byte[][][] tiles)
    {
        for (var y = 0; y < tiles.Length; y++)
        {
            for (var x = 0; x < tiles[y].Length; x++)
            {
                for (var z = 0; z < tiles[y][x].Length; z++)
                {
                    Tiles[(y * Length + z) * Width + x] = tiles[y][x][z];
                }
            }
        }
    }

    public void Place(Level level, int x, int y, int z)
    {
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                for (var k = 0; k < Length; k++)
                {
                    var id = Tiles[(j * Length + k) * Width + i];
                    if (id == 0) continue;

                    level.SetTile(x + i, y + j, z + k, id, false);
                }
            }
        }
    }
}