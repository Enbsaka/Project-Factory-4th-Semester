using Dunder_Store.E_commerce.Business.DTO;
using Dunder_Store.E_commerce.Business.Interfaces.IRepositories;
using Dunder_Store.E_commerce.Business.Interfaces.IServices;
using System.Net.Http.Json;

namespace Dunder_Store.E_commerce.Business.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly IPrecoRegiaoRepository _shippingPriceRepository;

        public ViaCepService(HttpClient httpClient, IPrecoRegiaoRepository shippingPriceRepository)
        {
            _httpClient = httpClient;
            _shippingPriceRepository = shippingPriceRepository;
        }

        public async Task<ViaCepResponseDTO?> BuscarEnderecoPorCepAsync(string cep)
        {
            cep = cep.Replace("-", "").Trim();

            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                return null;

            try
            {
                var result = await _httpClient.GetFromJsonAsync<ViaCepResponseDTO>($"https://viacep.com.br/ws/{cep}/json/");
                if (result == null) return null;
                if (result.Erro) return null;
                return result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<(ViaCepResponseDTO? endereco, decimal preco)> CalcularFreteAsync(string cep)
        {
            var endereco = await BuscarEnderecoPorCepAsync(cep);

            if (endereco == null || string.IsNullOrWhiteSpace(endereco.Uf))
                return (null, 0);

            string regiao = endereco.Uf switch
            {
                "AC" or "AM" or "AP" or "PA" or "RO" or "RR" or "TO" => "Norte",
                "AL" or "BA" or "CE" or "MA" or "PB" or "PE" or "PI" or "RN" or "SE" => "Nordeste",
                "DF" or "GO" or "MT" or "MS" => "Centro-Oeste",
                "ES" or "MG" or "RJ" or "SP" => "Sudeste",
                "PR" or "RS" or "SC" => "Sul",
                _ => "Sudeste"
            };

            var preco = await _shippingPriceRepository.ObterPrecoPorRegiaoAsync(regiao);
            return (endereco, preco);
        }
    }
}
