namespace Dunder_Store.DTO
{
    public class PedidoProdutoDTO
    {
        public Guid? ProdutoId { get; set; }
        public string CodigoDeBarra { get; set; } = string.Empty;
        public int Quantidade { get; set; } = 1;
    }

}
