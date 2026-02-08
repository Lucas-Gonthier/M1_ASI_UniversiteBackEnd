using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomain.JeuxDeDonnees;

public class BasicBdBuilder(IRepositoryFactory repositoryFactory) : BdBuilder(repositoryFactory)
{
    private const string Password = "Miage2025#";

    private readonly Etudiant[] _etudiants =
    [
        new()
        {
            EtudiantId = 1, NumEtud = "03BDKZ65", Nom = "Dupont", Prenom = "Antoine",
            Email = "antoine.dupont@etud.u-picardie.fr"
        },
        new()
        {
            EtudiantId = 2, NumEtud = "JEIZ03JZ", Nom = "Ntamak", Prenom = "Romain",
            Email = "roman.ntamak@etud.u-picardie.fr"
        },
        new()
        {
            EtudiantId = 3, NumEtud = "62830483", Nom = "Barassi", Prenom = "Pierre-Louis",
            Email = "pierre-louis.barassi@etud.u-picardie.fr"
        },
        new()
        {
            EtudiantId = 4, NumEtud = "J6HZK922", Nom = "Jelong", Prenom = "Antony",
            Email = "antony.jelong@etud.u-picardie.fr"
        },
        new()
        {
            EtudiantId = 5, NumEtud = "PAD89345", Nom = "Akki", Prenom = "Pita", Email = "pita.akki@etud.u-picardie.fr"
        },
        new()
        {
            EtudiantId = 6, NumEtud = "RG8647FF", Nom = "Mauvaka", Prenom = "Peato",
            Email = "peato.mauvaka@etud.u-picardie.fr"
        }
    ];

    private struct UserNonEtudiant
    {
        public string UserName;
        public string Email;
        public string Role;
    }

    private readonly UserNonEtudiant[] _usersNonEtudiants =
    [
        new() { UserName = "anne.lapujade@u-picardie.fr", Email = "anne.lapujade@u-picardie.fr", Role = "Responsable" },
        new() { UserName = "plouisberquez@gmail.com", Email = "plouisberquez@gmail.com", Role = "Responsable" },
        new() { UserName = "jabin.julian.univ@gmail.com", Email = "jabin.julian.univ@gmail.com", Role = "Responsable" },
        new() { UserName = "mehdy.chk@outlook.fr", Email = "mehdy.chk@outlook.fr", Role = "Responsable" },
        new()
        {
            UserName = "stephanie.dertin@u-picardie.fr", Email = "stephanie.dertin@u-picardie.fr", Role = "Scolarite"
        }
    ];

    private readonly Parcours[] _parcours =
    [
        new() { ParcoursId = 1, NomParcours = "M1", AnneeFormation = 1 },
        new() { ParcoursId = 2, NomParcours = "OSIE", AnneeFormation = 2 },
        new() { ParcoursId = 3, NomParcours = "ITD", AnneeFormation = 2 },
        new() { ParcoursId = 4, NomParcours = "IDD", AnneeFormation = 2 }
    ];

    private readonly Ue[] _ues =
    [
        new() { UeId = 1, NumeroUe = "ISI_01", Intitule = "Architecture des SI 1" },
        new() { UeId = 2, NumeroUe = "ISI_02", Intitule = "Conduite de projet" },
        new() { UeId = 3, NumeroUe = "GEO_05", Intitule = "Marketing" },
        new() { UeId = 4, NumeroUe = "INFO_18", Intitule = "Architecture des SI 2" }
    ];

    private struct Inscription
    {
        public long EtudiantId;
        public long ParcoursId;
    }

    private readonly Inscription[] _inscriptions =
    [
        new() { EtudiantId = 1, ParcoursId = 2 },
        new() { EtudiantId = 2, ParcoursId = 1 },
        new() { EtudiantId = 3, ParcoursId = 1 },
        new() { EtudiantId = 4, ParcoursId = 1 },
        new() { EtudiantId = 5, ParcoursId = 3 },
        new() { EtudiantId = 6, ParcoursId = 4 }
    ];

