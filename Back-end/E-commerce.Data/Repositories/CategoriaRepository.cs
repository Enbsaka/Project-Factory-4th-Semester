using Dunder_Store.Database;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ProdutosDbContext _context;

        public CategoriaRepository(ProdutosDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Produtos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(Guid id)
        {
            return await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria?> GetByNomeAsync(string nome)
        {
            var normalized = nome.Trim().ToLower();
            return await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == normalized);
        }

        public async Task<List<Categoria>> GetSubcategoriasAsync(Guid categoriaPaiId)
        {
            return await _context.Categorias
                .Where(c => c.CategoriaPaiId == categoriaPaiId)
                .Include(c => c.Subcategorias)
                .Include(c => c.Produtos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
        }

        public Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
