using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleBlogMVC.Data;
using SimpleBlogMVC.Services;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Data.Repositories.Interfaces;
using SimpleBlogMVC.Data.Repositories.Implementations;
using SimpleBlogMVC.Services.Interfaces;
using SimpleBlogMVC.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register repositories and services
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSassCompiler();

// Add memory cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Use global exception handler
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();