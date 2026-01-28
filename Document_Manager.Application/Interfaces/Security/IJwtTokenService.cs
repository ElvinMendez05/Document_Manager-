
using Document_Manager.Domain.Entities;

namespace Document_Manager.Application.Interfaces.Security
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
