using System.Text;

namespace Mars.PlayAudioNodePlugin.Host.Features;

public class AudioFormatRecognizer
{
    /// <summary>
    /// <list type="bullet">
    /// <item>Unknown</item>
    /// <item>WAV</item>
    /// <item>MP3</item>
    /// <item>AIFF/AIF</item>
    /// <item>OGG</item>
    /// </list>
    /// </summary>
    /// <param name="audioData"></param>
    /// <returns></returns>
    public static string RecognizeAudioFormat(byte[] audioData)
    {
        if (audioData.Length < 8)
            return "Unknown";

        // Проверка первых нескольких байт
        //using MemoryStream ms = new MemoryStream(audioData);
        //using BinaryReader reader = new BinaryReader(ms);
        //var headerBytes = reader.ReadBytes(8);
        var headerBytes = audioData.Take(8).ToArray();

        // WAV
        if ((headerBytes[0] == 'R') && (headerBytes[1] == 'I') &&
            (headerBytes[2] == 'F') && (headerBytes[3] == 'F'))
        {
            return "WAV";
        }

        // MP3
        else if ((headerBytes[0] == 'I') && (headerBytes[1] == 'D') &&
           (headerBytes[2] == '3'))
        {
            return "MP3";
        }

        // AIFF/AIF
        else if ((headerBytes[0] == 'F') && (headerBytes[1] == 'O') &&
                 (headerBytes[2] == 'R') && (headerBytes[3] == 'M'))
        {
            return "AIFF/AIF";
        }

        // OGG
        else if ((headerBytes[0] == 'O') && (headerBytes[1] == 'g') &&
                 (headerBytes[2] == 'g') && (headerBytes[3] == 'S')) // oggS magic number
        {
            return "OGG";
        }

        // Если ни один из форматов не соответствует
        return $"Unknown: {ByteArrayToString(headerBytes)}";
    }

    /// <summary>
    /// <list type="bullet">
    /// <item>Unknown</item>
    /// <item>WAV</item>
    /// <item>MP3</item>
    /// <item>AIFF/AIF</item>
    /// <item>OGG</item>
    /// </list>
    /// </summary>
    /// <param name="audioData"></param>
    /// <returns></returns>
    public static string RecognizeAudioFormat(Stream inputStream)
    {
        // https://gist.github.com/iahu/396eaf109ed0969382abdbc9c3f0f029
        using var reader = new BinaryReader(inputStream, new UTF8Encoding(), true);
        var bytes = reader.ReadBytes(8);
        inputStream.Position = 0;
        inputStream.Seek(0, SeekOrigin.Begin);
        return RecognizeAudioFormat(bytes);
    }

    public static string ByteArrayToString(byte[] bytes)
    {
        string hexValues = string.Join(",", Array.ConvertAll(bytes, b => $"0x{b:X2}"));

        return $"{Encoding.UTF8.GetString(bytes)}({hexValues})";
    }
}