using Microsoft.AspNetCore.Identity;

namespace Identity.Infraestructura
{
    public class ApplicationUser : IdentityUser
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
