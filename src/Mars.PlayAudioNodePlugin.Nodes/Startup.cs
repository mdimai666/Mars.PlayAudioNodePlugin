using Mars.Plugin.Front;
using Mars.Plugin.Front.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Mars.PlayAudioNodePlugin.Nodes;

public class PlayAudioNodePluginFront : IWebAssemblyPluginFront
{
    public void ConfigureServices(WebAssemblyHostBuilder builder)
    {
        //Console.WriteLine("> plugin ConfigureServices!");

        //NodesLocator.RegisterAssembly(typeof(PlayAudioNode).Assembly);
        //NodeFormsLocator.RegisterAssembly(typeof(PlayAudioNodeForm).Assembly);
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
        app.Services.AutoFrontRegisterHelper([GetType().Assembly]);
        //Console.WriteLine("> plugin ConfigureApplication!");
    }
}
