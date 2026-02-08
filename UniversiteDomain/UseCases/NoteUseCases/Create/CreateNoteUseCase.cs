using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        var etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.EtudiantId == etudiantId);
        if (etudiants == null || etudiants.Count == 0)
            throw new ArgumentException($"Étudiant avec l'ID {etudiantId} non trouvé");

        var ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.UeId == ueId);
        if (ues == null || ues.Count == 0)
            throw new ArgumentException($"UE avec l'ID {ueId} non trouvée");

        return await ExecuteAsync(etudiants[0], ues[0], valeur);
    }

    public async Task<Note> ExecuteAsync(Etudiant etudiant, Ue ue, float valeur)
    {
        var note = new Note
        {
            EtudiantId = etudiant.EtudiantId,
            UeId = ue.UeId,
            Valeur = valeur
        };
        return await ExecuteAsync(etudiant, note, ue);
    }

    private async Task<Note> ExecuteAsync(Etudiant etudiant, Note note, Ue ue)
    {
        await CheckBusinessRules(etudiant, note, ue);
        var noteRepository = repositoryFactory.NoteRepository();
        var et = await noteRepository.CreateAsync(note);
        noteRepository.SaveChangesAsync().Wait();
        return et;
    }

    private static Task CheckBusinessRules(Etudiant etudiant, Note note, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(ue);

        if (note.EtudiantId != 0 && note.EtudiantId != etudiant.EtudiantId)
            throw new InvalidOperationException(
                $"Incohérence: Note.EtudiantId={note.EtudiantId} ≠ Etudiant.EtudiantId={etudiant.EtudiantId}");
        if (note.UeId != 0 && note.UeId != ue.UeId)
            throw new InvalidOperationException($"Incohérence: Note.UeId={note.UeId} ≠ Ue.UeId={ue.UeId}");

        if (note.Valeur < 0f || note.Valeur > 20f)
            throw new InvalidNoteValeurException(
                $"{note.Valeur} incorrect - Une note doit être comprise entre 0 et 20");

        if (etudiant.Notes.Any(n => n.UeId == ue.UeId))
            throw new DuplicateNoteOnUeException(
                $"L'étudiant {etudiant.EtudiantId} a déjà une note pour l'UE {ue.UeId}");

        var ueInParcours = etudiant.ParcoursSuivi?.UEsEnseignees.Any(u => u.UeId == ue.UeId) == true;
        if (!ueInParcours)
            throw new NoteOnUeNotInParcoursException(
                $"L'étudiant {etudiant.EtudiantId} n'est pas inscrit à l'UE {ue.UeId}");

        return Task.CompletedTask;
    }
    
    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}