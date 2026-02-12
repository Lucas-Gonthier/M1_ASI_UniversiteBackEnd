using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Update;
using UniversiteDomain.UseCases.UeUseCases.Delete;

namespace UniversiteDomainUnitTest;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase()
    {
        // Arrange
        var ueSansId = new Ue
        {
            NumeroUe = "INFO_01",
            Intitule = "Programmation avancée"
        };

        var ueCree = new Ue
        {
            UeId = 1,
            NumeroUe = "INFO_01",
            Intitule = "Programmation avancée"
        };

        var mockUeRepo = new Mock<IUeRepository>();

        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>()); // pas de doublon

        mockUeRepo
            .Setup(r => r.CreateAsync(ueSansId))
            .ReturnsAsync(ueCree);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new CreateUeUseCase(mockFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(ueSansId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.UeId, Is.EqualTo(ueCree.UeId));
            Assert.That(result.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
            Assert.That(result.Intitule, Is.EqualTo(ueCree.Intitule));
        });
    }

    [Test]
    public async Task UpdateUeUseCase()
    {
        // Arrange
        var ueExistante = new Ue
        {
            UeId = 1,
            NumeroUe = "INFO_01",
            Intitule = "Programmation avancée"
        };

        var ueModifiee = new Ue
        {
            UeId = 1,
            NumeroUe = "INFO_01_MOD",
            Intitule = "Programmation avancée - Modifié"
        };

        var mockUeRepo = new Mock<IUeRepository>();

        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync([ueExistante]);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new UpdateUeUseCase(mockFactory.Object);

        // Act
        var result = await useCase.ExecuteAsync(ueModifiee);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.UeId, Is.EqualTo(1));
            Assert.That(result.NumeroUe, Is.EqualTo("INFO_01_MOD"));
            Assert.That(result.Intitule, Is.EqualTo("Programmation avancée - Modifié"));
        });
    }

    [Test]
    public Task DeleteUeUseCase()
    {
        // Arrange
        const long ueId = 1;

        var ueExistante = new Ue
        {
            UeId = ueId,
            NumeroUe = "INFO_01",
            Intitule = "Programmation avancée"
        };

        var mockUeRepo = new Mock<IUeRepository>();

        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync([ueExistante]);

        mockUeRepo
            .Setup(r => r.DeleteAsync(ueId))
            .Returns(Task.CompletedTask);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new DeleteUeUseCase(mockFactory.Object);

        // Act & Assert — ne doit pas lever d'exception
        Assert.DoesNotThrowAsync(async () => await useCase.ExecuteAsync(ueId));

        // Vérifier que DeleteAsync a été appelé
        mockUeRepo.Verify(r => r.DeleteAsync(ueId), Times.Once);
        return Task.CompletedTask;
    }
}