    private struct UeDansParcours
    {
        public long UeId;
        public long ParcoursId;
    }

    private readonly UeDansParcours[] _maquette =
    [
        new() { UeId = 1, ParcoursId = 1 },
        new() { UeId = 2, ParcoursId = 1 },
        new() { UeId = 3, ParcoursId = 1 },
        new() { UeId = 4, ParcoursId = 2 },
        new() { UeId = 4, ParcoursId = 3 },
        new() { UeId = 4, ParcoursId = 4 }
    ];

    private struct Note
    {
        public long EtudiantId;
        public long UeId;
        public float Valeur;
    }

    private readonly Note[] _notes =
    [
        new() { UeId = 1, EtudiantId = 2, Valeur = 12 },
        new() { UeId = 1, EtudiantId = 3, Valeur = (float)8.5 },
        new() { UeId = 1, EtudiantId = 4, Valeur = 16 },
        new() { UeId = 2, EtudiantId = 2, Valeur = 14 },
        new() { UeId = 2, EtudiantId = 3, Valeur = 6 },
        new() { UeId = 3, EtudiantId = 4, Valeur = (float)11.5 },
        new() { UeId = 4, EtudiantId = 1, Valeur = 10 },
        new() { UeId = 4, EtudiantId = 5, Valeur = (float)18.3 },
        new() { UeId = 4, EtudiantId = 6, Valeur = 12 }
    ];

    private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;

    protected override async Task RegenererBdAsync()
    {
        // Ici je décide de supprimer et recréer la BD
        await _repositoryFactory.EnsureDeletedAsync();
        await _repositoryFactory.EnsureCreatedAsync();
    }

    protected override async Task BuildEtudiantsAsync()
    {
        foreach (var e in _etudiants)
        {
            await new CreateEtudiantUseCase(_repositoryFactory).ExecuteAsync(e);
        }
    }

    protected override async Task BuildParcoursAsync()
    {
        foreach (var parcours in _parcours)
        {
            await new CreateParcoursUseCase(_repositoryFactory).ExecuteAsync(parcours);
        }
    }

    protected override async Task BuildUesAsync()
    {
        foreach (var ue in _ues)
        {
            await new CreateUeUseCase(_repositoryFactory).ExecuteAsync(ue);
        }
    }

    protected override async Task InscrireEtudiantsAsync()
    {
        foreach (var i in _inscriptions)
        {
            await new AddEtudiantDansParcoursUseCase(_repositoryFactory).ExecuteAsync(i.ParcoursId, i.EtudiantId);
        }
    }

    protected override async Task BuildMaquetteAsync()
    {
        foreach (var u in _maquette)
        {
            await new AddUeDansParcoursUseCase(_repositoryFactory).ExecuteAsync(u.ParcoursId, u.UeId);
        }
    }

    protected override async Task NoterAsync()
    {
        foreach (var note in _notes)
        {
            await new CreateNoteUseCase(_repositoryFactory).ExecuteAsync(note.EtudiantId, note.UeId, note.Valeur);
        }
    }

    protected override async Task BuildRolesAsync()
    {
        // Création des rôles dans la table aspnetroles
        await new CreateUniversiteRoleUseCase(_repositoryFactory).ExecuteAsync(Roles.Responsable);
        await new CreateUniversiteRoleUseCase(_repositoryFactory).ExecuteAsync(Roles.Scolarite);
        await new CreateUniversiteRoleUseCase(_repositoryFactory).ExecuteAsync(Roles.Etudiant);
    }

    protected override async Task BuildUsersAsync()
    {
        var uc = new CreateUniversiteUserUseCase(_repositoryFactory);
        // Création des étudiants
        foreach (var etudiant in _etudiants)
        {
            await uc.ExecuteAsync(etudiant.Email, etudiant.Email, Password, Roles.Etudiant, etudiant);
        }

        // Création des responsbles
        foreach (var user in _usersNonEtudiants)
        {
            await uc.ExecuteAsync(user.Email, user.Email, Password, user.Role, null);
        }
    }
}