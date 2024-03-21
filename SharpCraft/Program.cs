global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Numerics;

namespace SharpCraft;

class Program
{
    private static Timer _timer;
    private static Level _level;
    private static LevelRenderer _levelRenderer;
    private static Player _player;
    private static RayCollision _rayCast;

    private static void Main()
    {
        InitWindow(1024, 768, "Minecraft");
        DisableCursor();

        Resources.Load();

        _timer = new Timer(60.0f);
        _level = new Level(256, 64, 256);
        _levelRenderer = new LevelRenderer(_level);
        _player = new Player(_level);

        while (!WindowShouldClose())
        {
            _timer.Advance();
            for (var i = 0; i < _timer.Ticks; i++) Tick();
            Update();

            BeginDrawing();
            Draw();

            EndDrawing();
        }

        _level.Save();

        ResourceManager.Unload();
        Resources.Unload();

        CloseWindow();
    }

    private static void Draw()
    {
        _player.MoveCamera(_timer.LastPassedTime);

        ClearBackground(ColorFromNormalized(new Vector4(0.5f, 0.8f, 1.0f, 1.0f)));
        
        BeginMode3D(_player.Camera);

        _levelRenderer.Draw();
        _levelRenderer.DrawHit(_rayCast);

        EndMode3D();

        DrawFPS(0, 0);
    }

    private static void Update()
    {
        var mouseDelta = GetMouseDelta();
        _player.Rotate(mouseDelta.Y, mouseDelta.X);
        
        _rayCast = _level.DoRayCast(
            GetMouseRay(new Vector2(GetScreenWidth(), GetScreenHeight()) / 2, _player.Camera),
            4.0f);
        
        if (IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point + _rayCast.Normal / 2;

            _level.SetTile(hitPoint, 1);
        }

        if (IsMouseButtonPressed(MouseButton.Right) && _rayCast.Hit)
        {
            var hitPoint = _rayCast.Point - _rayCast.Normal / 2;

            _level.SetTile(hitPoint, 0);
        }

        if (IsKeyPressed(KeyboardKey.Enter)) _level.Save();
    }

    private static void Tick()
    {
        _player.Tick();
    }
}