using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Gui.Screens;
using SharpCraft.Level;
using SharpCraft.Level.Tiles;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

namespace SharpCraft.Scenes;

public class GameScene : IScene
{
    private static readonly int MaxTileId = TileRegistry.GetNonNullTileCount();

    private Timer _timer;
    private Level.Level _level;
    private LevelRenderer _levelRenderer;
    private Player _player;
    private RayCollision _rayCast;

    private byte _currentTile = 1;

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

        _gameScreen = new GameOverlayScreen();
        _pauseScreen = new PauseScreen();
    }

    public void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
        {
            _paused = !_paused;
            
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
        
        if (!_paused)
        {
            FrameRateUpdate();
        }
        
        _timer.Advance();
        for (var i = 0; i < _timer.Ticks; i++) TickedUpdate();
        
        if (_paused) _pauseScreen.Update();
        else _gameScreen.Update();
    }

    private void FrameRateUpdate()
    {
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
        _gameScreen.BlockSelection.CurrentTile = _currentTile;

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
        
        _gameScreen.Draw();
        if (_paused) _pauseScreen.Draw();
    }

    public void Dispose()
    {
        _level.Save();
    }
}