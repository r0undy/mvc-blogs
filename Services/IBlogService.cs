using mvc_blogs.Models;

namespace mvc_blogs.Services;

/// <summary>
/// Abstraction over all blog data access.
/// Swap out with an EF Core implementation later without touching controllers.
/// </summary>
public interface IBlogService
{
    // Posts
    Task<IEnumerable<Post>> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(int id);
    Task<IEnumerable<Post>> GetPostsByCategoryAsync(int categoryId);
    Task<IEnumerable<Post>> GetPostsByTagAsync(int tagId);
    Task<IEnumerable<Post>> GetFeaturedPostsAsync();
    Task<IEnumerable<Post>> SearchPostsAsync(string query);

    // Authors
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);

    // Categories
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category?> GetCategoryBySlugAsync(string slug);

    // Tags
    Task<IEnumerable<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag?> GetTagBySlugAsync(string slug);

    // Comments
    Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId);
    Task<bool> AddCommentAsync(Comment comment);
}
