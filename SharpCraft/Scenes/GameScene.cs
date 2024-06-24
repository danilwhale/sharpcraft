using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Framework;
using SharpCraft.Gui.Screens;
using SharpCraft.Level;
using SharpCraft.Level.Blocks;
using SharpCraft.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
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
    private Mesh _mesh;

    public GameScene()
    {
        Input.SetCursorMode(CursorMode.Disabled);

        _timer = new Timer(60.0f);
        _level = new Level.Level(256, 64, 256);
        _levelRenderer = new LevelRenderer(_level);
        _player = new Player(_level);

        _gameScreen = new GameOverlayScreen();
        _pauseScreen = new PauseScreen();
        _player.Editor.SelectionElement = _gameScreen.BlockSelection;

        _mesh = new Mesh(Program.Gl);
        _mesh.Upload(Resources.DefaultTerrainMaterial.Shader, new[]
        {
            new Vertex(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f),
            new Vertex(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f),
            new Vertex(1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f)
        }, new ushort[]
        {
            0, 1, 2
        });
        
        Program.Gl.ClearColor(0.5f, 0.8f, 1.0f, 1.0f);
    }

    public void Update(double deltaTime)
    {
        if (Input.IsKeyPressed(Key.Escape))
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

        Program.MainWindow.Title = $"SharpCraft - {Program.Fps} FPS";
    }

    private void SetPause(bool pause)
    {
        _paused = pause;
        
        if (_paused)
        {
            Input.SetCursorMode(CursorMode.Normal);
            Program.MainWindow.FramesPerSecond = 15.0;
            _timer.TimeScale = 0.0f;
        }
        else
        {
            Input.SetCursorMode(CursorMode.Disabled);
            Program.MainWindow.FramesPerSecond = 0.0;
            _timer.TimeScale = 1.0f;
        }
    }

    private void FrameRateUpdate()
    {
        HandleInput();
        _player.Update();
        _player.Entity.MoveCamera(_timer.LastPassedTime);
    }

    private void HandleInput()
    {
        if (Input.IsKeyPressed(Key.Enter)) _level.Save();
    }

    private void TickedUpdate()
    {
        _player.Tick();
    }

    public void Render(MatrixStack matrices, double deltaTime)
    {
        Program.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _player.Entity.Camera.Begin(matrices);

        _levelRenderer.Draw(matrices, BlockLayer.Solid);
        _levelRenderer.Draw(matrices, BlockLayer.Translucent);
        _player.Draw();
        _mesh.Draw(matrices, PrimitiveType.Triangles, Resources.DefaultTerrainMaterial, Matrix4x4.CreateTranslation(_player.Entity.Camera.Position - Vector3.UnitZ));

        Camera.End(matrices);
        
        _gameScreen.Draw();
        if (_paused) _pauseScreen.Draw();
    }

    public void Dispose()
    {
        _level.Save();
        _levelRenderer.Dispose();
    }
}