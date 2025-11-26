using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController(IClienteService clienteService, IPedidoService pedidoService) : ControllerBase
    {
        private readonly IClienteService _clienteService = clienteService;
        private readonly IPedidoService _pedidoService = pedidoService;

        private Guid? GetLoggedClienteId()
        {
            var id = User?.FindFirst("Id")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(id, out var guid)) return guid;
            return null;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Paginador<ClienteDTOOutput>>> GetClientes(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;

            var clientes = await _clienteService.GetAllAsync();
            var totalItens = clientes.Count();

            var clientesPaginados = clientes
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(c => new ClienteDTOOutput(
                    c.Id, c.Nome, c.Cpf, c.Email, c.Senha, c.Cep, c.NumEndereco,
                    c.Logradouro ?? string.Empty, c.Bairro ?? string.Empty, c.Localidade ?? string.Empty, c.Uf ?? string.Empty, c.IsAdmin, c.DataCadastro))
                .ToList();

            if (clientesPaginados.Count == 0)
                return NoContent();

            var resultado = new Paginador<ClienteDTOOutput>(
                clientesPaginados,
                totalItens,
                pagina,
                tamanhoPagina
            );

            return Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClienteDTOOutput>> GetClienteId(Guid id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            var clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Senha, cliente.Cep, cliente.NumEndereco,
                cliente.Logradouro ?? string.Empty, cliente.Bairro ?? string.Empty, cliente.Localidade ?? string.Empty, cliente.Uf ?? string.Empty, cliente.IsAdmin, cliente.DataCadastro);
            return Ok(clienteDTO);
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> GetTotalClientes()
        {
            var total = (await _clienteService.GetAllAsync()).Count();
            return Ok(total);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteDTOOutput>> CreateCliente([FromForm] ClienteDTOInput novoClienteDTO)
        {
            var novoCliente = new Cliente(
                novoClienteDTO.Nome,
                novoClienteDTO.Cpf,
                novoClienteDTO.Email,
                novoClienteDTO.Senha,
                novoClienteDTO.Cep,
                novoClienteDTO.NumEndereco,
                novoClienteDTO.Logradouro,
                novoClienteDTO.Bairro,
                novoClienteDTO.Localidade,
                novoClienteDTO.Uf
            )
            {
                IsAdmin = novoClienteDTO.IsAdmin ?? false
            };

            var usuarioEhAdmin = User?.IsInRole("Admin") ?? false;
            try
            {
                var criado = await _clienteService.CriarClienteComRegrasAsync(novoCliente, usuarioEhAdmin);
                var clienteDTO = new ClienteDTOOutput(criado.Id, criado.Nome, criado.Cpf, criado.Email, criado.Senha, criado.Cep, criado.NumEndereco,
                    criado.Logradouro ?? string.Empty, criado.Bairro ?? string.Empty, criado.Localidade ?? string.Empty, criado.Uf ?? string.Empty, criado.IsAdmin, criado.DataCadastro);
                return CreatedAtAction(nameof(GetClienteId), new { id = criado.Id }, clienteDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteDTOOutput>> LoginCliente([FromForm] LoginDTO loginDTO)
        {
            var cliente = await _clienteService.AutenticarAsync(loginDTO.Email, loginDTO.Senha);
            if (cliente == null)
                return Unauthorized("Email ou senha inválidos");

            var clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Senha, cliente.Cep, cliente.NumEndereco,
                cliente.Logradouro ?? string.Empty, cliente.Bairro ?? string.Empty, cliente.Localidade ?? string.Empty, cliente.Uf ?? string.Empty, cliente.IsAdmin, cliente.DataCadastro);
            return Ok(clienteDTO);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<ActionResult<ClienteDTOOutput>> GetMeuPerfil()
        {
            var userId = GetLoggedClienteId();
            if (userId == null)
                return Unauthorized();

            var cliente = await _clienteService.GetByIdAsync(userId.Value);
            if (cliente == null)
                return NotFound();

            var clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Senha, cliente.Cep, cliente.NumEndereco,
                cliente.Logradouro ?? string.Empty, cliente.Bairro ?? string.Empty, cliente.Localidade ?? string.Empty, cliente.Uf ?? string.Empty, cliente.IsAdmin, cliente.DataCadastro);
            return Ok(clienteDTO);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCliente(Guid id, ClienteDTOInput clienteAtualizadoDTO)
        {
            try
            {
                await _clienteService.AtualizarClienteComRegrasAsync(id, clienteAtualizadoDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("me")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> UpdateMeuPerfil([FromForm] ClienteDTOInput clienteAtualizadoDTO)
        {
            var userId = GetLoggedClienteId();
            if (userId == null)
                return Unauthorized();

            try
            {
                await _clienteService.AtualizarMeuPerfilComRegrasAsync(userId.Value, clienteAtualizadoDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            try
            {
                await _clienteService.RemoverClienteComRegrasAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
