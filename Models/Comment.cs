using System.ComponentModel.DataAnnotations;

namespace mvc_blogs.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public int PostId { get; set; }

    [Required, StringLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string AuthorEmail { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Moderator-approved before showing publicly.</summary>
    public bool IsApproved { get; set; } = false;
}
