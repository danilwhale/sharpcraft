using System.Numerics;
using SharpCraft.Extensions;
using SharpCraft.World.Rendering;
using SharpCraft.Particles;
using SharpCraft.Tiles;
using SharpCraft.World;

namespace SharpCraft.Entities;

public sealed class PlayerController(PlayerEntity entity, WorldRenderer worldRenderer, ParticleSystem particleSystem)
{
    public byte CurrentTile = 1;
    private RayCollision _rayCast;
    private int _pitchFactor = 1;
    private EditMode _editMode = EditMode.Remove;
    
    public void Update()
    {
        _rayCast = entity.World.DoRayCast(entity.Camera.GetForwardRay(), 4.0f);
        
        HandleMouseInput();
        HandleInput();
    }

    private void HandleMouseInput()
    {
        if (!IsCursorHidden()) return;
        
        var mouseDelta = GetMouseDelta();
        entity.Rotate(mouseDelta.Y * _pitchFactor, -mouseDelta.X);
        
        if (IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        {
            TilePosition hitPoint;
            
            switch (_editMode)
            {
                case EditMode.Remove:
                    hitPoint = _rayCast.Point - _rayCast.Normal / 2;

                    var oldTile = entity.World.GetTile(hitPoint);
                    if (entity.World.TrySetTile(hitPoint, 0))
                    {
                        Registries.Tiles.Registry[oldTile]?.Break(entity.World, hitPoint.X, hitPoint.Y, hitPoint.Z,
                            particleSystem);
                    }
                    
                    break;
                
                case EditMode.Place:
                    hitPoint = _rayCast.Point + _rayCast.Normal / 2;

                    var tile = Registries.Tiles.Registry[CurrentTile];
                    var box = tile!.GetBox(hitPoint.X, hitPoint.Y, hitPoint.Z);
                    if (!entity.System?.IsAreaFree(box) ?? false)
                    {
                        break;
                    }

                    entity.World.TrySetTile(hitPoint, CurrentTile);
                    
                    break;
            }
        }

        if (IsMouseButtonPressed(MouseButton.Right))
        {
            _editMode = _editMode == EditMode.Remove ? EditMode.Place : EditMode.Remove;
        }
    }

    private void HandleInput()
    {
        if (IsKeyPressed(KeyboardKey.One)) CurrentTile = Registries.Tiles.Rock.Id;
        if (IsKeyPressed(KeyboardKey.Two)) CurrentTile = Registries.Tiles.Dirt.Id;
        if (IsKeyPressed(KeyboardKey.Three)) CurrentTile = Registries.Tiles.Stone.Id;
        if (IsKeyPressed(KeyboardKey.Four)) CurrentTile = Registries.Tiles.Wood.Id;

        // hide silly sapling texture
        if (IsKeyPressed(KeyboardKey.Six)) CurrentTile = Registries.Tiles.Bush.Id;

        if (IsKeyPressed(KeyboardKey.G))
        {
            entity.System?.Add(new ZombieEntity(entity.World, entity.Position));
        }

        if (IsKeyPressed(KeyboardKey.Y))
        {
            _pitchFactor *= -1;
        }
    }

    public void Draw()
    {
        worldRenderer.DrawHit(_rayCast, _editMode, CurrentTile);
    }
}