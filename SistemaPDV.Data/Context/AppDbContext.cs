using Microsoft.EntityFrameworkCore;
using SistemaPDV.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SistemaPDV.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite - banco local
            optionsBuilder.UseSqlite("Data Source=sistemaPDV.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de precisão decimal
            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Venda>()
                .Property(v => v.ValorTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.PrecoUnitario)
                .HasPrecision(18, 2);

           
        }

        private string HashPassword(string password)
        {
            // IMPORTANTE: Use BCrypt.Net-Next em produção
            // Aqui está simplificado para o exemplo
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password)
            );
        }
    }
}