using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CupomController : ControllerBase
    {
        private readonly ICupomService _cupomService;

        public CupomController(ICupomService cupomService)
        {
            _cupomService = cupomService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cupom>>> GetAll()
        {
            var cupons = await _cupomService.GetAllAsync();
            return Ok(cupons);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<Cupom>> GetByCodigo(string codigo)
        {
            var cupom = await _cupomService.GetByCodigoAsync(codigo);
            if (cupom == null) return NotFound("Cupom inválido ou expirado.");
            return Ok(cupom);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Cupom>> CriarCupom([FromForm] CupomDTO dto)
        {
            var novoCupom = new Cupom
            {
                Codigo = dto.Codigo,
                DescontoPercentual = dto.DescontoPercentual,
                DataExpiracao = dto.DataExpiracao,
                Ativo = dto.Ativo
            };

            var criado = await _cupomService.CriarCupomAsync(novoCupom);
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = criado.Codigo }, criado);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AtualizarCupom(Guid id, [FromForm] CupomPatchDTO dto)
        {
            var cupom = await _cupomService.GetByIdAsync(id);
            if (cupom == null) return NotFound("Cupom não encontrado.");

            cupom.Codigo = dto.Codigo?.Trim().ToUpper() ?? cupom.Codigo;
            cupom.DescontoPercentual = dto.DescontoPercentual ?? cupom.DescontoPercentual;
            cupom.DataExpiracao = dto.DataExpiracao ?? cupom.DataExpiracao;
            cupom.Ativo = dto.Ativo ?? cupom.Ativo;

            var atualizado = await _cupomService.AtualizarCupomAsync(cupom);
            if (!atualizado) return BadRequest("Erro ao atualizar o cupom.");

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var removido = await _cupomService.RemoverCupomAsync(id);
            if (!removido) return NotFound("Cupom não encontrado.");
            return NoContent();
        }
    }
}
