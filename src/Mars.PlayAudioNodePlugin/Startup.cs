using Mars.Host.Shared.Services;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Mars.PlayAudioNodePlugin;
using Mars.PlayAudioNodePlugin.Host;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Nodes;
using Mars.PlayAudioNodePlugin.Nodes.Forms;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;
using Mars.PlayAudioNodePlugin.NodesImplement;
using Mars.Plugin.Abstractions;
using Mars.Plugin.PluginHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebApplicationPlugin(typeof(MainMarsPlayAudioNodePlugin))]

namespace Mars.PlayAudioNodePlugin;

public class MainMarsPlayAudioNodePlugin : WebApplicationPlugin
{
    public const string PluginPackageName = "mdimai666.Mars.PlayAudioNodePlugin";

    public override void ConfigureWebApplicationBuilder(WebApplicationBuilder builder, PluginSettings settings)
    {
        builder.Services.AddPlayAudioService();
    }

    public override void ConfigureWebApplication(WebApplication app, PluginSettings settings)
    {
        NodesLocator.RegisterAssembly(typeof(PlayAudioNode).Assembly);
        NodeFormsLocator.RegisterAssembly(typeof(PlayAudioNodeForm).Assembly);
        NodeImplementFabirc.RegisterAssembly(typeof(PlayAudioNodeImpl).Assembly);

        var logger = MarsLogger.GetStaticLogger<MainMarsPlayAudioNodePlugin>();

        app.MapGet("/api/PlayAudioNodePlugin/OutputDevices", (IPlayAudioService pas) =>
        {
            var devices = pas.OutputDevices();
            return devices;
        });

#if DEBUG
        app.UseDevelopingServePluginFilesDefinition(this.GetType().Assembly, settings, [typeof(PlayAudioNodePluginFront).Assembly]);
#endif

        //var op = app.Services.GetRequiredService<IOptionService>();
        //op.RegisterOption<Example1Plugin1>(appendToInitialSiteData: true);
        //op.SetConstOption(new Example1PluginConstOptionForFront() { ForFrontValue = "123" }, appendToInitialSiteData: true);
    }

}
