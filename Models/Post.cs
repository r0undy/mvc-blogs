using System.ComponentModel.DataAnnotations;

namespace mvc_blogs.Models;

public class Post
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Excerpt { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string ThumbnailImage { get; set; } = string.Empty;
    public string HeroImage { get; set; } = string.Empty;
    public string AuthorImage { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }

    // Extended fields
    public int CategoryId { get; set; }
    public List<int> TagIds { get; set; } = [];
    public int ViewCount { get; set; } = 0;
    public bool IsPublished { get; set; } = true;
}
