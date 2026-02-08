using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.EtudiantUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    private const string DefaultPassword = "Miage2025#";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EtudiantDto>>> GetAllAsync()
    {
        try
        {
            var etudiants = await new GetAllEtudiantsUseCase(repositoryFactory).ExecuteAsync();
            return Ok(etudiants.Select(EtudiantDto.ToDto));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("etudiant", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EtudiantDto>> GetAsync(long id)
    {
        try
        {
            var etudiant = await new GetEtudiantUseCase(repositoryFactory).ExecuteAsync(id);
            if (etudiant == null) return NotFound();
            return Ok(EtudiantDto.ToDto(etudiant));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("etudiant", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost]
    public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!CreateEtudiantUseCase.IsAuthorized(role))
            return Unauthorized();

        var etudiant = etudiantDto.ToEntity();

        try
        {
            etudiant = await new CreateEtudiantUseCase(repositoryFactory).ExecuteAsync(etudiant);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("etudiant", e.Message);
            return ValidationProblem();
        }

        try
        {
            await new CreateUniversiteUserUseCase(repositoryFactory)
                .ExecuteAsync(etudiant.Email, etudiant.Email, DefaultPassword, Roles.Etudiant, etudiant);
        }
        catch (Exception e)
        {
            await new DeleteEtudiantUseCase(repositoryFactory).ExecuteAsync(etudiant.EtudiantId);
            ModelState.AddModelError("user", e.Message);
            return ValidationProblem();
        }

        var dto = EtudiantDto.ToDto(etudiant);
        return CreatedAtAction("Get", new { id = dto.Id }, dto);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<EtudiantDto>> PutAsync(long id, [FromBody] EtudiantDto etudiantDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!UpdateEtudiantUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var etudiant = etudiantDto.ToEntity();
            etudiant.EtudiantId = id;
            
            var updated = await new UpdateEtudiantUseCase(repositoryFactory).ExecuteAsync(etudiant);
            return Ok(EtudiantDto.ToDto(updated));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("etudiant", e.Message);
            return ValidationProblem();
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!DeleteEtudiantUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            await new DeleteEtudiantUseCase(repositoryFactory).ExecuteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("delete", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("complet/{id:long}")]
    public async Task<ActionResult<EtudiantCompletDto>> GetCompletAsync(long id)
    {
        var (role, user) = GetAuthenticatedUser();

        if (!GetEtudiantCompletUseCase.IsAuthorized(role, user, id))
            return Unauthorized();

        try
        {
            var etudiant = await new GetEtudiantCompletUseCase(repositoryFactory).ExecuteAsync(id);

            if (etudiant == null)
                return NotFound();

            return Ok(EtudiantCompletDto.ToDto(etudiant));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("etudiant", e.Message);
            return ValidationProblem();
        }
    }

    private (string Role, IUniversiteUser User) GetAuthenticatedUser()
    {
        var claims = HttpContext.User;

        if (claims.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("Non authentifié");

        var emailClaim = claims.FindFirst(ClaimTypes.Email)
                         ?? throw new UnauthorizedAccessException("Email non trouvé");
        var email = emailClaim.Value;

        var user = new FindUniversiteUserByEmailUseCase(repositoryFactory)
                       .ExecuteAsync(email).Result
                   ?? throw new UnauthorizedAccessException("Utilisateur non trouvé");

        var roleClaim = claims.FindFirst(ClaimTypes.Role)
                        ?? throw new UnauthorizedAccessException("Rôle non défini");
        var role = roleClaim.Value;

        var isInRole = new IsInRoleUseCase(repositoryFactory).ExecuteAsync(email, role).Result;
        return !isInRole ? throw new UnauthorizedAccessException("Rôle invalide") : (role, user);
    }
}