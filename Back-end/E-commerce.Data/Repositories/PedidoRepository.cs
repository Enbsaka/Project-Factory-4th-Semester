using Dunder_Store.Database;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Data.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public PedidoRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _dbContext.Pedidos.ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Paginador<Pedido>> GetPagedAsync(int pagina, int tamanhoPagina)
        {
            var totalItens = await _dbContext.Pedidos.CountAsync();
            var pedidos = await _dbContext.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cupom)
                .OrderByDescending(p => p.DataPedido)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new Paginador<Pedido>(pedidos, totalItens, pagina, tamanhoPagina);
        }

        public async Task<Pedido?> GetDetalhadoByIdAsync(Guid id)
        {
            return await _dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .Include(p => p.Cupom)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Paginador<Pedido>> GetByClientePagedAsync(Guid clienteId, PedidoStatus? status, int pagina, int tamanhoPagina)
        {
            var query = _dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .Include(p => p.Cupom)
                .Where(p => p.ClienteId == clienteId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            var totalItens = await query.CountAsync();
            var pedidos = await query
                .OrderByDescending(p => p.DataPedido)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new Paginador<Pedido>(pedidos, totalItens, pagina, tamanhoPagina);
        }

        public async Task<Pedido?> GetCarrinhoByClienteAsync(Guid clienteId)
        {
            return await _dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .Include(p => p.Cupom)
                .FirstOrDefaultAsync(p => p.ClienteId == clienteId && p.Status == PedidoStatus.Carrinho);
        }

        public async Task<List<Pedido>> GetFinalizadosInRangeAsync(DateTime inicio, DateTime fim)
        {
            return await _dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cupom)
                .Where(p => p.Status == PedidoStatus.Finalizado && p.DataPedido >= inicio && p.DataPedido < fim)
                .ToListAsync();
        }

        public async Task AddAsync(Pedido pedido)
        {
            await _dbContext.Pedidos.AddAsync(pedido);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            _dbContext.Pedidos.Update(pedido);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Pedido pedido)
        {
            _dbContext.Pedidos.Remove(pedido);
            await _dbContext.SaveChangesAsync();
        }
    }
}
