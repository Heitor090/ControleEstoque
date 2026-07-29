using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public UsuarioService(AppDbContext context, IPasswordService passwordService, ITokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        #region Registro

        public async Task<UsuarioDto> RegistrarClienteAsync(CriarClienteDto dto)
        {
            var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (emailJaExiste)
                throw new InvalidOperationException("Esse email já está cadastrado.");

            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = _passwordService.HashPassword(dto.Senha),
                CPF = dto.CPF,
                Perfil = PerfilUsuario.Cliente
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return MapearParaDto(cliente);
        }

        public async Task<UsuarioDto> RegistrarCaixaAsync(CriarCaixaDto dto)
        {
            var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (emailJaExiste)
                throw new InvalidOperationException("Esse email já está cadastrado.");

            var caixa = new Caixa
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = _passwordService.HashPassword(dto.Senha),
                Turno = dto.Turno,
                Perfil = PerfilUsuario.Caixa
            };

            _context.Caixas.Add(caixa);
            await _context.SaveChangesAsync();
            return MapearParaDto(caixa);
        }

        public async Task<UsuarioDto> RegistrarGerenteAsync(CriarGerenteDto dto)
        {
            var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (emailJaExiste)
                throw new InvalidOperationException("Esse email já está cadastrado.");

            var gerente = new Gerente
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = _passwordService.HashPassword(dto.Senha),
                Setor = dto.Setor,
                Perfil = PerfilUsuario.Gerente
            };

            _context.Gerentes.Add(gerente);
            await _context.SaveChangesAsync();
            return MapearParaDto(gerente);
        }

        #endregion

        #region Atualização

        public async Task AtualizarClienteAsync(AtualizarClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.Id);
            if (cliente == null)
            {
                // Tenta promover/atualizar perfil se existia sob outro tipo
                await AlterarPerfilUsuarioAsync(dto.Id, "Cliente", dto.CPF);
                cliente = await _context.Clientes.FindAsync(dto.Id);
                if (cliente == null) throw new KeyNotFoundException("Cliente não encontrado.");
            }

            if (cliente.Email != dto.Email)
            {
                var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != dto.Id);
                if (emailJaExiste)
                    throw new InvalidOperationException("Esse email já está cadastrado.");
            }

            cliente.Nome = dto.Nome;
            cliente.Email = dto.Email;
            cliente.CPF = dto.CPF;

            if (!string.IsNullOrEmpty(dto.Senha))
                cliente.SenhaHash = _passwordService.HashPassword(dto.Senha);

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarCaixaAsync(AtualizarCaixaDto dto)
        {
            var caixa = await _context.Caixas.FindAsync(dto.Id);
            if (caixa == null)
            {
                await AlterarPerfilUsuarioAsync(dto.Id, "Caixa", dto.Turno);
                caixa = await _context.Caixas.FindAsync(dto.Id);
                if (caixa == null) throw new KeyNotFoundException("Caixa não encontrado.");
            }

            if (caixa.Email != dto.Email)
            {
                var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != dto.Id);
                if (emailJaExiste)
                    throw new InvalidOperationException("Esse email já está cadastrado.");
            }

            caixa.Nome = dto.Nome;
            caixa.Email = dto.Email;
            caixa.Turno = dto.Turno;

            if (!string.IsNullOrEmpty(dto.Senha))
                caixa.SenhaHash = _passwordService.HashPassword(dto.Senha);

            _context.Caixas.Update(caixa);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarGerenteAsync(AtualizarGerenteDto dto)
        {
            var gerente = await _context.Gerentes.FindAsync(dto.Id);
            if (gerente == null)
            {
                await AlterarPerfilUsuarioAsync(dto.Id, "Gerente", dto.Setor);
                gerente = await _context.Gerentes.FindAsync(dto.Id);
                if (gerente == null) throw new KeyNotFoundException("Gerente não encontrado.");
            }

            if (gerente.Email != dto.Email)
            {
                var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != dto.Id);
                if (emailJaExiste)
                    throw new InvalidOperationException("Esse email já está cadastrado.");
            }

            gerente.Nome = dto.Nome;
            gerente.Email = dto.Email;
            gerente.Setor = dto.Setor;

            if (!string.IsNullOrEmpty(dto.Senha))
                gerente.SenhaHash = _passwordService.HashPassword(dto.Senha);

            _context.Gerentes.Update(gerente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AlterarPerfilUsuarioAsync(int id, string novoPerfil, string? extraData)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            if (!Enum.TryParse<PerfilUsuario>(novoPerfil, out var perfilEnum))
                throw new ArgumentException("Perfil inválido.");

            if (usuario.Perfil != perfilEnum)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Usuarios SET Discriminator = {0}, Perfil = {1} WHERE Id = {2}",
                    novoPerfil, (int)perfilEnum, id);

                _context.Entry(usuario).State = EntityState.Detached;
                usuario = await _context.Usuarios.FindAsync(id);
            }

            if (usuario is Cliente c && !string.IsNullOrEmpty(extraData)) c.CPF = extraData;
            else if (usuario is Caixa caixa && !string.IsNullOrEmpty(extraData)) caixa.Turno = extraData;
            else if (usuario is Gerente g && !string.IsNullOrEmpty(extraData)) g.Setor = extraData;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consulta

        public async Task<IEnumerable<UsuarioDto>> ListarTodosUsuariosAsync()
        {
            var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();
            return usuarios.Select(MapearParaDto);
        }

        public async Task<UsuarioDto?> ObterUsuarioPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return usuario != null ? MapearParaDto(usuario) : null;
        }

        public async Task<UsuarioDto?> ObterUsuarioPorEmailAsync(string email)
        {
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            return usuario != null ? MapearParaDto(usuario) : null;
        }

        #endregion

        #region Deleção

        public async Task RemoverUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Autenticação

        public async Task<TokenDto?> AutenticarAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null)
                return null;

            if (!_passwordService.VerifyPassword(dto.Senha, usuario.SenhaHash))
                return null;

            var token = _tokenService.GerarToken(usuario);

            return new TokenDto
            {
                Token = token,
                Usuario = MapearParaDto(usuario),
                ExpiresIn = DateTime.UtcNow.AddHours(24)
            };
        }

        #endregion

        #region Mapeador

        private static UsuarioDto MapearParaDto(Usuario usuario)
        {
            var dto = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString()
            };

            if (usuario is Cliente cliente)
                dto.CPF = cliente.CPF;
            else if (usuario is Caixa caixa)
                dto.Turno = caixa.Turno;
            else if (usuario is Gerente gerente)
                dto.Setor = gerente.Setor;

            return dto;
        }

        #endregion
    }
}