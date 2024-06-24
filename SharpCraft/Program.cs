using Serilog;
using SharpCraft.Framework;
using SharpCraft.Scenes;
using SharpCraft.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace SharpCraft;

internal static class Program
{
    private static IScene? _scene;

    public static IScene? Scene
    {
        get => _scene;
        set
        {
            _scene?.Dispose();
            _scene = value;
        }
    }
    
    public static IWindow MainWindow = null!;
    public static GL Gl = null!;

    private static Vector2D<int> _windowedSize;
    private static Vector2D<int> _windowedPos;
    private static bool _isFullscreen;

    public static int Fps;
    private static int _frames;
    private static double _lastSecond;

    private static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        var options = WindowOptions.Default with
        {
            Title = "SharpCraft",
            Size = new Vector2D<int>(1024, 768),
            VSync = false
        };

        MainWindow = Window.Create(options);

        MainWindow.Load += () =>
        {
            Gl = MainWindow.CreateOpenGL();
            MainWindow.Center();

            Input.CreateContext(MainWindow);

            Resources.Load();
            Scene = new GameScene();
            
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
        };

        MainWindow.Update += deltaTime =>
        {
            if (Input.IsKeyPressed(Key.F11))
            {
                ToggleFullscreen();
            }

            _frames++;

            if (MainWindow.Time >= _lastSecond + 1.0)
            {
                Fps = _frames;
                _frames = 0;
                _lastSecond = MainWindow.Time;
            }
            
            Scene?.Update(deltaTime);
            Input.Update();
        };
        MainWindow.Render += deltaTime =>
        {
            var matrices = new MatrixStack();
            Scene?.Render(matrices, deltaTime);
        };
        MainWindow.FramebufferResize += newSize =>
        {
            Gl.Viewport(newSize);
        };
        MainWindow.Closing += () =>
        {
            Scene?.Dispose();
            ResourceManager.Unload();
            Resources.Unload();
        };

        MainWindow.Run();
    }

    private static void ToggleFullscreen()
    {
        _isFullscreen = !_isFullscreen;

        if (_isFullscreen)
        {
            var monitor = MainWindow.Monitor;
            
            if (monitor == null)
            {
                return;
            }

            if (monitor.VideoMode.Resolution == null)
            {
                Log.Warning("how the fuck did you get monitor with null resolution????");
                return;
            }
            
            _windowedPos = MainWindow.Position;
            _windowedSize = MainWindow.Size;

            MainWindow.WindowBorder = WindowBorder.Hidden;
            MainWindow.Position = Vector2D<int>.Zero;
            MainWindow.Size = monitor.VideoMode.Resolution.Value;
        }
        else
        {
            MainWindow.WindowBorder = WindowBorder.Resizable;
            MainWindow.Position = _windowedPos;
            MainWindow.Size = _windowedSize;
        }
    }
}