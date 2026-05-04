using FluentAssertions;
using Mars.PlayAudioNodePlugin.Host.Services;

namespace Test.PlayAudioNodePlugin;

public class AudioPlayTests
{
    private const string AudioSamplesPath = @"../../../Samples";

    public static IEnumerable<object[]> AudioListData()
    {
        yield return new object[] { "game-win.mp3" };
        yield return new object[] { "guiclick.ogg" };
        yield return new object[] { "streat_audio_without-header.wav" };
    }

    [Theory]
    [MemberData(nameof(AudioListData))]
    public async Task PlayByPath_AnyFormat_ShuldSuccess(string fileName)
    {
        //Arrange
        var pas = new PlayAudioService();
        var file = Path.Combine(AudioSamplesPath, fileName);

        //Act
        var action = () => pas.Play(file);

        //Assert
        await action.Should().NotThrowAsync();
    }

    [Theory]
    [MemberData(nameof(AudioListData))]
    public async Task PlayStream_AnyFormat_ShuldSuccess(string fileName)
    {
        //Arrange
        var pas = new PlayAudioService();
        var file = Path.Combine(AudioSamplesPath, fileName);
        var bytes = await File.ReadAllBytesAsync(file);
        var ms = new MemoryStream(bytes);

        //Act
        var action = () => pas.Play(ms);

        //Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Stop_BreakAudio_ShuldStop()
    {
        //Arrange
        var pas = new PlayAudioService();
        var file = Path.Combine(AudioSamplesPath, "game-win.mp3");

        //Act
        _ = pas.Play(file);
        await Task.Delay(500);
        pas.StopAll();

        //Assert
        // just listen
    }
}
