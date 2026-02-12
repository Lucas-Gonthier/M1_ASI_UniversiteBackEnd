using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        var existing = await CheckBusinessRules(parcours);

        existing.NomParcours = parcours.NomParcours;
        existing.AnneeFormation = parcours.AnneeFormation;

        await repositoryFactory.ParcoursRepository().UpdateAsync(existing);
        await repositoryFactory.ParcoursRepository().SaveChangesAsync();
        return existing;
    }

    private async Task<Parcours> CheckBusinessRules(Parcours parcours)
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

        return existing[0];
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}