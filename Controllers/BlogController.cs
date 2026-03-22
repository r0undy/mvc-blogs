using Microsoft.AspNetCore.Mvc;
using mvc_blogs.Models;

namespace mvc_blogs.Controllers;

public class BlogController : Controller
{

    public IActionResult Index()
    {
        var posts = LoadPostsFromJson();
        if (posts == null)
        {
            return Problem("Could not load posts from Data/posts.json. Please check the file format.");
        }
        return View(posts);
    }


    public IActionResult Details(int id)
    {
        var posts = LoadPostsFromJson();
        if (posts == null)
        {
            return Problem("Could not load posts from Data/posts.json. Please check the file format.");
        }
        Post? post = posts.FirstOrDefault(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }
        return View(post);
    }


    private static List<Post>? LoadPostsFromJson()
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "posts.json");
            if (!System.IO.File.Exists(filePath))
                return new List<Post>();
            var json = System.IO.File.ReadAllText(filePath);
            var posts = System.Text.Json.JsonSerializer.Deserialize<List<Post>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return posts;
        }
        catch
        {
            return null;
        }
    }
}
