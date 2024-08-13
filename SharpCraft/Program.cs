global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using SharpCraft.Gui;
using SharpCraft.Scenes;
using SharpCraft.Utilities;
using TinyDialogsNet;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    public const string Version = "0.0.12a_03";

    public static IScene Scene = null!;
    
    private static void Main()
    {
        GpuUtil.TryForceNvidiaGpu();
        
        InitializeLogging();
        
        InitWindow(854, 480, "SharpCraft " + Version);
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