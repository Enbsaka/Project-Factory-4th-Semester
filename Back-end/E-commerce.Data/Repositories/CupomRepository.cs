using Dunder_Store.Database;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Data.Repositories
{
    public class CupomRepository : ICupomRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public CupomRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Cupom> AddAsync(Cupom cupom)
        {
            _dbContext.Cupons.Add(cupom);
            await _dbContext.SaveChangesAsync();
            return cupom;
        }

        public async Task<IEnumerable<Cupom>> GetAllAsync()
        {
            return await _dbContext.Cupons.AsNoTracking()
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();
        }

        public async Task<Cupom?> GetByCodigoAsync(string codigo)
        {
            var normalized = codigo.Trim().ToUpper();
            return await _dbContext.Cupons.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Codigo == normalized && c.Ativo);
        }

        public async Task<Cupom?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Cupons.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Cupom cupom)
        {
            var existente = await _dbContext.Cupons.FindAsync(cupom.Id);
            if (existente == null) return false;

            existente.Codigo = cupom.Codigo;
            existente.DescontoPercentual = cupom.DescontoPercentual;
            existente.DataExpiracao = cupom.DataExpiracao;
            existente.Ativo = cupom.Ativo;

            _dbContext.Cupons.Update(existente);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var cupom = await _dbContext.Cupons.FindAsync(id);
            if (cupom == null) return false;

            _dbContext.Cupons.Remove(cupom);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
