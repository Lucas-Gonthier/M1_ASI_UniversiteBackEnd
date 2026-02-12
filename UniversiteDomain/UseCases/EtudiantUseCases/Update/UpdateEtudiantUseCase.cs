using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        var existing = await CheckBusinessRules(etudiant);

        existing.Nom = etudiant.Nom;
        existing.Prenom = etudiant.Prenom;
        existing.Email = etudiant.Email;

        await repositoryFactory.EtudiantRepository().UpdateAsync(existing);
        await repositoryFactory.EtudiantRepository().SaveChangesAsync();
        return existing;
    }

    private async Task<Etudiant> CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiant.EtudiantId);

        // Vérifier que l'étudiant existe
        var existing = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.EtudiantId == etudiant.EtudiantId);
        if (existing == null || existing.Count == 0)
            throw new ArgumentException($"Étudiant avec l'ID {etudiant.EtudiantId} non trouvé");

        // Vérifier le nom
        if (string.IsNullOrWhiteSpace(etudiant.Nom) || etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException(etudiant.Nom + " doit contenir au moins 3 caractères");

        // Vérifier le prénom
        if (string.IsNullOrWhiteSpace(etudiant.Prenom) || etudiant.Prenom.Length < 3)
            throw new InvalidPrenomEtudiantException(etudiant.Prenom + " doit contenir au moins 3 caractères");

        return existing[0];
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}