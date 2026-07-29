namespace ControleEstoque.API.DTOs
{
    public class CriarFormaPagamentoDto
    {
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }

    public class FormaPagamentoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }
}