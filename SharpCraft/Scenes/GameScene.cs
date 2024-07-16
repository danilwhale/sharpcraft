﻿using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Gui;
using SharpCraft.World.Rendering;
using SharpCraft.Particles;
using SharpCraft.Rendering;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

namespace SharpCraft.Scenes;

public sealed class GameScene : IScene
{
    private readonly Timer _timer;

    private readonly World.World _world;
    private readonly WorldRenderer _worldRenderer;

    private readonly PlayerEntity _playerEntity;
    private readonly Player _player;

    private readonly EntitySystem _entitySystem;
    private readonly ParticleSystem _particleSystem;
    private readonly ElementSystem _elementSystem;

    private int _frames;
    private double _lastSecondTime;

    private TextElement _statsText;

    private bool _isMouseLocked;

    public GameScene()
    {
        LockMouse();

        _timer = new Timer(20.0f);

        _entitySystem = new EntitySystem();
        _particleSystem = new ParticleSystem();
        _elementSystem = new ElementSystem();

        _world = new World.World(256, 64, 256);
        _worldRenderer = new WorldRenderer(_world);

        _playerEntity = new PlayerEntity(_world);
        _player = new Player(_playerEntity, _worldRenderer, _particleSystem);
        _entitySystem.Add(_playerEntity);

        for (var i = 0; i < 10; i++)
        {
            var zombie = new ZombieEntity(_world, new Vector3(128.0f, 0.0f, 128.0f));
            zombie.SetRandomLevelPosition();
            _entitySystem.Add(zombie);
        }

        _elementSystem.Add(new TilePreviewElement(_player));
        _elementSystem.Add(new CrosshairElement());
        _elementSystem.Add(new TextElement
        {
            Text = Program.Version, 
            Position = new Vector2(2.0f, 2.0f), 
            DropShadow = true
        });
        _elementSystem.Add(_statsText = new TextElement
        {
            Position = new Vector2(2.0f, 12.0f),
            DropShadow = true
        });

        Assets.SetMaterialShader("char.png", WorldShader.Shader);
        Assets.SetMaterialShader("terrain.png", WorldShader.ChunkShader);
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
            _statsText.Text = $"{_frames} fps, {Chunklet.Updates} chunk updates";
            
            _frames = 0;
            Chunklet.Updates = 0;

            _lastSecondTime = time;
        }

        HandleInput();
        _player.Update();
        
        _elementSystem.Update();
    }

    private void HandleInput()
    {
        if (!IsWindowFocused() && _isMouseLocked) UnlockMouse();
        
        if (IsKeyPressed(KeyboardKey.Escape))
        {
            if (_isMouseLocked) UnlockMouse();
            else LockMouse();
        }
        
        if (IsKeyPressed(KeyboardKey.Enter))
        {
            _world.Save();
        }

        if (IsKeyPressed(KeyboardKey.F11))
        {
            ToggleBorderlessWindowed();
        }
        
        if (_isMouseLocked) _player.Rotate();
    }

    private void LockMouse()
    {
        DisableCursor();
        _isMouseLocked = true;
    }

    private void UnlockMouse()
    {
        EnableCursor();
        _isMouseLocked = false;
    }

    private void TickedUpdate()
    {
        _world.Tick();
        _entitySystem.Tick();
        _particleSystem.Tick();
    }

    public void Draw()
    {
        _playerEntity.MoveCamera(_timer.LastPartialTicks);

        ClearBackground(new Color(128, 204, 255, 255));

        BeginMode3D(_playerEntity.Camera);

        _worldRenderer.UpdateDirtyChunks();

        var frustum = Frustum.Instance;

        BeginShaderMode(WorldShader.Shader);

        WorldShader.SetIsLit(true);
        _worldRenderer.Draw(RenderLayer.Lit);
        _entitySystem.Draw(_timer.LastPartialTicks, frustum, RenderLayer.Lit);
        _particleSystem.Draw(_playerEntity, _timer.LastPartialTicks, RenderLayer.Lit);

        EndShaderMode();

        BeginShaderMode(WorldShader.Shader);

        WorldShader.SetIsLit(false);
        _worldRenderer.Draw(RenderLayer.Shadow);
        _entitySystem.Draw(_timer.LastPartialTicks, frustum, RenderLayer.Shadow);
        _particleSystem.Draw(_playerEntity, _timer.LastPartialTicks, RenderLayer.Shadow);

        EndShaderMode();

        _player.Draw();

        EndMode3D();

        _elementSystem.Draw();
    }

    public void Dispose()
    {
        _world.Save();
        _worldRenderer.Dispose();
        _entitySystem.Dispose();
    }
}