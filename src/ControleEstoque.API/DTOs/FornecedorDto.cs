namespace ControleEstoque.API.DTOs
{
    public class FornecedorDto
    {
        public int Id { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
    }

    public class CriarFornecedorDto
    {
        public string NomeFantasia { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
    }

    public class AtualizarFornecedorDto
    {
        public int Id { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
    }
}