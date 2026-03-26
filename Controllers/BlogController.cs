using Microsoft.AspNetCore.Mvc;
using mvc_blogs.Models;
using mvc_blogs.Services;

namespace mvc_blogs.Controllers;

public class BlogController(IBlogService blogService, ILogger<BlogController> logger) : Controller
{
    // GET /Blog
    public async Task<IActionResult> Index()
    {
        var posts = await blogService.GetAllPostsAsync();
        return View(posts);
    }

    // GET /Blog/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var post = await blogService.GetPostByIdAsync(id);
        if (post is null)
        {
            logger.LogWarning("Post {Id} not found", id);
            return NotFound();
        }

        ViewData["Comments"] = await blogService.GetCommentsByPostAsync(id);
        return View(post);
    }

    // GET /Blog/Search?q=docker
    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return RedirectToAction(nameof(Index));

        var results = await blogService.SearchPostsAsync(q);
        ViewData["Query"] = q;
        return View(results);
    }

    // GET /Blog/Featured
    public async Task<IActionResult> Featured()
    {
        var posts = await blogService.GetFeaturedPostsAsync();
        return View(posts);
    }

    // POST /Blog/Comment
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment(Comment comment)
    {
        if (!ModelState.IsValid)
        {
            TempData["CommentError"] = "Please fill in all required fields correctly.";
            return RedirectToAction(nameof(Details), new { id = comment.PostId });
        }

        await blogService.AddCommentAsync(comment);
        TempData["CommentSuccess"] = "Your comment has been submitted for moderation.";
        return RedirectToAction(nameof(Details), new { id = comment.PostId });
    }
}
