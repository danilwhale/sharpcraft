namespace SharpCraft.Tiles;

public readonly struct TileGroup(Tile[] tiles)
{
    public bool HasTile(byte tileId)
    {
        foreach (var t in tiles)
        {
            if (t.Id == tileId)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool HasTile(Tile? tile) => HasTile(tile?.Id ?? 0);
}