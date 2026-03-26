using System.Text.Json;
using mvc_blogs.Models;

namespace mvc_blogs.Services;

/// <summary>
/// JSON-file-backed implementation of IBlogService.
/// Uses in-memory seed data for Authors, Categories, Tags, and Comments.
/// Replace with an EF Core DbContext for a real database backend.
/// </summary>
public class BlogService : IBlogService
{
    private readonly string _dataDirectory;
    private readonly ILogger<BlogService> _logger;

    // --------------- In-memory seed data (replace with DB tables) ---------------
    private static readonly List<Author> _authors =
    [
        new() { Id = 1, Name = "Alice Tan",   Email = "alice@blog.dev",   Bio = "Full-stack engineer & open-source contributor.", TwitterHandle = "@alicetan",  JoinedAt = new DateTime(2023, 1, 15), IsActive = true },
        new() { Id = 2, Name = "Bob Reyes",   Email = "bob@blog.dev",     Bio = "DevOps advocate and cloud architect.",           TwitterHandle = "@bobreyes",   JoinedAt = new DateTime(2023, 4, 20), IsActive = true },
        new() { Id = 3, Name = "Cara Santos", Email = "cara@blog.dev",    Bio = "UX designer turned front-end developer.",       TwitterHandle = "@carasantos", JoinedAt = new DateTime(2024, 2, 10), IsActive = true },
    ];

    private static readonly List<Category> _categories =
    [
        new() { Id = 1, Name = "Technology",    Slug = "technology",    Description = "Latest in tech and software.",        IconClass = "bi-cpu",          CreatedAt = DateTime.UtcNow },
        new() { Id = 2, Name = "Design",        Slug = "design",        Description = "UI/UX, typography, and aesthetics.", IconClass = "bi-palette",      CreatedAt = DateTime.UtcNow },
        new() { Id = 3, Name = "DevOps",        Slug = "devops",        Description = "CI/CD, containers, and the cloud.",  IconClass = "bi-cloud-upload", CreatedAt = DateTime.UtcNow },
        new() { Id = 4, Name = "Career",        Slug = "career",        Description = "Advice for developers.",             IconClass = "bi-briefcase",    CreatedAt = DateTime.UtcNow },
    ];

    private static readonly List<Tag> _tags =
    [
        new() { Id = 1, Name = "ASP.NET Core", Slug = "aspnet-core", Color = "#512bd4" },
        new() { Id = 2, Name = "C#",           Slug = "csharp",      Color = "#239120" },
        new() { Id = 3, Name = "Docker",       Slug = "docker",      Color = "#2496ed" },
        new() { Id = 4, Name = "CSS",          Slug = "css",         Color = "#264de4" },
        new() { Id = 5, Name = "Career",       Slug = "career",      Color = "#f59e0b" },
    ];

    // Thread-safe in-memory comment store (persists only during app lifetime)
    private static readonly List<Comment> _comments =
    [
        new() { Id = 1, PostId = 1, AuthorName = "Jane",  AuthorEmail = "jane@x.com", Body = "Great post!",         CreatedAt = DateTime.UtcNow, IsApproved = true },
        new() { Id = 2, PostId = 1, AuthorName = "Mark",  AuthorEmail = "mark@x.com", Body = "Very informative.",   CreatedAt = DateTime.UtcNow, IsApproved = true },
        new() { Id = 3, PostId = 2, AuthorName = "Lena",  AuthorEmail = "lena@x.com", Body = "Loved the examples.", CreatedAt = DateTime.UtcNow, IsApproved = true },
    ];
    private static int _nextCommentId = 4;
    private static readonly Lock _commentLock = new();
    // ---------------------------------------------------------------------------

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BlogService(IWebHostEnvironment env, ILogger<BlogService> logger)
    {
        // Data folder: <ContentRoot>/Data — works in dev and after publish
        _dataDirectory = Path.Combine(env.ContentRootPath, "Data");
        _logger = logger;
    }

