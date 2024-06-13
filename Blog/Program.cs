using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Desabilita a necessidade de confirmação de email
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Adicione a criação do administrador padrão
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    await CreateDefaultAdminAsync(userManager, roleManager, logger);
}

app.Run();

async Task CreateDefaultAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
{
    var adminRole = "Admin";

    // Verifica se a role Admin existe
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (!roleResult.Succeeded)
        {
            logger.LogError("Failed to create Admin role");
            return;
        }
    }

    // Verifica se a conta de admin existe
    var adminEmail = "admin@admin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true }; // Marca o email como confirmado
        var userResult = await userManager.CreateAsync(adminUser, "Admin@123");

        if (userResult.Succeeded)
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
            if (addToRoleResult.Succeeded)
            {
                logger.LogInformation("Admin user created successfully");
            }
            else
            {
                logger.LogError("Failed to add Admin user to Admin role");
            }
        }
        else
        {
            logger.LogError("Failed to create Admin user");
        }
    }
    else
    {
        logger.LogInformation("Admin user already exists");
    }
}
