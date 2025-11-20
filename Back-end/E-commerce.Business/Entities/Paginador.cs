namespace Dunder_Store.Entities
{
    public class Paginador<T>
    {
        public IEnumerable<T> Itens { get; set; } = new List<T>();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalItens { get; set; }

        public bool TemPaginaAnterior => PaginaAtual > 1;
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;

        public Paginador() { }

        public Paginador(IEnumerable<T> itens, int totalItens, int paginaAtual, int tamanhoPagina)
        {
            Itens = itens;
            TotalItens = totalItens;
            PaginaAtual = paginaAtual;
            TamanhoPagina = tamanhoPagina;
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina);
        }
    }
}
