using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Get;

public class FindUniversiteUserByEmailUseCase(IRepositoryFactory factory)
{
    public async Task<IUniversiteUser?> ExecuteAsync(string email)
    {
        await CheckBusinessRules(email);
        var user = await factory.UniversiteUserRepository().FindByEmailAsync(email);
        return user;
    }

    private Task CheckBusinessRules(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UniversiteUserRepository());
        return Task.CompletedTask;
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite) || role.Equals(Roles.Etudiant);
    }
}