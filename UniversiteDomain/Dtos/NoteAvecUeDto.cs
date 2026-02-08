using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteAvecUeDto
{
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    public UeDto UeDto { get; set; } = new();
    public float Valeur { get; set; }

    public NoteAvecUeDto ToDto(Note note)
    {
        EtudiantId = note.EtudiantId;
        UeId = note.UeId;
        UeDto = UeDto.ToDto(note.Ue);
        Valeur = note.Valeur;
        return this;
    }

    public Note ToEntity()
    {
        return new Note { EtudiantId = EtudiantId, UeId = UeId, Valeur = Valeur };
    }

    public static List<NoteAvecUeDto> ToDtos(List<Note> notes)
    {
        return notes.Select(note => new NoteAvecUeDto().ToDto(note)).ToList();
    }

    public static List<Note> ToEntities(List<NoteAvecUeDto> noteDtos)
    {
        return noteDtos.Select(noteDto => noteDto.ToEntity()).ToList();
    }
}