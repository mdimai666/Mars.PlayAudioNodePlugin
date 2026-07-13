using System.Runtime.InteropServices;
using System.Text;
using Mars.PlayAudioNodePlugin.Host.Features;
using Mars.PlayAudioNodePlugin.Host.Services;
using NAudio.Wave;
using NAudio.Wave.Alsa;
using NLayer.NAudioSupport;

Console.WriteLine("NAudio!");
Console.WriteLine("==========");

#if !f0
Console.WriteLine("outputDevices");

//AudioPluginInitializer.InitializeNativeLibraries();
var pas = new PlayAudioService();

//foreach (var device in pas.GetAdvancedLinuxDevices())
//{
//    Console.WriteLine($"device - {device.Key}={device.Value}");
//}
//return;

var devices = pas.OutputDevices();

foreach (var device in devices)
{
    Console.WriteLine($"{device.DeviceId}={device.FriendlyName}");
}

var samplesDirPrefix = "../../../../../tests/Test.PlayAudioNodePlugin/Samples/";

//var file = samplesDirPrefix + "game-win.mp3";

var file = samplesDirPrefix + "guiclick.ogg";
//await PlayAsync(file);
//using var fileReader = new FileStream(file, FileMode.Open);
#if STOP_TEST
_ = pas.Play(file);
await Task.Delay(500);
//await pas.Play(file);
pas.StopAll();
#else
var audioPath = samplesDirPrefix + "streat_audio_without-header.wav";
//audioPath = samplesDirPrefix + "guiclick.ogg";
//audioPath = samplesDirPrefix + "game-win.mp3";
var bytes = await File.ReadAllBytesAsync(audioPath);
var ms = new MemoryStream(bytes);

var outputDeviceId = "";
outputDeviceId = OperatingSystem.IsWindows() ? "{0.0.0.00000000}.{2826b110-4b0f-4840-90c0-79a3175a14a7}" : "pulse";

await pas.Play(ms, outputDeviceId: outputDeviceId);
//await pas.Play(audioPath);
#endif

async Task PlayAsync(string filePath, float volume = 1.0f)
{
    volume = Math.Clamp(volume, 0f, 1f); // 0.0 — тишина, 1.0 — макс. громкость

    using var audioFile = new AudioFileReader(filePath);
    using var outputDevice = new WaveOutEvent();
    audioFile.Volume = volume;
    var tcs = new TaskCompletionSource();

    outputDevice.PlaybackStopped += (s, e) =>
    {
        if (e.Exception != null)
            tcs.TrySetException(e.Exception);
        else
            tcs.TrySetResult();
    };

    outputDevice.Init(audioFile);
    outputDevice.Play();

    await tcs.Task;
}

#endif

#if ALSA
string samplesDirPrefix;
if (OperatingSystem.IsWindows())
    samplesDirPrefix = "C:\\Users\\D\\Documents\\VisualStudio\\2025\\Mars.PlayAudioNodePlugin\\tests\\Test.PlayAudioNodePlugin\\Samples\\";
else
    samplesDirPrefix = "/mnt/c/Users/D/Documents/VisualStudio/2025/Mars.PlayAudioNodePlugin/tests/Test.PlayAudioNodePlugin/Samples/";

var file = samplesDirPrefix + "game-win.mp3";

using var mp3 = new Mp3FileReaderBase(file,
    waveFormat => new Mp3FrameDecompressor(waveFormat));
using var output = new AlsaOut();
output.Init(mp3);
output.Play();

while (output.PlaybackState == PlaybackState.Playing)
{
    await Task.Delay(200);
} 
#endif
