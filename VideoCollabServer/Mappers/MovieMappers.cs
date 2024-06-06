using VideoCollab.Core.Domain.Models;
using VideoCollabServer.Dtos.Movie;

namespace VideoCollabServer.Mappers;

public static class MovieMappers
{
    public static PinnedMovieDto ToPinnedDto(this Movie movieModel)
    {
        return new PinnedMovieDto
        {
            Id = movieModel.Id,
            Description = movieModel.Description,
            ImageUrl = movieModel.Links.Find(l => l.Type == LinkType.Image && l.Movie?.Id == movieModel.Id)?.Url,
            Name = movieModel.Name
        };
    }
    
    public static MovieItemDto ToItemDto(this Movie movieModel, string userId)
    {
        return new MovieItemDto
        {
            Id = movieModel.Id,
            Description = movieModel.Description,
            ImageUrl = movieModel.Links.Find(l => l.Type == LinkType.Image && l.Movie?.Id == movieModel.Id)?.Url,
            Name = movieModel.Name,
            Pinned = movieModel.UsersPinnedMovie.Select(u => u.Id).Contains(userId)
        };
    }

    public static MoviePageDto ToMoviePageDto(this Movie movieModel)
    {
        return new MoviePageDto
        {
            Id = movieModel.Id,
            Name = movieModel.Name,
            ImageUrl = movieModel.Links.FirstOrDefault(l => l.Type == LinkType.Image)?.Url,
            TrailerUrl = movieModel.Links.FirstOrDefault(l => l.Type == LinkType.Trailer)?.Url,
            Description = movieModel.Description,
            Status = movieModel.Status.ToString()
        };
    }
}