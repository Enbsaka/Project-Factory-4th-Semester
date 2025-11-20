using Dunder_Store.E_commerce.Business.DTO;
using System.Threading.Tasks;

namespace Dunder_Store.E_commerce.Business.Interfaces.IServices
{
    public interface IViaCepService
    {
        Task<ViaCepResponseDTO?> BuscarEnderecoPorCepAsync(string cep);
        Task<(ViaCepResponseDTO? endereco, decimal preco)> CalcularFreteAsync(string cep);
    }
}
