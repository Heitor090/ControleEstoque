using ControleEstoque.API.DTOs;

namespace ControleEstoque.API.Services
{
    public interface IFornecedorService
    {
        Task<IEnumerable<FornecedorDto>> ObterTodosAsync();
        Task<FornecedorDto?> ObterPorIdAsync(int id);
        Task<FornecedorDto> CriarAsync(CriarFornecedorDto dto);
        Task<bool> AtualizarAsync(AtualizarFornecedorDto dto);
        Task<bool> RemoverAsync(int id);
    }
}