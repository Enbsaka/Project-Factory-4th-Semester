using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(Guid id);
        Task<Categoria?> GetByNomeAsync(string nome);
        Task<List<Categoria>> GetSubcategoriasAsync(Guid categoriaPaiId);

        Task AddAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(Categoria categoria);

        Task SaveChangesAsync();
    }
}
