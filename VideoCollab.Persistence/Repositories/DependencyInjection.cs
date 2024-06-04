using Microsoft.Extensions.DependencyInjection;
using VideoCollab.Core.Domain.Abstractions;

namespace VideoCollab.Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ILinkRepository, LinkRepository>()
            .AddScoped<IMovieRepository, MovieRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRoomRepository, RoomRepository>();
    }
}