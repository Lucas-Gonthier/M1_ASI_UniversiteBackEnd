using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Get;

public class IsInRoleUseCase(IRepositoryFactory factory)
{
    public async Task<bool> ExecuteAsync(string email, string role)
    {
        await CheckBusinessRules(email);
        return await factory.UniversiteUserRepository().IsInRoleAsync(email, role);
    }

    private Task CheckBusinessRules(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UniversiteUserRepository());
        return Task.CompletedTask;
    }
}