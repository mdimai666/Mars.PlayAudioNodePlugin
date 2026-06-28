using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mars.Core.Attributes;
using Mars.Core.Extensions;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Fields;

namespace Mars.PlayAudioNodePlugin.Nodes.Nodes;

[FunctionApiDocument("./_plugin/Mars.PlayAudioNodePlugin/docs/PlayAudioNode/PlayAudioNode{.lang}.md")]
public class PlayAudioNode : Node
{
    public override string DisplayName => Name.AsNullIfEmpty() ?? (PlayFromBuiltInSounds ? BuiltInSoundsName : base.DisplayName);

    public InputConfig<PlayAudioConfigNode> Config { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Display(Name = "File uri", Description = "empty for Play buffer from msg.Payload")]
    public string AudioUri { get; set; } = "";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Display(Name = "Set play volume", Description = "-1 by default. Values above 1.0 amplify the sound.")]
    [Range(-1f, float.MaxValue)]
    public float Volume { get; set; } = -1;

    public bool PlayFromBuiltInSounds { get; set; }
    public string BuiltInSoundsName { get; set; } = "success-1.mp3";

    public PlayAudioNode()
    {
        Inputs = [new()];
        Color = "#e84468";
        Outputs = [new() { Label = "after play" }];
        Icon = "/_plugin/Mars.PlayAudioNodePlugin/icons/play-sound.svg";
    }
}
