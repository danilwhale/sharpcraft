namespace SharpCraft.Level.Tiles.Types;

public class LeavesTile(byte id) : Tile(id, 9)
{
    public override bool IsLightBlocker()
    {
        return false;
    }
}