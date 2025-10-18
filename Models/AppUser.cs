using Microsoft.AspNetCore.Identity;

namespace first_.NET_project.Models;

public class AppUser: IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
