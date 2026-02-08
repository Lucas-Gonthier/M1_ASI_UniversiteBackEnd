using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UEExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        await repositoryFactory.UeRepository().UpdateAsync(ue);
        await repositoryFactory.UeRepository().SaveChangesAsync();
        return ue;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ue.UeId);

        // Vérifier que l'UE existe
        var existing = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.UeId == ue.UeId);
        if (existing == null || existing.Count == 0)
            throw new ArgumentException($"UE avec l'ID {ue.UeId} non trouvée");

        // Vérifier l'intitulé
        if (string.IsNullOrWhiteSpace(ue.Intitule))
            throw new InvalidIntituleUeException("L'intitulé de l'UE ne peut pas être vide");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}
