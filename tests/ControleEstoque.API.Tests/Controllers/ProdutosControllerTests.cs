using ControleEstoque.API.Controllers;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ControleEstoque.API.Tests.Controllers;

public class ProdutosControllerTests
{
    [Fact]
    public async Task ObterPorId_ProdutoNaoEncontrado_DeveRetornarNotFound()
    {
        var produtoMock = new Mock<IProdutoService>();
        produtoMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync((ProdutoDto?)null);
        var controller = new ProdutosController(produtoMock.Object);

        var result = await controller.ObterPorId(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Criar_ProdutoCriado_DeveRetornarCreatedAtAction()
    {
        var novoProduto = new ProdutoDto { Id = 1, Nome = "Teclado", Preco = 99.90m, QuantidadeEstoque = 10, FornecedorId = 1 };
        var produtoMock = new Mock<IProdutoService>();
        produtoMock.Setup(x => x.CriarAsync(It.IsAny<CriarProdutoDto>())).ReturnsAsync(novoProduto);
        var controller = new ProdutosController(produtoMock.Object);

        var result = await controller.Criar(new CriarProdutoDto { Nome = "Teclado", Preco = 99.90m, QuantidadeEstoque = 10, FornecedorId = 1 });

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProdutosController.ObterPorId), createdResult.ActionName);
        Assert.Equal(1, ((ProdutoDto)createdResult.Value!).Id);
    }

    [Fact]
    public async Task Atualizar_IdDiferente_DeveRetornarBadRequest()
    {
        var produtoMock = new Mock<IProdutoService>();
        var controller = new ProdutosController(produtoMock.Object);

        var result = await controller.Atualizar(2, new AtualizarProdutoDto { Id = 1, Nome = "Mouse", Preco = 50, QuantidadeEstoque = 5, FornecedorId = 1 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("ID da rota difere", badRequest.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task Excluir_QuandoServicoCompleta_DeveRetornarNoContent()
    {
        var produtoMock = new Mock<IProdutoService>();
        produtoMock.Setup(x => x.RemoverAsync(1)).ReturnsAsync(true);
        var controller = new ProdutosController(produtoMock.Object);

        var result = await controller.Excluir(1);

        Assert.IsType<NoContentResult>(result);
    }
}
