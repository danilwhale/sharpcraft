global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Runtime.CompilerServices;
using SharpCraft.Scenes;
using SharpCraft.Utilities;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    public static IScene Scene = null!;

    private static readonly string[] RequiredFiles = ["terrain.png", "char.png"]; 

    private static void Main()
    {
        GpuUtil.TryForceNvidiaGpu();
        
        InitWindow(1024, 768, "SharpCraft");
        SetTraceLogLevel(TraceLogLevel.Warning);

        if (!RequiredFiles.All(file => File.Exists(Path.Join(Assets.Root, file))))
        {
            Scene = new NoAssetsScene();
        }
        else Scene = new GameScene();

        while (!WindowShouldClose())
        {
            Scene.Update();
            
            BeginDrawing();
            
            Scene.Draw();
            
            EndDrawing();
        }

        Scene.Dispose();
        
        Assets.Unload();

        CloseWindow();
    }
}