namespace SharpCraft.Entities;

public sealed class EntitySystem : IDisposable
{
    private readonly List<Entity> _entities = [];

    public void Add(Entity entity)
    {
        entity.System = this;
        _entities.Add(entity);
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

    public void Draw(float lastPartTicks)
    {
        foreach (var entity in _entities)
        {
            entity.Draw(lastPartTicks);
        }
    }

    public void Dispose()
    {
        foreach (var entity in _entities)
        {
            entity.Dispose();
        }
    }
}