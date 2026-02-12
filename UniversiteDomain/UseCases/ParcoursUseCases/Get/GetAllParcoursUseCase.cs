using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetAllParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<Parcours>> ExecuteAsync()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        return await repositoryFactory.ParcoursRepository().FindAllAsync();
    }

    public static bool IsAuthorized(string role)
    {
        // Lecture publique pour tous les rôles authentifiés
        return true;
    }
}