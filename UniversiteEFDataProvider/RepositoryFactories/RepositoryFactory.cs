using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(UniversiteDbContext context) : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;

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