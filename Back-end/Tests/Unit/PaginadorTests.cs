using System.Collections.Generic;
using Dunder_Store.Entities;
using Xunit;

namespace Dunder_Store.UnitTests
{
    public class PaginadorTests
    {
        [Fact]
        public void CalculaTotalPaginasEChecaNavegacao()
        {
            var itens = new List<int> { 1, 2, 3 };
            var paginador = new Paginador<int>(itens, totalItens: 25, paginaAtual: 2, tamanhoPagina: 10);

            Assert.Equal(3, paginador.TotalPaginas);
            Assert.True(paginador.TemPaginaAnterior);
            Assert.True(paginador.TemProximaPagina);
        }
    }
}