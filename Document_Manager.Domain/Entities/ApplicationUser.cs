
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Manager.Domain.Entities
{
    [Table("ApplicationUser")]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string FullName { get; set; }
    }
}
