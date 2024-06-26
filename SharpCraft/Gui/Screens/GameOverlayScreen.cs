using System.Diagnostics;
using System.Numerics;
using SharpCraft.Gui.Elements;
using SharpCraft.Level;

namespace SharpCraft.Gui.Screens;

public class GameOverlayScreen : Screen
{
    private const double MegaByte = 1024.0 * 1024.0;

    private TextElement _debugText = new()
    {
        Size = 24.0f,
        Position = new Vector2(8.0f)
    };

    private CrosshairElement _crosshair = new(24, 24, 2);
    public BlockSelectionElement BlockSelection = new();

    private int _fps;
    private int _frames;
    private double _lastSecondTime;
    private readonly Player _player;

    public GameOverlayScreen(Player player)
    {
        Elements.Add(_debugText);
        Elements.Add(_crosshair);
        Elements.Add(BlockSelection);
        _player = player;
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

        var process = Process.GetCurrentProcess();

        _debugText.Text = $"{_fps} FPS\n" +
                          $"{Chunk.Updates} chunk updates\n" +
                          $"Memory:\n" +
                          $"- Heap: {GC.GetTotalMemory(false) / MegaByte:0.###} MB\n" +
                          $"- Process: {process.PrivateMemorySize64 / MegaByte:0.###}/{process.WorkingSet64 / MegaByte:0.###} MB\n" +
                          $"XYZ: {_player.Entity.Camera.Position:#.###}";
    }

    public override void Draw()
    {
        DrawElements();
    }
}