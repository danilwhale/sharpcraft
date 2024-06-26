using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Gui.Screens;
using SharpCraft.Level;
using SharpCraft.Level.Blocks;
using SharpCraft.Rendering;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

namespace SharpCraft.Scenes;

public class GameScene : IScene
{
    private Timer _timer;
    private Level.Level _level;
    private LevelRenderer _levelRenderer;
    private Player _player;

    private GameOverlayScreen _gameScreen;
    private PauseScreen _pauseScreen;
    private bool _paused;

    public GameScene()
    {
        DisableCursor();

        _timer = new Timer(60.0f);
        _level = new Level.Level(256, 64, 256);
        _levelRenderer = new LevelRenderer(_level);
        _player = new Player(_level);

        _gameScreen = new GameOverlayScreen(_player);
        _pauseScreen = new PauseScreen();
        _player.Editor.SelectionElement = _gameScreen.BlockSelection;
    }

    public void Update()
    {
        if (!IsWindowFocused() && !_paused) SetPause(true);
        
        if (IsKeyPressed(KeyboardKey.Escape))
        {
            SetPause(!_paused);
        }
        
        if (!_paused)
        {
            FrameRateUpdate();
        }
        
        _timer.Advance();
        for (var i = 0; i < _timer.Ticks; i++) TickedUpdate();
        
        if (_paused) _pauseScreen.Update();
        else _gameScreen.Update();
    }

    private void SetPause(bool pause)
    {
        _paused = pause;
        
        if (_paused)
        {
            EnableCursor();
            SetTargetFPS(15);
            _timer.TimeScale = 0.0f;
        }
        else
        {
            DisableCursor();
            SetTargetFPS(0);
            _timer.TimeScale = 1.0f;
        }
    }

    private void FrameRateUpdate()
    {
        HandleInput();
        _player.Update();
        ChunkShader.Update(_player);
    }

    private void HandleInput()
    {
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
        _player.Entity.MoveCamera(_timer.LastPassedTime);

        ClearBackground(ColorFromNormalized(new Vector4(0.5f, 0.8f, 1.0f, 1.0f)));

        BeginMode3D(_player.Entity.Camera);

        _levelRenderer.Draw(BlockLayer.Solid);
        _levelRenderer.Draw(BlockLayer.Translucent);
        _player.Draw();

        EndMode3D();
        
        _gameScreen.Draw();
        if (_paused) _pauseScreen.Draw();
    }

    public void Dispose()
    {
        _level.Save();
    }
}