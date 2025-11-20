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
            var clientes = await _clienteService.GetAllAsync();
            if (clientes.Count(c => c.Cpf == novoClienteDTO.Cpf) > 0)
                return BadRequest("Já existe um cliente com este CPF");

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
            );

            var solicitarAdmin = novoClienteDTO.IsAdmin ?? false;
            var usuarioEhAdmin = User?.IsInRole("Admin") ?? false;
            novoCliente.IsAdmin = solicitarAdmin && usuarioEhAdmin;

            await _clienteService.CriarClienteAsync(novoCliente);

            var clienteDTO = new ClienteDTOOutput(novoCliente.Id, novoCliente.Nome, novoCliente.Cpf, novoCliente.Email, novoCliente.Senha, novoCliente.Cep, novoCliente.NumEndereco,
                novoCliente.Logradouro ?? string.Empty, novoCliente.Bairro ?? string.Empty, novoCliente.Localidade ?? string.Empty, novoCliente.Uf ?? string.Empty, novoCliente.IsAdmin, novoCliente.DataCadastro);
            return CreatedAtAction(nameof(GetClienteId), new { id = novoCliente.Id }, clienteDTO);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteDTOOutput>> LoginCliente([FromForm] LoginDTO loginDTO)
        {
            var clientes = await _clienteService.GetAllAsync();
            var cliente = clientes.FirstOrDefault(c => c.Email == loginDTO.Email && c.Senha == loginDTO.Senha);

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
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            var clientes = await _clienteService.GetAllAsync();
            if (clientes.Count(c => c.Id != id && c.Cpf == clienteAtualizadoDTO.Cpf) > 0)
                return BadRequest("Já existe um cliente com esse CPF");

            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Nome)) cliente.Nome = clienteAtualizadoDTO.Nome;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Cpf)) cliente.Cpf = clienteAtualizadoDTO.Cpf;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Email)) cliente.Email = clienteAtualizadoDTO.Email;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Senha)) cliente.Senha = clienteAtualizadoDTO.Senha;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Cep)) cliente.Cep = clienteAtualizadoDTO.Cep;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.NumEndereco)) cliente.NumEndereco = clienteAtualizadoDTO.NumEndereco;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Logradouro)) cliente.Logradouro = clienteAtualizadoDTO.Logradouro;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Bairro)) cliente.Bairro = clienteAtualizadoDTO.Bairro;


            if (clienteAtualizadoDTO.IsAdmin.HasValue)
                cliente.IsAdmin = clienteAtualizadoDTO.IsAdmin.Value;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Localidade)) cliente.Localidade = clienteAtualizadoDTO.Localidade;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Uf)) cliente.Uf = clienteAtualizadoDTO.Uf;

            await _clienteService.AtualizarClienteAsync(cliente);
            return NoContent();
        }

        [HttpPatch("me")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> UpdateMeuPerfil([FromForm] ClienteDTOInput clienteAtualizadoDTO)
        {
            var userId = GetLoggedClienteId();
            if (userId == null)
                return Unauthorized();

            var cliente = await _clienteService.GetByIdAsync(userId.Value);
            if (cliente == null)
                return NotFound();

            var clientes = await _clienteService.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Cpf) && clientes.Count(c => c.Id != userId.Value && c.Cpf == clienteAtualizadoDTO.Cpf) > 0)
                return BadRequest("Já existe um cliente com esse CPF");

            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Nome)) cliente.Nome = clienteAtualizadoDTO.Nome;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Cpf)) cliente.Cpf = clienteAtualizadoDTO.Cpf;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Email)) cliente.Email = clienteAtualizadoDTO.Email;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Senha)) cliente.Senha = clienteAtualizadoDTO.Senha;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Cep)) cliente.Cep = clienteAtualizadoDTO.Cep;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.NumEndereco)) cliente.NumEndereco = clienteAtualizadoDTO.NumEndereco;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Logradouro)) cliente.Logradouro = clienteAtualizadoDTO.Logradouro;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Bairro)) cliente.Bairro = clienteAtualizadoDTO.Bairro;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Localidade)) cliente.Localidade = clienteAtualizadoDTO.Localidade;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.Uf)) cliente.Uf = clienteAtualizadoDTO.Uf;

            await _clienteService.AtualizarClienteAsync(cliente);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            var pedidosDoCliente = await _pedidoService.GetPedidosPorClienteAsync(id, null, 1, int.MaxValue);

            // Política: preservar receita e histórico
            // Se o cliente possui pedidos finalizados, bloqueia a deleção do cliente
            var possuiFinalizados = pedidosDoCliente.Itens.Any(p => p.Status == Entities.PedidoStatus.Finalizado);
            if (possuiFinalizados)
            {
                return BadRequest("Cliente possui pedidos finalizados; não é possível deletar o cliente.");
            }

            await _pedidoService.RemoverPedidosClienteAsync(id);
            await _clienteService.RemoverClienteAsync(id);

            return NoContent();
        }
    }
}
