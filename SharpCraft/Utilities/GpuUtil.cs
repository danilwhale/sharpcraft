using System.Runtime.InteropServices;

namespace SharpCraft.Utilities;

public static partial class GpuUtil
{
    [LibraryImport("nvapi64.dll", EntryPoint = "fake")]
    private static partial void LoadNvApi64();
    
    [LibraryImport("nvapi.dll", EntryPoint = "fake")]
    private static partial void LoadNvApi32();
    
    public static void TryForceNvidiaGpu()
    {
        if (!OperatingSystem.IsWindows()) return;

        try
        {
            if (Environment.Is64BitProcess) LoadNvApi64();
            else LoadNvApi32();
        }
        catch
        {
            // ignored
        }
    }
}