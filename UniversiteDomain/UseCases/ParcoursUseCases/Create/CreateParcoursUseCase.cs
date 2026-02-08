using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

// ReSharper disable once IdentifierTypo
namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        var par = await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        repositoryFactory.ParcoursRepository().SaveChangesAsync().Wait();
        return par;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.ParcoursId == 0 ? null : parcours.ParcoursId);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation == 0 ? null : parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // On recherche un étudiant avec le même numéro étudiant
        var existe = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(e => e.ParcoursId.Equals(parcours.ParcoursId));

        // Si un étudiant avec le même numéro étudiant existe déjà, on lève une exception personnalisée
        if (existe is { Count: > 0 })
            throw new DuplicateParcoursIdException(parcours.ParcoursId +
                                                   " - cet id de parcours est déjà affecté à un parcours");

        // Récupérer l'année actuelle
        var anneeActuelle = DateTime.Now.Year;
        // On vérifie que l'année de formation est valide (entre 1 et l'année actuelle)
        if (parcours.AnneeFormation > anneeActuelle)
            throw new InvalidAnneeFormationException(parcours.AnneeFormation +
                                                     " incorrect - L'année de formation ne peut pas être dans le futur");

        // Le métier définit qu'un nom de parcours doit contenir au moins 2 caractères
        if (parcours.NomParcours.Length < 2)
            throw new InvalidNomParcoursException(parcours.NomParcours +
                                                  " incorrect - Le nom du parcours doit contenir au moins 2 caractères");
    }
}