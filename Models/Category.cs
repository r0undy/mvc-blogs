using System.ComponentModel.DataAnnotations;

namespace mvc_blogs.Models;

public class Category
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>URL-friendly slug, e.g. "tech-news"</summary>
    [Required, StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    public string IconClass { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
