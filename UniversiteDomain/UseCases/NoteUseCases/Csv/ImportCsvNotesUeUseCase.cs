using System.Globalization;
using CsvHelper;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Csv;

public class ImportCsvNotesUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<ImportCsvResultat> ExecuteAsync(long ueId, Stream csvStream)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);
        ArgumentNullException.ThrowIfNull(csvStream);

        // Lire le CSV
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<NoteCsvLigneDtoMap>();
        var lignes = csv.GetRecords<NoteCsvLigneDto>().ToList();

        // Récupérer l'UE
        var ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.UeId == ueId);
        if (ues.Count == 0)
            throw new ArgumentException($"UE avec l'ID {ueId} non trouvée");
        var ue = ues[0];

        // Récupérer tous les étudiants en base
        var tousLesEtudiants = await repositoryFactory.EtudiantRepository().FindAllAsync();
        var etudiantsParNumEtud = tousLesEtudiants.ToDictionary(e => e.NumEtud, e => e);

        // Récupérer les notes existantes pour cette UE
        var notesExistantes = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.UeId == ueId);
        var notesDict = notesExistantes.ToDictionary(n => n.EtudiantId, n => n);

        // Phase 1 : Validation complète (aucune écriture tant qu'il y a des erreurs)
        var erreurs = new List<string>();

        for (var i = 0; i < lignes.Count; i++)
        {
            var ligne = lignes[i];
            var numLigne = i + 2; // +2 car ligne 1 = header, i est 0-based

            // Vérifier que le NumeroUe correspond
            if (ligne.NumeroUe != ue.NumeroUe)
            {
                erreurs.Add(
                    $"Ligne {numLigne} : le NumeroUe '{ligne.NumeroUe}' ne correspond pas à l'UE attendue '{ue.NumeroUe}'");
            }

            // Vérifier que l'étudiant existe
            if (!etudiantsParNumEtud.ContainsKey(ligne.NumEtud))
            {
                erreurs.Add($"Ligne {numLigne} : étudiant '{ligne.NumEtud}' non trouvé en base");
            }

            // Vérifier la note si renseignée
            if (ligne.Note.HasValue)
            {
                if (ligne.Note.Value < 0 || ligne.Note.Value > 20)
                {
                    erreurs.Add($"Ligne {numLigne} : la note {ligne.Note.Value} doit être comprise entre 0 et 20");
                }
            }
        }

        if (erreurs.Count > 0)
            throw new InvalidCsvException(erreurs);

        // Phase 2 : Enregistrement des notes
        var notesCreees = 0;
        var notesMisesAJour = 0;
        var notesIgnorees = 0;

        foreach (var ligne in lignes)
        {
            if (!ligne.Note.HasValue)
            {
                notesIgnorees++;
                continue;
            }

            var etudiant = etudiantsParNumEtud[ligne.NumEtud];

            if (notesDict.TryGetValue(etudiant.EtudiantId, out var noteExistante))
            {
                // Mise à jour
                noteExistante.Valeur = ligne.Note.Value;
                await repositoryFactory.NoteRepository().UpdateAsync(noteExistante);
                notesMisesAJour++;
            }
            else
            {
                // Création
                var nouvelleNote = new Note
                {
                    EtudiantId = etudiant.EtudiantId,
                    UeId = ueId,
                    Valeur = ligne.Note.Value
                };
                await repositoryFactory.NoteRepository().CreateAsync(nouvelleNote);
                notesCreees++;
            }
        }

        await repositoryFactory.NoteRepository().SaveChangesAsync();

        return new ImportCsvResultat
        {
            NotesCreees = notesCreees,
            NotesMisesAJour = notesMisesAJour,
            NotesIgnorees = notesIgnorees
        };
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}

public class ImportCsvResultat
{
    public int NotesCreees { get; init; }
    public int NotesMisesAJour { get; init; }
    public int NotesIgnorees { get; init; }
}