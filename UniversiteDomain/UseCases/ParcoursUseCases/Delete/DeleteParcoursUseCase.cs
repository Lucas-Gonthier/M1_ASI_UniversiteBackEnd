using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Delete;

public class DeleteParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long parcoursId)
    {
        await CheckBusinessRules(parcoursId);
        await repositoryFactory.ParcoursRepository().DeleteAsync(parcoursId);
        await repositoryFactory.ParcoursRepository().SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long parcoursId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parcoursId);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var parcoursList = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.ParcoursId == parcoursId);
        if (parcoursList == null || parcoursList.Count == 0)
            throw new ArgumentException($"Parcours avec l'ID {parcoursId} non trouvé");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}
