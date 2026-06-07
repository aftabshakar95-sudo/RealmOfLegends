using Microsoft.AspNetCore.Identity;

namespace RealmOfLegends.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EmailConfirmedAt { get; set; }
    }
}
