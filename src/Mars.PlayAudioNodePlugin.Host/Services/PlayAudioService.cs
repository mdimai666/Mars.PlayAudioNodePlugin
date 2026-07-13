using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Runtime.Versioning;
using AngleSharp.Dom;
using Mars.PlayAudioNodePlugin.Host.Features;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Shared.Dto;
using NAudio.CoreAudioApi;
using NAudio.SoundFile;
using NAudio.Wave;
using NAudio.Wave.Alsa;
using NAudio.Wave.SampleProviders;
using NLayer.NAudioSupport;

namespace Mars.PlayAudioNodePlugin.Host.Services;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
internal class PlayAudioService : IPlayAudioService
{
    private CancellationTokenSource cancellationTokenSource = new();
    private readonly ConcurrentDictionary<IWavePlayer, byte> activeOutputDevices = new();

    public PlayAudioService()
    {
        AudioPluginInitializer.InitializeNativeLibraries();
    }

    public async Task Play(Stream stream, float volume = 1, string outputDeviceId = "")
    {
        WaveStream? audioFile = null;

        try
        {
            var ext = AudioFormatRecognizer.RecognizeAudioFormat(stream);
            stream.Position = 0;
            audioFile = CreateReaderStream(stream, ext);

            await PlayAudioInternal(audioFile, volume, outputDeviceId);
        }
        finally
        {
            audioFile?.Dispose();
        }
    }

    public async Task Play(string filepath, float volume = 1, string outputDeviceId = "")
    {
        if (filepath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            if (!OperatingSystem.IsWindows())
                throw new NotSupportedException("HTTP streaming is not fully supported on non-Windows platforms. Please download the file first.");

            using var httpDevice = CreateOutputDevice(outputDeviceId);
            using var httpAudioFile = new MediaFoundationReader(filepath);
            // Для HTTP стриминга оставляем прямую логику без усложнения стримами
            await PlayAudioStream(httpDevice, httpAudioFile, volume);
            return;
        }

        FileStream? fileStream = null;
        WaveStream? audioFile = null;

        try
        {
            fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var ext = Path.GetExtension(filepath).TrimStart('.').ToUpperInvariant();

            audioFile = CreateReaderStream(fileStream, ext);

            await PlayAudioInternal(audioFile, volume, outputDeviceId);
        }
        finally
        {
            audioFile?.Dispose();
            fileStream?.Dispose();
        }
    }

    private async Task PlayAudioInternal(WaveStream audioFile, float volume, string outputDeviceId)
    {
        var outputDevice = CreateOutputDevice(outputDeviceId);
        activeOutputDevices.TryAdd(outputDevice, 0);

        try
        {
            await PlayAudioStream(outputDevice, audioFile, volume);
        }
        finally
        {
            activeOutputDevices.TryRemove(outputDevice, out _);
            outputDevice.Dispose();
        }
    }

    // Общий цикл ожидания окончания проигрывания
    private async Task PlayAudioStream(IWavePlayer outputDevice, WaveStream audioFile, float volume)
    {
        var sampleProvider = audioFile.ToSampleProvider();
        var volumeProvider = new VolumeSampleProvider(sampleProvider) { Volume = volume };

        outputDevice.Init(volumeProvider);
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
        cancellationTokenSource.Cancel();

        foreach (var device in activeOutputDevices.Keys)
        {
            try
            {
                device.Stop();
            }
            catch
            {
                // Игнорируем ошибки при остановке
            }
        }
        activeOutputDevices.Clear();

        cancellationTokenSource = new CancellationTokenSource();
    }

    private IWavePlayer CreateOutputDevice(string outputDeviceId)
    {
        if (OperatingSystem.IsWindows())
        {
            if (string.IsNullOrEmpty(outputDeviceId))
            {
                return new WasapiOut(AudioClientShareMode.Shared, 200);
            }

            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDevice(outputDeviceId);
            return new WasapiOut(device, AudioClientShareMode.Shared, false, 200);
        }
        else if (OperatingSystem.IsLinux())
        {
            //TODO: Выбор устройства воспроизведения под линуксом не протестировано.
            // Для Linux используется ALSA устройство (например, "default" или "hw:0,0")
            string alsaDeviceName = string.IsNullOrEmpty(outputDeviceId) ? "default" : outputDeviceId;
            return new AlsaOut(alsaDeviceName);
        }

        throw new PlatformNotSupportedException("Audio playback is not supported on this platform");
    }

