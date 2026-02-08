using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(
    UniversiteDbContext context,
    RoleManager<UniversiteRole>? roleManager = null,
    UserManager<UniversiteUser>? userManager = null) : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private IUniversiteRoleRepository? _universiteRoles;
    private IUniversiteUserRepository? _universiteUsers;

    public IParcoursRepository ParcoursRepository()
    {
        return _parcours ??= new ParcoursRepository(context ?? throw new InvalidOperationException());
    }

    public IEtudiantRepository EtudiantRepository()
    {
        return _etudiants ??= new EtudiantRepository(context ?? throw new InvalidOperationException());
    }

    public IUeRepository UeRepository()
    {
        return _ues ??= new UeRepository(context ?? throw new InvalidOperationException());
    }

    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        if (roleManager == null)
            throw new InvalidOperationException(
                "RoleManager n'a pas été injecté dans RepositoryFactory. Impossible d'instancier UniversiteRoleRepository.");

        return _universiteRoles ??= new UniversiteRoleRepository(context, roleManager);
    }

    public IUniversiteUserRepository UniversiteUserRepository()
    {
        if (userManager == null || roleManager == null)
            throw new InvalidOperationException(
                "UserManager et/ou RoleManager n'ont pas été injectés dans RepositoryFactory. Impossible d'instancier UniversiteUserRepository.");

        return _universiteUsers ??= new UniversiteUserRepository(context, userManager, roleManager);
    }

    public INoteRepository NoteRepository()
    {
        return _notes ??= new NoteRepository(context ?? throw new InvalidOperationException());
    }

    public Task SaveChangesAsync()
    {
        context.SaveChangesAsync().Wait();
        return Task.CompletedTask;
    }

    public Task EnsureCreatedAsync()
    {
        context.Database.EnsureCreated();
        return Task.CompletedTask;
    }

    public Task EnsureDeletedAsync()
    {
        context.Database.EnsureDeleted();
        return Task.CompletedTask;
    }
}