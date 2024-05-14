using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Mappers;

namespace VideoCollabServer.Repositories;

public class UserRepository(ApplicationContext context, IAuthService authService) : IUserRepository
{
    private ApplicationContext Context { get; } = context;
    private IAuthService AuthService { get; } = authService;

    public async Task<UserProfileDto?> GetByIdAsync(string id)
    {
        var user = await Context.Users
            .Include(u => u.PinnedMovies)
            .ThenInclude(m => m.Links)
            .Include(u => u.OwnedRooms)
            .ThenInclude(r => r.Playlist)
            .FirstAsync(u => u.Id == id);

        return user.ToProfileDto();
    }

    public async Task<bool> PinMovieAsync(string id, int movieId)
    {
        var user = await Context.Users
            .Include(u => u.PinnedMovies)
            .FirstAsync(u => u.Id == id);
        var movie = await Context.Movies.FindAsync(movieId);
        
        if (movie == null) 
            return false;

        if (user.PinnedMovies.Contains(movie))
            return true;

        movie.UsersPinnedMovie.Add(user);

        await Context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UnpinMovieAsync(string id, int movieId)
    {
        var user = await Context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
        
        var movie = await Context.Movies
            .Include(m => m.UsersPinnedMovie)
            .FirstOrDefaultAsync(m => m.Id == movieId);
        
        if (movie == null || user == null || !movie.UsersPinnedMovie.Contains(user))
            return false;
        
        movie.UsersPinnedMovie.Remove(user);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<AuthedUserDto>> CreateAsync(AuthUserDto authUserDto)
    {
        return await AuthService.RegisterAsync(authUserDto);
    }

    public async Task<Result<AuthedUserDto>> LoginAsync(AuthUserDto authUserDto)
    {
        return await AuthService.LoginAsync(authUserDto);
    }
}