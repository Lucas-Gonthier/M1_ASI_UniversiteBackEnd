using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class EtudiantRepository(UniversiteDbContext context) : Repository<Etudiant>(context), IEtudiantRepository
{
    private readonly UniversiteDbContext _context = context;

    public async Task AffecterParcoursAsync(long idEtudiant, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(_context.Etudiants);
        ArgumentNullException.ThrowIfNull(_context.Parcours);
        var e = (await _context.Etudiants.FindAsync(idEtudiant))!;
        var p = (await _context.Parcours.FindAsync(idParcours))!;
        e.ParcoursSuivi = p;
        await _context.SaveChangesAsync();
    }

    public async Task AffecterParcoursAsync(Etudiant etudiant, Parcours parcours)
    {
        await AffecterParcoursAsync(etudiant.EtudiantId, parcours.ParcoursId);
    }
}