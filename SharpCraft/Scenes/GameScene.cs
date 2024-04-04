using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Level;
using SharpCraft.Level.Tiles;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

namespace SharpCraft.Scenes;

public class GameScene : IScene
{
    private const byte MaxTileId = 2;

    private Timer _timer;
    private Level.Level _level;
    private LevelRenderer _levelRenderer;
    private Player _player;
    private RayCollision _rayCast;

    private int _fps;
    private int _frames;
    private double _lastSecondTime;

    private byte _currentTile = 1;

    public GameScene()
    {
        DisableCursor();

        _timer = new Timer(60.0f);
        _level = new Level.Level(256, 64, 256);
        _levelRenderer = new LevelRenderer(_level);
        _player = new Player(_level);
    }

    public void Update()
    {
        _timer.Advance();
        for (var i = 0; i < _timer.Ticks; i++) TickedUpdate();
        FrameRateUpdate();
    }

    private void FrameRateUpdate()
    {
        _frames++;

        var time = GetTime();
        if (time - _lastSecondTime >= 1.0)
        {
            _fps = _frames;
            _frames = 0;

            Chunk.Updates = 0;

            _lastSecondTime = time;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        var mouseDelta = GetMouseDelta();
        _player.Rotate(mouseDelta.Y, mouseDelta.X);

        _rayCast = _level.DoRayCast(
            GetMouseRay(new Vector2(GetScreenWidth(), GetScreenHeight()) / 2, _player.Camera),
            4.0f);

        if (IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point + _rayCast.Normal / 2;

            _level.SetTile(hitPoint, _currentTile);
        }

        if (IsMouseButtonPressed(MouseButton.Right) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point - _rayCast.Normal / 2;

            _level.SetTile(hitPoint, 0);
        }

        var mouseScroll = GetMouseWheelMove();

        if (mouseScroll < 0) _currentTile = (byte)(_currentTile - 1 < 1 ? MaxTileId : _currentTile - 1);
        else if (mouseScroll > 0) _currentTile = (byte)(_currentTile + 1 > MaxTileId ? 1 : _currentTile + 1);

        if (IsKeyPressed(KeyboardKey.Enter)) _level.Save();

        if (IsKeyPressed(KeyboardKey.F11))
        {
            ToggleBorderlessWindowed();
        }
    }

    private void TickedUpdate()
    {
        _player.Tick();
    }

    public void Draw()
    {
        _player.MoveCamera(_timer.LastPassedTime);

        ClearBackground(ColorFromNormalized(new Vector4(0.5f, 0.8f, 1.0f, 1.0f)));

        BeginMode3D(_player.Camera);

        _levelRenderer.Draw();
        _levelRenderer.DrawHit(_rayCast);

        EndMode3D();

        DrawGui();
    }

    private void DrawGui()
    {
        DrawText($"{_fps} FPS, {Chunk.Updates} chunk updates", 0, 0, 24, Color.White);

        DrawRectangle(16 - 4, GetScreenHeight() - 96 - 16 - 4, 96 + 8, 96 + 8, Color.White);

        var textureIndex = TileRegistry.Tiles[_currentTile].TextureIndex;
        DrawTexturePro(
            ResourceManager.GetTexture("terrain.png"),
            new Rectangle(textureIndex % 16 * 16, textureIndex / 16 * 16, 16, 16),
            new Rectangle(16, GetScreenHeight() - 96 - 16, 96, 96),
            Vector2.Zero,
            0.0f,
            Color.White);

        DrawCrosshair(24, 24, 2);
    }

    private void DrawCrosshair(int crosshairWidth, int crosshairHeight, int crosshairThickness)
    {
        Rlgl.SetBlendMode(BlendMode.SubtractColors);

        // x0 -> x1
        DrawRectangle(
            GetScreenWidth() / 2 - crosshairWidth / 2,
            GetScreenHeight() / 2 - crosshairThickness / 2,
            crosshairWidth,
            crosshairThickness,
            Color.White);

        // y0 -> y0.4
        DrawRectangle(
            GetScreenWidth() / 2 - crosshairThickness / 2,
            GetScreenHeight() / 2 - crosshairHeight / 2,
            crosshairThickness,
            crosshairHeight / 2 - crosshairThickness / 2,
            Color.White);

        // y0.6 -> y1
        DrawRectangle(
            GetScreenWidth() / 2 - crosshairThickness / 2,
            GetScreenHeight() / 2 + crosshairThickness / 2,
            crosshairThickness,
            crosshairHeight / 2 - crosshairThickness / 2,
            Color.White);

        Rlgl.SetBlendMode(BlendMode.Alpha);
    }

    public void Dispose()
    {
        _level.Save();
    }
}