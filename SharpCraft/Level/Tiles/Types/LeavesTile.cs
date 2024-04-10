namespace SharpCraft.Level.Tiles.Types;

public class LeavesTile(byte id, int textureIndex) : 
    Tile(id, textureIndex, new TileConfig(IsLightBlocker: false, Layer: TileLayer.Translucent))
{
    protected override bool ShouldKeepFace(Level level, int x, int y, int z, Face face)
    {
        return true;
    }
}