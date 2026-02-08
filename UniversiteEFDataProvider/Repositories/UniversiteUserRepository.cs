using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UniversiteUserRepository(
    UniversiteDbContext context,
    UserManager<UniversiteUser> userManager,
    RoleManager<UniversiteRole> roleManager) : Repository<IUniversiteUser>(context), IUniversiteUserRepository
{
    private readonly UniversiteDbContext _context = context;

    public async Task<IUniversiteUser?> AddUserAsync(string login, string email, string password, string role,
        Etudiant? etudiant)
    {
        var user = new UniversiteUser { UserName = login, Email = email, Etudiant = etudiant };
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        await _context.SaveChangesAsync();
        return result.Succeeded ? user : null;
    }

    public async Task<IUniversiteUser> FindByEmailAsync(string email)
    {
        return (await userManager.FindByEmailAsync(email))!;
    }

    public async Task UpdateAsync(IUniversiteUser entity, string userName, string email)
    {
        var user = (UniversiteUser)entity;
        user.UserName = userName;
        user.Email = email;
        await userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
    }

    public Task<List<string>> GetRolesAsync(IUniversiteUser user)
    {
        var u = (UniversiteUser)user;
        return Task.FromResult(userManager.GetRolesAsync(u).Result.ToList());
    }

    public async Task<int> DeleteAsync(long id)
    {
        Etudiant etud = _context.Etudiants.Find(id);
        UniversiteUser user = await userManager.FindByEmailAsync(etud.Email);
        if (user != null)
        {
            await userManager.DeleteAsync(user);
            int res = await _context.SaveChangesAsync();
            return 1;
        }

        return 0;
    }

    public async Task<bool> IsInRoleAsync(string email, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> CheckPasswordAsync(IUniversiteUser user, string password)
    {
        var u = (UniversiteUser)user;
        return await userManager.CheckPasswordAsync(u, password);
    }
}