    // ─── Posts ────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        var posts = await LoadPostsAsync();
        return posts.Where(p => p.IsPublished).OrderByDescending(p => p.PublishDate);
    }

    public async Task<Post?> GetPostByIdAsync(int id)
    {
        var posts = await LoadPostsAsync();
        return posts.FirstOrDefault(p => p.Id == id && p.IsPublished);
    }

    public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(int categoryId)
    {
        var posts = await LoadPostsAsync();
        return posts.Where(p => p.CategoryId == categoryId && p.IsPublished)
                    .OrderByDescending(p => p.PublishDate);
    }

    public async Task<IEnumerable<Post>> GetPostsByTagAsync(int tagId)
    {
        var posts = await LoadPostsAsync();
        return posts.Where(p => p.TagIds.Contains(tagId) && p.IsPublished)
                    .OrderByDescending(p => p.PublishDate);
    }

    public async Task<IEnumerable<Post>> GetFeaturedPostsAsync()
    {
        var posts = await LoadPostsAsync();
        return posts.Where(p => p.IsFeatured && p.IsPublished)
                    .OrderByDescending(p => p.PublishDate);
    }

    public async Task<IEnumerable<Post>> SearchPostsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];

        var posts = await LoadPostsAsync();
        var lower = query.ToLowerInvariant();
        return posts.Where(p =>
            p.IsPublished && (
                p.Title.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
                p.Excerpt.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
                p.Author.Contains(lower, StringComparison.OrdinalIgnoreCase)
            )).OrderByDescending(p => p.PublishDate);
    }

    // ─── Authors ──────────────────────────────────────────────────────────────

    public Task<IEnumerable<Author>> GetAllAuthorsAsync() =>
        Task.FromResult<IEnumerable<Author>>(_authors.Where(a => a.IsActive));

    public Task<Author?> GetAuthorByIdAsync(int id) =>
        Task.FromResult(_authors.FirstOrDefault(a => a.Id == id));

    // ─── Categories ───────────────────────────────────────────────────────────

    public Task<IEnumerable<Category>> GetAllCategoriesAsync() =>
        Task.FromResult<IEnumerable<Category>>(_categories);

    public Task<Category?> GetCategoryByIdAsync(int id) =>
        Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

    public Task<Category?> GetCategoryBySlugAsync(string slug) =>
        Task.FromResult(_categories.FirstOrDefault(c =>
            c.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase)));

    // ─── Tags ─────────────────────────────────────────────────────────────────

    public Task<IEnumerable<Tag>> GetAllTagsAsync() =>
        Task.FromResult<IEnumerable<Tag>>(_tags);

    public Task<Tag?> GetTagByIdAsync(int id) =>
        Task.FromResult(_tags.FirstOrDefault(t => t.Id == id));

    public Task<Tag?> GetTagBySlugAsync(string slug) =>
        Task.FromResult(_tags.FirstOrDefault(t =>
            t.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase)));

    // ─── Comments ─────────────────────────────────────────────────────────────

    public Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId)
    {
        var result = _comments
            .Where(c => c.PostId == postId && c.IsApproved)
            .OrderBy(c => c.CreatedAt);
        return Task.FromResult<IEnumerable<Comment>>(result);
    }

    public Task<bool> AddCommentAsync(Comment comment)
    {
        lock (_commentLock)
        {
            comment.Id = _nextCommentId++;
            comment.CreatedAt = DateTime.UtcNow;
            comment.IsApproved = false; // pending moderation
            _comments.Add(comment);
        }
        return Task.FromResult(true);
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private async Task<List<Post>> LoadPostsAsync()
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, "posts.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("posts.json not found at {Path}", filePath);
                return [];
            }

            await using var stream = File.OpenRead(filePath);
            var posts = await JsonSerializer.DeserializeAsync<List<Post>>(stream, _jsonOptions);
            return posts ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load posts from JSON");
            return [];
        }
    }
}
