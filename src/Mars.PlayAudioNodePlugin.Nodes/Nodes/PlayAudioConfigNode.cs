using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mars.Core.Attributes;
using Mars.Nodes.Core.Nodes;

namespace Mars.PlayAudioNodePlugin.Nodes.Nodes;

[FunctionApiDocument("./_plugin/Mars.PlayAudioNodePlugin/docs/PlayAudioConfigNode/PlayAudioConfigNode{.lang}.md")]
public class PlayAudioConfigNode : ConfigNode
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string DefaultOutputDevice { get; set; } = "";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Display(Name = "volume", Description = "0..1")]
    [Range(0, 1f)]
    public float DefaultPlayVolume { get; set; } = 1f;
}
