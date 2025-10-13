using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(Etudiant etudiant, Ue ue, float valeur)
    {
        var note = new Note(valeur);
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

    private Task CheckBusinessRules(Etudiant etudiant, Note note, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // Une note est comprise entre 0 et 20.
        if (note.Valeur is < 0 or > 20)
            throw new InvalidNoteValeurException(
                note.Valeur + " incorrect - Une note doit être comprise entre 0 et 20");

        // Un étudiant n'a qu'une seule note au maximum par UE
        if (etudiant.Notes != null && etudiant.Notes.Exists(n => n.Ue != null && n.Ue.UeId == ue.UeId))
            throw new DuplicateNoteOnUeException("L'étudiant " + etudiant.EtudiantId + " a déjà une note pour l'UE " + ue.UeId);

        // On vérifie que l'étudiant n'a des notes que sur les UEs de son parcours
        if (etudiant.ParcoursSuivi == null || !etudiant.ParcoursSuivi.UEsEnseignees.Contains(ue))
            throw new NoteOnUeNotInParcoursException("L'étudiant " + etudiant.EtudiantId + " n'est pas inscrit à l'UE " +
                                                     ue.UeId);
        return Task.CompletedTask;
    }
}