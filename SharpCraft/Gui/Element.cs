using System.Numerics;

namespace SharpCraft.Gui;

public abstract class Element
{
    public Vector2 Position;
    public ElementSystem? System;

    public abstract void Update();
    public abstract void Draw();
}