using System.Reflection;
using Mars.PlayAudioNodePlugin.Host.Services;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Mars.Plugin.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mars.PlayAudioNodePlugin.Host;

public static class MainPlayAudioNode
{
    public static IServiceCollection AddPlayAudioService(this IServiceCollection services, PluginSettings settings, Assembly mainPluginAssemly)
    {
        services.AddSingleton<IPlayAudioService, PlayAudioService>();
        services.AddSingleton<BuiltInSoundsService>(new BuiltInSoundsService(mainPluginAssemly));

        return services;
    }
}
