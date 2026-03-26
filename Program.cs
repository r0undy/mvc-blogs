using mvc_blogs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register blog service (swap BlogService for an EF Core impl to use a real DB)
builder.Services.AddScoped<IBlogService, BlogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

// SEO-friendly slug routes
app.MapControllerRoute(name: "category-slug", pattern: "category/{slug}", defaults: new { controller = "Category", action = "Browse" });
app.MapControllerRoute(name: "tag-slug",      pattern: "tag/{slug}",      defaults: new { controller = "Tag",      action = "Browse" });

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

