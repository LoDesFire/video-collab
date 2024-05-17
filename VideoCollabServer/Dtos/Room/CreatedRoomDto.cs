using VideoCollabServer.Dtos.User;

namespace VideoCollabServer.Dtos.Room;

public record CreatedRoomDto
{
    public string Id { get; set; } = null!;

    public JoinedUserDto Owner { get; set; } = null!;
}