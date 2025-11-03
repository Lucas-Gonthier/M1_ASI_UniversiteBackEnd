namespace UniversiteDomain.Entities;

public class Note
{
    public long EtudiantId { get; set; }
    public long UeId { get; set; }

    // La valeur de la note
    public float Valeur { get; set; }

    // Navigations
    public Etudiant Etudiant { get; set; } = null!;
    public Ue Ue { get; set; } = null!;

    public override string ToString() => $"Note : {Valeur}";
}