using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public abstract class Repository<T>(UniversiteDbContext context) : IRepository<T>
    where T : class
{
    public async Task<T> CreateAsync(T entity)
    {
        var res = context.Add(entity);
        await context.SaveChangesAsync();
        return res.Entity;
    }

    public async Task UpdateAsync(T entity)
    {
        var res = context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await FindAsync(id);

        if (entity != null)
        {
            try
            {
                context.Remove(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public async Task DeleteAsync(T entity)
    {
        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    // Clé primaire non composée
    public async Task<T?> FindAsync(long id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    // Clé primaire composée
    public async Task<T?> FindAsync(params object[] keyValues)
    {
        return await context.Set<T>().FindAsync(keyValues);
    }

    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> condition)
    {
        return await context.Set<T>().Where(condition).ToListAsync();
    }

    public async Task<List<T>> FindAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}