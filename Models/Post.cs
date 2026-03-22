namespace mvc_blogs.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string ThumbnailImage { get; set; } = string.Empty;
    public string HeroImage { get; set; } = string.Empty;
    public string AuthorImage { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
}
