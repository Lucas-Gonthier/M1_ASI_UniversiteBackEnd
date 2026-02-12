using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Update;

public class UpdateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        await CheckBusinessRules(etudiantId, ueId, valeur);

        var noteList = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        var note = noteList.First();

        note.Valeur = valeur;
        await repositoryFactory.NoteRepository().UpdateAsync(note);
        await repositoryFactory.NoteRepository().SaveChangesAsync();

        return note;
    }

    private async Task CheckBusinessRules(long etudiantId, long ueId, float valeur)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // Vérifier que la note est valide (entre 0 et 20)
        if (valeur < 0 || valeur > 20)
            throw new ArgumentOutOfRangeException(nameof(valeur), "La note doit être comprise entre 0 et 20");

        // Vérifier que la note existe
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