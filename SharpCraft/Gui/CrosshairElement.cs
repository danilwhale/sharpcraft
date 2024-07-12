namespace SharpCraft.Gui;

public sealed class CrosshairElement : Element
{
    public override void Update()
    {
        if (System == null) return;

        Position.X = System.ViewWidth / 2.0f;
        Position.Y = System.ViewHeight / 2.0f;
    }

    public override void Draw()
    {
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.Color3f(1.0f, 1.0f, 1.0f);
        
        Rlgl.Vertex2i(1, -4);
        Rlgl.Vertex2i(0, -4);
        Rlgl.Vertex2i(0, 5);
        Rlgl.Vertex2i(1, 5);
        
        Rlgl.Vertex2i(5, 0);
        Rlgl.Vertex2i(-4, 0);
        Rlgl.Vertex2i(-4, 1);
        Rlgl.Vertex2i(5, 1);
        
        Rlgl.End();
    }
}