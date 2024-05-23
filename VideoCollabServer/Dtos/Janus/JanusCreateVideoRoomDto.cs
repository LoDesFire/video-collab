using System.Text.Json.Serialization;

namespace VideoCollabServer.Dtos;

public record JanusCreateVideoRoomDto: JanusTextroomDto
{
    [JsonPropertyName("admin_key")] public string AdminKey { get; set; } = null!;
    [JsonPropertyName("is_private")] public bool Private { get; set; } = true;
    public int Publishers { get; set; } = 4;
    public int Bitrate { get; set; } = 128000;
    [JsonPropertyName("bitrate_cap")] public bool BitrateCap { get; set; } = true;
    [JsonPropertyName("fir_freq")] public int FirFreq { get; set; } = 2;
    public string Audiocodec { get; set; } = "opus,isac16";
    public string Videocodec { get; set; } = "vp9,h264,av1,h265";
    [JsonPropertyName("vp9_profile")] public int Vp9Profile { get; set; } = 1;
    [JsonPropertyName("lock_record")] public bool LockRecord { get; set; } = true;
}

// room-<unique room ID>: {
//         description = This is my awesome room
//         is_private = будет
//         secret = <optional password needed for manipulating (e.g. destroying) the room>
//         publishers = 4
//         bitrate = 128000
//         bitrate_cap = true
//         fir_freq = 2
//         audiocodec = opus,isac16
//         videocodec = vp9,h264,av1,h265 
//         vp9_profile = 1
//         lock_record = true
// }