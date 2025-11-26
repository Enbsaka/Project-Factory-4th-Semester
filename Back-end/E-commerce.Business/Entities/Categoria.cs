using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class Categoria
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public Guid? CategoriaPaiId { get; set; }

        [ForeignKey(nameof(CategoriaPaiId))]
        public Categoria? CategoriaPai { get; set; }

        [InverseProperty(nameof(Categoria.CategoriaPai))]
        public ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
