using Dunder_Store.DTO;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteAuthController(IConfiguration config, IClienteService clienteService, IAuthService authService) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IClienteService _clienteService = clienteService;
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO req)
        {
            var cliente = await _clienteService.AutenticarAsync(req.Email, req.Senha);
            if (cliente == null)
                return Unauthorized("Email ou senha inválidos");
            var claims = _authService.CreateClienteClaims(cliente);
            var (tokenString, expires) = _authService.CreateToken(claims);
            return Ok(new { token = tokenString, expires });
        }
    }
}