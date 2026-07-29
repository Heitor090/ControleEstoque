using ControleEstoque.API.Models;

namespace ControleEstoque.API.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<Pedido>> ObterTodosPedidosAsync();
        Task<Pedido?> ObterPedidoComDetalhesAsync(int pedidoId);
        Task<IEnumerable<Pedido>> ListarPedidosDoClienteAsync(int clienteId);
        Task<Pedido> CriarPedidoAsync(int clienteId, int formaPagamentoId, List<ItemPedido> itens);
        Task<bool> FinalizarPedidoAsync(int pedidoId, int caixaId);
    }
}
