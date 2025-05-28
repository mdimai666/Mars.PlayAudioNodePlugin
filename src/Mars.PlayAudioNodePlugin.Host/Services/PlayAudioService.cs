using System.Data;
using Mars.PlayAudioNodePlugin.Host.Features;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Shared.Dto;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Mars.PlayAudioNodePlugin.Host.Services;

public class PlayAudioService : IPlayAudioService
{
    CancellationTokenSource cancellationTokenSource = new();

    public async Task Play(Stream stream, float volume = 1, string outputDeviceId = "")
    {
        volume = Math.Clamp(volume, 0f, 1f); // 0.0 — тишина, 1.0 — макс. громкость

        var device = ResolveAudioDevice(outputDeviceId);

        ResolveFileStreamProvider(stream, out var audioFile);
        //var readerStream = new MediaFoundationReader(filePath);
        //ResolveFileStreamProvider(filepath, out var audioFile);
        using var outputDevice = new WasapiOut(device, AudioClientShareMode.Shared, true, 200);
        var sampleChannel = new SampleChannel(audioFile, forceStereo: false);
        sampleChannel.Volume = volume;

        outputDevice.Init(audioFile);
        outputDevice.Play();
        var ct = cancellationTokenSource.Token;

        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            if (ct.IsCancellationRequested)
            {
                outputDevice.Stop();
                break;
            }
            await Task.Delay(100);
        }
    }

    public async Task Play(string filepath, float volume = 1, string outputDeviceId = "")
    {
        volume = Math.Clamp(volume, 0f, 1f); // 0.0 — тишина, 1.0 — макс. громкость

        var device = ResolveAudioDevice(outputDeviceId);

        ResolveFileStreamProvider(filepath, out var audioFile);
        using var outputDevice = new WasapiOut(device, AudioClientShareMode.Shared, true, 200);
        //audioFile.Volume = volume;
        var sampleChannel = new SampleChannel(audioFile, forceStereo: false);
        sampleChannel.Volume = volume;

        outputDevice.Init(audioFile);
        outputDevice.Play();
        var ct = cancellationTokenSource.Token;

        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            if (ct.IsCancellationRequested)
            {
                outputDevice.Stop();
                break;
            }
            await Task.Delay(100);
        }
    }

    public void StopAll()
    {
        using var outputDevice = new WasapiOut();
        outputDevice.Stop();
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    internal void ResolveFileStreamProvider(string filePath, out WaveStream readerStream)
    {
        if (filePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
        {
            readerStream = new WaveFileReader(filePath);
            if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                readerStream = new BlockAlignReductionStream(readerStream);
            }
        }
        else if (filePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                readerStream = new Mp3FileReader(filePath);
            }
            else
            {
                readerStream = new MediaFoundationReader(filePath);
            }
        }
        else if (filePath.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".aif", StringComparison.OrdinalIgnoreCase))
        {
            readerStream = new AiffFileReader(filePath);
        }
        else if (filePath.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
        {
            readerStream = new NAudio.Vorbis.VorbisWaveReader(filePath);
        }
        else
        {
            readerStream = new MediaFoundationReader(filePath);
        }
    }

    internal MMDevice ResolveAudioDevice(string id)
    {
        using var enumerator = new MMDeviceEnumerator();

        if (string.IsNullOrEmpty(id))
        {
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return defaultDevice;
        }

        return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            .FirstOrDefault(x => x.ID == id) ?? throw new ArgumentException($"device id='{id}' not found");
    }

    internal IEnumerable<MMDevice> EnumerateAudioEndPoints()
    {
        using var enumerator = new MMDeviceEnumerator();
        var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            .Where(x => x.ID != defaultDevice.ID).Prepend(defaultDevice);

        return devices;
    }

    internal void ResolveFileStreamProvider(Stream inputStream, out WaveStream readerStream)
    {
        var ext = AudioFormatRecognizer.RecognizeAudioFormat(inputStream);

        if (ext == "WAV")
        {
            readerStream = new WaveFileReader(inputStream);
            if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                readerStream = new BlockAlignReductionStream(readerStream);
            }
        }
        else if (ext == "MP3")
        {
            //if (Environment.OSVersion.Version.Major < 6)
            //{
            readerStream = new Mp3FileReader(inputStream);
            //}
            //else
            //{
            //    readerStream = new MediaFoundationReader(inputStream);
            //}
        }
        else if (ext == "AIFF" || ext == "AIF")
        {
            readerStream = new AiffFileReader(inputStream);
        }
        else if (ext == "OGG")
        {
            readerStream = new NAudio.Vorbis.VorbisWaveReader(inputStream);
        }
        //else
        //{
        //    readerStream = new MediaFoundationReader(filePath);
        //}
        else
            throw new NotImplementedException($"type not recognized '{ext}'");
    }

    public OutputDeviceResponse[] OutputDevices()
    {
        var devices = EnumerateAudioEndPoints(); //default will first

        return devices.Select((d, i) => new OutputDeviceResponse
        {
            DeviceId = d.ID,
            DeviceName = d.DeviceFriendlyName,
            FriendlyName = d.FriendlyName,
            IsDefault = i == 0
        }).ToArray();
    }
}
