using ControleEstoque.API.DTOs;

namespace ControleEstoque.API.Services
{
    public interface IUsuarioService
    {
        // Registro
        Task<UsuarioDto> RegistrarClienteAsync(CriarClienteDto dto);
        Task<UsuarioDto> RegistrarCaixaAsync(CriarCaixaDto dto);
        Task<UsuarioDto> RegistrarGerenteAsync(CriarGerenteDto dto);

        // Atualização
        Task AtualizarClienteAsync(AtualizarClienteDto dto);
        Task AtualizarCaixaAsync(AtualizarCaixaDto dto);
        Task AtualizarGerenteAsync(AtualizarGerenteDto dto);
        Task<bool> AlterarPerfilUsuarioAsync(int id, string novoPerfil, string? extraData);

        // Consulta
        Task<IEnumerable<UsuarioDto>> ListarTodosUsuariosAsync();
        Task<UsuarioDto?> ObterUsuarioPorIdAsync(int id);
        Task<UsuarioDto?> ObterUsuarioPorEmailAsync(string email);

        // Deleção
        Task RemoverUsuarioAsync(int id);

        // Autenticação
        Task<TokenDto?> AutenticarAsync(LoginDto dto);
    }
}