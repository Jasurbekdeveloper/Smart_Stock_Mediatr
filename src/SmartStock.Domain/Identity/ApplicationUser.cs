using Microsoft.AspNetCore.Identity;

namespace SmartStock.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

