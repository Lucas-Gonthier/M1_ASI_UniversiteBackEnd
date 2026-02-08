using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Delete;

public class DeleteUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long ueId)
    {
        await CheckBusinessRules(ueId);
        await repositoryFactory.UeRepository().DeleteAsync(ueId);
        await repositoryFactory.UeRepository().SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long ueId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var ueList = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.UeId == ueId);
        if (ueList == null || ueList.Count == 0)
            throw new ArgumentException($"UE avec l'ID {ueId} non trouvée");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}
