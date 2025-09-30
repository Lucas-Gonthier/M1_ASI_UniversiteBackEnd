using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTest;

public class ParcoursUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idParcours = 1;
        String nomParcours = "Ue 1";
        int anneFormation = 2;

        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursAvant = new Parcours { NomParcours = nomParcours, AnneeFormation = anneFormation };

        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockParcours = new Mock<IParcoursRepository>();

        // Il faut ensuite aller dans le use case pour simuler les appels des fonctions vers la datasource
        // Nous devons simuler FindByCondition et Create
        // On dit à ce mock que le parcours n'existe pas déjà
        mockParcours
            .Setup(repo => repo.FindByConditionAsync(p => p.Id.Equals(idParcours)))
            .ReturnsAsync((List<Parcours>)null);
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        var parcoursFinal = new Parcours { Id = idParcours, NomParcours = nomParcours, AnneeFormation = anneFormation };
        mockParcours.Setup(repo => repo.CreateAsync(parcoursAvant)).ReturnsAsync(parcoursFinal);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);

        // Création du use case en utilisant le mock comme datasource
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(mockFactory.Object);

        // Appel du use case
        var parcoursTeste = await useCase.ExecuteAsync(parcoursAvant);

        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursFinal.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursFinal.AnneeFormation));
    }

    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant = new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours { Id = 3, NomParcours = "Ue 3", AnneeFormation = 1 };

        // On initialise des faux repositories
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants =
        [
            new() { Id = 1 }
        ];
        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);

        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = parcours;
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);

        mockParcours
            .Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);

        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);

        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcoursUseCase useCase = new AddEtudiantDansParcoursUseCase(mockFactory.Object);

        // Appel du use case
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }

    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        const long idParcours = 3;
        const long idUe = 1;

        // Parcours sans UE au départ
        var parcoursInitial = new Parcours
        {
            Id = idParcours,
            NomParcours = "Ue 3",
            AnneeFormation = 1,
            UEsEnseignees = []
        };

        var ue = new UE
        {
            Id = idUe,
            Intitule = "UE1",
            NumeroUE = "UE001"
        };

        // Parcours final (après ajout)
        var parcoursFinal = new Parcours
        {
            Id = idParcours,
            NomParcours = parcoursInitial.NomParcours,
            AnneeFormation = parcoursInitial.AnneeFormation,
            UEsEnseignees = [ue]
        };

        // On initialise les faux repositories
        var mockParcours = new Mock<IParcoursRepository>();
        var mockUe = new Mock<IUERepository>();

        // Le parcours existe mais n’a pas encore l’UE
        mockParcours
            .Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idParcours)))
            .ReturnsAsync([parcoursInitial]);
        
        // L’UE existe
        mockUe
            .Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idUe)))
            .ReturnsAsync([ue]);

        // Ajout de l’UE dans le parcours
        mockParcours
            .Setup(repo => repo.AddUEAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinal);

        // Factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(facto => facto.UERepository()).Returns(mockUe.Object);

        // Use case
        var useCase = new AddUEDansParcoursUseCase(mockFactory.Object);

        // Act
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idUe);

        // Assert
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.UEsEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UEsEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UEsEnseignees[0].Id, Is.EqualTo(idUe));
        Assert.That(parcoursTest.UEsEnseignees[0].Intitule, Is.EqualTo("UE1"));
    }
}