using System;
using System.Threading.Tasks;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface IPedidoProdutoRepository
    {
        Task RemoverPorProdutoIdAsync(Guid produtoId);
        Task RemoverPorProdutoIdsAsync(IEnumerable<Guid> produtoIds);
        Task<bool> ExisteEmPedidoFinalizadoAsync(Guid produtoId);
        Task<IEnumerable<object>> GetTopMaisVendidosAsync(int top);
    }
}
