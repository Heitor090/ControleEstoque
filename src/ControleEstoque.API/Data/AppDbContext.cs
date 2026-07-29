﻿using ControleEstoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Caixa> Caixas { get; set; }
        public DbSet<Gerente> Gerentes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<ContaReceber> ContasReceber { get; set; }
        public DbSet<FormaPagamento> FormasPagamento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Adiciona configurações extras, além dos decarators
            base.OnModelCreating(modelBuilder);
        }
    }
}
