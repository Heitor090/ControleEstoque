using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.API.Models
{
    public class Cliente : Usuario
    {
        [Required, StringLength(14)]
        public string CPF { get; set; } = string.Empty;

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
