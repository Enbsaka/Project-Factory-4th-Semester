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
                            .Where(pp => pp.ProdutoId.HasValue && pp.ProdutoId.Value == produtoId && pp.Pedido.Status == Entities.PedidoStatus.Carrinho)
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
                            .Where(pp => pp.ProdutoId.HasValue && ids.Contains(pp.ProdutoId.Value) && pp.Pedido.Status == Entities.PedidoStatus.Carrinho)
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
                .AnyAsync(pp => pp.ProdutoId.HasValue && pp.ProdutoId.Value == produtoId && pp.Pedido.Status == Entities.PedidoStatus.Finalizado);
        }

        public async Task<IEnumerable<object>> GetTopMaisVendidosAsync(int top)
        {
            var baseQuery = _dbContext.PedidoProdutos
                .Where(pp => pp.Pedido.Status == Entities.PedidoStatus.Finalizado && pp.ProdutoId.HasValue);

            var agrupado = await baseQuery
                .GroupBy(pp => pp.ProdutoId!.Value)
                .Select(g => new
                {
                    ProdutoId = g.Key,
                    QuantidadeVendida = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.QuantidadeVendida)
                .Take(top)
                .ToListAsync();

            var ids = agrupado.Select(x => x.ProdutoId).ToList();
            var produtos = await _dbContext.Produtos
                .Where(p => ids.Contains(p.Id))
                .Select(p => new { p.Id, p.Nome, p.Preco })
                .ToListAsync();

            var resultado = new List<object>();
            foreach (var item in agrupado)
            {
                var prod = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);
                if (prod == null)
                {
                    var snap = await baseQuery
                        .Where(pp => pp.ProdutoId == item.ProdutoId)
                        .OrderByDescending(pp => pp.Pedido.DataPedido)
                        .Select(pp => new { Nome = pp.ProdutoNome, Preco = pp.PrecoUnitario })
                        .FirstOrDefaultAsync();

                    resultado.Add(new
                    {
                        ProdutoId = item.ProdutoId,
                        Nome = string.IsNullOrWhiteSpace(snap?.Nome) ? "Produto removido" : snap!.Nome,
                        Preco = (snap?.Preco > 0m ? snap.Preco : 0m),
                        QuantidadeVendida = item.QuantidadeVendida
                    });
                }
                else
                {
                    resultado.Add(new
                    {
                        ProdutoId = item.ProdutoId,
                        Nome = prod.Nome,
                        Preco = prod.Preco,
                        QuantidadeVendida = item.QuantidadeVendida
                    });
                }
            }

            return resultado;
        }

        public async Task ReassignFinalizadosAsync(Guid oldProdutoId, Guid newProdutoId, string? nomeSnapshot, string? codigoSnapshot)
        {
            var itens = await _dbContext.PedidoProdutos
                .Include(pp => pp.Pedido)
                .Where(pp => pp.ProdutoId.HasValue && pp.ProdutoId.Value == oldProdutoId && pp.Pedido.Status == Entities.PedidoStatus.Finalizado)
                .ToListAsync();
            if (itens.Count == 0) return;
            foreach (var i in itens)
            {
                i.ProdutoId = newProdutoId;
                if (!string.IsNullOrWhiteSpace(nomeSnapshot)) i.ProdutoNome = nomeSnapshot;
                if (!string.IsNullOrWhiteSpace(codigoSnapshot)) i.ProdutoCodigoDeBarra = codigoSnapshot;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoverPorPedidoIdAsync(Guid pedidoId)
        {
            var itens = await _dbContext.PedidoProdutos
                .Where(pp => pp.PedidoId == pedidoId)
                .ToListAsync();
            if (itens.Count == 0) return;
            _dbContext.PedidoProdutos.RemoveRange(itens);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Dunder_Store.Entities.PedidoProduto> itens)
        {
            var list = itens?.ToList() ?? new List<Dunder_Store.Entities.PedidoProduto>();
            if (list.Count == 0) return;
            await _dbContext.PedidoProdutos.AddRangeAsync(list);
            await _dbContext.SaveChangesAsync();
        }
    }
}
