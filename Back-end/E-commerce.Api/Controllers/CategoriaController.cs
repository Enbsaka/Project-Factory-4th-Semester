using Dunder_Store.DTO;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dunder_Store.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _categoriaService.GetAllAsync();
            var result = categorias.Select(c => new
            {
                c.Id,
                c.Nome,
                c.CategoriaPaiId
            });

            return Ok(result);
        }

        [HttpGet("hierarquia")]
        public async Task<IActionResult> GetHierarquia()
        {
            var categorias = await _categoriaService.GetCategoriasHierarquicasAsync();
            return Ok(categorias);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            if (categoria == null)
                return NotFound("Categoria não encontrada.");

            return Ok(new
            {
                categoria.Id,
                categoria.Nome,
                categoria.CategoriaPaiId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CategoriaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var categoria = await _categoriaService.CriarCategoriaAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CategoriaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoriaService.AtualizarCategoriaAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _categoriaService.RemoverCategoriaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
