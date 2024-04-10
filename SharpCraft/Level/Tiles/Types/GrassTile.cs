namespace SharpCraft.Level.Tiles.Types;

public class GrassTile(byte id) : Tile(id, 3, TileConfig.Default)
{
    protected override int GetTextureIndexForFace(Face face)
    {
        return face switch
        {
            Face.Top => 0,
            Face.Right or Face.Left or Face.Front or Face.Back => 3,
            _ => 2
        };
    }
}