using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IPedidoService _pedidoService;
        private readonly IEmailService _emailService;

        public ClienteService(IClienteRepository clienteRepository, IPedidoService pedidoService, IEmailService emailService)
        {
            _clienteRepository = clienteRepository;
            _pedidoService = pedidoService;
            _emailService = emailService;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente?> GetByIdAsync(Guid id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task<Cliente> CriarClienteAsync(Cliente cliente)
        {
            await _clienteRepository.AddAsync(cliente);
            return cliente;
        }

        public async Task AtualizarClienteAsync(Cliente cliente)
        {
            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task AtualizarClienteComRegrasAsync(Guid id, DTO.ClienteDTOInput dto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) throw new Exception("Cliente não encontrado.");

            var todos = await _clienteRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(dto.Cpf) && todos.Any(c => c.Id != id && c.Cpf == dto.Cpf))
                throw new Exception("Já existe um cliente com esse CPF");

            if (!string.IsNullOrWhiteSpace(dto.Nome)) cliente.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Cpf)) cliente.Cpf = dto.Cpf;
            if (!string.IsNullOrWhiteSpace(dto.Email)) cliente.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Senha)) cliente.Senha = dto.Senha;
            if (!string.IsNullOrWhiteSpace(dto.Cep)) cliente.Cep = dto.Cep;
            if (!string.IsNullOrWhiteSpace(dto.NumEndereco)) cliente.NumEndereco = dto.NumEndereco;
            if (!string.IsNullOrWhiteSpace(dto.Logradouro)) cliente.Logradouro = dto.Logradouro;
            if (!string.IsNullOrWhiteSpace(dto.Bairro)) cliente.Bairro = dto.Bairro;
            if (dto.IsAdmin.HasValue) cliente.IsAdmin = dto.IsAdmin.Value;
            if (!string.IsNullOrWhiteSpace(dto.Localidade)) cliente.Localidade = dto.Localidade;
            if (!string.IsNullOrWhiteSpace(dto.Uf)) cliente.Uf = dto.Uf;

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task AtualizarMeuPerfilComRegrasAsync(Guid userId, DTO.ClienteDTOInput dto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(userId);
            if (cliente == null) throw new Exception("Cliente não encontrado.");

            var todos = await _clienteRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(dto.Cpf) && todos.Any(c => c.Id != userId && c.Cpf == dto.Cpf))
                throw new Exception("Já existe um cliente com esse CPF");

            if (!string.IsNullOrWhiteSpace(dto.Nome)) cliente.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Cpf)) cliente.Cpf = dto.Cpf;
            if (!string.IsNullOrWhiteSpace(dto.Email)) cliente.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Senha)) cliente.Senha = dto.Senha;
            if (!string.IsNullOrWhiteSpace(dto.Cep)) cliente.Cep = dto.Cep;
            if (!string.IsNullOrWhiteSpace(dto.NumEndereco)) cliente.NumEndereco = dto.NumEndereco;
            if (!string.IsNullOrWhiteSpace(dto.Logradouro)) cliente.Logradouro = dto.Logradouro;
            if (!string.IsNullOrWhiteSpace(dto.Bairro)) cliente.Bairro = dto.Bairro;
            if (!string.IsNullOrWhiteSpace(dto.Localidade)) cliente.Localidade = dto.Localidade;
            if (!string.IsNullOrWhiteSpace(dto.Uf)) cliente.Uf = dto.Uf;

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task RemoverClienteAsync(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente != null)
            {
                await _clienteRepository.DeleteAsync(cliente);
            }
        }

        public async Task<Cliente?> AutenticarAsync(string email, string senha)
        {
            return await _clienteRepository.GetByEmailAndSenhaAsync(email, senha);
        }

        public async Task<Cliente> CriarClienteComRegrasAsync(Cliente novo, bool usuarioEhAdmin)
        {
            var existentes = await _clienteRepository.GetAllAsync();
            if (existentes.Any(c => c.Cpf == novo.Cpf))
                throw new Exception("Já existe um cliente com este CPF");

            novo.IsAdmin = novo.IsAdmin && usuarioEhAdmin;
            await _clienteRepository.AddAsync(novo);
            await _emailService.SendWelcomeAsync(novo);
            return novo;
        }

        public async Task RemoverClienteComRegrasAsync(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            var pedidosDoCliente = await _pedidoService.GetPedidosPorClienteAsync(id, null, 1, int.MaxValue);
            var possuiFinalizados = pedidosDoCliente.Itens.Any(p => p.Status == Entities.PedidoStatus.Finalizado);
            if (possuiFinalizados)
                throw new Exception("Cliente possui pedidos finalizados; não é possível deletar o cliente.");

            await _pedidoService.RemoverPedidosClienteAsync(id);
            await _clienteRepository.DeleteAsync(cliente);
        }
    }
}
