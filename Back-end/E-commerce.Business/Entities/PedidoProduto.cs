using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class PedidoProduto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PedidoId { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public Pedido Pedido { get; set; } = null!;

        [Required]
        public Guid ProdutoId { get; set; }

        [ForeignKey(nameof(ProdutoId))]
        public Produto Produto { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }

        [NotMapped]
        public decimal ValorTotal => (PrecoUnitario > 0 ? PrecoUnitario : Produto.Preco) * Quantidade;
    }
}
