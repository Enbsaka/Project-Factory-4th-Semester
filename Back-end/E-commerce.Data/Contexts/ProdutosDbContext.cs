using Dunder_Store.E_commerce.Business.Entities;
using Dunder_Store.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Database
{
    public partial class ProdutosDbContext(DbContextOptions<ProdutosDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<PedidoProduto> PedidoProdutos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Cupom> Cupons { get; set; } = null!;
        public DbSet<PrecoRegiao> PrecoRegiao { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PedidoProduto>()
                .HasKey(pp => new { pp.PedidoId, pp.ProdutoId });

            modelBuilder.Entity<PedidoProduto>()
                .HasOne(pp => pp.Pedido)
                .WithMany(p => p.PedidoProdutos)
                .HasForeignKey(pp => pp.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoProduto>()
                .HasOne(pp => pp.Produto)
                .WithMany(p => p.PedidoProdutos)
                .HasForeignKey(pp => pp.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Produto>()
                .HasMany(p => p.Variacoes)
                .WithOne(v => v.ProdutoPai)
                .HasForeignKey(v => v.ProdutoPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.Subcategorias)
                .WithOne(c => c.CategoriaPai)
                .HasForeignKey(c => c.CategoriaPaiId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PrecoRegiao>().HasData(
                new PrecoRegiao { Id = 1, Regiao = "Norte", PrecoBase = 25.0m },
                new PrecoRegiao { Id = 2, Regiao = "Nordeste", PrecoBase = 20.0m },
                new PrecoRegiao { Id = 3, Regiao = "Centro-Oeste", PrecoBase = 18.0m },
                new PrecoRegiao { Id = 4, Regiao = "Sudeste", PrecoBase = 12.0m },
                new PrecoRegiao { Id = 5, Regiao = "Sul", PrecoBase = 15.0m }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
