global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpCraft.Level;
using SharpCraft.Scenes;
using SharpCraft.Utilities;
using Timer = SharpCraft.Utilities.Timer;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    public static IScene Scene = null!;

    private static void Main()
    {
        InitWindow(1024, 768, "SharpCraft");
        
        Resources.Load();

        if (!Directory.Exists("Assets"))
        {
            Scene = new NoAssetsScene();
        }
        else Scene = new SplashScene();

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