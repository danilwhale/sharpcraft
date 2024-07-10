using SharpCraft.Extensions;
using SharpCraft.World.Rendering;
using SharpCraft.Particles;
using SharpCraft.Registries;
using SharpCraft.Tiles;

namespace SharpCraft.Entities;

public sealed class Player(PlayerEntity entity, WorldRenderer worldRenderer, ParticleSystem particleSystem)
{
    private byte _currentTile = 1;
    private RayCollision _rayCast;
    
    public void Update()
    {
        var mouseDelta = GetMouseDelta();
        entity.Rotate(mouseDelta.Y, -mouseDelta.X);
        
        _rayCast = entity.World.DoRayCast(entity.Camera.GetForwardRay(), 4.0f);
        
        if (IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point + _rayCast.Normal / 2;

            entity.World.TrySetTile(hitPoint, _currentTile);
        }

        if (IsMouseButtonPressed(MouseButton.Right) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point - _rayCast.Normal / 2;

            var oldTile = entity.World.GetTile(hitPoint);
            if (entity.World.TrySetTile(hitPoint, 0))
            {
                var position = (TilePosition)hitPoint;
                Registries.Tiles.Registry[oldTile]?.Break(entity.World, position.X, position.Y, position.Z, particleSystem);
            }
        }
        
        if (IsKeyPressed(KeyboardKey.One)) _currentTile = 1;
        if (IsKeyPressed(KeyboardKey.Two)) _currentTile = 2;
        if (IsKeyPressed(KeyboardKey.Three)) _currentTile = 3;
        if (IsKeyPressed(KeyboardKey.Four)) _currentTile = 4;
        if (IsKeyPressed(KeyboardKey.Five)) _currentTile = 5;
        
        // hide silly sapling texture
        if (IsKeyPressed(KeyboardKey.Seven)) _currentTile = 6;

        if (IsKeyPressed(KeyboardKey.G))
        {
            entity.System?.Add(new ZombieEntity(entity.World, entity.Position));
        }
    }

    public void Draw()
    {
        worldRenderer.DrawHit(_rayCast);
    }
}