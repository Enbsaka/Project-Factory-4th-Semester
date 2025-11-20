using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(Guid id);
        Task<Cliente?> GetByCpfAsync(string cpf);
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(Cliente cliente);
    }
}
