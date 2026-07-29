using ControleEstoque.API.Models;

namespace ControleEstoque.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context, Services.IPasswordService passwordService)
        {
            context.Database.EnsureCreated();

            if (!context.Gerentes.Any())
            {
                var admin = new Gerente
                {
                    Nome = "Administrador",
                    Email = "admin@mail.com",
                    Setor = "TI",
                    Perfil = PerfilUsuario.Gerente,
                    SenhaHash = passwordService.HashPassword("admin123")
                };
                context.Gerentes.Add(admin);
                context.SaveChanges();
            }

            if (!context.FormasPagamento.Any())
            {
                context.FormasPagamento.AddRange(
                    new FormaPagamento { Nome = "Dinheiro", Ativo = true },
                    new FormaPagamento { Nome = "Cartão de Crédito", Ativo = true },
                    new FormaPagamento { Nome = "Pix", Ativo = true }
                );
                context.SaveChanges();
            }
        }
    }
}
