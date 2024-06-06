using VideoCollab.Core.Domain.Models;
using VideoCollabServer.Dtos.User;

namespace VideoCollabServer.Mappers;

public static class UserMappers
{
    public static UserProfileDto ToProfileDto(this User userModel)
    {
        return new UserProfileDto
        {
            OwnedRooms = userModel.OwnedRooms.Select(r => r.ToProfileDto()).ToList(),
            PinnedMovies = userModel.PinnedMovies.Select(m => m.ToPinnedDto()).ToList(),
        };
    }
}