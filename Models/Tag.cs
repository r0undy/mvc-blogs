using System.ComponentModel.DataAnnotations;

namespace mvc_blogs.Models;

public class Tag
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>URL-friendly slug, e.g. "asp-net-core"</summary>
    [Required, StringLength(50)]
    public string Slug { get; set; } = string.Empty;

    public string Color { get; set; } = "#6366f1"; // default indigo
}
