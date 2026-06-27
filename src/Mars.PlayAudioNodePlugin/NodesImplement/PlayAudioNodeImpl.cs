using Mars.Nodes.Core;
using Mars.Nodes.Core.Exceptions;
using Mars.Nodes.Host.Shared;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;
using Microsoft.Extensions.DependencyInjection;

namespace Mars.PlayAudioNodePlugin.NodesImplement;

public class PlayAudioNodeImpl : INodeImplement<PlayAudioNode>
{
    public PlayAudioNode Node { get; }
    public IRuntimeNodeScope RNS { get; set; }
    Node INodeImplement.Node => Node;
    IPlayAudioService _playAudioService;

    public PlayAudioNodeImpl(PlayAudioNode node, IRuntimeNodeScope rns)
    {
        Node = node;
        RNS = rns;

        Node.Config = RNS.GetConfig(node.Config);
        _playAudioService = rns.ServiceProvider.GetRequiredService<IPlayAudioService>();
    }

    public async Task Execute(NodeMsg input, ExecuteAction callback, ExecutionParameters parameters)
    {
        var volume = ResolveVolume();
        var outputDeviceId = ResolveOutputDeviceId();

        //replace by FileStorage

        if (!string.IsNullOrEmpty(Node.AudioUri))
        {
            await _playAudioService.Play(Node.AudioUri, volume, outputDeviceId);
        }
        else if (input.Payload is string payloadFilePath)
        {
            await _playAudioService.Play(payloadFilePath, volume, outputDeviceId);
        }
        else if (input.Payload is byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            await _playAudioService.Play(ms, volume, outputDeviceId);
        }
        else
        {
            throw new NodeExecuteException(Node, $"input.Payload '{input.Payload}' not support");
        }

        callback(input);

    }

    float ResolveVolume() => Node.Volume > 0 ? Node.Volume : Node.Config.Value?.DefaultPlayVolume ?? 1f;
    string ResolveOutputDeviceId() => Node.Config.Value?.DefaultOutputDevice ?? "";
}
