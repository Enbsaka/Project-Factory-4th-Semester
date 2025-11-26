using Dunder_Store.DTO;
using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IServices
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(Guid id);
        Task<Categoria?> GetByNomeAsync(string nome);
        Task<List<Categoria>> GetSubcategoriasAsync(Guid categoriaPaiId);

        Task<List<Categoria>> GetCategoriasHierarquicasAsync();

        Task<Categoria> CriarCategoriaAsync(CategoriaDTO dto);
        Task AtualizarCategoriaAsync(Guid id, CategoriaDTO dto);
        Task RemoverCategoriaAsync(Guid id);
    }
}
