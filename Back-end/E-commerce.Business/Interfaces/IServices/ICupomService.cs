using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IServices
{
    public interface ICupomService
    {
        Task<Cupom> CriarCupomAsync(Cupom cupom);
        Task<Cupom?> GetByCodigoAsync(string codigo);
        Task<Cupom?> GetByIdAsync(Guid id);
        Task<IEnumerable<Cupom>> GetAllAsync();
        Task<bool> AtualizarCupomAsync(Cupom cupom);
        Task<bool> RemoverCupomAsync(Guid id);
    }
}
