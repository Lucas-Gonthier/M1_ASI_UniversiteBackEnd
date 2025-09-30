namespace UniversiteDomain.Entities;

public class UE
{
    public long Id { get; set; }
    public string NumeroUE { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    
    public List<Parcours> EnseigneeDans { get; set; } = [];
    
    public override string ToString()
    {
        return $"ID {Id} : {NumeroUE} - {Intitule}";
    }
}