using Microsoft.AspNetCore.Mvc;
using mvc_blogs.Services;

namespace mvc_blogs.Controllers;

public class AuthorController(IBlogService blogService, ILogger<AuthorController> logger) : Controller
{
    // GET /Author
    public async Task<IActionResult> Index()
    {
        var authors = await blogService.GetAllAuthorsAsync();
        return View(authors);
    }

    // GET /Author/Profile/1
    public async Task<IActionResult> Profile(int id)
    {
        var author = await blogService.GetAuthorByIdAsync(id);
        if (author is null)
        {
            logger.LogWarning("Author {Id} not found", id);
            return NotFound();
        }

        // Fetch all posts and filter by author name (production: use AuthorId FK)
        var allPosts = await blogService.GetAllPostsAsync();
        var authorPosts = allPosts.Where(p =>
            p.Author.Equals(author.Name, StringComparison.OrdinalIgnoreCase));

        ViewData["AuthorPosts"] = authorPosts;
        return View(author);
    }
}
