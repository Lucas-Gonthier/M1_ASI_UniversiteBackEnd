using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant?> ExecuteAsync(long id)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        
        var etudiantsList = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.EtudiantId == id);
        return etudiantsList.FirstOrDefault();
    }

    public static bool IsAuthorized(string role)
    {
        return true;
    }
}
