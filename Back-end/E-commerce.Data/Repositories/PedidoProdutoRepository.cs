using Dunder_Store.Database;
using Dunder_Store.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Data.Repositories
{
    public class PedidoProdutoRepository : IPedidoProdutoRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public PedidoProdutoRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RemoverPorProdutoIdAsync(Guid produtoId)
        {
            var itens = await _dbContext.PedidoProdutos
                            .Include(pp => pp.Pedido)
                            .Where(pp => pp.ProdutoId == produtoId && pp.Pedido.Status == Entities.PedidoStatus.Carrinho)
                            .ToListAsync();

            if (itens.Count > 0)
            {
                _dbContext.PedidoProdutos.RemoveRange(itens);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoverPorProdutoIdsAsync(IEnumerable<Guid> produtoIds)
        {
            var ids = produtoIds?.ToList() ?? new List<Guid>();
            if (ids.Count == 0) return;
            var itens = await _dbContext.PedidoProdutos
                            .Include(pp => pp.Pedido)
                            .Where(pp => ids.Contains(pp.ProdutoId) && pp.Pedido.Status == Entities.PedidoStatus.Carrinho)
                            .ToListAsync();
            if (itens.Count > 0)
            {
                _dbContext.PedidoProdutos.RemoveRange(itens);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteEmPedidoFinalizadoAsync(Guid produtoId)
        {
            return await _dbContext.PedidoProdutos
                .Include(pp => pp.Pedido)
                .AnyAsync(pp => pp.ProdutoId == produtoId && pp.Pedido.Status == Entities.PedidoStatus.Finalizado);
        }

        public async Task<IEnumerable<object>> GetTopMaisVendidosAsync(int top)
        {
            var produtosMaisVendidos = await _dbContext.PedidoProdutos
                .Where(pp => pp.Pedido.Status == Entities.PedidoStatus.Finalizado)
                .GroupBy(pp => new { pp.ProdutoId, pp.Produto.Nome, pp.Produto.Preco })
                .Select(g => new
                {
                    ProdutoId = g.Key.ProdutoId,
                    Nome = g.Key.Nome,
                    Preco = g.Key.Preco,
                    QuantidadeVendida = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.QuantidadeVendida)
                .Take(top)
                .ToListAsync();

            return produtosMaisVendidos;
        }
    }
}
