using Dunder_Store.E_commerce.Business.DTO;
using Dunder_Store.E_commerce.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Dunder_Store.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViaCepController : ControllerBase
    {
        private readonly IViaCepService _viaCepService;

        public ViaCepController(IViaCepService viaCepService)
        {
            _viaCepService = viaCepService;
        }

        [HttpGet("{cep}")]
        [ProducesResponseType(typeof(ViaCepResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEndereco(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) || cep.Replace("-", "").Trim().Length != 8)
                return BadRequest("CEP inválido. Formato esperado: 8 dígitos.");

            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync(cep);
            if (endereco == null || string.IsNullOrWhiteSpace(endereco.Uf))
                return NotFound("Endereço não encontrado para o CEP informado.");

            return Ok(endereco);
        }


        [HttpGet("frete/{cep}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFrete(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) || cep.Replace("-", "").Trim().Length != 8)
                return BadRequest("CEP inválido. Formato esperado: 8 dígitos.");

            var (endereco, preco) = await _viaCepService.CalcularFreteAsync(cep);
            if (endereco == null || string.IsNullOrWhiteSpace(endereco.Uf))
                return NotFound("Endereço não encontrado para o CEP informado.");

            if (preco == 0m)
                return NotFound("Preço de frete não configurado para a região.");

            return Ok(new { endereco, preco });
        }
    }
}
