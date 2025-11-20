using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Dunder_Store.Interfaces.IRepositories;

namespace Dunder_Store.Services
{
    public class CupomService : ICupomService
    {
        private readonly ICupomRepository _repo;

        public CupomService(ICupomRepository repo)
        {
            _repo = repo;
        }

        public Task<Cupom> CriarCupomAsync(Cupom cupom)
        {
            cupom.Codigo = cupom.Codigo.Trim().ToUpper();
            return _repo.AddAsync(cupom);
        }

        public Task<Cupom?> GetByCodigoAsync(string codigo) => _repo.GetByCodigoAsync(codigo);

        public Task<Cupom?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Cupom>> GetAllAsync() => _repo.GetAllAsync();

        public Task<bool> AtualizarCupomAsync(Cupom cupom) => _repo.UpdateAsync(cupom);

        public Task<bool> RemoverCupomAsync(Guid id) => _repo.RemoveAsync(id);
    }
}
