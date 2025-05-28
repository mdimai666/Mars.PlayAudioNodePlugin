using Mars.PlayAudioNodePlugin.Host.Services;
using Mars.PlayAudioNodePlugin.Host.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Mars.PlayAudioNodePlugin.Host;

public static class MainPlayAudioNode
{
    public static IServiceCollection AddPlayAudioService(this IServiceCollection services)
    {
        services.AddSingleton<IPlayAudioService, PlayAudioService>();

        return services;
    }
}
