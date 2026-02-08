using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.SecurityUseCases.Create;

public class CreateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task<IUniversiteUser?> ExecuteAsync(string email, string userName, string password, string role,
        Etudiant? etudiant)
    {
        await CheckBusinessRules(userName, password, role, etudiant);
        var userCree = await factory.UniversiteUserRepository()
            .AddUserAsync(email, userName, password, role, etudiant);
        await factory.SaveChangesAsync();
        return userCree;
    }

    private async Task CheckBusinessRules(string userName, string password, string role, Etudiant? etudiant)
    {
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentOutOfRangeException.ThrowIfEqual(
            role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable) || role.Equals(Roles.Etudiant), false);
        // On vérifie que l'étudiant existe
        if (etudiant != null)
        {
            var etudiantRepository = factory.EtudiantRepository();
            ArgumentNullException.ThrowIfNull(etudiantRepository);
            var existe =
                await etudiantRepository.FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud));
            if (existe is { Count: 0 }) throw new EtudiantNotFoundException(etudiant.NumEtud + " - non trouvé");
        }
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}