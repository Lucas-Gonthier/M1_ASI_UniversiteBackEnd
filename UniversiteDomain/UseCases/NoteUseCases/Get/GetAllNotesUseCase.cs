using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetAllNotesUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<Note>> ExecuteAsync()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        return await repositoryFactory.NoteRepository().FindAllAsync();
    }

    public static bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
