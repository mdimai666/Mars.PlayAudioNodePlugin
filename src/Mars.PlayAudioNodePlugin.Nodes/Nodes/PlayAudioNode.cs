using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mars.Core.Attributes;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes;

namespace Mars.PlayAudioNodePlugin.Nodes.Nodes;

[FunctionApiDocument("./_plugin/Mars.PlayAudioNodePlugin/docs/PlayAudioNode/PlayAudioNode{.lang}.md")]
public class PlayAudioNode : Node
{
    public InputConfig<PlayAudioConfigNode> Config { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Display(Name = "File uri", Description = "empty for Play buffer from msg.Payload")]
    public string AudioUri { get; set; } = "";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Display(Name = "Set play volume", Description = "-1 by default")]
    [Range(-1f, 1f)]
    public float Volume { get; set; } = -1;

    public PlayAudioNode()
    {
        HaveInput = true;
        Color = "#e84468";
        Outputs = new List<NodeOutput> { new NodeOutput() { Label = "after play" } };
        Icon = "/_plugin/Mars.PlayAudioNodePlugin/icons/play-sound.svg";
    }
}
