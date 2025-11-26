using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.DTO
{
    public class CupomDTO
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal DescontoPercentual { get; set; }

        [Required]
        public DateTime DataExpiracao { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
