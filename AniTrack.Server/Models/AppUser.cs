using Microsoft.AspNetCore.Identity;

namespace AniTrack.Server.Models;

public class AppUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}