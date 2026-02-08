using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long etudiantId)
    {
        await CheckBusinessRules(etudiantId);
        await repositoryFactory.EtudiantRepository().DeleteAsync(etudiantId);
        await repositoryFactory.EtudiantRepository().SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long etudiantId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        
        // Vérifier que l'étudiant existe
        var etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.EtudiantId == etudiantId);
        if (etudiants == null || etudiants.Count == 0)
            throw new ArgumentException($"Étudiant avec l'ID {etudiantId} non trouvé");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
