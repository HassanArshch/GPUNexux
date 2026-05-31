using GpuStore.Data;
using GpuStore.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

var isProduction = builder.Environment.IsProduction();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
if (isProduction)
{
    
        // Ensure directories exist before configuring services
        Directory.CreateDirectory("/data/keys");  // <-- add this
        Directory.CreateDirectory("/data");       // ensure DB dir too

        builder.Services.AddDbContext<GpuContext>(options =>
            options.UseSqlite("Data Source=/data/gpustore.db"));

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/data/keys"))
            .SetApplicationName("GpuStore");  // <-- also add this

    var keyFiles = Directory.GetFiles("/data/keys");
    Console.WriteLine($"Data Protection key files: {keyFiles.Length}");
    foreach (var f in keyFiles)
        Console.WriteLine($"  Key file: {Path.GetFileName(f)}");

}
else
{
    builder.Services.AddDbContext<GpuContext>(options =>
        options.UseSqlite("Data Source=gpustore.db"));
}
// Add this with your other service configurations
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.LoginPath = "/Account/Login";
    options.Events.OnRedirectToLogin = context =>
    {
        var returnUrl = Uri.EscapeDataString(context.Request.Path);
        context.Response.Headers["Location"] = $"https://{context.Request.Host}/Account/Login?ReturnUrl={returnUrl}";
        context.Response.StatusCode = 302;
        return Task.CompletedTask;
    };
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie.Name = "GpuStore.Auth";
    options.SlidingExpiration = true;
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<GpuContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Auto-migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GpuContext>();
    db.Database.Migrate();
    Console.WriteLine($"DB exists: {File.Exists("/data/gpustore.db")}");
    // Seed admin role and user if they don't exist
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    // Create default admin user
    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User"
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123456");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        await userManager.SetLockoutEnabledAsync(adminUser, false);
        // or if already locked:
        await userManager.ResetAccessFailedCountAsync(adminUser);
        await userManager.SetLockoutEndDateAsync(adminUser, null);
    }
    else
    {
        await userManager.SetLockoutEnabledAsync(adminUser, false);
        // or if already locked:
        await userManager.ResetAccessFailedCountAsync(adminUser);
        await userManager.SetLockoutEndDateAsync(adminUser, null);
        // Force password reset every deploy (useful for debugging)
        var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
        await userManager.ResetPasswordAsync(adminUser, token, "Admin@123456");
    }
    var admin = await userManager.FindByEmailAsync("admin@gmail.com");

    Console.WriteLine("USER EXISTS: " + (admin != null));

    if (admin != null)
    {
        var hasPassword = await userManager.HasPasswordAsync(admin);
        Console.WriteLine("HAS PASSWORD: " + hasPassword);
    }
    var allUsers = userManager.Users.ToList();


   
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
