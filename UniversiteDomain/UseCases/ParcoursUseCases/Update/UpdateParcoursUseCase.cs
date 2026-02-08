using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        await repositoryFactory.ParcoursRepository().UpdateAsync(parcours);
        await repositoryFactory.ParcoursRepository().SaveChangesAsync();
        return parcours;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parcours.ParcoursId);

        // Vérifier que le parcours existe
        var existing = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.ParcoursId == parcours.ParcoursId);
        if (existing == null || existing.Count == 0)
            throw new ParcoursNotFoundException(parcours.ParcoursId.ToString());

        // Vérifier le nom
        if (string.IsNullOrWhiteSpace(parcours.NomParcours) || parcours.NomParcours.Length < 2)
            throw new InvalidNomParcoursException(parcours.NomParcours + " doit contenir au moins 2 caractères");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}
