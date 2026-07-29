using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        #region Autenticação e Registro Público

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar([FromBody] LoginDto dto)
        {
            try
            {
                var resultado = await _usuarioService.AutenticarAsync(dto);
                if (resultado == null)
                    return Unauthorized(new { message = "E-mail ou senha incorretos." });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("registrar-cliente")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarCliente([FromBody] CriarClienteDto dto)
        {
            try
            {
                var novoCliente = await _usuarioService.RegistrarClienteAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novoCliente.Id }, novoCliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Registro Administrativo (Apenas Gerentes)

        [HttpPost("registrar-caixa")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> RegistrarCaixa([FromBody] CriarCaixaDto dto)
        {
            try
            {
                var novoCaixa = await _usuarioService.RegistrarCaixaAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novoCaixa.Id }, novoCaixa);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("registrar-gerente")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> RegistrarGerente([FromBody] CriarGerenteDto dto)
        {
            try
            {
                var novoGerente = await _usuarioService.RegistrarGerenteAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novoGerente.Id }, novoGerente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Atualização e Promoção (Apenas Gerentes)

        [HttpPut("atualizar-cliente")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> AtualizarCliente([FromBody] AtualizarClienteDto dto)
        {
            try
            {
                await _usuarioService.AtualizarClienteAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("atualizar-caixa")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> AtualizarCaixa([FromBody] AtualizarCaixaDto dto)
        {
            try
            {
                await _usuarioService.AtualizarCaixaAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("atualizar-gerente")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> AtualizarGerente([FromBody] AtualizarGerenteDto dto)
        {
            try
            {
                await _usuarioService.AtualizarGerenteAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/alterar-perfil")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> AlterarPerfil(int id, [FromQuery] string novoPerfil, [FromQuery] string? extra)
        {
            try
            {
                var alterado = await _usuarioService.AlterarPerfilUsuarioAsync(id, novoPerfil, extra);
                if (!alterado) return NotFound(new { message = "Usuário não encontrado." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Consulta

        [HttpGet]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> ObterTodos()
        {
            var usuarios = await _usuarioService.ListarTodosUsuariosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
            if (usuario == null) return NotFound(new { message = "Usuário não encontrado." });
            return Ok(usuario);
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> ObterPorEmail(string email)
        {
            var usuario = await _usuarioService.ObterUsuarioPorEmailAsync(email);
            if (usuario == null) return NotFound(new { message = "Usuário não encontrado." });
            return Ok(usuario);
        }

        #endregion

        #region Deleção

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _usuarioService.RemoverUsuarioAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        #endregion
    }
}