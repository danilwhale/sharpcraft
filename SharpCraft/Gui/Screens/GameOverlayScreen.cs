using System.Numerics;
using SharpCraft.Gui.Elements;
using SharpCraft.Level;

namespace SharpCraft.Gui.Screens;

public class GameOverlayScreen : Screen
{
    private TextElement _fpsText = new()
    {
        Size = 24.0f,
        Position = new Vector2(8.0f)
    };

    private CrosshairElement _crosshair = new(24, 24, 2);
    public BlockSelectionElement BlockSelection = new();
    
    private int _fps;
    private int _frames;
    private double _lastSecondTime;
    
    public GameOverlayScreen()
    {
        Elements.Add(_fpsText);
        Elements.Add(_crosshair);
        Elements.Add(BlockSelection);
    }
    
    public override void Update()
    {
        UpdateElements();
        
        _frames++;

        var time = GetTime();
        if (time - _lastSecondTime >= 1.0)
        {
            _fps = _frames;
            _frames = 0;

            Chunk.Updates = 0;

            _lastSecondTime = time;
        }

        _fpsText.Text = $"{_fps} FPS, {Chunk.Updates} chunk updates";
    }

    public override void Draw()
    {
        DrawElements();
    }
}