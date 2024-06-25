global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using SharpCraft.Gui;
using SharpCraft.Gui.Screens;
using SharpCraft.Level;
using SharpCraft.Scenes;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    public static IScene Scene = null!;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void TraceLogCallback(int logLevel, sbyte* text, sbyte* args)
    {
        var level = logLevel switch
        {
            (int)TraceLogLevel.Trace => LogEventLevel.Verbose,
            (int)TraceLogLevel.Debug => LogEventLevel.Debug,
            (int)TraceLogLevel.Info => LogEventLevel.Information,
            (int)TraceLogLevel.Warning => LogEventLevel.Warning,
            (int)TraceLogLevel.Error => LogEventLevel.Error,
            (int)TraceLogLevel.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Verbose
        };
        
        Log.Write(level, Logging.GetLogMessage((nint)text, (nint)args));
    }

    private static unsafe void ConfigureLog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File($"Logs/Latest-{DateTime.Now:yyyy-MM-d_hh-mm-ss}.log")
            .CreateLogger();
        
        SetTraceLogCallback(&TraceLogCallback);
        
        Log.Debug("hi");
    }

    private static void Main()
    {
        ConfigureLog();
        
        InitWindow(1024, 768, "SharpCraft");
        SetExitKey(KeyboardKey.Null);
        
        Resources.Load();
        
        Scene = new SplashScene();

        while (!WindowShouldClose())
        {
            Scene.Update();
            
            BeginDrawing();
            
            Scene.Draw();
            
            EndDrawing();
        }

        Scene.Dispose();
        
        ResourceManager.Unload();
        Resources.Unload();

        CloseWindow();
    }
}