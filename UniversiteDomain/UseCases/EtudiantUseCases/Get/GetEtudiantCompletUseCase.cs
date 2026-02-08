using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantCompletUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant?> ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules();
        var etudiant = await factory.EtudiantRepository().FindEtudiantCompletAsync(idEtudiant);
        return etudiant;
    }

    private Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        var etudiantRepository = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        return Task.CompletedTask;
    }

    public static bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que ses notes
        return user.Etudiant != null && role.Equals(Roles.Etudiant) && user.Etudiant.EtudiantId == idEtudiant;
    }
}