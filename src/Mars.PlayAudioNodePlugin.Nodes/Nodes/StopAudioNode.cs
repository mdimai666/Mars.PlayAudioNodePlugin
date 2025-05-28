using Mars.Core.Attributes;
using Mars.Nodes.Core;

namespace Mars.PlayAudioNodePlugin.Nodes.Nodes;

[FunctionApiDocument("./_plugin/Mars.PlayAudioNodePlugin/docs/StopAudioNode/StopAudioNode{.lang}.md")]
public class StopAudioNode : Node
{
    public StopAudioNode()
    {
        HaveInput = true;
        Color = "#e84468";
        Outputs = [];
        Icon = "/_plugin/Mars.PlayAudioNodePlugin/icons/stop-sound.svg";
    }
}
