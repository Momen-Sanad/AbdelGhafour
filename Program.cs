using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Data;
using SuperMarket.Models;
using SuperMarket.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IUserService, UserService>(); // Custom UserService
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Set up identity with custom user and role
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Razor Pages and Session configuration
builder.Services.AddRazorPages()
    .AddSessionStateTempDataProvider();
builder.Services.AddSession();

// Add logging services
builder.Services.AddLogging();
builder.Logging.AddConsole();  
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Cookie configuration for authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
});

// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as needed
});

var app = builder.Build();

// Middleware setup
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  // Authentication middleware
app.UseAuthorization();   // Authorization middleware
app.UseSession();  // Enable session middleware

// Map Razor Pages
app.MapRazorPages();

// Run the app
app.Run();
