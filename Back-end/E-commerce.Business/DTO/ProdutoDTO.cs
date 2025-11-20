using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Dunder_Store.DTO
{
    public class ProdutoDTO
    {
        [Required] public string nome { get; set; } = string.Empty;
        [Required] public string descricao { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue)] public decimal preco { get; set; }
        [Required] public string codigoDeBarra { get; set; } = string.Empty;
        public IFormFile? imagem { get; set; }

        public string? cor { get; set; }
        public string? tamanho { get; set; }
        public Guid? produtoPaiId { get; set; }

        public Guid? CategoriaId { get; set; }
        public string? CategoriaNome { get; set; }
    }
}
