using SharpCraft.Extensions;
using SharpCraft.World.Rendering;
using SharpCraft.Particles;
using SharpCraft.Registries;
using SharpCraft.Tiles;

namespace SharpCraft.Entities;

public sealed class Player(PlayerEntity entity, WorldRenderer worldRenderer, ParticleSystem particleSystem)
{
    public byte CurrentTile = 1;
    private RayCollision _rayCast;
    
    public void Update()
    {
        var mouseDelta = GetMouseDelta();
        entity.Rotate(mouseDelta.Y, -mouseDelta.X);
        
        _rayCast = entity.World.DoRayCast(entity.Camera.GetForwardRay(), 4.0f);
        
        if (IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point + _rayCast.Normal / 2;

            entity.World.TrySetTile(hitPoint, CurrentTile);
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
    }

    public void Draw()
    {
        worldRenderer.DrawHit(_rayCast);
    }
}