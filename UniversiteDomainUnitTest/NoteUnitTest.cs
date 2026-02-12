using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Csv;

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

        var ue = new Ue
        {
            UeId = 1,
            Intitule = "Unité d'enseignement 1",
            NumeroUe = "UE1"
        };

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

    [Test]
    public async Task ImportCsvNotesUeUseCase_ValidCsv()
    {
        // Arrange
        var ue = new Ue { UeId = 1, NumeroUe = "UE1", Intitule = "Mathématiques" };

        var etudiant1 = new Etudiant
        {
            EtudiantId = 1, NumEtud = "et1", Nom = "Durant", Prenom = "Jean",
            Email = "jean.durant@etud.fr"
        };
        var etudiant2 = new Etudiant
        {
            EtudiantId = 2, NumEtud = "et2", Nom = "Martin", Prenom = "Marie",
            Email = "marie.martin@etud.fr"
        };

        // CSV avec 2 lignes : et1 a une note (15), et2 pas de note
        const string csvContent = "NumEtud,Nom,Prenom,NumeroUe,IntituleUe,Note\n" +
                                  "et1,Durant,Jean,UE1,Mathématiques,15\n" +
                                  "et2,Martin,Marie,UE1,Mathématiques,\n";

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync([ue]);

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindAllAsync())
            .ReturnsAsync([etudiant1, etudiant2]);

        var mockNoteRepo = new Mock<INoteRepository>();
        mockNoteRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync([]); // pas de notes existantes
        mockNoteRepo
            .Setup(r => r.CreateAsync(It.IsAny<Note>()))
            .ReturnsAsync((Note n) => n);

        var mockRepoFactory = new Mock<IRepositoryFactory>();
        mockRepoFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockRepoFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockRepoFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        // Act
        var useCase = new ImportCsvNotesUeUseCase(mockRepoFactory.Object);
        var resultat = await useCase.ExecuteAsync(1, stream);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultat.NotesCreees, Is.EqualTo(1));
            Assert.That(resultat.NotesMisesAJour, Is.EqualTo(0));
            Assert.That(resultat.NotesIgnorees, Is.EqualTo(1));
        });
    }
}