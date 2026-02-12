using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note?> ExecuteAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);

        var noteList = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        return noteList.FirstOrDefault();
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}