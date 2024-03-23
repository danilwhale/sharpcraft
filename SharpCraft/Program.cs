global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpCraft.Level;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    private static Timer _timer = null!;
    private static Level.Level _level = null!;
    private static LevelRenderer _levelRenderer = null!;
    private static Player _player = null!;
    private static RayCollision _rayCast;

    private static int _fps;
    private static int _frames;
    private static double _lastSecondTime;

    private static void Main()
    {
        InitWindow(1024, 768, "Minecraft");
        DisableCursor();

        Resources.Load();

        _timer = new Timer(60.0f);
        _level = new Level.Level(256, 64, 256);
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

        DrawText($"{_fps} FPS, {Chunk.Updates} chunk updates", 0, 0, 24, Color.White);
    }

    private static void Update()
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