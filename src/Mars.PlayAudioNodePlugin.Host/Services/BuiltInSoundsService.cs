using System.Reflection;

namespace Mars.PlayAudioNodePlugin.Host.Services;

public class BuiltInSoundsService
{
    string _soundsPath;
    public string SoundsPath => _soundsPath;

    public BuiltInSoundsService(Assembly mainPluginAssembly)
    {
        var dllPath = Path.GetDirectoryName(mainPluginAssembly.Location)!;
        _soundsPath = Path.GetFullPath(Path.Combine(dllPath, "wwwroot", "sounds"));

        if (!Directory.Exists(_soundsPath))
            _soundsPath = Path.GetFullPath(Path.Combine(dllPath, "..", "..", "..", "wwwroot", "sounds")); // trim /bin/Debug/net10.0/
    }

    public string[] SoundList()
    {
        return Directory.GetFiles(_soundsPath, "*.mp3", SearchOption.TopDirectoryOnly).Select(s => s[(_soundsPath.Length + 1)..]).ToArray();
    }

    public string FullPathByName(string name)
    {
        return Path.Combine(_soundsPath, name);
    }

}
