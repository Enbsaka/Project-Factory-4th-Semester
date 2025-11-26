using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(Guid id);

        Task<Paginador<Pedido>> GetPagedAsync(int pagina, int tamanhoPagina);
        Task<Pedido?> GetDetalhadoByIdAsync(Guid id);
        Task<Paginador<Pedido>> GetByClientePagedAsync(Guid clienteId, PedidoStatus? status, int pagina, int tamanhoPagina);
        Task<Pedido?> GetCarrinhoByClienteAsync(Guid clienteId);
        Task<List<Pedido>> GetFinalizadosInRangeAsync(DateTime inicio, DateTime fim);

        Task AddAsync(Pedido pedido);
        Task UpdateAsync(Pedido pedido);
        Task DeleteAsync(Pedido pedido);
    }
}
