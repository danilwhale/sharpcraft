using SharpCraft.Utilities;

namespace SharpCraft.Level.Generation.Structures;

public class Structure
{
    public readonly int Width;
    public readonly int Height;
    public readonly int Length;

    protected readonly byte[] Blocks;

    protected Structure(int width, int height, int length)
    {
        Blocks = new byte[width * height * length];
        Width = width;
        Height = height;
        Length = length;
    }

    protected void WriteBlocks(byte[][][] blocks)
    {
        for (var y = 0; y < blocks.Length; y++)
        {
            for (var x = 0; x < blocks[y].Length; x++)
            {
                for (var z = 0; z < blocks[y][x].Length; z++)
                {
                    Blocks[(y * Length + z) * Width + x] = blocks[y][x][z];
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
                    var id = Blocks.GetUnsafeRef((j * Length + k) * Width + i);
                    if (id == 0) continue;

                    level.SetBlock(x + i, y + j, z + k, id, false);
                }
            }
        }
    }
}