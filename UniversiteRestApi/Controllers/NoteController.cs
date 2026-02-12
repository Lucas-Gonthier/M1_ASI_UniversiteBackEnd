using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Csv;
using UniversiteDomain.UseCases.NoteUseCases.Delete;
using UniversiteDomain.UseCases.NoteUseCases.Get;
using UniversiteDomain.UseCases.NoteUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAllAsync()
    {
        var (role, _) = GetAuthenticatedUser();

        if (!GetAllNotesUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var notes = await new GetAllNotesUseCase(repositoryFactory).ExecuteAsync();
            return Ok(notes.Select(NoteDto.ToDto));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("etudiant/{etudiantId:long}")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetByEtudiantAsync(long etudiantId)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!GetNotesByEtudiantUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var notes = await new GetNotesByEtudiantUseCase(repositoryFactory).ExecuteAsync(etudiantId);
            return Ok(notes.Select(NoteDto.ToDto));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("ue/{ueId:long}")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetByUeAsync(long ueId)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!GetNotesByUeUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var notes = await new GetNotesByUeUseCase(repositoryFactory).ExecuteAsync(ueId);
            return Ok(notes.Select(NoteDto.ToDto));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("{etudiantId:long}/{ueId:long}")]
    public async Task<ActionResult<NoteDto>> GetAsync(long etudiantId, long ueId)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!GetNoteUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var note = await new GetNoteUseCase(repositoryFactory).ExecuteAsync(etudiantId, ueId);
            if (note == null) return NotFound();
            return Ok(NoteDto.ToDto(note));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> PostAsync([FromBody] NoteDto noteDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!CreateNoteUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var note = await new CreateNoteUseCase(repositoryFactory)
                .ExecuteAsync(noteDto.EtudiantId, noteDto.UeId, noteDto.Valeur);

            var dto = NoteDto.ToDto(note);
            return CreatedAtAction("Get",
                new { etudiantId = dto.EtudiantId, ueId = dto.UeId }, dto);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPut("{etudiantId:long}/{ueId:long}")]
    public async Task<ActionResult<NoteDto>> PutAsync(long etudiantId, long ueId, [FromBody] NoteDto noteDto)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!UpdateNoteUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var note = await new UpdateNoteUseCase(repositoryFactory)
                .ExecuteAsync(etudiantId, ueId, noteDto.Valeur);
            return Ok(NoteDto.ToDto(note));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpDelete("{etudiantId:long}/{ueId:long}")]
    public async Task<ActionResult> DeleteAsync(long etudiantId, long ueId)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!DeleteNoteUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            await new DeleteNoteUseCase(repositoryFactory).ExecuteAsync(etudiantId, ueId);
            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpGet("ue/{ueId:long}/csv")]
    public async Task<ActionResult> GetCsvAsync(long ueId)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!GenerateCsvNotesUeUseCase.IsAuthorized(role))
            return Unauthorized();

        try
        {
            var csvBytes = await new GenerateCsvNotesUeUseCase(repositoryFactory).ExecuteAsync(ueId);
            return File(csvBytes, "text/csv", $"notes_UE_{ueId}.csv");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
            return ValidationProblem();
        }
    }

    [HttpPost("ue/{ueId:long}/csv")]
    public async Task<ActionResult> PostCsvAsync(long ueId, IFormFile fichierCsv)
    {
        var (role, _) = GetAuthenticatedUser();

        if (!ImportCsvNotesUeUseCase.IsAuthorized(role))
            return Unauthorized();

        if (fichierCsv == null || fichierCsv.Length == 0)
            return BadRequest("Aucun fichier CSV fourni");

        try
        {
            using var stream = fichierCsv.OpenReadStream();
            var resultat = await new ImportCsvNotesUeUseCase(repositoryFactory).ExecuteAsync(ueId, stream);
            return Ok(resultat);
        }
        catch (InvalidCsvException e)
        {
            return BadRequest(new { e.Message, e.Erreurs });
        }
        catch (Exception e)
        {
            ModelState.AddModelError("note", e.Message);
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