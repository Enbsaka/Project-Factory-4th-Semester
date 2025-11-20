using Dunder_Store.DTO;
using Dunder_Store.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteAuthController(IConfiguration config, ProdutosDbContext dbContext) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly ProdutosDbContext _dbContext = dbContext;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO req)
        {
            var cliente = _dbContext.Clientes.FirstOrDefault(c => c.Email == req.Email && c.Senha == req.Senha);
            if (cliente == null)
                return Unauthorized("Email ou senha inválidos");

            var jwtCfg = _config.GetSection("Jwt");
            var key = jwtCfg.GetValue<string>("Key");
            if (string.IsNullOrWhiteSpace(key))
                return StatusCode(500, "Configuração JWT inválida: 'Key' não definida.");
            var issuer = jwtCfg.GetValue<string>("Issuer");
            var audience = jwtCfg.GetValue<string>("Audience");
            var expiryMinutes = jwtCfg.GetValue<int>("ExpiryMinutes");

            var roleValue = cliente.IsAdmin ? "Admin" : "Cliente";
            var devAdminEmail = _config.GetValue<string>("DevAdminEmail");
            if (!string.IsNullOrWhiteSpace(devAdminEmail) &&
                string.Equals(cliente.Email, devAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                roleValue = "Admin";
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, cliente.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                new Claim("Id", cliente.Id.ToString()),
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Role, roleValue),
                new Claim("role", roleValue)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, expires = token.ValidTo });
        }
    }
}