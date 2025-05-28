using Mars.Nodes.Core;
using Mars.PlayAudioNodePlugin.Nodes.Forms;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;
using Mars.Plugin.Front.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Mars.PlayAudioNodePlugin.Nodes;

public class PlayAudioNodePluginFront : IWebAssemblyPluginFront
{
    public void ConfigureServices(WebAssemblyHostBuilder builder)
    {
        //Console.WriteLine("> plugin ConfigureServices!");

        NodesLocator.RegisterAssembly(typeof(PlayAudioNode).Assembly);
        NodeFormsLocator.RegisterAssembly(typeof(PlayAudioNodeForm).Assembly);
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
        //Console.WriteLine("> plugin ConfigureApplication!");
    }
}

