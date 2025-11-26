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

        public async Task<bool> AtualizarCupomComPatchAsync(Guid id, DTO.CupomPatchDTO dto)
        {
            var cupom = await _repo.GetByIdAsync(id);
            if (cupom == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Codigo)) cupom.Codigo = dto.Codigo.Trim().ToUpper();
            if (dto.DescontoPercentual.HasValue) cupom.DescontoPercentual = dto.DescontoPercentual.Value;
            if (dto.DataExpiracao.HasValue) cupom.DataExpiracao = dto.DataExpiracao.Value;
            if (dto.Ativo.HasValue) cupom.Ativo = dto.Ativo.Value;

            return await _repo.UpdateAsync(cupom);
        }
    }
}
