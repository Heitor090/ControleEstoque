using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleEstoque.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IEnumerable<Pedido> pedidos;

            if (userRole == "Cliente" && int.TryParse(userIdClaim, out int clienteId))
            {
                pedidos = await _pedidoService.ListarPedidosDoClienteAsync(clienteId);
            }
            else
            {
                pedidos = await _pedidoService.ObterTodosPedidosAsync();
            }

            var dtos = pedidos.Select(MapearParaDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var pedido = await _pedidoService.ObterPedidoComDetalhesAsync(id);
            if (pedido == null) return NotFound(new { message = "Pedido não encontrado." });

            return Ok(MapearParaDto(pedido));
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarPedidoDto pedidoDto)
        {
            try
            {
                var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(clienteIdClaim) || !int.TryParse(clienteIdClaim, out int clienteId))
                {
                    return BadRequest(new { message = "Cliente inválido ou não autenticado." });
                }

                var itensPedido = pedidoDto.Itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList();

                var novoPedido = await _pedidoService.CriarPedidoAsync(clienteId, pedidoDto.FormaPagamentoId, itensPedido);

                var pedidoCompleto = await _pedidoService.ObterPedidoComDetalhesAsync(novoPedido.Id);
                return CreatedAtAction(nameof(ObterPorId), new { id = novoPedido.Id }, MapearParaDto(pedidoCompleto ?? novoPedido));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/finalizar")]
        [Authorize(Roles = "Caixa,Gerente")]
        public async Task<IActionResult> Finalizar(int id)
        {
            var caixaIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(caixaIdClaim) || !int.TryParse(caixaIdClaim, out int caixaId))
            {
                return BadRequest(new { message = "Operador de caixa não identificado." });
            }

            var finalizado = await _pedidoService.FinalizarPedidoAsync(id, caixaId);
            if (!finalizado) return NotFound(new { message = "Pedido não encontrado." });

            return NoContent();
        }

        private static PedidoDto MapearParaDto(Pedido pedido)
        {
            return new PedidoDto
            {
                Id = pedido.Id,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                ClienteId = pedido.ClienteId,
                ClienteNome = pedido.Cliente?.Nome ?? string.Empty,
                CaixaId = pedido.CaixaId,
                CaixaNome = pedido.Caixa?.Nome ?? string.Empty,
                FormaPagamentoId = pedido.FormaPagamentoId,
                FormaPagamentoNome = pedido.FormaPagamento?.Nome ?? string.Empty,
                Itens = pedido.Itens.Select(i => new ItemPedidoDto
                {
                    Id = i.Id,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto?.Nome ?? string.Empty
                }).ToList()
            };
        }
    }
}
