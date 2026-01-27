
using Microsoft.AspNetCore.Identity;

namespace Document_Manager.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; private set; } = null!;
    }
}
