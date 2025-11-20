using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente Cliente { get; set; } = null!;

        [Required]
        public DateTime DataPedido { get; set; }

        [Required]
        public PedidoStatus Status { get; set; } = PedidoStatus.Carrinho;

        public List<PedidoProduto> PedidoProdutos { get; set; } = new();

        public Guid? CupomId { get; set; }
        [ForeignKey(nameof(CupomId))]
        public Cupom? Cupom { get; set; }

        public decimal? FreteValor { get; set; }

        [NotMapped]
        public decimal ValorTotalSemDesconto => PedidoProdutos.Sum(pp => (pp.PrecoUnitario > 0 ? pp.PrecoUnitario : pp.Produto.Preco) * pp.Quantidade);

        [NotMapped]
        public decimal ValorTotal
        {
            get
            {
                var total = ValorTotalSemDesconto;
                if (Cupom != null && Cupom.Ativo && Cupom.DataExpiracao >= DateTime.Now)
                {
                    var desconto = (Cupom.DescontoPercentual / 100m) * total;
                    total -= desconto;
                }
                total += FreteValor ?? 0m;
                return total;
            }
        }

        public Pedido(Cliente cliente, DateTime data)
        {
            Cliente = cliente;
            ClienteId = cliente.Id;
            DataPedido = data;
            Status = PedidoStatus.Carrinho;
        }

        private Pedido() { }
    }
}
