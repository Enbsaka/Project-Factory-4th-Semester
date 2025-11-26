using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController(IProdutoService produtoService, ICategoriaService categoriaService) : ControllerBase
    {
        private readonly IProdutoService _produtoService = produtoService;
        private readonly ICategoriaService _categoriaService = categoriaService;

        private string? ToAbsoluteImageUrl(string? storedUrl)
        {
            if (string.IsNullOrWhiteSpace(storedUrl)) return storedUrl;

            if (Uri.TryCreate(storedUrl, UriKind.Absolute, out var uri))
            {
                var path = uri.AbsolutePath;
                var file = System.IO.Path.GetFileName(path);
                if (!string.IsNullOrEmpty(file) && path.Contains("/produtos"))
                {
                    return $"{Request.Scheme}://{Request.Host}/produtos/{file}";
                }
                return storedUrl;
            }

            var rel = storedUrl.TrimStart('/');
            if (rel.StartsWith("produtos"))
            {
                var file = System.IO.Path.GetFileName(rel);
                return $"{Request.Scheme}://{Request.Host}/produtos/{file}";
            }

            var onlyFile = System.IO.Path.GetFileName(rel);
            if (!string.IsNullOrEmpty(onlyFile))
            {
                return $"{Request.Scheme}://{Request.Host}/produtos/{onlyFile}";
            }

            return storedUrl;
        }

        [HttpGet]
        public async Task<ActionResult<Paginador<Produto>>> GetAllProdutos(
            string? nome = null,
            string? cor = null,
            string? tamanho = null,
            string? categoria = null,
            Guid? categoriaId = null,
            [FromQuery(Name = "categoriaIds")] Guid[]? categoriaIds = null,
            [FromQuery(Name = "paginaAtual")] int pagina = 1,
            [FromQuery(Name = "tamanhoPagina")] int itensPorPagina = 10)
        {
            Guid[]? idsFinal = categoriaIds;
            if (idsFinal == null || idsFinal.Length == 0)
            {
                var valores = Request.Query["categoriaIds"];
                if (valores.Count == 0)
                    valores = Request.Query["categoriaIds[]"];

                if (valores.Count > 0)
                {
                    var lista = new List<Guid>();
                    foreach (var entrada in valores)
                    {
                        if (string.IsNullOrEmpty(entrada)) continue;
                        var partes = entrada.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        foreach (var parte in partes)
                        {
                            if (Guid.TryParse(parte, out var gid)) lista.Add(gid);
                        }
                    }
                    if (lista.Count > 0) idsFinal = lista.ToArray();
                }
            }

            var produtos = await _produtoService.GetAllAsync(nome, cor, tamanho, categoria, categoriaId, idsFinal, pagina, itensPorPagina);
            if (produtos == null) return NotFound("Nenhum produto encontrado.");

            foreach (var p in produtos.Itens)
            {
                p.ImagemURL = ToAbsoluteImageUrl(p.ImagemURL);
                if (p.Variacoes != null)
                {
                    foreach (var v in p.Variacoes)
                    {
                        v.ImagemURL = ToAbsoluteImageUrl(v.ImagemURL);
                    }
                }
            }

            return Ok(produtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProdutoById(Guid id)
        {
            var produto = await _produtoService.GetByIdAsync(id);
            if (produto is null) return NotFound("Produto não encontrado.");

            produto.ImagemURL = ToAbsoluteImageUrl(produto.ImagemURL);
            if (produto.Variacoes != null)
            {
                foreach (var v in produto.Variacoes)
                {
                    v.ImagemURL = ToAbsoluteImageUrl(v.ImagemURL);
                }
            }

            return Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Produto>> CreateProduto([FromForm] ProdutoDTO novoProdutoDTO)
        {
            if (novoProdutoDTO.CategoriaId == null && !string.IsNullOrEmpty(novoProdutoDTO.CategoriaNome))
            {
                var categoria = await _categoriaService.GetByNomeAsync(novoProdutoDTO.CategoriaNome);
                if (categoria == null)
                    return BadRequest($"Categoria '{novoProdutoDTO.CategoriaNome}' não existe.");
                novoProdutoDTO.CategoriaId = categoria.Id;
            }

            var novoProduto = new Produto
            {
                Nome = novoProdutoDTO.nome,
                Descricao = novoProdutoDTO.descricao,
                Preco = novoProdutoDTO.preco,
                CodigoDeBarra = novoProdutoDTO.codigoDeBarra ?? string.Empty,
                CategoriaId = novoProdutoDTO.CategoriaId ?? Guid.Empty,
                Cor = novoProdutoDTO.cor,
                Tamanho = novoProdutoDTO.tamanho,
                ProdutoPaiId = novoProdutoDTO.produtoPaiId
            };
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                if (novoProdutoDTO.imagem != null && novoProdutoDTO.imagem.Length > 0)
                {
                    using var stream = novoProdutoDTO.imagem.OpenReadStream();
                    var produtoCriado = await _produtoService.CriarProdutoComImagemAsync(novoProduto, stream, novoProdutoDTO.imagem.FileName, baseUrl);
                    return CreatedAtAction(nameof(GetProdutoById), new { id = produtoCriado.Id }, produtoCriado);
                }
                else
                {
                    var produtoCriado = await _produtoService.CriarProdutoAsync(novoProduto);
                    return CreatedAtAction(nameof(GetProdutoById), new { id = produtoCriado.Id }, produtoCriado);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduto(Guid id, [FromForm] ProdutoPatchDTO dto, IFormFile? novaImagem)
        {
            var produto = await _produtoService.GetByIdAsync(id);
            if (produto is null)
                return NotFound("Produto não encontrado.");

            if (dto.CategoriaId == null && !string.IsNullOrEmpty(dto.CategoriaNome))
            {
                var categoria = await _categoriaService.GetByNomeAsync(dto.CategoriaNome);
                if (categoria == null)
                    return BadRequest($"Categoria '{dto.CategoriaNome}' não existe.");
                dto.CategoriaId = categoria.Id;
            }

            produto.Nome = dto.nome ?? produto.Nome;
            produto.Descricao = dto.descricao ?? produto.Descricao;
            produto.Preco = dto.preco.HasValue ? dto.preco.Value : produto.Preco;
            produto.CodigoDeBarra = dto.codigoDeBarra ?? produto.CodigoDeBarra;
            produto.Cor = dto.cor ?? produto.Cor;
            produto.Tamanho = dto.tamanho ?? produto.Tamanho;
            if (dto.CategoriaId.HasValue)
            {
                produto.CategoriaId = dto.CategoriaId.Value;
            }
            produto.ProdutoPaiId = dto.produtoPaiId ?? produto.ProdutoPaiId;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            if (novaImagem != null && novaImagem.Length > 0)
            {
                using var stream = novaImagem.OpenReadStream();
                await _produtoService.AtualizarProdutoComImagemAsync(produto, stream, novaImagem.FileName, baseUrl);
            }
            else
            {
                await _produtoService.AtualizarProdutoAsync(produto);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            var existente = await _produtoService.GetByIdAsync(id);
            if (existente is null)
                return NotFound("Produto não encontrado.");

            try
            {
                await _produtoService.RemoverProdutoAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao remover produto: {ex.Message}");
            }
        }

        [HttpDelete("todos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTodosProdutos()
        {
            try
            {
                await _produtoService.RemoverTodosProdutosAsync();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao remover produtos: {ex.Message}");
            }
        }
    }
}
