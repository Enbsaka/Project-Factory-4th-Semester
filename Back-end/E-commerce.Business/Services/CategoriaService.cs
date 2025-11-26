using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _categoriaRepository.GetAllAsync();
        }

        public async Task<Categoria?> GetByIdAsync(Guid id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task<Categoria?> GetByNomeAsync(string nome)
        {
            return await _categoriaRepository.GetByNomeAsync(nome);
        }

        public async Task<List<Categoria>> GetSubcategoriasAsync(Guid categoriaPaiId)
        {
            return await _categoriaRepository.GetSubcategoriasAsync(categoriaPaiId);
        }

        public async Task<List<Categoria>> GetCategoriasHierarquicasAsync()
        {
            var todas = await _categoriaRepository.GetAllAsync();

            var categoriasRaiz = todas.Where(c => c.CategoriaPaiId == null).ToList();

            foreach (var raiz in categoriasRaiz)
            {
                MontarArvore(raiz, todas);
            }

            return categoriasRaiz;
        }

        private void MontarArvore(Categoria categoria, List<Categoria> todas)
        {
            categoria.Subcategorias = todas
                .Where(c => c.CategoriaPaiId == categoria.Id)
                .ToList();

            foreach (var sub in categoria.Subcategorias)
            {
                MontarArvore(sub, todas);
            }
        }

        public async Task<Categoria> CriarCategoriaAsync(CategoriaDTO dto)
        {
            Guid? categoriaPaiGuid = null;
            if (!string.IsNullOrEmpty(dto.CategoriaPaiId))
                categoriaPaiGuid = Guid.Parse(dto.CategoriaPaiId);

            if (categoriaPaiGuid.HasValue)
            {
                var categoriaPai = await _categoriaRepository.GetByIdAsync(categoriaPaiGuid.Value);
                if (categoriaPai == null)
                    throw new Exception("Categoria pai não encontrada.");
            }

            var categoria = new Categoria
            {
                Nome = dto.Nome,
                CategoriaPaiId = categoriaPaiGuid
            };

            await _categoriaRepository.AddAsync(categoria);
            await _categoriaRepository.SaveChangesAsync();

            return categoria;
        }

        public async Task AtualizarCategoriaAsync(Guid id, CategoriaDTO dto)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                throw new Exception("Categoria não encontrada.");

            categoria.Nome = dto.Nome;

            if (!string.IsNullOrEmpty(dto.CategoriaPaiId))
            {
                var novaPaiId = Guid.Parse(dto.CategoriaPaiId);

                if (novaPaiId == id)
                    throw new Exception("Uma categoria não pode ser sua própria pai.");

                var categoriaPai = await _categoriaRepository.GetByIdAsync(novaPaiId);
                if (categoriaPai == null)
                    throw new Exception("Categoria pai não encontrada.");

                categoria.CategoriaPaiId = novaPaiId;
            }
            else
            {
                categoria.CategoriaPaiId = null;
            }

            await _categoriaRepository.UpdateAsync(categoria);
            await _categoriaRepository.SaveChangesAsync();
        }

        public async Task RemoverCategoriaAsync(Guid id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                throw new Exception("Categoria não encontrada.");

            var subcategorias = await _categoriaRepository.GetSubcategoriasAsync(id);
            if (subcategorias.Count > 0)
                throw new Exception("Não é possível excluir uma categoria que possui subcategorias.");

            if (categoria.Produtos != null && categoria.Produtos.Count > 0)
                throw new Exception("Não é possível excluir uma categoria que possui produtos.");

            await _categoriaRepository.DeleteAsync(categoria);
            await _categoriaRepository.SaveChangesAsync();
        }

    }
}
