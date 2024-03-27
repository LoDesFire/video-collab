using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Models;

namespace VideoCollabServer.Mappers;

public static class MovieMappers
{
    public static MovieProfileRoomDto ToProfileRoomDto(this Movie movieModel)
    {
        return new MovieProfileRoomDto
        {
            Id = movieModel.Id,
            Name = movieModel.Name
        };
    }
    
    public static PinnedMovieDto ToPinnedDto(this Movie movieModel)
    {
        return new PinnedMovieDto
        {
            Id = movieModel.Id,
            Description = movieModel.Description,
            ImageUrl = movieModel.Links.Find(l => l.Type == LinkType.Image || l.Movie?.Id == movieModel.Id)?.Url,
            Name = movieModel.Name
        };
    }
}