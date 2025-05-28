using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;
using Microsoft.Extensions.DependencyInjection;

namespace Mars.PlayAudioNodePlugin.NodesImplement;

public class StopAudioNodeImpl : INodeImplement<StopAudioNode>, INodeImplement
{
    public StopAudioNode Node { get; }
    public IRED RED { get; set; }
    Node INodeImplement<Node>.Node => Node;
    IPlayAudioService _playAudioService;

    public StopAudioNodeImpl(StopAudioNode node, IRED red)
    {
        Node = node;
        RED = red;

        _playAudioService = red.ServiceProvider.GetRequiredService<IPlayAudioService>();
    }

    public Task Execute(NodeMsg input, ExecuteAction callback)
    {
        _playAudioService.StopAll();
        RED.DebugMsg(DebugMessage.NodeMessage(Node.Id, "stop all audio"));
        return Task.CompletedTask;
    }
}
