using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Create;

namespace UniversiteDomainUnitTest;

public class NoteUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateNoteUseCase()
    {
        // Arrange
        const long idParcours = 1;
        const string nomParcours = "Parcours 1";
        const int anneeFormation = 1;
        
        const long id = 1;
        const string numEtud = "et1";
        const string nom = "Durant";
        const string prenom = "Jean";
        const string email = "jean.durant@etud.u-picardie.fr";

        var ue = new Ue(1, "Unité d'enseignement 1", "UE1");
        
        var parcours = new Parcours
        {
            ParcoursId = idParcours,
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation,
            UEsEnseignees = [ue]
        };
        
        var etudiant = new Etudiant
        {
            EtudiantId = id,
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email,
            ParcoursSuivi = parcours
        };
        
        var mockNoteRepo = new Mock<INoteRepository>();
        
        mockNoteRepo
            .Setup(r => r.CreateAsync(It.IsAny<Note>()))
            .ReturnsAsync((Note n) => n); // retourne la note passée en paramètre
        
        var mockRepositoryFactory = new Mock<IRepositoryFactory>();
        mockRepositoryFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        // Act
        var note = await new CreateNoteUseCase(mockRepositoryFactory.Object).ExecuteAsync(etudiant, ue, 15.5f);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(note, Is.Not.Null);
            Assert.That(note.Valeur, Is.EqualTo(15.5f));
        });
    }
}