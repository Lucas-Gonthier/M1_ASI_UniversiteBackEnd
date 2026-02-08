using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UEExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        var createdUe = await repositoryFactory.UeRepository().CreateAsync(ue);
        await repositoryFactory.UeRepository().SaveChangesAsync();
        return createdUe;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // On recherche une UE avec le même numéro d'UE
        var existe = await repositoryFactory.UeRepository()
            .FindByConditionAsync(e => e.NumeroUe.Equals(ue.NumeroUe));

        // Si une UE avec le même numéro existe déjà, on lève une exception personnalisée
        if (existe is { Count: > 0 })
            throw new DuplicateNumeroUeException(ue.NumeroUe +
                                                 " - ce numéro d'UE est déjà affecté à une UE");

        // Le métier définit que l'intitulé doit contenir au moins 3 caractères
        if (ue.Intitule.Length < 3)
            throw new InvalidIntituleUeException(ue.Intitule +
                                                 " incorrect - L'intitulé de l'UE doit contenir plus de 3 caractères");
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}
