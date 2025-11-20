using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface IProdutoRepository
    {
        Task<Paginador<Produto>> GetAllAsync(string? nome = null, string? cor = null, string? tamanho = null,
            string? categoria = null, Guid? categoriaId = null, Guid[]? categoriaIds = null, int pagina = 1, int itensPorPagina = 10);

        Task<Produto?> GetByIdAsync(Guid id);
        Task<IEnumerable<Produto>> GetVariacoesByProdutoPaiAsync(Guid produtoPaiId);
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(Produto produto);
        Task DeleteRangeAsync(IEnumerable<Produto> produtos);
    }
}
