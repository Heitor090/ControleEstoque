using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.API.Models
{
    public enum PerfilUsuario { Cliente, Caixa, Gerente }

    public abstract class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        public PerfilUsuario Perfil { get; set; }
    }
}
