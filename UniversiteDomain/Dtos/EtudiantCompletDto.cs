using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class EtudiantCompletDto
{
    public long Id { get; init; }
    public string NumEtud { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string Prenom { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public ParcoursDto ParcoursSuivi { get; set; } = new();
    public List<NoteAvecUeDto> NotesObtenues { get; init; } = [];

    public static EtudiantCompletDto ToDto(Etudiant etudiant)
    {
        return new EtudiantCompletDto
        {
            Id = etudiant.EtudiantId,
            NumEtud = etudiant.NumEtud,
            Nom = etudiant.Nom,
            Prenom = etudiant.Prenom,
            Email = etudiant.Email,
            ParcoursSuivi = etudiant.ParcoursSuivi != null
                ? ParcoursDto.ToDto(etudiant.ParcoursSuivi)
                : new ParcoursDto(),
            NotesObtenues = NoteAvecUeDto.ToDtos(etudiant.Notes.ToList())
        };
    }

    public Etudiant ToEntity()
    {
        var notes = NoteAvecUeDto.ToEntities(NotesObtenues);
        return new Etudiant
            { EtudiantId = Id, NumEtud = NumEtud, Nom = Nom, Prenom = Prenom, Email = Email, Notes = notes };
    }
}