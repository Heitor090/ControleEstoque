
using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using ControleEstoque.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ControleEstoque.API.Tests.Services
{
    public class PedidoServiceTest
    {
        private static AppDbContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new AppDbContext(options);
        }
        [Fact]
        public async Task CriarPedidoAsync_ComEstoqueSuficiente_DeveCriarPedidoEeduzirEstoque()
        {
            //arrange
            using var context = CreateContext(Guid.NewGuid().ToString());
            var service = new PedidoService(context);
            var cliente = new Cliente {Id = 1, Email = "cliente@teste.com", Perfil = PerfilUsuario.Cliente};
            var formaPagamento = new FormaPagamento { Id = 1, Nome = "Cartão de Crédito", Ativo = true};
            var produto = new Produto { Id = 1, Nome = "Produto Teste", FornecedorId= 1, Preco = 100, QuantidadeEstoque = 10 };
            context.Add(cliente);
            context.Add(formaPagamento);
            context.Add(produto);
            await context.SaveChangesAsync();

          

            //act
           var pedido = await service.CriarPedidoAsync(cliente.Id, formaPagamento.Id, new List<ItemPedido>
            {
                new ItemPedido { ProdutoId = produto.Id, Quantidade = 3 }
            });


            //assert
            Assert.NotNull(pedido);
            Assert.Equal("Aberto", pedido.Status);

            var produtoNOBanco = await context.Produtos.FindAsync(produto.Id);
            Assert.NotNull(produtoNOBanco);
            Assert.Equal(7, produtoNOBanco.QuantidadeEstoque);

        }
    }
}
