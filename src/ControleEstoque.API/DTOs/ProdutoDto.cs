namespace ControleEstoque.API.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string? ImagemUrl { get; set; }
        public int FornecedorId { get; set; }
        public FornecedorDto? Fornecedor { get; set; }
    }

    public class CriarProdutoDto
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string? ImagemUrl { get; set; }
        public int FornecedorId { get; set; }
    }

    public class AtualizarProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string? ImagemUrl { get; set; }
        public int FornecedorId { get; set; }
    }
}