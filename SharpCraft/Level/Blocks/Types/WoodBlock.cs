namespace SharpCraft.Level.Blocks.Types;

public class WoodBlock(byte id) : Block(id, 7, BlockConfig.Default)
{
    protected override int GetTextureIndexForFace(Face face)
    {
        return face switch
        {
            Face.Top or Face.Bottom => 8,
            _ => 7
        };
    }
}