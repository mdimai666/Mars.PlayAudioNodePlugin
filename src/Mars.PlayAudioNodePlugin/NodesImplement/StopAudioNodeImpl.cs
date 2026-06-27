using Mars.Nodes.Core;
using Mars.Nodes.Host.Shared;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;
using Microsoft.Extensions.DependencyInjection;

namespace Mars.PlayAudioNodePlugin.NodesImplement;

public class StopAudioNodeImpl : INodeImplement<StopAudioNode>
{
    public StopAudioNode Node { get; }
    public IRuntimeNodeScope RNS { get; set; }
    Node INodeImplement.Node => Node;
    IPlayAudioService _playAudioService;

    public StopAudioNodeImpl(StopAudioNode node, IRuntimeNodeScope rns)
    {
        Node = node;
        RNS = rns;

        _playAudioService = rns.ServiceProvider.GetRequiredService<IPlayAudioService>();
    }

    public Task Execute(NodeMsg input, ExecuteAction callback, ExecutionParameters parameters)
    {
        _playAudioService.StopAll();
        RNS.DebugMsg(DebugMessage.NodeMessage(Node.Id, "stop all audio"));
        return Task.CompletedTask;
    }
}
