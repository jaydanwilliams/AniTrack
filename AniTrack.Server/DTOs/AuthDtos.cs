using System.ComponentModel.DataAnnotations;

namespace AniTrack.Server.Dtos;

public class RegisterRequest
{
    [Required, StringLength(20, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public record UserDto(string Id, string UserName, string Email, List<string> Roles);