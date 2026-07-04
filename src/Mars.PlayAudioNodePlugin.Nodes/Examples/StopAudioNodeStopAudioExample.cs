using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes.Common;
using Mars.Nodes.Core.Nodes.Functions;
using Mars.Nodes.Core.Utils;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;

namespace Mars.PlayAudioNodePlugin.Nodes.Examples;

public class StopAudioNodeStopAudioExample : INodeExample<StopAudioNode>
{
    public string Name => "Stop play audio";
    public string Description => "";

    public IReadOnlyCollection<Node> Handle(IEditorState editorState)
    {
        return NodesWorkflowBuilder.Create()
            .AddNext(new InjectNode())
            .AddNext(NodesWorkflowBuilder.Create()
                        .AddNext(new PlayAudioNode
                        {
                            PlayFromBuiltInSounds = true,
                            BuiltInSoundsName = "RU/Задача выполнена.wav",
                        }),
                     NodesWorkflowBuilder.Create()
                        .AddNext(new DelayNode { DelayMillis = 500 })
                        .AddNext(new StopAudioNode())
                    )
            .Build();
    }
}
