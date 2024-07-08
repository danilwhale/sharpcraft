using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Extensions;
using SharpCraft.Level;
using SharpCraft.Rendering.Models;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

namespace SharpCraft.Scenes;

public sealed class GameScene : IScene
{
    private readonly Timer _timer;
    private readonly Level.Level _level;
    private readonly LevelRenderer _levelRenderer;
    private readonly Player _player;
    private RayCollision _rayCast;
    private readonly List<Zombie> _zombies = new(100);

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

        for (var i = 0; i < _zombies.Capacity; i++)
        {
            _zombies.Add(new Zombie(_level));
        }

        var material = Assets.GetTextureMaterial("terrain.png");
        material.Shader = LoadShaderFromMemory(null, Assets.GetText("Terrain.fsh"));
        Assets.SetMaterial("terrain", material);
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
        _player.Rotate(mouseDelta.Y, -mouseDelta.X);
        
        _rayCast = _level.DoRayCast(_player.Camera.GetForwardRay(), 4.0f);
        
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

        if (IsKeyPressed(KeyboardKey.Enter))
        {
            _level.Save();
        }

        if (IsKeyPressed(KeyboardKey.F11))
        {
            ToggleBorderlessWindowed();
        }

        if (IsKeyPressed(KeyboardKey.One)) _currentTile = 1;
        if (IsKeyPressed(KeyboardKey.Two)) _currentTile = 2;
        if (IsKeyPressed(KeyboardKey.Three)) _currentTile = 3;
        if (IsKeyPressed(KeyboardKey.Four)) _currentTile = 4;
        if (IsKeyPressed(KeyboardKey.Five)) _currentTile = 5;
        
        // hide silly sapling texture
        if (IsKeyPressed(KeyboardKey.Seven)) _currentTile = 6;
    }

    private void TickedUpdate()
    {
        _player.Tick();
        _level.Tick();

        foreach (var zombie in _zombies)
        {
            zombie.Tick();
        }
    }

    public void Draw()
    {
        _player.MoveCamera(_timer.LastPartialTicks);

        ClearBackground(new Color(128, 204, 255, 255));
        
        BeginMode3D(_player.Camera);

        _levelRenderer.Draw(ChunkLayer.Solid);
        
        foreach (var zombie in _zombies)
        {
            zombie.Draw(_timer.LastPartialTicks);
        }
        
        _levelRenderer.Draw(ChunkLayer.Translucent);
        
        _levelRenderer.DrawHit(_rayCast);

        EndMode3D();

        DrawText($"{_fps} FPS, {Chunk.Updates} chunk updates", 0, 0, 24, Color.White);
    }
    
    public void Dispose()
    {
        _level.Save();
        _levelRenderer.Dispose();

        foreach (var zombie in _zombies)
        {
            zombie.Dispose();
        }
    }
}