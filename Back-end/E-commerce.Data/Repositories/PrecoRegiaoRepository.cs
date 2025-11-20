using Dunder_Store.Database;
using Microsoft.EntityFrameworkCore;
using Dunder_Store.E_commerce.Business.Interfaces.IRepositories;
using Dunder_Store.E_commerce.Business.Entities;

namespace Dunder_Store.E_commerce.Data.Repositories
{
    public class PrecoRegiaoRepository : IPrecoRegiaoRepository
    {
        private readonly ProdutosDbContext _context;

        public PrecoRegiaoRepository(ProdutosDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> ObterPrecoPorRegiaoAsync(string regiao)
        {
            var preco = await _context.Set<PrecoRegiao>()
                .Where(r => r.Regiao == regiao)
                .Select(r => r.PrecoBase)
                .FirstOrDefaultAsync();

            return preco;
        }
    }
}
