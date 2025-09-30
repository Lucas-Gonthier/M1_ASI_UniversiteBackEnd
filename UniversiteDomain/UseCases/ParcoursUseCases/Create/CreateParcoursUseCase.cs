using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Util;

// ReSharper disable once IdentifierTypo
namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateEtudiantUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(long id, string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours(){Id = id, NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours par = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return par;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.Id);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche un étudiant avec le même numéro étudiant
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(e=>e.Id.Equals(parcours.Id));

        // Si un étudiant avec le même numéro étudiant existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new Dupli(parcours.Id+ " - cet id de parcours est déjà affecté à un parcours");
        
        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(parcours.AnneeFormation)) throw new (parcours.Email + " - Email mal formé");
        
        // On vérifie si l'email est déjà utilisé
        existe = await parcoursRepository.FindByConditionAsync(e=>e.Email.Equals(parcours.Email));
        // Une autre façon de tester la vacuité de la liste
        if (existe is {Count:>0}) throw new DuplicateEmailException(parcours.Email +" est déjà affecté à un étudiant");
        // Le métier définit que les nom doite contenir plus de 3 lettres
        if (parcours.Nom.Length < 3) throw new InvalidNomparcoursException(parcours.Nom +" incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }
}