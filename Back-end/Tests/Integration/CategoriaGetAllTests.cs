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
    public class CategoriaGetAllTests : IClassFixture<TestingWebAppFactory>
    {
        private readonly TestingWebAppFactory _factory;
        public CategoriaGetAllTests(TestingWebAppFactory factory) { _factory = factory; }

        [Fact]
        public async Task GetAll_RetornaCamposBasicos()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProdutosDbContext>();
                var raiz = new Categoria { Id = Guid.NewGuid(), Nome = "Jeans" };
                var sub = new Categoria { Id = Guid.NewGuid(), Nome = "Calças", CategoriaPaiId = raiz.Id };
                db.Categorias.Add(raiz);
                db.Categorias.Add(sub);
                await db.SaveChangesAsync();
            }

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Categoria");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<dynamic[]>();
            Assert.NotNull(data);
            Assert.True(data.Any());
        }
    }
}