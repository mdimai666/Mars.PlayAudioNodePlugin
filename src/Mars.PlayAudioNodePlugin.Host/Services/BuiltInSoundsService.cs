using System.Reflection;
using Mars.Nodes.Core.Implements.Utils;

namespace Mars.PlayAudioNodePlugin.Host.Services;

public class BuiltInSoundsService
{
    string _soundsPath;
    public string SoundsPath => _soundsPath;

    string[]? _files;

    public BuiltInSoundsService(Assembly mainPluginAssembly)
    {
        var dllPath = Path.GetDirectoryName(mainPluginAssembly.Location)!;
        _soundsPath = Path.GetFullPath(Path.Combine(dllPath, "wwwroot", "sounds"));

        if (!Directory.Exists(_soundsPath))
            _soundsPath = Path.GetFullPath(Path.Combine(dllPath, "..", "..", "..", "wwwroot", "sounds")); // trim /bin/Debug/net10.0/
    }

    public string[] SoundList()
    {
        if (_files is not null) return _files;

        var fileListUtility = new FileListUtility();
        return _files ??= fileListUtility.GetFiles(_soundsPath,
                                                    includeFilter: ".mp3,.wav",
                                                    maxDepth: 3,
                                                    returnRelativePaths: true,
                                                    useRootGitIgnore: false)
                                        .Select(f => f.Replace('\\', '/'))
                                        .ToArray();
    }

    public string FullPathByName(string name)
    {
        return Path.Combine(_soundsPath, name);
    }

}
