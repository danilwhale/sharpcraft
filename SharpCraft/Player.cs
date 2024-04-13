using SharpCraft.Entities;

namespace SharpCraft;

public class Player(Level.Level level)
{
    public readonly BlockEditor Editor = new(level, 4.0f);
    public readonly PlayerEntity Entity = new(level);

    public void Tick()
    {
        Entity.Tick();
    }

    public void Update()
    {
        Entity.Update();
        
        var mouseDelta = GetMouseDelta();
        Entity.Rotate(mouseDelta.Y, mouseDelta.X);
        
        Editor.Update(Entity);
    }

    public void Draw()
    {
        Editor.Draw();
    }
}