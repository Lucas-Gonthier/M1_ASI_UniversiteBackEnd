namespace UniversiteDomain.Entities;

public class Note
{
    public long EtudiantId { get; init; }
    public long UeId { get; init; }

    public float Valeur { get; set; }

    public Etudiant Etudiant { get; init; } = null!;
    public Ue Ue { get; init; } = null!;

    public override string ToString() => $"Note : {Valeur}";
}