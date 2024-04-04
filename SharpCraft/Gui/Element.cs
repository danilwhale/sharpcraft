using System.Numerics;

namespace SharpCraft.Gui;

public abstract class Element
{
    public Vector2 Position;

    public abstract void Update();
    public abstract void Draw();
}