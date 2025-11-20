using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
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

        public async Task RemoverClienteAsync(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente != null)
            {
                await _clienteRepository.DeleteAsync(cliente);
            }
        }
    }
}
