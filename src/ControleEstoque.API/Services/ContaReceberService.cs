using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Services
{
    public class ContaReceberService : IContaReceberService
    {
        private readonly AppDbContext _context;

        public ContaReceberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContaReceberDto>> ObterTodosAsync()
        {
            return await _context.ContasReceber
                .Include(c => c.Cliente)
                .AsNoTracking()
                .Select(c => new ContaReceberDto
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Valor = c.Valor,
                    DataVencimento = c.DataVencimento,
                    DataPagamento = c.DataPagamento,
                    Status = c.Status,
                    ClienteId = c.ClienteId,
                    Cliente = c.Cliente != null ? new ClienteDto 
                    { 
                        Id = c.Cliente.Id, 
                        Nome = c.Cliente.Nome 
                    } : null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ContaReceberDto>> ObterPorClienteIdAsync(int clienteId)
        {
            return await _context.ContasReceber
                .Include(c => c.Cliente)
                .Where(c => c.ClienteId == clienteId)
                .AsNoTracking()
                .Select(c => new ContaReceberDto
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Valor = c.Valor,
                    DataVencimento = c.DataVencimento,
                    DataPagamento = c.DataPagamento,
                    Status = c.Status,
                    ClienteId = c.ClienteId,
                    Cliente = c.Cliente != null ? new ClienteDto 
                    { 
                        Id = c.Cliente.Id, 
                        Nome = c.Cliente.Nome 
                    } : null
                })
                .ToListAsync();
        }

        public async Task<ContaReceberDto?> ObterPorIdAsync(int id)
        {
            var conta = await _context.ContasReceber
                .Include(c => c.Cliente)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conta == null) return null;

            return new ContaReceberDto
            {
                Id = conta.Id,
                Descricao = conta.Descricao,
                Valor = conta.Valor,
                DataVencimento = conta.DataVencimento,
                DataPagamento = conta.DataPagamento,
                Status = conta.Status,
                ClienteId = conta.ClienteId,
                Cliente = conta.Cliente != null ? new ClienteDto 
                { 
                    Id = conta.Cliente.Id, 
                    Nome = conta.Cliente.Nome 
                } : null
            };
        }

        public async Task<ContaReceberDto> CriarAsync(CriarContaReceberDto dto)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
            if (!clienteExiste)
            {
                throw new ArgumentException("O cliente informado não existe.");
            }

            var conta = new ContaReceber
            {
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                DataVencimento = dto.DataVencimento,
                DataPagamento = dto.DataPagamento,
                Status = dto.Status,
                ClienteId = dto.ClienteId
            };

            _context.ContasReceber.Add(conta);
            await _context.SaveChangesAsync();

            return new ContaReceberDto
            {
                Id = conta.Id,
                Descricao = conta.Descricao,
                Valor = conta.Valor,
                DataVencimento = conta.DataVencimento,
                DataPagamento = conta.DataPagamento,
                Status = conta.Status,
                ClienteId = conta.ClienteId
            };
        }

        public async Task<bool> AtualizarAsync(AtualizarContaReceberDto dto)
        {
            var conta = await _context.ContasReceber.FindAsync(dto.Id);
            if (conta == null) return false;

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
            if (!clienteExiste)
            {
                throw new ArgumentException("O cliente informado não existe.");
            }

            conta.Descricao = dto.Descricao;
            conta.Valor = dto.Valor;
            conta.DataVencimento = dto.DataVencimento;
            conta.DataPagamento = dto.DataPagamento;
            conta.Status = dto.Status;
            conta.ClienteId = dto.ClienteId;

            _context.ContasReceber.Update(conta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var conta = await _context.ContasReceber.FindAsync(id);
            if (conta == null) return false;

            _context.ContasReceber.Remove(conta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}