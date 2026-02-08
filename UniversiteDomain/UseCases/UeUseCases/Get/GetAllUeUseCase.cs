using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetAllUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<Ue>> ExecuteAsync()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        return await repositoryFactory.UeRepository().FindAllAsync();
    }

    public static bool IsAuthorized(string role)
    {
        return true;
    }
}
