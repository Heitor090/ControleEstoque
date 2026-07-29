using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using ControleEstoque.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ControleEstoque.API.Tests.Services;

public class UsuarioServiceTests
{
    private static AppDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AutenticarAsync_UsuarioNaoExiste_DeveRetornarNulo()
    {
        using var context = CreateContext(Guid.NewGuid().ToString());
        var passwordMock = new Mock<IPasswordService>();
        var tokenMock = new Mock<ITokenService>();
        var service = new UsuarioService(context, passwordMock.Object, tokenMock.Object);

        var resultado = await service.AutenticarAsync(new LoginDto { Email = "naoexiste@x.com", Senha = "senha" });

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AutenticarAsync_SenhaIncorreta_DeveRetornarNulo()
    {
        using var context = CreateContext(Guid.NewGuid().ToString());
        context.Clientes.Add(new Cliente
        {
            Nome = "Joao",
            Email = "joao@x.com",
            SenhaHash = "hash",
            CPF = "12345678901",
            Perfil = PerfilUsuario.Cliente
        });
        await context.SaveChangesAsync();

        var passwordMock = new Mock<IPasswordService>();
        passwordMock.Setup(x => x.VerifyPassword("senha", "hash")).Returns(false);
        var tokenMock = new Mock<ITokenService>();
        var service = new UsuarioService(context, passwordMock.Object, tokenMock.Object);

        var resultado = await service.AutenticarAsync(new LoginDto { Email = "joao@x.com", Senha = "senha" });

        Assert.Null(resultado);
        tokenMock.Verify(x => x.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarClienteAsync_EmailJaExiste_DeveLancarInvalidOperationException()
    {
        using var context = CreateContext(Guid.NewGuid().ToString());
        context.Clientes.Add(new Cliente
        {
            Nome = "Maria",
            Email = "maria@x.com",
            SenhaHash = "hash",
            CPF = "11122233344",
            Perfil = PerfilUsuario.Cliente
        });
        await context.SaveChangesAsync();

        var passwordMock = new Mock<IPasswordService>();
        var tokenMock = new Mock<ITokenService>();
        var service = new UsuarioService(context, passwordMock.Object, tokenMock.Object);

        var dto = new CriarClienteDto
        {
            Nome = "Maria",
            Email = "maria@x.com",
            Senha = "senha",
            CPF = "11122233344"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegistrarClienteAsync(dto));
    }

    [Fact]
    public async Task RemoverUsuarioAsync_UsuarioNaoExiste_DeveLancarKeyNotFoundException()
    {
        using var context = CreateContext(Guid.NewGuid().ToString());
        var passwordMock = new Mock<IPasswordService>();
        var tokenMock = new Mock<ITokenService>();
        var service = new UsuarioService(context, passwordMock.Object, tokenMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.RemoverUsuarioAsync(1));
    }
}
