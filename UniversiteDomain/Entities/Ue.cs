namespace UniversiteDomain.Entities;

public class Ue
{
    public long UeId { get; set; }
    public string NumeroUe { get; init; } = string.Empty;
    public string Intitule { get; init; } = string.Empty;

    public ICollection<Parcours> EnseigneeDans { get; init; } = new List<Parcours>();
    public ICollection<Note> NotesDesEtudiants { get; init; } = new List<Note>();

    public override string ToString() => $"ID {UeId} : {NumeroUe} - {Intitule}";
}