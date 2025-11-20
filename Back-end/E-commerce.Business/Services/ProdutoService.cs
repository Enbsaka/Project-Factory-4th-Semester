using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
{
    public class ProdutoService(IProdutoRepository produtoRepository, IPedidoProdutoRepository pedidoProdutoRepository) : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository = produtoRepository;
        private readonly IPedidoProdutoRepository _pedidoProdutoRepository = pedidoProdutoRepository;

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

            await _produtoRepository.AddAsync(produto);
            return produto;
        }

        public async Task AtualizarProdutoAsync(Produto produto)
        {
            var existente = await _produtoRepository.GetByIdAsync(produto.Id);
            if (existente == null) throw new Exception("Produto não encontrado.");

            existente.Nome = produto.Nome ?? existente.Nome;
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
                    await _produtoRepository.UpdateAsync(v);
                }
            }
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
                    if (await _pedidoProdutoRepository.ExisteEmPedidoFinalizadoAsync(produto.Id))
                        throw new Exception("Um ou mais produtos já constam em pedidos finalizados; não é possível remover todos.");

                    deletaveis.Add(produto);
                    if (produto.Variacoes != null)
                    {
                        foreach (var v in produto.Variacoes)
                        {
                            if (await _pedidoProdutoRepository.ExisteEmPedidoFinalizadoAsync(v.Id))
                                throw new Exception("Variações de produto constam em pedidos finalizados; não é possível remover todos.");
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

            // Regra: preservar histórico — não permitir remover se já foi vendido (pedido finalizado)
            if (await _pedidoProdutoRepository.ExisteEmPedidoFinalizadoAsync(produto.Id))
                throw new Exception("Produto presente em pedidos finalizados; não pode ser removido.");

            await _pedidoProdutoRepository.RemoverPorProdutoIdAsync(produto.Id);

            if (produto.Variacoes != null)
            {
                foreach (var v in produto.Variacoes)
                {
                    // Mesma regra aplicada às variações
                    if (await _pedidoProdutoRepository.ExisteEmPedidoFinalizadoAsync(v.Id))
                        throw new Exception("Variação presente em pedidos finalizados; não pode ser removida.");
                    await _pedidoProdutoRepository.RemoverPorProdutoIdAsync(v.Id);
                    await _produtoRepository.DeleteAsync(v);
                }
            }

            await _produtoRepository.DeleteAsync(produto);
        }
    }
}
