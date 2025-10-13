namespace UniversiteDomain.Entities;

public class Ue(long ueId, string NumeroUe, string Intitule)
{
    public long UeId { get; init; } = ueId;
    public string NumeroUe { get; init; } = string.Empty;
    public string Intitule { get; init; } = string.Empty;

    public List<Parcours> EnseigneeDans { get; set; } = [];

    public List<Note> NotesDesEtudiants { get; set; } = [];

    public override string ToString()
    {
        return $"ID {UeId} : {NumeroUe} - {Intitule}";
    }
}