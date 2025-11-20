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
            var isVariation = novoProdutoDTO.produtoPaiId.HasValue;
            var imagem = novoProdutoDTO.imagem;
            if (!isVariation && (imagem == null || imagem.Length == 0))
                return BadRequest("Imagem obrigatória para produto base.");

            if (novoProdutoDTO.CategoriaId == null && !string.IsNullOrEmpty(novoProdutoDTO.CategoriaNome))
            {
                var categoria = await _categoriaService.GetByNomeAsync(novoProdutoDTO.CategoriaNome);
                if (categoria == null)
                    return BadRequest($"Categoria '{novoProdutoDTO.CategoriaNome}' não existe.");
                novoProdutoDTO.CategoriaId = categoria.Id;
            }

            if (novoProdutoDTO.CategoriaId == null)
                return BadRequest("Categoria obrigatória. Informe o nome ou o ID.");

            var subcats = await _categoriaService.GetSubcategoriasAsync(novoProdutoDTO.CategoriaId.Value);
            if (subcats != null && subcats.Count > 0)
                return BadRequest("Produtos devem ser cadastrados apenas em subcategorias (folhas).");

            string? imagemUrl = null;
            if (imagem != null && imagem.Length > 0)
            {
                string nomePasta = "produtos";
                string caminhoDaPasta = Path.Combine("wwwroot", nomePasta);
                Directory.CreateDirectory(caminhoDaPasta);

                string nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
                string caminhoCompleto = Path.Combine(caminhoDaPasta, nomeArquivo);

                using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                await imagem.CopyToAsync(stream);

                string urlServidor = $"{Request.Scheme}://{Request.Host}";
                imagemUrl = $"{urlServidor}/{nomePasta}/{nomeArquivo}";
            }

            var novoProduto = new Produto
            {
                Nome = novoProdutoDTO.nome,
                Descricao = novoProdutoDTO.descricao,
                Preco = novoProdutoDTO.preco,
                CodigoDeBarra = novoProdutoDTO.codigoDeBarra,
                ImagemURL = imagemUrl,
                CategoriaId = novoProdutoDTO.CategoriaId.Value,
                Cor = novoProdutoDTO.cor,
                Tamanho = novoProdutoDTO.tamanho,
                ProdutoPaiId = novoProdutoDTO.produtoPaiId
            };

            var produtoCriado = await _produtoService.CriarProdutoAsync(novoProduto);
            return CreatedAtAction(nameof(GetProdutoById), new { id = produtoCriado.Id }, produtoCriado);
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
                var subcatsUpdate = await _categoriaService.GetSubcategoriasAsync(dto.CategoriaId.Value);
                if (subcatsUpdate != null && subcatsUpdate.Count > 0)
                    return BadRequest("Produtos devem estar vinculados a subcategorias (folhas).");
                produto.CategoriaId = dto.CategoriaId.Value;
            }
            produto.ProdutoPaiId = dto.produtoPaiId ?? produto.ProdutoPaiId;

            if (novaImagem != null && novaImagem.Length > 0)
            {
                string nomePasta = "produtos";
                string caminhoDaPasta = Path.Combine("wwwroot", nomePasta);
                Directory.CreateDirectory(caminhoDaPasta);

                if (!string.IsNullOrEmpty(produto.ImagemURL))
                {
                    var nomeAntigo = Path.GetFileName(new Uri(produto.ImagemURL).AbsolutePath);
                    var caminhoAntigo = Path.Combine(caminhoDaPasta, nomeAntigo);
                    if (System.IO.File.Exists(caminhoAntigo))
                        System.IO.File.Delete(caminhoAntigo);
                }

                string nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(novaImagem.FileName)}";
                string caminhoCompleto = Path.Combine(caminhoDaPasta, nomeArquivo);

                using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                await novaImagem.CopyToAsync(stream);

                string urlServidor = $"{Request.Scheme}://{Request.Host}";
                produto.ImagemURL = $"{urlServidor}/{nomePasta}/{nomeArquivo}";
            }

            await _produtoService.AtualizarProdutoAsync(produto);
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao remover produto: {ex.Message}");
            }
        }

        [HttpDelete("todos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTodosProdutos()
        {
            await _produtoService.RemoverTodosProdutosAsync();
            return NoContent();
        }

        [HttpPost("import-csv")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> ImportProdutosCsv(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0) return BadRequest("Arquivo CSV obrigatório.");

            using var stream = new MemoryStream();
            await arquivo.CopyToAsync(stream);
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, true);

            var header = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(header)) return BadRequest("Cabeçalho ausente.");

            var esperado = new[] { "nome", "descricao", "preco", "codigoDeBarra", "imagemUrl", "cor", "tamanho", "categoriaCaminho" };
            var colsHeader = SplitCsvSemicolon(header);
            if (colsHeader.Count != esperado.Length) return BadRequest("Cabeçalho inválido.");
            for (int i = 0; i < esperado.Length; i++) if (!string.Equals(colsHeader[i], esperado[i], StringComparison.OrdinalIgnoreCase)) return BadRequest("Cabeçalho inválido.");

            var arvore = await _categoriaService.GetCategoriasHierarquicasAsync();
            var criados = 0;
            var erros = new List<string>();

            while (!reader.EndOfStream)
            {
                var linha = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(linha)) continue;
                var cols = SplitCsvSemicolon(linha);
                if (cols.Count < esperado.Length)
                {
                    erros.Add("Linha incompleta");
                    continue;
                }

                var nome = cols[0]?.Trim() ?? string.Empty;
                var descricao = cols[1]?.Trim() ?? string.Empty;
                if (!decimal.TryParse(cols[2], out var preco)) { erros.Add($"Preço inválido: {cols[2]}"); continue; }
                var codigo = string.IsNullOrWhiteSpace(cols[3]) ? ($"IMP-{Guid.NewGuid():N}") : cols[3].Trim();
                var imagemUrl = string.IsNullOrWhiteSpace(cols[4]) ? null : cols[4].Trim();
                var cor = string.IsNullOrWhiteSpace(cols[5]) ? null : cols[5].Trim();
                var tamanho = string.IsNullOrWhiteSpace(cols[6]) ? null : cols[6].Trim();
                var caminho = cols[7]?.Trim() ?? string.Empty;

                var categoriaId = FindCategoriaIdByPath(arvore, caminho);
                if (!categoriaId.HasValue) { erros.Add($"Categoria não encontrada: {caminho}"); continue; }
                var subcats = await _categoriaService.GetSubcategoriasAsync(categoriaId.Value);
                if (subcats != null && subcats.Count > 0) { erros.Add($"Categoria não é folha: {caminho}"); continue; }

                var novo = new Produto
                {
                    Nome = nome,
                    Descricao = descricao,
                    Preco = preco,
                    CodigoDeBarra = codigo,
                    ImagemURL = imagemUrl,
                    CategoriaId = categoriaId.Value,
                    Cor = cor,
                    Tamanho = tamanho
                };

                try
                {
                    await _produtoService.CriarProdutoAsync(novo);
                    criados++;
                }
                catch (Exception ex)
                {
                    erros.Add(ex.Message);
                }
            }

            return Ok(new { criados, erros });
        }

        private static List<string> SplitCsvSemicolon(string line)
        {
            var res = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else inQuotes = !inQuotes;
                }
                else if (c == ';' && !inQuotes)
                {
                    res.Add(sb.ToString());
                    sb.Clear();
                }
                else sb.Append(c);
            }
            res.Add(sb.ToString());
            for (int i = 0; i < res.Count; i++)
            {
                var v = res[i].Trim();
                if (v.StartsWith("\"") && v.EndsWith("\"")) v = v.Substring(1, v.Length - 2);
                res[i] = v;
            }
            return res;
        }

        private static Guid? FindCategoriaIdByPath(List<Categoria> raiz, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            var parts = path.Split('>', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0) return null;
            Categoria? atual = raiz.FirstOrDefault(c => string.Equals(c.Nome, parts[0], StringComparison.OrdinalIgnoreCase));
            if (atual == null) return null;
            for (int i = 1; i < parts.Length; i++)
            {
                atual = atual.Subcategorias.FirstOrDefault(c => string.Equals(c.Nome, parts[i], StringComparison.OrdinalIgnoreCase));
                if (atual == null) return null;
            }
            return atual.Id;
        }

}
}
