global using static Raylib_cs.Raylib;
global using Raylib_cs;
using System.Runtime.CompilerServices;
using SharpCraft.Scenes;
using SharpCraft.Utilities;
using TinyDialogsNet;

[assembly: DisableRuntimeMarshalling]

namespace SharpCraft;

internal static class Program
{
    private const string Version = "rd-161348";

    public static IScene Scene = null!;
    
    private static void Main()
    {
        GpuUtil.TryForceNvidiaGpu();

        InitWindow(1024, 768, "SharpCraft");
        SetTraceLogLevel(TraceLogLevel.Warning);

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

        CloseWindow();
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