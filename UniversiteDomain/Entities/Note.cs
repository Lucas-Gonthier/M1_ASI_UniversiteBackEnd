namespace UniversiteDomain.Entities;

public class Note(float valeur)
{
    public float Valeur { get; } = valeur;

    public Ue? Ue { get; set; }

    public Etudiant? Etudiant { get; set; }

    public override string ToString()
    {
        return $"Note : {Valeur}";
    }
}