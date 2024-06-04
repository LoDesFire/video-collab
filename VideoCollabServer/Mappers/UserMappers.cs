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
            Users = userModel.RecentCallUsers.Select(u => u.ToRecentCallDto()).ToList()
        };
    }

    private static UserRecentCallDto ToRecentCallDto(this User userModel)
    {
        return new UserRecentCallDto
        {
            Id = userModel.Id,
            Username = userModel.UserName
        };
    }
}