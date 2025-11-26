using System.Security.Claims;

namespace Dunder_Store.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<bool> ValidateAdminCredentialsAsync(string username, string password);
        (string token, DateTime expires) CreateToken(IEnumerable<Claim> claims);
        Claim[] CreateClienteClaims(Entities.Cliente cliente);
    }
}