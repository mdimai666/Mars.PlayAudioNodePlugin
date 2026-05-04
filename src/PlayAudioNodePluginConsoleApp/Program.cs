using Mars.PlayAudioNodePlugin.Host.Services;
using NAudio.Wave;

Console.WriteLine("NAudio!");
Console.WriteLine("==========");

Console.WriteLine("outputDevice");

var pas = new PlayAudioService();
var devices = pas.EnumerateAudioEndPoints();

foreach (var device in devices)
{
    Console.WriteLine($"{device.ID}={device.FriendlyName}");
}

var samplesDirPrefix = "../../../../tests/Test.PlayAudioNodePlugin/Samples/";

var file = samplesDirPrefix + "game-win.mp3";

//var file = samplesDirPrefix + "guiclick.ogg";
//await PlayAsync(file);
//using var fileReader = new FileStream(file, FileMode.Open);
#if STOP_TEST
_ = pas.Play(file);
await Task.Delay(500);
//await pas.Play(file);
pas.StopAll();
#else
var audioPath = samplesDirPrefix + samplesDirPrefix + "streat_audio_without-header.wav";
//audioPath = samplesDirPrefix + "guiclick.ogg";
//audioPath = samplesDirPrefix + "game-win.mp3";
var bytes = await File.ReadAllBytesAsync(audioPath);
var ms = new MemoryStream(bytes);
await pas.Play(ms);
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
