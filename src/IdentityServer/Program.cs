using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer;

var builder = WebApplication.CreateBuilder(args);

// Add Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("IdentityDb"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add HttpClientFactory
builder.Services.AddHttpClient();

// Add Controllers
builder.Services.AddControllers();

// Add IdentityServer4
builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients)
    .AddAspNetIdentity<IdentityUser>()
    .AddDeveloperSigningCredential(); // Only for development

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Seed roles and users
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create roles
    var roles = new[] { "Manager", "Store customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create test users
    // Manager user
    var managerEmail = "manager@test.com";
    var managerUser = await userManager.FindByEmailAsync(managerEmail);
    if (managerUser == null)
    {
        managerUser = new IdentityUser
        {
            UserName = "manager",
            Email = managerEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(managerUser, "Manager@123");
        await userManager.AddToRoleAsync(managerUser, "Manager");
    }

    // Store customer user
    var customerEmail = "customer@test.com";
    var customerUser = await userManager.FindByEmailAsync(customerEmail);
    if (customerUser == null)
    {
        customerUser = new IdentityUser
        {
            UserName = "customer",
            Email = customerEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(customerUser, "Customer@123");
        await userManager.AddToRoleAsync(customerUser, "Store customer");
    }
}

app.UseIdentityServer();
app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "IdentityServer4 is running").WithName("Home");

app.Run();

public partial class Program { }
