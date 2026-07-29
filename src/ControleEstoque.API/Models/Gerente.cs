using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.API.Models
{
    public class Gerente : Usuario
    {
        [Required, StringLength(50)]
        public string Setor { get; set; } = string.Empty;
    }
}
