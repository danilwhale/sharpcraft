using SharpCraft.Rendering;
using SharpCraft.World.Rendering;

namespace SharpCraft.Entities;

public sealed class EntitySystem : IDisposable
{
    private readonly List<Entity> _entities = [];

    public void Add(Entity entity)
    {
        _entities.Add(entity);
        entity.System = this;
    }

    public IEnumerable<T> OfType<T>() where T : Entity
    {
        return _entities.OfType<T>();
    }

    public void Tick()
    {
        for (var i = 0; i < _entities.Count; i++)
        {
            var entity = _entities[i];
            entity.Tick();

            if (!entity.IsDestroyed) continue;
            _entities.Remove(entity);
            i--;
        }
    }

    public void Draw(float lastPartTicks, Frustum frustum, RenderLayer layer)
    {
        foreach (var entity in _entities)
        {
            if (frustum.IsBoxOutside(entity.Box))
            {
                continue;
            }
            
            if (entity.IsLit() ^ layer == RenderLayer.Lit)
            {
                continue;
            }
            
            entity.Draw(lastPartTicks);
        }
    }

    public bool IsAreaFree(BoundingBox box)
    {
        return !_entities.Any(e =>
        {
            if (!(box.Max.X > e.Box.Min.X && box.Min.X < e.Box.Max.X)) return false;
            if (!(box.Max.Y > e.Box.Min.Y && box.Min.Y < e.Box.Max.Y)) return false;
            if (!(box.Max.Z > e.Box.Min.Z && box.Min.Z < e.Box.Max.Z)) return false;

            return true;
        });
    }

    public void Dispose()
    {
        foreach (var entity in _entities)
        {
            entity.Dispose();
        }
    }
}