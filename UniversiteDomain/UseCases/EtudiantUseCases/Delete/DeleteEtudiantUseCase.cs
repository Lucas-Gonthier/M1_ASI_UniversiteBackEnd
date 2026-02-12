using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long etudiantId)
    {
        var etudiant = await CheckBusinessRules(etudiantId);

        var user = await repositoryFactory.UniversiteUserRepository().FindByEmailAsync(etudiant.Email);
        await repositoryFactory.UniversiteUserRepository().DeleteAsync(user);

        await repositoryFactory.EtudiantRepository().DeleteAsync(etudiantId);
        await repositoryFactory.EtudiantRepository().SaveChangesAsync();
    }

    private async Task<Etudiant> CheckBusinessRules(long etudiantId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.EtudiantId == etudiantId);
        if (etudiants == null || etudiants.Count == 0)
            throw new ArgumentException($"Étudiant avec l'ID {etudiantId} non trouvé");

        return etudiants[0];
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}