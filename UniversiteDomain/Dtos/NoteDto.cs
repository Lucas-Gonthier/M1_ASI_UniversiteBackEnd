using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteDto
{
    public long EtudiantId { get; init; }
    public long UeId { get; init; }
    public float Valeur { get; init; }

    public static NoteDto ToDto(Note note)
    {
        return new NoteDto
        {
            EtudiantId = note.EtudiantId,
            UeId = note.UeId,
            Valeur = note.Valeur
        };
    }

    public Note ToEntity()
    {
        return new Note { EtudiantId = EtudiantId, UeId = UeId, Valeur = Valeur };
    }
}
