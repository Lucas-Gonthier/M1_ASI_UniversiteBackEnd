using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
{
        
    // GET: api/<EtudiantController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return ["value1", "value2"];
    }

    // GET api/Etudiant/5
    [HttpGet("{id:long}", Name = "GetEtudiantById")]
    public async Task<ActionResult<EtudiantDto>> GetAsync(long id)
    {
        var repo = repositoryFactory.EtudiantRepository();
        var etudiants = await repo.FindByConditionAsync(e => e.EtudiantId == id);
        var etudiant = etudiants.FirstOrDefault();
        if (etudiant == null) return NotFound();

        return Ok(EtudiantDto.ToDto(etudiant));
    }

    // POST api/Etudiant
    [HttpPost]
    public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
    {
        var etudiant = etudiantDto.ToEntity();
        var uc = new CreateEtudiantUseCase(repositoryFactory.EtudiantRepository());
        etudiant = await uc.ExecuteAsync(etudiant);

        var dto = EtudiantDto.ToDto(etudiant);

        return CreatedAtRoute("GetEtudiantById", new { id = etudiant.EtudiantId }, dto);
    }

    // PUT api/<EtudiantController>/5
    [HttpPut("{id:int}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<EtudiantController>/5
    [HttpDelete("{id:int}")]
    public void Delete(int id)
    {
    }
}