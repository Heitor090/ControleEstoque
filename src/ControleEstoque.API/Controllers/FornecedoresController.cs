using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FornecedoresController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [HttpGet]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> ObterTodos()
        {
            var fornecedores = await _fornecedorService.ObterTodosAsync();
            return Ok(fornecedores);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Gerente,Caixa")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var fornecedor = await _fornecedorService.ObterPorIdAsync(id);
            if (fornecedor == null) return NotFound(new { message = "Fornecedor não encontrado." });
            return Ok(fornecedor);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Criar([FromBody] CriarFornecedorDto dto)
        {
            var novoFornecedor = await _fornecedorService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = novoFornecedor.Id }, novoFornecedor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarFornecedorDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "O ID da rota difere do ID do fornecedor." });

            var atualizado = await _fornecedorService.AtualizarAsync(dto);
            if (!atualizado) return NotFound(new { message = "Fornecedor não encontrado." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Excluir(int id)
        {
            var removido = await _fornecedorService.RemoverAsync(id);
            if (!removido) return NotFound(new { message = "Fornecedor não encontrado." });
            return NoContent();
        }
    }
}