using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleEstoque.API.Models
{
    public class ContaReceber
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Descricao { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DataVencimento { get; set; }

        public DateTime? DataPagamento { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Pendente";

        [Required]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }
    }
}