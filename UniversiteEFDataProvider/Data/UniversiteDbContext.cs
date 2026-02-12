using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Data;

public class UniversiteDbContext : IdentityDbContext<UniversiteUser, UniversiteRole, string>
{
    private static readonly ILoggerFactory ConsoleLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }

    public UniversiteDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(ConsoleLogger)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Etudiant>(e =>
        {
            e.HasKey(x => x.EtudiantId);

            e.HasOne(x => x.ParcoursSuivi)
                .WithMany(p => p.Inscrits)
                .HasForeignKey(x => x.ParcoursId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasMany(x => x.Notes)
                .WithOne(n => n.Etudiant)
                .HasForeignKey(n => n.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Parcours>(p =>
        {
            p.HasKey(x => x.ParcoursId);

            p.HasMany(x => x.UEsEnseignees)
                .WithMany(u => u.EnseigneeDans);
        });

        modelBuilder.Entity<Ue>(u =>
        {
            u.HasKey(x => x.UeId);

            u.HasMany(x => x.NotesDesEtudiants)
                .WithOne(n => n.Ue)
                .HasForeignKey(n => n.UeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Note>(n =>
        {
            n.HasKey(x => new { x.EtudiantId, x.UeId });

            n.HasOne(x => x.Etudiant)
                .WithMany(e => e.Notes)
                .HasForeignKey(x => x.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            n.HasOne(x => x.Ue)
                .WithMany(u => u.NotesDesEtudiants)
                .HasForeignKey(x => x.UeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Propriétés de la table UniversiteUser
        //OneToOne vers UniversityUser
        modelBuilder.Entity<UniversiteUser>()
            .HasOne<Etudiant>(user => user.Etudiant)
            .WithOne()
            .HasForeignKey<Etudiant>();
        modelBuilder.Entity<Etudiant>()
            .HasOne<UniversiteUser>()
            .WithOne(user => user.Etudiant)
            .HasForeignKey<UniversiteUser>(user => user.EtudiantId);
        // Permet d'inclure automatiquement l'étudiant dans le user sans avoir besoin de préciser la jointure
        modelBuilder.Entity<UniversiteUser>().Navigation<Etudiant>(user => user.Etudiant).AutoInclude();
        modelBuilder.Entity<UniversiteRole>();
    }

    public DbSet<Parcours>? Parcours { get; set; }
    public DbSet<Etudiant>? Etudiants { get; set; }
    public DbSet<Ue>? Ues { get; set; }
    public DbSet<Note>? Notes { get; set; }
}