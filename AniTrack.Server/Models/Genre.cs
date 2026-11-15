namespace AniTrack.Server.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Anime> Anime { get; set; } = new List<Anime>();
}