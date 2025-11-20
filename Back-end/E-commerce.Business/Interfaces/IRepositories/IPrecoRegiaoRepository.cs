using System.Threading.Tasks;

namespace Dunder_Store.E_commerce.Business.Interfaces.IRepositories
{
    public interface IPrecoRegiaoRepository
    {
        Task<decimal> ObterPrecoPorRegiaoAsync(string regiao);
    }
}
