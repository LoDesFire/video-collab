using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Dtos.Room;

namespace VideoCollabServer.Dtos.User;

public record UserProfileDto
{
    public List<PinnedMovieDto> PinnedMovies { get; set; } = null!;
    public List<UserRecentCallDto> Users { get; set; } = null!;
    public List<RoomProfileDto> OwnedRooms { get; set; } = null!;
}