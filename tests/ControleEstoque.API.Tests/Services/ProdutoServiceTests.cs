using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using ControleEstoque.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ControleEstoque.API.Tests.Services;

public class ProdutoServiceTests
{
	private static AppDbContext CreateContext(string databaseName)
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName)
			.Options;

		return new AppDbContext(options);
	}

	[Fact]
	public async Task CriarAsync_FornecedorNaoExiste_DeveLancarArgumentException()
	{
		using var context = CreateContext(Guid.NewGuid().ToString());
		var service = new ProdutoService(context);

		var dto = new CriarProdutoDto
		{
			Nome = "Teclado",
			Preco = 150,
			QuantidadeEstoque = 10,
			FornecedorId = 99
		};

		await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(dto));
	}

	[Fact]
	public async Task CriarAsync_FornecedorExiste_DeveCriarProduto()
	{
		using var context = CreateContext(Guid.NewGuid().ToString());
		context.Fornecedores.Add(new Fornecedor { NomeFantasia = "Fornecedor X", CNPJ = "12345678901234" });
		await context.SaveChangesAsync();

		var service = new ProdutoService(context);
		var dto = new CriarProdutoDto
		{
			Nome = "Monitor",
			Preco = 899.90m,
			QuantidadeEstoque = 5,
			FornecedorId = 1
		};

		var result = await service.CriarAsync(dto);

		Assert.NotNull(result);
		Assert.Equal(dto.Nome, result.Nome);
		Assert.Equal(dto.Preco, result.Preco);
		Assert.Equal(dto.QuantidadeEstoque, result.QuantidadeEstoque);
		Assert.Equal(dto.FornecedorId, result.FornecedorId);

		var produtoNoBanco = await context.Produtos.FindAsync(result.Id);
		Assert.NotNull(produtoNoBanco);
	}

	[Fact]
	public async Task AtualizarAsync_FornecedorInexistente_DeveLancarArgumentException()
	{
		using var context = CreateContext(Guid.NewGuid().ToString());
		context.Produtos.Add(new Produto { Nome = "Mouse", Preco = 50, QuantidadeEstoque = 20, FornecedorId = 1 });
		await context.SaveChangesAsync();

		var service = new ProdutoService(context);
		var dto = new AtualizarProdutoDto
		{
			Id = 1,
			Nome = "Mouse Sem Fio",
			Preco = 70,
			QuantidadeEstoque = 20,
			FornecedorId = 99
		};

		await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarAsync(dto));
	}
}
