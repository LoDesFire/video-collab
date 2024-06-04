using Microsoft.Extensions.DependencyInjection;
using VideoCollab.Core.Domain.Abstractions;

namespace VideoCollab.Core.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ITokenService, TokenService>()
            .AddTransient<IJanusPluginService, JanusPluginService>()
            .AddTransient<IRoomService, RoomService>()
            .AddTransient<IMovieService, MovieService>()
            .AddTransient<IUserService, UserService>()
            .AddScoped<JanusTextroomService>()
            .AddScoped<JanusVideoRoomService>()
            .AddSingleton<IHlsService, HlsService>();
    }
}