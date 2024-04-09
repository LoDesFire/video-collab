using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Mappers;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class UserRepository : IUserRepository
{
    private ApplicationContext Context { get; set; }
    private IAuthService AuthService { get; set; }

    public UserRepository(ApplicationContext context, IAuthService authService)
    {
        Context = context;
        AuthService = authService;
    }

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
            .Include(u => u.PinnedMovies)
            .FirstAsync(u => u.Id == id);
        var movie = await Context.Movies.FindAsync(movieId);
        
        if (movie == null || !movie.UsersPinnedMovie.Contains(user))
            return false;
        
        movie.UsersPinnedMovie.Remove(user);
        
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<AuthResult> CreateAsync(AuthUserDto authUserDto)
    {
        return await AuthService.RegisterAsync(authUserDto);
    }

    public async Task<AuthResult> LoginAsync(AuthUserDto authUserDto)
    {
        return await AuthService.LoginAsync(authUserDto);
    }
}