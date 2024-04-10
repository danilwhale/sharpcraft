namespace SharpCraft.Level.Tiles.Types;

public class WoodTile(byte id) : Tile(id, 7, TileConfig.Default)
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