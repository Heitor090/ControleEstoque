using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly AppDbContext _context;

        public FornecedorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FornecedorDto>> ObterTodosAsync()
        {
            return await _context.Fornecedores
                .AsNoTracking()
                .Select(f => new FornecedorDto
                {
                    Id = f.Id,
                    NomeFantasia = f.NomeFantasia,
                    CNPJ = f.CNPJ
                })
                .ToListAsync();
        }

        public async Task<FornecedorDto?> ObterPorIdAsync(int id)
        {
            var fornecedor = await _context.Fornecedores
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornecedor == null) return null;

            return new FornecedorDto
            {
                Id = fornecedor.Id,
                NomeFantasia = fornecedor.NomeFantasia,
                CNPJ = fornecedor.CNPJ
            };
        }

        public async Task<FornecedorDto> CriarAsync(CriarFornecedorDto dto)
        {
            var fornecedor = new Fornecedor
            {
                NomeFantasia = dto.NomeFantasia,
                CNPJ = dto.CNPJ
            };

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            return new FornecedorDto
            {
                Id = fornecedor.Id,
                NomeFantasia = fornecedor.NomeFantasia,
                CNPJ = fornecedor.CNPJ
            };
        }

        public async Task<bool> AtualizarAsync(AtualizarFornecedorDto dto)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(dto.Id);
            if (fornecedor == null) return false;

            fornecedor.NomeFantasia = dto.NomeFantasia;
            fornecedor.CNPJ = dto.CNPJ;

            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null) return false;

            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}