using Microsoft.AspNetCore.Mvc;
using mvc_blogs.Services;

namespace mvc_blogs.Controllers;

public class TagController(IBlogService blogService, ILogger<TagController> logger) : Controller
{
    // GET /Tag
    public async Task<IActionResult> Index()
    {
        var tags = await blogService.GetAllTagsAsync();
        return View(tags);
    }

    // GET /Tag/Posts/1  — by ID
    public async Task<IActionResult> Posts(int id)
    {
        var tag = await blogService.GetTagByIdAsync(id);
        if (tag is null)
        {
            logger.LogWarning("Tag {Id} not found", id);
            return NotFound();
        }

        var posts = await blogService.GetPostsByTagAsync(id);
        ViewData["Tag"] = tag;
        return View(posts);
    }

    // GET /Tag/Browse/csharp  — by slug (SEO-friendly)
    public async Task<IActionResult> Browse(string slug)
    {
        var tag = await blogService.GetTagBySlugAsync(slug);
        if (tag is null)
        {
            logger.LogWarning("Tag slug '{Slug}' not found", slug);
            return NotFound();
        }

        var posts = await blogService.GetPostsByTagAsync(tag.Id);
        ViewData["Tag"] = tag;
        return View("Posts", posts);
    }
}