    private WaveStream CreateReaderStream(Stream stream, string format)
    {
        return format switch
        {
            "WAV" => OperatingSystem.IsWindows()
                ? new StreamMediaFoundationReader(stream)
                : new WaveFileReader(stream),

            "MP3" => new Mp3FileReaderBase(stream, new Mp3FileReaderBase.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf))),

            "AIFF" or "AIF" => new AiffFileReader(stream),

            "OGG" or "FLAC" or "OPUS" => new SoundFileReader(stream),

            _ => OperatingSystem.IsWindows()
                ? new StreamMediaFoundationReader(stream) // Фоллбэк для Windows (использует системные кодеки)
                : throw new NotSupportedException($"Unsupported audio format on this platform: {format}")
        };
    }

    public OutputDeviceResponse[] OutputDevices()
    {
        if (OperatingSystem.IsWindows())
        {
            var devices = EnumerateWindowsAudioEndPoints();
            return devices.Select((d, i) => new OutputDeviceResponse
            {
                DeviceId = d.ID,
                DeviceName = d.DeviceFriendlyName,
                FriendlyName = d.FriendlyName,
                IsDefault = i == 0
            }).ToArray();
        }
        else if (OperatingSystem.IsLinux())
        {
            var devices = EnumerateLinuxAudioEndPoints();
            return devices.Select((d, i) => new OutputDeviceResponse
            {
                DeviceId = d.Name,
                DeviceName = d.Name,
                FriendlyName = d.Name,
                IsDefault = i == 0
            }).ToArray();
        }

        throw new PlatformNotSupportedException("Audio device enumeration is not supported on this platform");
    }

    [SupportedOSPlatform("windows")]
    private IEnumerable<MMDevice> EnumerateWindowsAudioEndPoints()
    {
        using var enumerator = new MMDeviceEnumerator();
        var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            .Where(x => x.ID != defaultDevice.ID).Prepend(defaultDevice);

        return devices;
    }

    [SupportedOSPlatform("linux")]
    private IEnumerable<AlsaDeviceInfo> EnumerateLinuxAudioEndPoints()
    {
        return AlsaDeviceEnumerator.GetPlaybackDevices();
    }

    /**
     TODO: Еще не проверял, чтобы под линуком можно было выбрать устройство.
     Компонент AlsaOut в NAudio умеет работать с именами PulseAudio.
    Если вы передадите туда deviceId, полученный из pactl (например, alsa_output.pci-0000_00_1f.3.analog-stereo), ALSA перенаправит поток точечно в этот аудиовыход.
     */
    [SupportedOSPlatform("linux")]
    internal Dictionary<string, string> GetAdvancedLinuxDevices()
    {
        var devices = new Dictionary<string, string>
        {
            { "default", "Системный выход по умолчанию" }
        };

        if (!OperatingSystem.IsLinux()) return devices;

        try
        {
            // Вызываем системную утилиту pactl, которая общается с PulseAudio/PipeWire
            var startInfo = new ProcessStartInfo
            {
                FileName = "pactl",
                Arguments = "list short sinks", // Получаем краткий список аудиовыходов
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Вывод pactl выглядит так:
                // 52  alsa_output.pci-0000_00_1f.3.analog-stereo  module-alsa-card  s16le 2ch 48000Hz
                // 105 bluez_output.74_5C_43_A1_B2_C3.a2dp-sink     module-bluez5-device s16le 2ch 44100Hz
                var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var parts = line.Split('\t');
                    if (parts.Length > 1)
                    {
                        string deviceId = parts[1].Trim(); // Идентификатор для ALSA/Pulse (например, alsa_output...)

                        // Делаем имя красивым и понятным для пользователя
                        string displayName = deviceId;
                        if (deviceId.Contains("bluez")) displayName = "🎧 Bluetooth Наушники/Гарнитура";
                        else if (deviceId.Contains("hdmi")) displayName = "📺 Дисплей / HDMI аудиовыход";
                        else if (deviceId.Contains("analog-stereo")) displayName = "🔊 Встроенные динамики / Колонки";

                        if (!devices.ContainsKey(deviceId))
                        {
                            devices.Add(deviceId, displayName);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении списка аудиоустройств: {ex.Message}");
            // Если pactl не установлен в дистрибутиве, останется только дефолтный девайс
        }

        return devices;
    }
}
