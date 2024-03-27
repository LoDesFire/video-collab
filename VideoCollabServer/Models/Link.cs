namespace VideoCollabServer.Models;

public enum LinkType
{
    Image,
    Trailer,
    Film,
    Room
}

public class Link
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public LinkType Type { get; set; }
    public Movie? Movie { get; set; }
}