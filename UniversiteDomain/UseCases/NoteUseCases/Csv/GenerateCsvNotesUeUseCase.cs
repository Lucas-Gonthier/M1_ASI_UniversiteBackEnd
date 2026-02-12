using System.Globalization;
using CsvHelper;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Csv;

public class GenerateCsvNotesUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<byte[]> ExecuteAsync(long ueId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);

        // Récupérer l'UE avec ses parcours, étudiants inscrits et notes existantes
        var ue = await repositoryFactory.UeRepository().FindUeWithEtudiantsAsync(ueId)
                 ?? throw new ArgumentException($"UE avec l'ID {ueId} non trouvée");

        // Récupérer tous les étudiants des parcours qui contiennent cette UE
        var etudiants = ue.EnseigneeDans
            .Where(p => p.Inscrits != null)
            .SelectMany(p => p.Inscrits!)
            .DistinctBy(e => e.EtudiantId)
            .OrderBy(e => e.Nom)
            .ThenBy(e => e.Prenom)
            .ToList();

        // Construire les lignes CSV
        var notesDict = ue.NotesDesEtudiants.ToDictionary(n => n.EtudiantId, n => n.Valeur);

        var lignes = etudiants.Select(e => new NoteCsvLigneDto
        {
            NumEtud = e.NumEtud,
            Nom = e.Nom,
            Prenom = e.Prenom,
            NumeroUe = ue.NumeroUe,
            IntituleUe = ue.Intitule,
            Note = notesDict.TryGetValue(e.EtudiantId, out var valeur) ? valeur : null
        }).ToList();

        // Générer le CSV en mémoire
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<NoteCsvLigneDtoMap>();
        await csv.WriteRecordsAsync(lignes);
        await writer.FlushAsync();

        return memoryStream.ToArray();
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}