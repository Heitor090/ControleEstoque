using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.API.Models
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required, StringLength(14)]
        public string CNPJ { get; set; } = string.Empty;

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
