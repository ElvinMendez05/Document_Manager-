
using Microsoft.AspNetCore.Identity;

namespace Document_Manager.Infrastructure.Identity
{
    //Herencia de Usuario gracias a Identity
    public class ApplicationUser : IdentityUser
    {
    
        public string? FullName { get; set; }
    }
}
