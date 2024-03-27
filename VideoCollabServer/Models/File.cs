namespace VideoCollabServer.Models;

public enum FileType
{
    Video,
    Audio
}

public class File
{
    public int Id { get; set; }
    public string Path { get; set; } = null!;
    public FileType Type { get; set; }
    public Movie? Movie { get; set; }
}