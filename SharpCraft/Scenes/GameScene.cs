using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Gui;
using SharpCraft.World.Rendering;
using SharpCraft.Particles;
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
    
    private int _fps;
    private int _frames;
    private double _lastSecondTime;
    
    public GameScene()
    {
        DisableCursor();

        _timer = new Timer(20.0f);
        
        _entitySystem = new EntitySystem();
        _particleSystem = new ParticleSystem();
        _elementSystem = new ElementSystem();
        
        _world = new World.World(256, 64, 256);
        _worldRenderer = new WorldRenderer(_world);
        
        _playerEntity = new PlayerEntity(_world);
        _player = new Player(_playerEntity, _worldRenderer, _particleSystem);
        _entitySystem.Add(_playerEntity);

        for (var i = 0; i < 100; i++)
        {
            var zombie = new ZombieEntity(_world, new Vector3(128.0f, 0.0f, 128.0f));
            zombie.SetRandomLevelPosition();
            _entitySystem.Add(zombie);
        }
        
        _elementSystem.Add(new TilePreviewElement(_player));
        _elementSystem.Add(new CrosshairElement());

        Assets.SetMaterialShader("terrain.png", LoadShaderFromMemory(null, Assets.GetText("Terrain.fsh")));
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

            Chunklet.Updates = 0;

            _lastSecondTime = time;
        }
        
        HandleInput();
        _player.Update();
        
        _elementSystem.Update();
    }

    private void HandleInput()
    {
        if (IsKeyPressed(KeyboardKey.Enter))
        {
            _world.Save();
        }

        if (IsKeyPressed(KeyboardKey.F11))
        {
            ToggleBorderlessWindowed();
        }
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
        
        _worldRenderer.Draw(RenderLayer.Solid);
        
        _entitySystem.Draw(_timer.LastPartialTicks);
        _particleSystem.Draw(_playerEntity, _timer.LastPartialTicks);
        
        _worldRenderer.Draw(RenderLayer.Translucent);
        
        _player.Draw();

        EndMode3D();
        
        _elementSystem.Draw();

        DrawText($"{_fps} FPS, {Chunklet.Updates} chunklet updates", 0, 0, 11, Color.White);
    }
    
    public void Dispose()
    {
        _world.Save();
        _worldRenderer.Dispose();
        _entitySystem.Dispose();
    }
}