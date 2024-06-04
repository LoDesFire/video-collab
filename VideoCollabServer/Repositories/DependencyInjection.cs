using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ILinkRepository, LinkRepository>()
            .AddScoped<IMovieRepository, MovieRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRoomRepository, RoomRepository>()
            .AddScoped<ITranscodingMovieRepository, TranscodingMovieRepository>();
    }
}