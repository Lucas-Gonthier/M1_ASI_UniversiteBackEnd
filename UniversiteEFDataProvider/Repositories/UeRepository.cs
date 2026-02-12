using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    private readonly UniversiteDbContext _context = context;

    public async Task<Ue?> FindUeWithEtudiantsAsync(long ueId)
    {
        return await _context.Set<Ue>()
            .Include(u => u.EnseigneeDans)
                .ThenInclude(p => p.Inscrits)
            .Include(u => u.NotesDesEtudiants)
            .FirstOrDefaultAsync(u => u.UeId == ueId);
    }
}