using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
{
    public class ProdutoService(IProdutoRepository produtoRepository, IPedidoProdutoRepository pedidoProdutoRepository, ICategoriaService categoriaService, IFileStorageService fileStorage) : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository = produtoRepository;
        private readonly IPedidoProdutoRepository _pedidoProdutoRepository = pedidoProdutoRepository;
        private readonly ICategoriaService _categoriaService = categoriaService;
        private readonly IFileStorageService _fileStorage = fileStorage;

        public async Task<Paginador<Produto>> GetAllAsync(string? nome = null, string? cor = null, string? tamanho = null,
            string? categoria = null, Guid? categoriaId = null, Guid[]? categoriaIds = null, int pagina = 1, int itensPorPagina = 10)
        {
            return await _produtoRepository.GetAllAsync(nome, cor, tamanho, categoria, categoriaId, categoriaIds, pagina, itensPorPagina);
        }

        public async Task<Produto?> GetByIdAsync(Guid id) => await _produtoRepository.GetByIdAsync(id);

        public async Task<Produto> CriarProdutoAsync(Produto produto)
        {
            if (produto.ProdutoPaiId.HasValue)
            {
                var pai = await _produtoRepository.GetByIdAsync(produto.ProdutoPaiId.Value);
                if (pai == null) throw new Exception("Produto pai não existe.");
            }
            if (string.IsNullOrWhiteSpace(produto.CodigoDeBarra))
            {
                var prefix = "789";
                var lastBase = await _produtoRepository.GetMaxEAN13BaseAsync(prefix);
                var nextBase = lastBase != null ? IncrementBase12(lastBase) : "789000000001";
                var checksum = CalculateEan13Checksum(nextBase);
                var code = nextBase + checksum;
                if (await _produtoRepository.ExistsByCodigoAsync(code))
                {
                    nextBase = IncrementBase12(nextBase);
                    checksum = CalculateEan13Checksum(nextBase);
                    code = nextBase + checksum;
                }
                produto.CodigoDeBarra = code;
            }

            await _produtoRepository.AddAsync(produto);
            return produto;
        }

        public async Task<Produto> CriarProdutoComImagemAsync(Produto novo, Stream? imagemStream, string? imagemFileName, string baseUrl)
        {
            if (novo.ProdutoPaiId.HasValue)
            {
                var pai = await _produtoRepository.GetByIdAsync(novo.ProdutoPaiId.Value);
                if (pai == null) throw new Exception("Produto pai não existe.");
            }
            if (novo.CategoriaId == Guid.Empty)
                throw new Exception("Categoria obrigatória.");

            var subcats = await _categoriaService.GetSubcategoriasAsync(novo.CategoriaId);
            if (subcats != null && subcats.Count > 0)
                throw new Exception("Produtos devem ser cadastrados apenas em subcategorias (folhas).");

            if (imagemStream == null)
                throw new Exception("Imagem obrigatória para produto base.");

            novo.ImagemURL = _fileStorage.SaveProductImage(imagemStream, imagemFileName ?? "imagem.png", null, baseUrl);

            return await CriarProdutoAsync(novo);
        }

        private static string IncrementBase12(string base12)
        {
            var num = long.Parse(base12);
            num++;
            var s = num.ToString().PadLeft(12, '0');
            if (!s.StartsWith("789")) s = "789" + s.Substring(3);
            return s;
        }

        private static string CalculateEan13Checksum(string base12)
        {
            var sum = 0;
            for (int i = 0; i < 12; i++)
            {
                var digit = base12[i] - '0';
                var posFromRight = 12 - i;
                var weight = (posFromRight % 2 == 1) ? 3 : 1;
                sum += digit * weight;
            }
            var mod = sum % 10;
            var check = (10 - mod) % 10;
            return check.ToString();
        }

        public async Task AtualizarProdutoAsync(Produto produto)
        {
            var existente = await _produtoRepository.GetByIdAsync(produto.Id);
            if (existente == null) throw new Exception("Produto não encontrado.");

            var nomeOriginal = existente.Nome;
            var novoNome = produto.Nome ?? existente.Nome;
            var nomeAlterado = produto.Nome != null && !string.Equals(novoNome, nomeOriginal, StringComparison.Ordinal);
            existente.Nome = novoNome;
            existente.Descricao = produto.Descricao ?? existente.Descricao;
            existente.Preco = produto.Preco > 0 ? produto.Preco : existente.Preco;
            existente.CodigoDeBarra = produto.CodigoDeBarra ?? existente.CodigoDeBarra;
            existente.Cor = produto.Cor ?? existente.Cor;
            existente.Tamanho = produto.Tamanho ?? existente.Tamanho;
            existente.CategoriaId = produto.CategoriaId != Guid.Empty ? produto.CategoriaId : existente.CategoriaId;

            await _produtoRepository.UpdateAsync(existente);

            if (existente.Variacoes != null && existente.Variacoes.Count > 0)
            {
                foreach (var v in existente.Variacoes)
                {
                    v.CategoriaId = existente.CategoriaId;
                    v.Preco = existente.Preco;
                    if (nomeAlterado)
                    {
                        var sufixo = new List<string>();
                        if (!string.IsNullOrWhiteSpace(v.Cor)) sufixo.Add(v.Cor);
                        if (!string.IsNullOrWhiteSpace(v.Tamanho)) sufixo.Add(v.Tamanho);
                        var combinado = sufixo.Count > 0 ? $"{novoNome} - {string.Join(" / ", sufixo)}" : novoNome;
                        v.Nome = combinado;
                    }
                    await _produtoRepository.UpdateAsync(v);
                }
            }
        }

        public async Task AtualizarProdutoComImagemAsync(Produto existente, Stream? novaImagemStream, string? novaImagemFileName, string baseUrl)
        {
            if (existente.CategoriaId != Guid.Empty)
            {
                var subcatsUpdate = await _categoriaService.GetSubcategoriasAsync(existente.CategoriaId);
                if (subcatsUpdate != null && subcatsUpdate.Count > 0)
                    throw new Exception("Produtos devem estar vinculados a subcategorias (folhas).");
            }

            if (novaImagemStream != null)
            {
                existente.ImagemURL = _fileStorage.SaveProductImage(novaImagemStream, novaImagemFileName ?? "imagem.png", existente.ImagemURL, baseUrl);
            }

            await AtualizarProdutoAsync(existente);
        }

        public async Task RemoverTodosProdutosAsync()
        {
            const int pageSize = 200;
            var page = 1;
            while (true)
            {
                var paged = await _produtoRepository.GetAllAsync(pagina: page, itensPorPagina: pageSize);
                var produtos = paged.Itens.ToList();
                if (produtos.Count == 0) break;

                var deletaveis = new List<Produto>();
                var variacoesDeletaveis = new List<Produto>();
                foreach (var produto in produtos)
                {
                    deletaveis.Add(produto);
                    if (produto.Variacoes != null)
                    {
                        foreach (var v in produto.Variacoes)
                        {
                            variacoesDeletaveis.Add(v);
                        }
                    }
                }

                var idsParaRemoverRelacoes = deletaveis.Select(p => p.Id).Concat(variacoesDeletaveis.Select(v => v.Id)).ToList();
                if (idsParaRemoverRelacoes.Count > 0)
                    await _pedidoProdutoRepository.RemoverPorProdutoIdsAsync(idsParaRemoverRelacoes);

                if (variacoesDeletaveis.Count > 0)
                    await _produtoRepository.DeleteRangeAsync(variacoesDeletaveis);

                if (deletaveis.Count > 0)
                    await _produtoRepository.DeleteRangeAsync(deletaveis);

                page++;
            }
        }

        public async Task RemoverProdutoAsync(Guid id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) throw new Exception("Produto não encontrado.");

            await _pedidoProdutoRepository.RemoverPorProdutoIdAsync(produto.Id);

            if (produto.Variacoes != null)
            {
                foreach (var v in produto.Variacoes)
                {
                    await _pedidoProdutoRepository.RemoverPorProdutoIdAsync(v.Id);
                    await _produtoRepository.DeleteAsync(v);
                }
            }

            await _produtoRepository.DeleteAsync(produto);
        }
    }
}
