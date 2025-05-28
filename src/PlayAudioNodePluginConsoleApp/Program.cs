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

var file = @"..\..\..\Samples\game-win.mp3";
//var file = @"..\..\..\Samples\guiclick.ogg";
//PlaySound(file);
//using var fileReader = new FileStream(file, FileMode.Open);
_ = pas.Play(file);
await Task.Delay(500);
//await pas.Play(file);
pas.StopAll();

async void PlaySound(string filePath, float volume = 1.0f)
{
    volume = Math.Clamp(volume, 0f, 1f); // 0.0 — тишина, 1.0 — макс. громкость

    using var audioFile = new AudioFileReader(filePath);
    using var outputDevice = new WaveOutEvent();
    audioFile.Volume = volume;
    outputDevice.Init(audioFile);
    outputDevice.Play();

    // Ждем завершения (для консольных приложений)
    while (outputDevice.PlaybackState == PlaybackState.Playing)
    {
        //Thread.Sleep(100);
        await Task.Delay(100);
    }
}
