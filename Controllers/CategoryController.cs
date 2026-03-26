using Microsoft.AspNetCore.Mvc;
using mvc_blogs.Services;

namespace mvc_blogs.Controllers;

public class CategoryController(IBlogService blogService, ILogger<CategoryController> logger) : Controller
{
    // GET /Category
    public async Task<IActionResult> Index()
    {
        var categories = await blogService.GetAllCategoriesAsync();
        return View(categories);
    }

    // GET /Category/Posts/1  — by ID
    public async Task<IActionResult> Posts(int id)
    {
        var category = await blogService.GetCategoryByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("Category {Id} not found", id);
            return NotFound();
        }

        var posts = await blogService.GetPostsByCategoryAsync(id);
        ViewData["Category"] = category;
        return View(posts);
    }

    // GET /Category/Browse/technology  — by slug (SEO-friendly)
    public async Task<IActionResult> Browse(string slug)
    {
        var category = await blogService.GetCategoryBySlugAsync(slug);
        if (category is null)
        {
            logger.LogWarning("Category slug '{Slug}' not found", slug);
            return NotFound();
        }

        var posts = await blogService.GetPostsByCategoryAsync(category.Id);
        ViewData["Category"] = category;
        return View("Posts", posts);
    }
}
