namespace UniversiteDomain.Entities;

public class Etudiant
{
    public long EtudiantId { get; set; }
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // FK optionnelle vers Parcours
    public long? ParcoursId { get; set; }
    public Parcours? ParcoursSuivi { get; set; }

    public ICollection<Note> Notes { get; set; } = new List<Note>();

    public override string ToString()
        => $"ID {EtudiantId} : {NumEtud} - {Nom} {Prenom} inscrit en " + ParcoursSuivi;
}