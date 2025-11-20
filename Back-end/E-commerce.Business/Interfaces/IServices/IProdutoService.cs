using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IServices
{
    public interface IProdutoService
    {
        Task<Paginador<Produto>> GetAllAsync(string? nome = null, string? cor = null, string? tamanho = null,
            string? categoria = null, Guid? categoriaId = null, Guid[]? categoriaIds = null, int pagina = 1, int itensPorPagina = 10);

        Task<Produto?> GetByIdAsync(Guid id);
        Task<Produto> CriarProdutoAsync(Produto produto);
        Task AtualizarProdutoAsync(Produto produto);
        Task RemoverProdutoAsync(Guid id);

        Task RemoverTodosProdutosAsync();
    }
}
