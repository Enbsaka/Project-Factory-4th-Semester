using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.E_commerce.Business.Entities
{
    public class PrecoRegiao
    {
        [Key]
        public int Id { get; set; }
        public string Regiao { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
    }
}
