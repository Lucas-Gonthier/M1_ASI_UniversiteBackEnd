namespace UniversiteDomain.Entities;

public class Note
{
    public float Valeur { get; set; } = 0.0f;
    
    public override string ToString()
    {
        return $"Note : {Valeur}";
    }
}