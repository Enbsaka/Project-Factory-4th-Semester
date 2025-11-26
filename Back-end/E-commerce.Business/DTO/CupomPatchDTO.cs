namespace Dunder_Store.DTO
{
    public class CupomPatchDTO
    {
        public string? Codigo { get; set; }
        public decimal? DescontoPercentual { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public bool? Ativo { get; set; }
    }
}
