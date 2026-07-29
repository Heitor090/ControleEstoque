using ControleEstoque.API.DTOs;

namespace ControleEstoque.API.Services
{
    public interface IFormaPagamentoService
    {
        Task<IEnumerable<FormaPagamentoDto>> ObterTodasAsync();
        Task<FormaPagamentoDto?> ObterPorIdAsync(int id);
        Task<FormaPagamentoDto> CriarAsync(CriarFormaPagamentoDto dto);
        Task<bool> AtualizarAsync(int id, CriarFormaPagamentoDto dto);
        Task<bool> RemoverAsync(int id);
    }
}
