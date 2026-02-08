namespace UniversiteDomain.Entities;

public class Etudiant
{
    public long EtudiantId { get; set; }
    public string NumEtud { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string Prenom { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public long? ParcoursId { get; init; }
    public Parcours? ParcoursSuivi { get; set; }

    public ICollection<Note> Notes { get; init; } = new List<Note>();

    public override string ToString()
        => $"ID {EtudiantId} : {NumEtud} - {Nom} {Prenom} inscrit en " + ParcoursSuivi;
}