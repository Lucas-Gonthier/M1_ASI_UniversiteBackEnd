using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    private readonly UniversiteDbContext _context = context;

    public Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        return AddEtudiantAsync(parcours.ParcoursId, etudiant.EtudiantId);
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(_context.Etudiants);
        ArgumentNullException.ThrowIfNull(_context.Parcours);
        var e = (await _context.Etudiants.FindAsync(idEtudiant))!;
        var p = (await _context.Parcours.FindAsync(idParcours))!;
        p.Inscrits?.Add(e);
        e.ParcoursSuivi = p;
        await _context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        return await AddEtudiantAsync(parcours!.ParcoursId, etudiants.Select(e => e.EtudiantId).ToArray());
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(_context.Etudiants);
        ArgumentNullException.ThrowIfNull(_context.Parcours);
        ArgumentNullException.ThrowIfNull(idEtudiants);
        var tasks = idEtudiants.Select(id => AddEtudiantAsync(idParcours, id)).Cast<Task>().ToList();

        await Task.WhenAll(tasks);
        return (await _context.Parcours.FindAsync(idParcours))!;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(_context.Parcours);
        ArgumentNullException.ThrowIfNull(_context.Ues);

        var p = await _context.Parcours
                    .Include(x => x.UEsEnseignees)
                    .FirstOrDefaultAsync(x => x.ParcoursId == idParcours)
                ?? throw new ArgumentException("Parcours not found");

        var ue = await _context.Ues.FindAsync(idUe)
                 ?? throw new ArgumentException("Ue not found");

        if (p.UEsEnseignees.All(x => x.UeId != idUe))
            p.UEsEnseignees.Add(ue);

        await _context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUEs)
    {
        ArgumentNullException.ThrowIfNull(_context.Parcours);
        ArgumentNullException.ThrowIfNull(_context.Ues);
        ArgumentNullException.ThrowIfNull(idUEs);

        var p = await _context.Parcours
                    .Include(x => x.UEsEnseignees)
                    .FirstOrDefaultAsync(x => x.ParcoursId == idParcours)
                ?? throw new ArgumentException("Parcours not found");

        var requestedIds = idUEs.Distinct().ToArray();
        if (requestedIds.Length == 0)
            return p;

        var alreadyLinked = new HashSet<long>(
            p.UEsEnseignees.Select(u => u.UeId)
        );

        var idsToAdd = requestedIds.Where(id => !alreadyLinked.Contains(id)).ToArray();
        if (idsToAdd.Length == 0)
            return p;

        var uesToAdd = await _context.Ues
            .Where(u => idsToAdd.Contains(u.UeId))
            .ToListAsync();

        // Associer
        foreach (var ue in uesToAdd)
            p.UEsEnseignees.Add(ue);

        await _context.SaveChangesAsync();
        return p;
    }
}