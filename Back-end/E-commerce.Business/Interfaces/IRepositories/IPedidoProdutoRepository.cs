using System;
using System.Threading.Tasks;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface IPedidoProdutoRepository
    {
        Task RemoverPorProdutoIdAsync(Guid produtoId);
        Task RemoverPorProdutoIdsAsync(IEnumerable<Guid> produtoIds);
        Task<bool> ExisteEmPedidoFinalizadoAsync(Guid produtoId);
        Task ReassignFinalizadosAsync(Guid oldProdutoId, Guid newProdutoId, string? nomeSnapshot, string? codigoSnapshot);
        Task<IEnumerable<object>> GetTopMaisVendidosAsync(int top);
        Task RemoverPorPedidoIdAsync(Guid pedidoId);
        Task AddRangeAsync(IEnumerable<Dunder_Store.Entities.PedidoProduto> itens);
    }
}
