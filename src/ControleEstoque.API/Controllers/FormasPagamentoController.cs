using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormasPagamentoController : ControllerBase
    {
        private readonly IFormaPagamentoService _service;

        public FormasPagamentoController(IFormaPagamentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var formas = await _service.ObterTodasAsync();
            return Ok(formas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var forma = await _service.ObterPorIdAsync(id);
            if (forma == null) return NotFound(new { message = "Forma de pagamento não encontrada." });
            return Ok(forma);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Criar([FromBody] CriarFormaPagamentoDto dto)
        {
            var novaForma = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = novaForma.Id }, novaForma);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CriarFormaPagamentoDto dto)
        {
            var atualizada = await _service.AtualizarAsync(id, dto);
            if (!atualizada) return NotFound(new { message = "Forma de pagamento não encontrada." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _service.RemoverAsync(id);
            if (!excluido) return NotFound(new { message = "Forma de pagamento não encontrada." });
            return NoContent();
        }
    }
}