using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface ICupomRepository
    {
        Task<Cupom?> GetByIdAsync(Guid id);
        Task<Cupom?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Cupom>> GetAllAsync();
        Task<Cupom> AddAsync(Cupom cupom);
        Task<bool> UpdateAsync(Cupom cupom);
        Task<bool> RemoveAsync(Guid id);
    }
}
