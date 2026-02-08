using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNotesByEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<Note>> ExecuteAsync(long etudiantId)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        
        return await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.EtudiantId == etudiantId);
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
