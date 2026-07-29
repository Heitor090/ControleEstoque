using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContasReceberController : ControllerBase
    {
        private readonly IContaReceberService _contaReceberService;

        public ContasReceberController(IContaReceberService contaReceberService)
        {
            _contaReceberService = contaReceberService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Cliente" && int.TryParse(userIdClaim, out int clienteId))
            {
                var contasDoCliente = await _contaReceberService.ObterPorClienteIdAsync(clienteId);
                return Ok(contasDoCliente);
            }

            var contas = await _contaReceberService.ObterTodosAsync();
            return Ok(contas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var conta = await _contaReceberService.ObterPorIdAsync(id);
            if (conta == null) return NotFound(new { message = "Conta a receber não encontrada." });
            return Ok(conta);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> Criar([FromBody] CriarContaReceberDto dto)
        {
            try
            {
                var novaConta = await _contaReceberService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novaConta.Id }, novaConta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarContaReceberDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "O ID da rota difere do ID da conta a receber." });

            try
            {
                var atualizado = await _contaReceberService.AtualizarAsync(dto);
                if (!atualizado) return NotFound(new { message = "Conta a receber não encontrada." });
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> Excluir(int id)
        {
            var removido = await _contaReceberService.RemoverAsync(id);
            if (!removido) return NotFound(new { message = "Conta a receber não encontrada." });
            return NoContent();
        }
    }
}