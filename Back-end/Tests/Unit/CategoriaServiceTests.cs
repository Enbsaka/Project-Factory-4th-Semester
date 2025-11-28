using System;
using System.Threading.Tasks;
using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Services;
using Moq;
using Xunit;

namespace Dunder_Store.UnitTests
{
    public class CategoriaServiceTests
    {
        [Fact]
        public async Task AtualizarCategoria_NaoPermiteSerPaiDeSiMesma()
        {
            var repo = new Mock<ICategoriaRepository>();
            var id = Guid.NewGuid();
            var existente = new Categoria { Id = id, Nome = "A" };
            repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existente);

            var service = new CategoriaService(repo.Object);
            var dto = new CategoriaDTO { Nome = "A", CategoriaPaiId = id.ToString() };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AtualizarCategoriaAsync(id, dto));
            Assert.Contains("não pode ser sua própria pai", ex.Message);
        }
    }
}