using Microsoft.AspNetCore.Mvc;
using Dunder_Store.Interfaces.IServices;
using System.Security.Claims;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AdminAuthController(IAuthService auth)
        {
            _auth = auth;
        }

        public class LoginRequest { public string username { get; set; } = string.Empty; public string password { get; set; } = string.Empty; }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var credenciaisValidas = await _auth.ValidateAdminCredentialsAsync(req.username, req.password);

            if (!credenciaisValidas)
                return Unauthorized("Credenciais inválidas");

            var claims = new[]
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, req.username),
                new Claim(ClaimTypes.Name, req.username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("role", "Admin")
            };
            var (tokenString, expires) = _auth.CreateToken(claims);
            return Ok(new { token = tokenString, expires });
        }
    }
}
