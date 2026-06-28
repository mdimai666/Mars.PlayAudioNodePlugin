using Mars.Host.Shared.Services;
using Mars.PlayAudioNodePlugin;
using Mars.PlayAudioNodePlugin.Host;
using Mars.PlayAudioNodePlugin.Host.Services;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.PlayAudioNodePlugin.Nodes;
using Mars.PlayAudioNodePlugin.Nodes.Nodes;
using Mars.Plugin.Abstractions;
using Mars.Plugin.Kit.Host;
using Mars.Plugin.PluginHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: WebApplicationPlugin(typeof(MainMarsPlayAudioNodePlugin))]

namespace Mars.PlayAudioNodePlugin;

public class MainMarsPlayAudioNodePlugin : WebApplicationPlugin
{
    public const string PluginPackageName = "mdimai666.Mars.PlayAudioNodePlugin";

    public override void ConfigureWebApplicationBuilder(WebApplicationBuilder builder, PluginSettings settings)
    {
        builder.Services.AddPlayAudioService(settings, GetType().Assembly);
    }

    public override void ConfigureWebApplication(WebApplication app, PluginSettings settings)
    {
        app.Services.AutoHostRegisterHelper([GetType().Assembly, typeof(PlayAudioNode).Assembly]);

        var logger = MarsLogger.GetStaticLogger<MainMarsPlayAudioNodePlugin>();

        app.MapGet("/api/PlayAudioNodePlugin/OutputDevices", (IPlayAudioService pas) =>
        {
            return pas.OutputDevices();
        });

        app.MapGet("/api/PlayAudioNodePlugin/BuiltInSounds", (BuiltInSoundsService ss) =>
        {
            return TypedResults.Ok(ss.SoundList());
        });

        app.MapPut("/api/PlayAudioNodePlugin/DemoPlayBuiltInSound", ([FromBody] string soundName, BuiltInSoundsService ss, IPlayAudioService pas) =>
        {
            var filePath = ss.FullPathByName(soundName);
            _ = pas.Play(filePath, 1f);
            return TypedResults.Ok();
        });

#if DEBUG
        app.UseDevelopingServePluginFilesDefinition(GetType().Assembly, settings, [typeof(PlayAudioNodePluginFront).Assembly, GetType().Assembly]);
#endif

        //var op = app.Services.GetRequiredService<IOptionService>();
        //op.RegisterOption<Example1Plugin1>(appendToInitialSiteData: true);
        //op.SetConstOption(new Example1PluginConstOptionForFront() { ForFrontValue = "123" }, appendToInitialSiteData: true);
    }

}
