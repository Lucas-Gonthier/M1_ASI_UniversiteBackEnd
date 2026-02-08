using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue?> ExecuteAsync(long id)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        
        var ueList = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.UeId == id);
        return ueList.FirstOrDefault();
    }

    public static bool IsAuthorized(string role)
    {
        return true;
    }
}
