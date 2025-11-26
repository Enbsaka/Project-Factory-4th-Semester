namespace Dunder_Store.DTO
{
    public class ProdutoPatchDTO
    {
        public string? nome { get; set; }
        public string? descricao { get; set; }
        public decimal? preco { get; set; }
        public string? codigoDeBarra { get; set; }
        public string? cor { get; set; }
        public string? tamanho { get; set; }
        public Guid? produtoPaiId { get; set; }
        public Guid? CategoriaId { get; set; }
        public string? CategoriaNome { get; set; }
    }
}
