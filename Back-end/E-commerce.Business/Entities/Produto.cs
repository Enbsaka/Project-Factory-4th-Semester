using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public string CodigoDeBarra { get; set; } = string.Empty;

        public string? ImagemURL { get; set; }
        public string? Cor { get; set; }
        public string? Tamanho { get; set; }

        [Required]
        public Guid CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public Guid? ProdutoPaiId { get; set; }
        public Produto? ProdutoPai { get; set; }
        public List<Produto>? Variacoes { get; set; }

        public List<PedidoProduto> PedidoProdutos { get; set; } = new();
    }
}
