using System.Linq.Expressions;
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
        // Arrange
        const long idParcours = 1;
        const string nomParcours = "UE 1";
        const int anneeFormation = 2;

        var parcoursAvant = new Parcours
        {
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        var parcoursFinal = new Parcours
        {
            ParcoursId = idParcours,
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        var mockParcoursRepo = new Mock<IParcoursRepository>();

        mockParcoursRepo
            .Setup(r => r.FindByConditionAsync(p => p.ParcoursId == idParcours))
            .ReturnsAsync([]);

        mockParcoursRepo
            .Setup(r => r.CreateAsync(parcoursAvant))
            .ReturnsAsync(parcoursFinal);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);

        var useCase = new CreateParcoursUseCase(mockFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(parcoursAvant);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ParcoursId, Is.EqualTo(parcoursFinal.ParcoursId));
            Assert.That(result.NomParcours, Is.EqualTo(parcoursFinal.NomParcours));
            Assert.That(result.AnneeFormation, Is.EqualTo(parcoursFinal.AnneeFormation));
        });
    }

    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        // Arrange
        const long idEtudiant = 1;
        const long idParcours = 3;

        var etudiant = new Etudiant
        {
            EtudiantId = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "1"
        };

        var parcours = new Parcours
        {
            ParcoursId = idParcours,
            NomParcours = "UE 1",
            AnneeFormation = 1
        };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockParcoursRepo = new Mock<IParcoursRepository>();

        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(e => e.EtudiantId == idEtudiant))
            .ReturnsAsync([etudiant]);

        parcours.Inscrits!.Add(etudiant);

        mockParcoursRepo
            .Setup(r => r.FindByConditionAsync(p => p.ParcoursId == idParcours))
            .ReturnsAsync([parcours]);

        mockParcoursRepo
            .Setup(r => r.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcours);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);

        var useCase = new AddEtudiantDansParcoursUseCase(mockFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(idParcours, idEtudiant);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ParcoursId, Is.EqualTo(parcours.ParcoursId));
            Assert.That(result.Inscrits, Is.Not.Null);
            Assert.That(result.Inscrits, Has.Count.EqualTo(1));
            Assert.That(result.Inscrits![0].EtudiantId, Is.EqualTo(idEtudiant));
        });
    }

    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        // Arrange
        const long idParcours = 3;
        const long idUe = 1;

        var parcoursInitial = new Parcours
        {
            ParcoursId = idParcours,
            NomParcours = "Développement",
            AnneeFormation = 1,
            UEsEnseignees = []
        };

        var ue = new Ue
        {
            UeId = idUe,
            Intitule = "Unité d'enseignement 1",
            NumeroUe = "UE1"
        };

        var parcoursFinal = new Parcours
        {
            ParcoursId = idParcours,
            NomParcours = parcoursInitial.NomParcours,
            AnneeFormation = parcoursInitial.AnneeFormation,
            UEsEnseignees = [ue]
        };

        var mockParcoursRepo = new Mock<IParcoursRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync([ue]);

        mockParcoursRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync([parcoursInitial]);

        mockParcoursRepo
            .Setup(r => r.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinal);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(idParcours, idUe);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ParcoursId, Is.EqualTo(parcoursFinal.ParcoursId));
            Assert.That(result.UEsEnseignees, Is.Not.Null);
            Assert.That(result.UEsEnseignees, Has.Count.EqualTo(1));
            Assert.That(result.UEsEnseignees[0].UeId, Is.EqualTo(idUe));
            Assert.That(result.UEsEnseignees[0].Intitule, Is.EqualTo("UE1"));
        });
    }
}