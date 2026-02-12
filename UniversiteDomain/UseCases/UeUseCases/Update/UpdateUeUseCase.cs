using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UEExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        var existing = await CheckBusinessRules(ue);
        
        existing.NumeroUe = ue.NumeroUe;
        existing.Intitule = ue.Intitule;
        
        await repositoryFactory.UeRepository().UpdateAsync(existing);
        await repositoryFactory.UeRepository().SaveChangesAsync();
        return existing;
    }

    private async Task<Ue> CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ue.UeId);

        var existing = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.UeId == ue.UeId);
        if (existing == null || existing.Count == 0)
            throw new ArgumentException($"UE avec l'ID {ue.UeId} non trouvée");

        if (string.IsNullOrWhiteSpace(ue.Intitule))
            throw new InvalidIntituleUeException("L'intitulé de l'UE ne peut pas être vide");

        return existing[0];
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}
