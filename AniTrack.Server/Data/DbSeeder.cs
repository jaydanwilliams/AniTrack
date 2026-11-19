using AniTrack.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace AniTrack.Server.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        // Make sure the Admin role exists
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roles.RoleExistsAsync(AppRoles.Admin))
            await roles.CreateAsync(new IdentityRole(AppRoles.Admin));

        // Give the admin role to the user named in config, if they've registered
        var adminUserName = config["SeedAdmin:UserName"];
        if (string.IsNullOrWhiteSpace(adminUserName)) return;

        var users = services.GetRequiredService<UserManager<AppUser>>();
        var admin = await users.FindByNameAsync(adminUserName);

        if (admin is not null && !await users.IsInRoleAsync(admin, AppRoles.Admin))
            await users.AddToRoleAsync(admin, AppRoles.Admin);
    }
}