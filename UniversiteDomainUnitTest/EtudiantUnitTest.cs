using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Update;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;

namespace UniversiteDomainUnitTest;

public class EtudiantUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateEtudiantUseCase()
    {
        // Arrange
        const long id = 1;
        const string numEtud = "et1";
        const string nom = "Durant";
        const string prenom = "Jean";
        const string email = "jean.durant@etud.u-picardie.fr";

        var etudiantSansId = new Etudiant
        {
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email
        };

        var etudiantCree = new Etudiant
        {
            EtudiantId = id,
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email
        };

        // Mock du repository étudiant
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();

        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant>()); // aucun étudiant trouvé

        mockEtudiantRepo
            .Setup(r => r.CreateAsync(etudiantSansId))
            .ReturnsAsync(etudiantCree);

        var mockRepoFactory = new Mock<IRepositoryFactory>();
        mockRepoFactory
            .Setup(f => f.EtudiantRepository())
            .Returns(mockEtudiantRepo.Object);

        var useCase = new CreateEtudiantUseCase(mockRepoFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(etudiantSansId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.EtudiantId, Is.EqualTo(etudiantCree.EtudiantId));
            Assert.That(result.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
            Assert.That(result.Nom, Is.EqualTo(etudiantCree.Nom));
            Assert.That(result.Prenom, Is.EqualTo(etudiantCree.Prenom));
            Assert.That(result.Email, Is.EqualTo(etudiantCree.Email));
        });
    }

    [Test]
    public async Task UpdateEtudiantUseCase()
    {
        // Arrange
        var etudiantExistant = new Etudiant
        {
            EtudiantId = 1,
            NumEtud = "et1",
            Nom = "Durant",
            Prenom = "Jean",
            Email = "jean.durant@etud.u-picardie.fr"
        };

        var etudiantModifie = new Etudiant
        {
            EtudiantId = 1,
            NumEtud = "et1",
            Nom = "Durant-Modifié",
            Prenom = "Jean-Pierre",
            Email = "jp.durant@etud.u-picardie.fr"
        };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();

        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync([etudiantExistant]);

        var mockRepoFactory = new Mock<IRepositoryFactory>();
        mockRepoFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);

        var useCase = new UpdateEtudiantUseCase(mockRepoFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(etudiantModifie);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.EtudiantId, Is.EqualTo(1));
            Assert.That(result.Nom, Is.EqualTo("Durant-Modifié"));
            Assert.That(result.Prenom, Is.EqualTo("Jean-Pierre"));
            Assert.That(result.Email, Is.EqualTo("jp.durant@etud.u-picardie.fr"));
        });
    }

    [Test]
    public Task DeleteEtudiantUseCase()
    {
        // Arrange
        const long etudiantId = 1;

        var etudiantExistant = new Etudiant
        {
            EtudiantId = etudiantId,
            NumEtud = "et1",
            Nom = "Durant",
            Prenom = "Jean",
            Email = "jean.durant@etud.u-picardie.fr"
        };

        var mockUserEntity = new Mock<IUniversiteUser>();

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync([etudiantExistant]);
        mockEtudiantRepo
            .Setup(r => r.DeleteAsync(etudiantId))
            .Returns(Task.CompletedTask);

        var mockUserRepo = new Mock<IUniversiteUserRepository>();
        mockUserRepo
            .Setup(r => r.FindByEmailAsync(etudiantExistant.Email))
            .ReturnsAsync(mockUserEntity.Object);
        mockUserRepo
            .Setup(r => r.DeleteAsync(mockUserEntity.Object))
            .Returns(Task.CompletedTask);

        var mockRepoFactory = new Mock<IRepositoryFactory>();
        mockRepoFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockRepoFactory.Setup(f => f.UniversiteUserRepository()).Returns(mockUserRepo.Object);

        var useCase = new DeleteEtudiantUseCase(mockRepoFactory.Object);

        // Act & Assert — ne doit pas lever d'exception
        Assert.DoesNotThrowAsync(async () => await useCase.ExecuteAsync(etudiantId));

        // Vérifier que les méthodes ont été appelées
        mockUserRepo.Verify(r => r.FindByEmailAsync(etudiantExistant.Email), Times.Once);
        mockUserRepo.Verify(r => r.DeleteAsync(mockUserEntity.Object), Times.Once);
        mockEtudiantRepo.Verify(r => r.DeleteAsync(etudiantId), Times.Once);
        return Task.CompletedTask;
    }
}