using System.IO.Compression;

namespace SharpCraft.Utilities;

public static class AssetsExtractor
{
    private static readonly string[] RequiredFiles = [
        "terrain.png", "char.png", "default.gif", 
        // "dirt.png", "grass.png", "rock.png", "water.png"
    ];

    public static bool AreFilesPresent()
    {
        return RequiredFiles.All(f => File.Exists(Path.Join(Assets.Root, f)));
    }
    
    public static void Extract(string jarFile)
    {
        using var stream = File.OpenRead(jarFile);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

        foreach (var file in RequiredFiles)
        {
            var entry = zip.GetEntry(file);
            if (entry == null)
            {
                throw new Exception("File not found in .jar: " + file);
            }
            
            entry.ExtractToFile(Path.Join(Assets.Root, file), true);
        }
    }
}