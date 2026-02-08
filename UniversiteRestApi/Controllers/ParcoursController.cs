using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParcoursDto>>> GetAllAsync()
    {
        try
        {
            var parcours = await new GetAllParcoursUseCase(repositoryFactory).ExecuteAsync();
            return Ok(parcours.Select(ParcoursDto.ToDto));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("parcours", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ParcoursDto>> GetAsync(long id)
    {
        try
        {
            var parcours = await new GetParcoursUseCase(repositoryFactory).ExecuteAsync(id);
            if (parcours == null) return NotFound();
            return Ok(ParcoursDto.ToDto(parcours));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("parcours", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost]
    public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!CreateParcoursUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var parcours = await new CreateParcoursUseCase(repositoryFactory)
                .ExecuteAsync(parcoursDto.ToEntity());

            var dto = ParcoursDto.ToDto(parcours);
            return CreatedAtAction("Get", new { id = dto.Id }, dto);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("parcours", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ParcoursDto>> PutAsync(long id, [FromBody] ParcoursDto parcoursDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!UpdateParcoursUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var parcours = parcoursDto.ToEntity();
            parcours.ParcoursId = id;
            
            var updated = await new UpdateParcoursUseCase(repositoryFactory).ExecuteAsync(parcours);
            return Ok(ParcoursDto.ToDto(updated));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("parcours", e.Message);
            return ValidationProblem();
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!DeleteParcoursUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            await new DeleteParcoursUseCase(repositoryFactory).ExecuteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("parcours", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost("{idParcours:long}/etudiants/{idEtudiant:long}")]
    public async Task<ActionResult> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!AddEtudiantDansParcoursUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            await new AddEtudiantDansParcoursUseCase(repositoryFactory)
                .ExecuteAsync(idParcours, idEtudiant);
            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("inscription", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost("{idParcours:long}/ues/{idUe:long}")]
    public async Task<ActionResult> AddUeAsync(long idParcours, long idUe)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!AddUeDansParcoursUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            await new AddUeDansParcoursUseCase(repositoryFactory)
                .ExecuteAsync(idParcours, idUe);
            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("maquette", e.Message);
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