using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var produtos = await _produtoService.ObterTodosAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            if (produto == null) return NotFound(new { message = "Produto não encontrado." });
            return Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Criar([FromBody] CriarProdutoDto dto)
        {
            try
            {
                var novoProduto = await _produtoService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novoProduto.Id }, novoProduto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "O ID da rota difere do ID do produto." });

            try
            {
                var atualizado = await _produtoService.AtualizarAsync(dto);
                if (!atualizado) return NotFound(new { message = "Produto não encontrado." });
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Excluir(int id)
        {
            var removido = await _produtoService.RemoverAsync(id);
            if (!removido) return NotFound(new { message = "Produto não encontrado." });
            return NoContent();
        }
    }
}