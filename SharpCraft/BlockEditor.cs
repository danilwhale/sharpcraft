using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Gui.Elements;
using SharpCraft.Level.Tiles;

namespace SharpCraft;

public class BlockEditor(Level.Level level, float maxHitDistance)
{
    private static readonly int MaxTileId = TileRegistry.GetNonNullTileCount();

    public BlockSelectionElement SelectionElement;
    public byte CurrentTile = 1;
    
    private RayCollision _rayCast;

    public void Update(PlayerEntity playerEntity)
    {
        var mouseScroll = GetMouseWheelMove();

        if (mouseScroll > 0)
        {
            CurrentTile = (byte)(CurrentTile - 1 < 1 ? MaxTileId : CurrentTile - 1);
        }
        else if (mouseScroll < 0)
        {
            CurrentTile = (byte)(CurrentTile + 1 > MaxTileId ? 1 : CurrentTile + 1);
        }
        
        SelectionElement.CurrentTile = CurrentTile;
        
        _rayCast = level.DoRayCast(
            GetMouseRay(new Vector2(GetScreenWidth(), GetScreenHeight()) / 2, playerEntity.Camera),
            maxHitDistance);
        
        HandleInput();
    }

    private void HandleInput()
    {
        if (IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point + _rayCast.Normal / 2;

            level.SetTile(hitPoint, CurrentTile);
        }

        if (IsMouseButtonPressed(MouseButton.Right) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point - _rayCast.Normal / 2;

            level.SetTile(hitPoint, 0);
        }
    }

    public void Draw()
    {
        if (!_rayCast.Hit) return;
        
        var position = (TilePosition)(_rayCast.Point - _rayCast.Normal / 2.0f);

        var id = level.GetTile(position);
        var tile = TileRegistry.Tiles[id];
        if (tile == null) return;

        Rlgl.DisableDepthTest();

        var collision = tile.GetCollision(position.X, position.Y, position.Z);
        var size = collision.Max - collision.Min;
        DrawCubeWiresV(collision.Min + size / 2, size, Color.Black);
        
        Rlgl.EnableDepthTest();
    }
}