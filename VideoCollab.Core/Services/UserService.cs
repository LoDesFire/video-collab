using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Services;

public class UserService(IUserRepository userRepository, IMovieRepository movieRepository): IUserService
{
    public async Task<User?> GetByIdAsync(string id)
    {
        return await userRepository.GetUserProfileByIdAsync(id);
    }

    public async Task<bool> PinMovieAsync(string id, int movieId)
    {
        var user = await userRepository.GetUserPinnedMoviesByIdAsync(id);
        var movie = await movieRepository.GetMovieByIdAsync(movieId);
        
        if (movie == null) 
            return false;
    
        if (user.PinnedMovies.Contains(movie))
            return true;

        movie.UsersPinnedMovie.Add(user);

        await userRepository.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UnpinMovieAsync(string id, int movieId)
    {
        var user = await userRepository.GetUserByIdAsync(id);

        var movie = await movieRepository.GetMoviePinnedByIdAsync(movieId);
        
        if (movie == null || !movie.UsersPinnedMovie.Contains(user))
            return false;
        
        movie.UsersPinnedMovie.Remove(user);
        await userRepository.SaveChangesAsync();
        return true;
    }
}