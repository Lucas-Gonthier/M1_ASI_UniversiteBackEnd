using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetAllEtudiantsUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<Etudiant>> ExecuteAsync()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        return await repositoryFactory.EtudiantRepository().FindAllAsync();
    }

    public static bool IsAuthorized(string role)
    {
        return true;
    }
}
