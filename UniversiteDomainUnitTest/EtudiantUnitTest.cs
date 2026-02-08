using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

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
}