using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes.Common;
using Mars.Nodes.Core.Utils;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;

namespace Mars.PlayAudioNodePlugin.Nodes.Examples;

public class PlayAudioNodeUseBuiltInSoundExample : INodeExample<PlayAudioNode>
{
    public string Name => "Use builtin sound";
    public string Description => "";

    public IReadOnlyCollection<Node> Handle(IEditorState editorState)
    {
        return NodesWorkflowBuilder.Create()
            .AddNext(new InjectNode())
            .AddNext(new PlayAudioNode
            {
                PlayFromBuiltInSounds = true,
                BuiltInSoundsName = "RU/Задача выполнена.wav",
            })
            .Build();
    }
}
