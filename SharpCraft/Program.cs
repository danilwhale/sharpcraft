global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using SharpCraft.Gui;
using SharpCraft.Rendering;
using SharpCraft.Scenes;
using SharpCraft.Utilities;
using SharpCraft.World.Rendering;
using TinyDialogsNet;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    public const string Version = "0.0.11a";
    private const string InternalVersion = "2";

    public static IScene Scene = null!;
    
    private static void Main()
    {
        GpuUtil.TryForceNvidiaGpu();
        
        InitializeLogging();
        
        InitWindow(854, 480, $"SharpCraft {Version}+{InternalVersion}");
        SetWindowState(ConfigFlags.ResizableWindow);
        GenerateWindowIcon();
        SetTraceLogLevel(TraceLogLevel.Warning);
        SetExitKey(KeyboardKey.Null);

        if (!TryPrepareAssets())
        {
            CloseWindow();
            return;
        }

        Scene = new GameScene();

        while (!WindowShouldClose())
        {
            Scene.Update();

            BeginDrawing();
            Scene.Draw();
            EndDrawing();
        }

        Scene.Dispose();

        Assets.Unload();
        FontManager.Unload();

        CloseWindow();
    }

    private static unsafe void InitializeLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File("Latest.log")
            .WriteTo.Console()
            .CreateLogger();
        
        SetTraceLogCallback(&RaylibTraceLogCallback);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void RaylibTraceLogCallback(int logLevel, sbyte* format, sbyte* args)
    {
        Log.Write((TraceLogLevel)logLevel switch
        {
            TraceLogLevel.Trace => LogEventLevel.Verbose,
            TraceLogLevel.Debug => LogEventLevel.Debug,
            TraceLogLevel.Info => LogEventLevel.Information,
            TraceLogLevel.Warning => LogEventLevel.Warning,
            TraceLogLevel.Error => LogEventLevel.Error,
            TraceLogLevel.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Verbose
        }, Logging.GetLogMessage((nint)format, (nint)args));
    }

    private static void GenerateWindowIcon()
    {
        var target = LoadRenderTexture(64, 64);
        
        BeginDrawing();
        BeginTextureMode(target);
        
        ClearBackground(Color.Blank);
        
        Rlgl.MatrixMode(MatrixMode.Projection);
        Rlgl.LoadIdentity();
        Rlgl.Ortho(0.0, target.Texture.Width, target.Texture.Height, 0.0, 0.01, 1000.0);
        
        Rlgl.MatrixMode(MatrixMode.ModelView);
        Rlgl.LoadIdentity();
        Rlgl.Translatef(0.0f, 0.0f, -200.0f);
        
        Rlgl.Scalef(40.0f, 40.0f, 40.0f);

        Rlgl.Rotatef(30.0f, 1.0f, 0.0f, 0.0f);
        Rlgl.Rotatef(45.0f, 0.0f, 1.0f, 0.0f);

        Rlgl.Translatef(2.6f, -0.85f, -1.5f);
        
        Rlgl.Scalef(-1.0f, 1.0f, 1.0f);
        
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.SetTexture(Assets.GetTexture("IconResources.png").Id);
        
        Registries.Tiles.Grass.Build(RlglVertexBuilder.Instance, null, 0, 0, 0, RenderLayer.Lit);

        Rlgl.SetTexture(Rlgl.GetTextureIdDefault());
        Rlgl.End();
        
        EndTextureMode();
        EndDrawing();

        var image = LoadImageFromTexture(target.Texture);
        UnloadRenderTexture(target);

        SetWindowIcon(image);
        UnloadImage(image);
    }

    private static bool TryPrepareAssets()
    {
        var lastVersionPath = Path.Join(Assets.Root, ".last_version");
        
        if (AssetsExtractor.AreFilesPresent() &&
            File.Exists(lastVersionPath) && File.ReadAllText(lastVersionPath) == Version) return true;
        
        try
        {
            var result = TinyDialogs.OpenFileDialog(
                $"Select {Version} .jar file",
                filter: new FileFilter(".jar files", ["*.jar"])
            );

            if (result.Canceled)
            {
                return false;
            }

            AssetsExtractor.Extract(result.Paths.First());
            File.WriteAllText(lastVersionPath, Version);
        }
        catch (Exception e)
        {
            TinyDialogs.MessageBox(
                "Failed to prepare assets", 
                e.ToString(), 
                MessageBoxDialogType.Ok, 
                MessageBoxIconType.Error, 
                MessageBoxButton.Ok
            );
            
            return false;
        }

        return true;
    }
}