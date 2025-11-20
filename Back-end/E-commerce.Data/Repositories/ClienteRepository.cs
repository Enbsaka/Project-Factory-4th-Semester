using Dunder_Store.Database;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public ClienteRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _dbContext.Clientes.ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> GetByCpfAsync(string cpf)
        {
            return await _dbContext.Clientes.FirstOrDefaultAsync(c => c.Cpf == cpf);
        }

        public async Task AddAsync(Cliente cliente)
        {
            await _dbContext.Clientes.AddAsync(cliente);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _dbContext.Clientes.Update(cliente);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cliente cliente)
        {
            _dbContext.Clientes.Remove(cliente);
            await _dbContext.SaveChangesAsync();
        }
    }
}
