using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Services
{
    public class FormaPagamentoService : IFormaPagamentoService
    {
        private readonly AppDbContext _context;

        public FormaPagamentoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormaPagamentoDto>> ObterTodasAsync()
        {
            return await _context.FormasPagamento
                .AsNoTracking()
                .Select(f => new FormaPagamentoDto { Id = f.Id, Nome = f.Nome, Ativo = f.Ativo })
                .ToListAsync();
        }

        public async Task<FormaPagamentoDto?> ObterPorIdAsync(int id)
        {
            var forma = await _context.FormasPagamento
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
            if (forma == null) return null;

            return new FormaPagamentoDto { Id = forma.Id, Nome = forma.Nome, Ativo = forma.Ativo };
        }

        public async Task<FormaPagamentoDto> CriarAsync(CriarFormaPagamentoDto dto)
        {
            var forma = new FormaPagamento { Nome = dto.Nome, Ativo = dto.Ativo };
            _context.FormasPagamento.Add(forma);
            await _context.SaveChangesAsync();

            return new FormaPagamentoDto { Id = forma.Id, Nome = forma.Nome, Ativo = forma.Ativo };
        }

        public async Task<bool> AtualizarAsync(int id, CriarFormaPagamentoDto dto)
        {
            var forma = await _context.FormasPagamento.FindAsync(id);
            if (forma == null) return false;

            forma.Nome = dto.Nome;
            forma.Ativo = dto.Ativo;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var forma = await _context.FormasPagamento.FindAsync(id);
            if (forma == null) return false;

            _context.FormasPagamento.Remove(forma);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}