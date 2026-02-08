using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours?> ExecuteAsync(long id)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        
        var parcoursList = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.ParcoursId == id);
        return parcoursList.FirstOrDefault();
    }

    public static bool IsAuthorized(string role)
    {
        return true;
    }
}
