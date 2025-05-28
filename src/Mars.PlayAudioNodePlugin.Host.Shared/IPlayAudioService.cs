
using Mars.PlayAudioNodePlugin.Shared.Dto;

namespace Mars.PlayAudioNodePlugin.Host.Shared;

public interface IPlayAudioService
{
    Task Play(string filepath, float volume = 1.0f, string outputDeviceId = "");
    Task Play(Stream stream, float volume = 1, string outputDeviceId = "");

    /// <summary>
    /// Default device will first
    /// </summary>
    /// <returns></returns>
    OutputDeviceResponse[] OutputDevices();
    void StopAll();
}
