using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ITokenService, TokenService>()
            .AddTransient<IJanusPluginService, JanusPluginService>()
            .AddScoped<JanusTextroomService>()
            .AddScoped<JanusVideoRoomService>()
            .AddSingleton<IHlsService, HlsService>();
    }
}