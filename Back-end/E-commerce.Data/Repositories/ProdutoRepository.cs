using Dunder_Store.Database;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public ProdutoRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Paginador<Produto>> GetAllAsync(
            string? nome = null,
            string? cor = null,
            string? tamanho = null,
            string? categoria = null,
            Guid? categoriaId = null,
            Guid[]? categoriaIds = null,
            int pagina = 1,
            int itensPorPagina = 10)
        {
            var query = _dbContext.Produtos
                .Include(p => p.Variacoes)
                .Include(p => p.Categoria)
                .Where(p => p.ProdutoPaiId == null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(p => p.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(cor))
                query = query.Where(p => p.Cor == cor);

            if (!string.IsNullOrEmpty(tamanho))
                query = query.Where(p => p.Tamanho == tamanho);

            if (categoriaIds != null && categoriaIds.Length > 0)
            {
                var todas = await _dbContext.Categorias
                    .Select(c => new { c.Id, c.CategoriaPaiId })
                    .ToListAsync();

                var ids = new HashSet<Guid>();
                void Coletar(Guid id)
                {
                    if (!ids.Add(id)) return;
                    foreach (var filho in todas.Where(c => c.CategoriaPaiId == id))
                    {
                        Coletar(filho.Id);
                    }
                }

                foreach (var cid in categoriaIds)
                {
                    Coletar(cid);
                }

                query = query.Where(p => ids.Contains(p.CategoriaId));
            }
            else if (categoriaId.HasValue)
            {
                var todas = await _dbContext.Categorias
                    .Select(c => new { c.Id, c.CategoriaPaiId })
                    .ToListAsync();

                var ids = new HashSet<Guid>();
                void Coletar(Guid id)
                {
                    if (!ids.Add(id)) return;
                    foreach (var filho in todas.Where(c => c.CategoriaPaiId == id))
                    {
                        Coletar(filho.Id);
                    }
                }
                Coletar(categoriaId.Value);

                query = query.Where(p => ids.Contains(p.CategoriaId));
            }
            else if (!string.IsNullOrEmpty(categoria))
                query = query.Where(p => p.Categoria != null && p.Categoria.Nome == categoria);

            var totalItens = await query.CountAsync();

            var produtos = await query
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToListAsync();

            foreach (var p in produtos)
            {
                if (p.Variacoes == null)
                    p.Variacoes = new();
            }

            return new Paginador<Produto>(produtos, totalItens, pagina, itensPorPagina);
        }

        public async Task<Produto?> GetByIdAsync(Guid id)
        {
            var produto = await _dbContext.Produtos
                .Include(p => p.Variacoes)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto != null && produto.Variacoes == null)
                produto.Variacoes = new();

            return produto;
        }

        public async Task<IEnumerable<Produto>> GetVariacoesByProdutoPaiAsync(Guid produtoPaiId)
        {
            var variacoes = await _dbContext.Produtos
                .Where(p => p.ProdutoPaiId == produtoPaiId)
                .ToListAsync();

            foreach (var v in variacoes)
            {
                if (v.Variacoes == null)
                    v.Variacoes = new();
            }

            return variacoes;
        }

        public async Task AddAsync(Produto produto)
        {
            await _dbContext.Produtos.AddAsync(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _dbContext.Produtos.Update(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Produto produto)
        {
            _dbContext.Produtos.Remove(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Produto> produtos)
        {
            var list = produtos?.ToList() ?? new List<Produto>();
            if (list.Count == 0) return;
            _dbContext.Produtos.RemoveRange(list);
            await _dbContext.SaveChangesAsync();
        }
    }
}
