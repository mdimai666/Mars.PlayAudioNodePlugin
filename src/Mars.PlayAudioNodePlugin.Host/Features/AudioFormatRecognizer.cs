using System.Text;

namespace Mars.PlayAudioNodePlugin.Host.Features;

public static class AudioFormatRecognizer
{
    /// <summary>
    /// Распознает формат аудио по массиву байт. Добавлена поддержка FLAC, OPUS и "сырых" MP3.
    /// </summary>
    public static string RecognizeAudioFormat(byte[] audioData)
    {
        // Для точного определения MP3 фреймов и заголовков Opus нам нужно до 36 байт данных
        if (audioData == null || audioData.Length < 4)
            return "Unknown";

        // Проверка WAV (RIFF)
        if (audioData[0] == 'R' && audioData[1] == 'I' && audioData[2] == 'F' && audioData[3] == 'F')
        {
            return "WAV";
        }

        // Проверка MP3 с тегом ID3 (например, "ID3...")
        if (audioData[0] == 'I' && audioData[1] == 'D' && audioData[2] == '3')
        {
            return "MP3";
        }

        // Проверка MP3 без тегов (Frame Sync: первые 11-12 бит установлены в 1 -> 0xFF и 0xE0/0xF0)
        if (audioData[0] == 0xFF && (audioData[1] & 0xE0) == 0xE0)
        {
            return "MP3";
        }

        // Проверка FLAC (сигнатура "fLaC")
        if (audioData[0] == 0x66 && audioData[1] == 0x4C && audioData[2] == 0x61 && audioData[3] == 0x43)
        {
            return "FLAC";
        }

        // Проверка контейнера OGG (сигнатура "OggS")
        if (audioData[0] == 'O' && audioData[1] == 'g' && audioData[2] == 'g' && audioData[3] == 'S')
        {
            // Дифференциация OGG Vorbis и OGG Opus. 
            // В Ogg-контейнере заголовок пакета OpusHead обычно начинается с 28-го байта.
            if (audioData.Length >= 36)
            {
                // Ищем строку "OpusHead" на 28-й позиции
                if (audioData[28] == 'O' && audioData[29] == 'p' && audioData[30] == 'u' && audioData[31] == 's' &&
                    audioData[32] == 'H' && audioData[33] == 'e' && audioData[34] == 'a' && audioData[35] == 'd')
                {
                    return "OPUS";
                }
            }
            return "OGG";
        }

        // Проверка "голого" заголовка OPUS (иногда встречается в специфических стримах без Ogg)
        if (audioData[0] == 'O' && audioData[1] == 'p' && audioData[2] == 'u' && audioData[3] == 's')
        {
            return "OPUS";
        }

        // Проверка AIFF/AIF
        if (audioData[0] == 'F' && audioData[1] == 'O' && audioData[2] == 'r' && audioData[3] == 'M')
        {
            return "AIFF"; // Свели к единому стилю возврата с плеером
        }

        // Если формат не подошел, выводим хекс первых 8 байт
        var debugBytes = audioData.Take(Math.Min(8, audioData.Length)).ToArray();
        return $"Unknown: {ByteArrayToString(debugBytes)}";
    }

    /// <summary>
    /// Безопасно вычитывает первые 36 байт из потока и передает на распознавание.
    /// </summary>
    public static string RecognizeAudioFormat(Stream inputStream)
    {
        // https://gist.github.com/iahu/396eaf109ed0969382abdbc9c3f0f029

        if (inputStream == null || !inputStream.CanRead)
            return "Unknown";

        // Сохраняем исходную позицию на случай, если поток зашли не с нуля
        long originalPosition = inputStream.Position;

        // Нам нужно до 36 байт для надежного парсинга Opus внутри Ogg контейнера
        byte[] buffer = new byte[36];
        int bytesRead = 0;

        try
        {
            // Читаем байты напрямую из потока, не оборачивая в BinaryReader (чтобы не плодить лишние аллокации)
            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
        }
        finally
        {
            // Гарантированно возвращаем указатель потока строго на место
            inputStream.Position = originalPosition;
        }

        // Если поток совсем пустой
        if (bytesRead == 0) return "Unknown";

        // Если вычитали меньше 36 байт, ужмем массив, чтобы метод распознавания не проверял пустые индексы
        if (bytesRead < buffer.Length)
        {
            Array.Resize(ref buffer, bytesRead);
        }

        return RecognizeAudioFormat(buffer);
    }

    public static string ByteArrayToString(byte[] bytes)
    {
        string hexValues = string.Join(",", Array.ConvertAll(bytes, b => $"0x{b:X2}"));

        // Очищаем строку от нечитаемых управляющих ASCII символов перед выводом на экран
        string ascii = Encoding.UTF8.GetString(bytes);
        string cleanAscii = new(ascii.Where(c => !char.IsControl(c)).ToArray());

        return $"{cleanAscii}({hexValues})";
    }
}
