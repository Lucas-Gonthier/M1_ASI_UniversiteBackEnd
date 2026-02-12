using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Delete;

public class DeleteNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long etudiantId, long ueId)
    {
        await CheckBusinessRules(etudiantId, ueId);

        var noteList = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        var note = noteList.FirstOrDefault();

        if (note != null)
        {
            await repositoryFactory.NoteRepository().DeleteAsync(note);
            await repositoryFactory.NoteRepository().SaveChangesAsync();
        }
    }

    private async Task CheckBusinessRules(long etudiantId, long ueId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var noteList = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        if (noteList == null || noteList.Count == 0)
            throw new ArgumentException($"Note pour l'étudiant {etudiantId} et l'UE {ueId} non trouvée");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}