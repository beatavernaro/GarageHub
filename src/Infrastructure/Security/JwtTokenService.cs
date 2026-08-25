using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Security;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public class JwtTokenService(
    IConfiguration configuration)
    : ITokenService
{
    private readonly IConfiguration _configuration =
        configuration;

    public string GerarToken(
        Usuario usuario,
        DateTime expiracao)
    {
        var key =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT Key não configurada.");

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer não configurado.");

        var audience =
            _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience não configurado.");

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                usuario.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                usuario.Email),

            new Claim(
                "name",
                usuario.Nome),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiracao,
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}