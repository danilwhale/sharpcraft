namespace SharpCraft.Tiles;

public readonly struct TileGroup(Tile[] tiles)
{
    public bool HasTile(byte tileId)
    {
        // ignore linq, it's too slow
        for (var i = 0; i < tiles.Length; i++)
        {
            var otherTileId = tiles[i].Id;
            if (otherTileId == tileId)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool HasTile(Tile? tile) => HasTile(tile?.Id ?? 0);
}