using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.DTO
{
    public class CategoriaDTO
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public string? CategoriaPaiId { get; set; }
    }

    public class CategoriaHierarquicaDTO
    {   
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<CategoriaHierarquicaDTO>? Subcategorias { get; set; }
    }
}
