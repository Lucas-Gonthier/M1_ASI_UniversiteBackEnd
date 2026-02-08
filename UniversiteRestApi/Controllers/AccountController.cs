using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos.Securite;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IRepositoryFactory repositoryFactory, IConfiguration configuration)
    : ControllerBase
{
    private readonly IUniversiteUserRepository _userRepository = repositoryFactory.UniversiteUserRepository();

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        // On commence par vérifier que l'email correspond bien à un user reconnu
        var user = (UniversiteUser)await _userRepository.FindByEmailAsync(login.Email);
        // On vérifie que le mot de passe est correct
        if (!await _userRepository.CheckPasswordAsync(user, login.Password)) return Unauthorized();
        // Authentification réussie
        // On récupère les infos et on construit le token Jwt
        // Liste des rôles remplis par le user
        var userRoles = await _userRepository.GetRolesAsync(user);
        // Récolte desinformations concernant le user
        List<Claim> authClaims =
        [
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, login.Email!),
            new("userId", user.Id)
        ];
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        // Construction du jeton Jwt
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            claims: authClaims,
            expires: DateTime.Now.AddMinutes(double.Parse(configuration["Jwt:ExpiryMinutes"]!)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256)
        );
        // Renvoi du token au front
        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}