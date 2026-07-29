using ControleEstoque.API.DTOs;

namespace ControleEstoque.API.Services
{
    public interface IContaReceberService
    {
        Task<IEnumerable<ContaReceberDto>> ObterTodosAsync();
        Task<IEnumerable<ContaReceberDto>> ObterPorClienteIdAsync(int clienteId);
        Task<ContaReceberDto?> ObterPorIdAsync(int id);
        Task<ContaReceberDto> CriarAsync(CriarContaReceberDto dto);
        Task<bool> AtualizarAsync(AtualizarContaReceberDto dto);
        Task<bool> RemoverAsync(int id);
    }
}