using ControleEstoque.API.Controllers;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ControleEstoque.API.Tests.Controllers;

public class UsuariosControllerTests
{
    [Fact]
    public async Task Autenticar_QuandoCredenciaisInvalidas_DeveRetornarUnauthorized()
    {
        var usuarioMock = new Mock<IUsuarioService>();
        usuarioMock.Setup(x => x.AutenticarAsync(It.IsAny<LoginDto>())).ReturnsAsync((TokenDto?)null);
        var controller = new UsuariosController(usuarioMock.Object);

        var result = await controller.Autenticar(new LoginDto { Email = "erro@x.com", Senha = "senha" });

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Contains("incorretos", unauthorized.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task RegistrarCliente_QuandoSucesso_DeveRetornarCreatedAtAction()
    {
        var usuarioMock = new Mock<IUsuarioService>();
        usuarioMock.Setup(x => x.RegistrarClienteAsync(It.IsAny<CriarClienteDto>())).ReturnsAsync(
            new UsuarioDto { Id = 1, Nome = "Ana", Email = "ana@x.com", Perfil = "Cliente", CPF = "12345678901" });

        var controller = new UsuariosController(usuarioMock.Object);
        var result = await controller.RegistrarCliente(new CriarClienteDto { Nome = "Ana", Email = "ana@x.com", Senha = "senha", CPF = "12345678901" });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UsuariosController.ObterPorId), created.ActionName);
    }

    [Fact]
    public async Task Excluir_UsuarioNaoEncontrado_DeveRetornarNotFound()
    {
        var usuarioMock = new Mock<IUsuarioService>();
        usuarioMock.Setup(x => x.RemoverUsuarioAsync(10)).ThrowsAsync(new KeyNotFoundException("Usuário não encontrado."));
        var controller = new UsuariosController(usuarioMock.Object);

        var result = await controller.Excluir(10);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("Usuário não encontrado", notFound.Value?.ToString() ?? string.Empty);
    }
}
