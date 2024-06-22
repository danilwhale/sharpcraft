namespace SharpCraft.Level.Blocks.Types;

public class GrassBlock(byte id) : Block(id, 3, BlockConfig.Default)
{
    public override int GetTextureIndexForFace(Face face)
    {
        return face switch
        {
            Face.Top => 0,
            Face.Right or Face.Left or Face.Front or Face.Back => 3,
            _ => 2
        };
    }
}