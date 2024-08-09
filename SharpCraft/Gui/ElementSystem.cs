using System.Numerics;

namespace SharpCraft.Gui;

public sealed class ElementSystem
{
    public float ViewWidth;
    public float ViewHeight;
    
    private readonly List<Element> _elements = [];

    public ElementSystem()
    {
        UpdateViewSize();
    }

    public void Add(Element element)
    {
        _elements.Add(element);
        element.System = this;
    }

    public void Update()
    {
        if (IsWindowResized())
        {
            UpdateViewSize();
        }
        
        foreach (var element in _elements)
        {
            element.Update();
        }
    }

    private void UpdateViewSize()
    {
        CalculateViewSize(out ViewWidth, out ViewHeight);
    }
    
    public void Draw()
    {
        Rlgl.MatrixMode(MatrixMode.Projection);
        Rlgl.PushMatrix();
        Rlgl.LoadIdentity();
        Rlgl.Ortho(0.0, ViewWidth, ViewHeight, 0.0, 0.01, 1000.0);
        
        Rlgl.MatrixMode(MatrixMode.ModelView);
        Rlgl.LoadIdentity();
        Rlgl.Translatef(0.0f, 0.0f, -200.0f);
        
        foreach (var element in _elements)
        {
            Rlgl.PushMatrix();
            Rlgl.Translatef(element.Position.X, element.Position.Y, 0.0f);
            
            element.Draw();
            
            Rlgl.PopMatrix();
        }
        
        Rlgl.PopMatrix();
        
        Rlgl.MatrixMode(MatrixMode.Projection);
        Rlgl.PopMatrix();
    }
    
    public static void CalculateViewSize(out float width, out float height)
    {
        var screenWidth = GetScreenWidth();
        var screenHeight = GetScreenHeight();

        width = screenWidth * 240.0f / screenHeight;
        height = screenHeight * 240.0f / screenHeight;
    }
}