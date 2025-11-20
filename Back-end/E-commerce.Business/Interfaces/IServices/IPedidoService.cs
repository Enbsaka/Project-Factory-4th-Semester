using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IServices
{
    public interface IPedidoService
    {
        Task<Paginador<Pedido>> GetPedidosAsync(int pagina = 1, int tamanhoPagina = 10);
        Task<Pedido?> GetDetalhadoAsync(Guid id);
        Task<Paginador<Pedido>> GetPedidosPorClienteAsync(Guid clienteId, PedidoStatus? status = null, int pagina = 1, int tamanhoPagina = 10);

        Task<Pedido> GetOrCreateCarrinhoAsync(Guid clienteId);

        Task<Pedido> CriarPedidoAsync(string clienteCpf, List<(string CodigoDeBarra, int Quantidade)> itens, string? cupomCodigo, decimal? freteValor);

        Task AtualizarItensAsync(Guid pedidoId, List<(string CodigoDeBarra, int Quantidade)> itens);
        Task AtualizarCupomAsync(Guid pedidoId, string? cupomCodigo);
        Task AtualizarFreteAsync(Guid pedidoId, decimal? freteValor);
        Task FinalizarPedidoAsync(Guid id);

        Task RemoverPedidoAsync(Guid id);
        Task RemoverPedidosClienteAsync(Guid clienteId, bool incluirFinalizados = false);

        Task<IEnumerable<object>> GetProdutosMaisVendidosAsync(int top = 5);
        Task<decimal> GetReceitaMensalAsync(DateTime referencia);
    }
}
