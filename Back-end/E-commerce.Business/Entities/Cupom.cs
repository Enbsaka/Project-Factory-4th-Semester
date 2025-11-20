using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.Entities
{
    public class Cupom
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal DescontoPercentual { get; set; }

        [Required]
        public DateTime DataExpiracao { get; set; }

        public bool Ativo { get; set; } = true;

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
