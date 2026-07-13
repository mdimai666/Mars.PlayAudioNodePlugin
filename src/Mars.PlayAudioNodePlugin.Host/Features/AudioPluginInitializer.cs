using System.Reflection;
using System.Runtime.InteropServices;

namespace Mars.PlayAudioNodePlugin.Host.Features;

public static class AudioPluginInitializer
{
    private static int _isInitialized = 0;

    public static void InitializeNativeLibraries()
    {
        if (OperatingSystem.IsWindows())
        {
            if (Interlocked.CompareExchange(ref _isInitialized, 1, 0) != 0)
            {
                return;
            }

            RegisterLibsFolder();
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetDllDirectory(string lpPathName);

    public static void RegisterLibsFolder()
    {
        string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                         ?? AppContext.BaseDirectory;
        string libsFolder = Path.Combine(baseDir, "libs");

        if (Directory.Exists(libsFolder))
        {
            // Windows теперь гарантированно будет искать libsndfile-1.dll 
            // и ВСЕ её C++ зависимости в этой папке.
            SetDllDirectory(libsFolder);
        }
    }
}
