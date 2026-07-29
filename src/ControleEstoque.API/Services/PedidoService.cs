using ControleEstoque.API.Data;
using ControleEstoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _context;

        public PedidoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> ObterTodosPedidosAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Include(p => p.FormaPagamento)
                .Include(p => p.Cliente)
                .Include(p => p.Caixa)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pedido> CriarPedidoAsync(int clienteId, int formaPagamentoId, List<ItemPedido> itens)
        {
            var formaPgto = await _context.FormasPagamento.FindAsync(formaPagamentoId);
            if (formaPgto == null || !formaPgto.Ativo) throw new Exception("Forma de pagamento não encontrada ou inativa.");

            foreach (var item in itens)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                if (produto == null) throw new Exception($"Produto {item.ProdutoId} não encontrado no estoque.");
                if (produto.QuantidadeEstoque < item.Quantidade) throw new Exception($"Estoque insuficiente para o produto {produto.Nome}.");

                item.PrecoUnitario = produto.Preco;
                produto.QuantidadeEstoque -= item.Quantidade;
            }

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                FormaPagamentoId = formaPagamentoId,
                DataPedido = DateTime.Now,
                Status = "Aberto",
                Itens = itens
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<IEnumerable<Pedido>> ListarPedidosDoClienteAsync(int clienteId)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Include(p => p.FormaPagamento)
                .Include(p => p.Cliente)
                .Include(p => p.Caixa)
                .Where(p => p.ClienteId == clienteId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pedido?> ObterPedidoComDetalhesAsync(int pedidoId)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Include(p => p.FormaPagamento)
                .Include(p => p.Cliente)
                .Include(p => p.Caixa)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == pedidoId);
        }

        public async Task<bool> FinalizarPedidoAsync(int pedidoId, int caixaId)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null) return false;

            pedido.Status = "Finalizado";
            pedido.CaixaId = caixaId;

            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
