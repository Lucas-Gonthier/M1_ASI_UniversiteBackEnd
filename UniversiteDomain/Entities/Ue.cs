namespace UniversiteDomain.Entities;

public class Ue
{
    public long UeId { get; set; }
    public string NumeroUe { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;

    public ICollection<Parcours> EnseigneeDans { get; set; } = new List<Parcours>();
    public ICollection<Note> NotesDesEtudiants { get; set; } = new List<Note>();

    public override string ToString() => $"ID {UeId} : {NumeroUe} - {Intitule}";
}