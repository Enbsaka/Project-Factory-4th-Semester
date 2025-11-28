using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Dunder_Store.Database;
using Dunder_Store.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dunder_Store.IntegrationTests
{
    public class CategoriaControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private readonly TestingWebAppFactory _factory;

        public CategoriaControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetHierarquia_RetornaCategoriasRaiz()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProdutosDbContext>();
                var raiz = new Categoria { Id = Guid.NewGuid(), Nome = "Masculino" };
                var sub = new Categoria { Id = Guid.NewGuid(), Nome = "Camisetas", CategoriaPaiId = raiz.Id };
                db.Categorias.Add(raiz);
                db.Categorias.Add(sub);
                await db.SaveChangesAsync();
            }

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Categoria/hierarquia");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<Categoria[]>();
            Assert.NotNull(data);
            Assert.True(data.Any(c => c.Nome == "Masculino"));
            var masculino = data.First(c => c.Nome == "Masculino");
            Assert.NotNull(masculino.Subcategorias);
            Assert.True(masculino.Subcategorias.Any(s => s.Nome == "Camisetas"));
        }
    }
}