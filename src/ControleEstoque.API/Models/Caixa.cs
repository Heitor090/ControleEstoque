using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.API.Models
{
    public class Caixa : Usuario
    {
        [Required, StringLength(50)]
        public string Turno { get; set; } = string.Empty;

        public ICollection<Pedido> PedidosFechados { get; set; } = new List<Pedido>();
    }
}
