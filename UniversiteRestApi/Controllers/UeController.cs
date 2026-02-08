using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Delete;
using UniversiteDomain.UseCases.UeUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Update;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UeDto>>> GetAllAsync()
    {
        try
        {
            var ues = await new GetAllUeUseCase(repositoryFactory).ExecuteAsync();
            return Ok(ues.Select(UeDto.ToDto));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("ue", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UeDto>> GetAsync(long id)
    {
        try
        {
            var ue = await new GetUeUseCase(repositoryFactory).ExecuteAsync(id);
            if (ue == null) return NotFound();
            return Ok(UeDto.ToDto(ue));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("ue", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost]
    public async Task<ActionResult<UeDto>> PostAsync([FromBody] UeDto ueDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!CreateUeUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var ue = await new CreateUeUseCase(repositoryFactory)
                .ExecuteAsync(ueDto.ToEntity());

            var dto = UeDto.ToDto(ue);
            return CreatedAtAction("Get", new { id = dto.Id }, dto);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("ue", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<UeDto>> PutAsync(long id, [FromBody] UeDto ueDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!UpdateUeUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var ue = ueDto.ToEntity();
            ue.UeId = id;

            var updated = await new UpdateUeUseCase(repositoryFactory).ExecuteAsync(ue);
            return Ok(UeDto.ToDto(updated));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("ue", e.Message);
            return ValidationProblem();
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!DeleteUeUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            await new DeleteUeUseCase(repositoryFactory).ExecuteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("ue", e.Message);
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