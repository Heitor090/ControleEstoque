using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControleEstoque.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace ControleEstoque.API.Services
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario)
        {
            var chaveSecreta = _configuration["Jwt:ChaveSecreta"];
            var emissor = _configuration["Jwt:Emissor"];
            var audiencia = _configuration["Jwt:Audiencia"];

            if (string.IsNullOrEmpty(chaveSecreta) || string.IsNullOrEmpty(emissor) || string.IsNullOrEmpty(audiencia))
                throw new InvalidOperationException("Configurações JWT não encontradas.");

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
                new Claim("role", usuario.Perfil.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: emissor,
                audience: audiencia,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
