using Dunder_Store.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dunder_Store.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public Task<bool> ValidateAdminCredentialsAsync(string username, string password)
        {
            var adminUsersSection = _config.GetSection("AdminUsers");
            var hasAdminUsers = adminUsersSection.Exists() && adminUsersSection.GetChildren().Any();
            if (hasAdminUsers)
            {
                foreach (var child in adminUsersSection.GetChildren())
                {
                    var user = child.GetValue<string>("Username");
                    var pass = child.GetValue<string>("Password");
                    if (username == user && password == pass) return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            else
            {
                var adminCfg = _config.GetSection("AdminCredentials");
                var configuredUser = adminCfg.GetValue<string>("Username");
                var configuredPass = adminCfg.GetValue<string>("Password");
                return Task.FromResult(username == configuredUser && password == configuredPass);
            }
        }

        public (string token, DateTime expires) CreateToken(IEnumerable<Claim> claims)
        {
            var jwtCfg = _config.GetSection("Jwt");
            var key = jwtCfg.GetValue<string>("Key");
            var issuer = jwtCfg.GetValue<string>("Issuer");
            var audience = jwtCfg.GetValue<string>("Audience");
            var expiryMinutes = jwtCfg.GetValue<int>("ExpiryMinutes");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, token.ValidTo);
        }

        public Claim[] CreateClienteClaims(Entities.Cliente cliente)
        {
            var roleValue = cliente.IsAdmin ? "Admin" : "Cliente";
            var devAdminEmail = _config.GetValue<string>("DevAdminEmail");
            if (!string.IsNullOrWhiteSpace(devAdminEmail) && string.Equals(cliente.Email, devAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                roleValue = "Admin";
            }
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, cliente.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                new Claim("Id", cliente.Id.ToString()),
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Role, roleValue),
                new Claim("role", roleValue)
            };
        }
    }
}