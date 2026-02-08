using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UEExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

public class AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
    {
        await CheckBusinessRules(idParcours, idUe);
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
    }

    private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idParcours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);

        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());

        // On recherche l'ue
        var ue = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.UeId.Equals(idUe));
        if (ue == null) throw new UeNotFoundException(idUe.ToString());

        // On recherche le parcours
        var parcours =
            await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.ParcoursId.Equals(idParcours));
        if (parcours == null) throw new ParcoursNotFoundException(idParcours.ToString());

        // On vérifie que l'Ue n'est pas déjà dans le parcours
        // Des ues sont déjà enregistrées dans le parcours
        // On recherche si l'UE qu'on veut ajouter n'existe pas déjà
        var inscrites = parcours[0].UEsEnseignees;
        var trouve = inscrites.FindAll(e => e.UeId.Equals(idUe));
        if (trouve.Count > 0)
            throw new DuplicateUeDansParcoursException(idUe + " est déjà présente dans le parcours : " + idParcours);
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable);
    }
}