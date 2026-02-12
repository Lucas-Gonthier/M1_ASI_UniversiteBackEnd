using CsvHelper.Configuration;

namespace UniversiteDomain.Dtos;

public class NoteCsvLigneDto
{
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NumeroUe { get; set; } = string.Empty;
    public string IntituleUe { get; set; } = string.Empty;
    public float? Note { get; set; }
}

public sealed class NoteCsvLigneDtoMap : ClassMap<NoteCsvLigneDto>
{
    public NoteCsvLigneDtoMap()
    {
        Map(m => m.NumEtud).Name("NumEtud");
        Map(m => m.Nom).Name("Nom");
        Map(m => m.Prenom).Name("Prenom");
        Map(m => m.NumeroUe).Name("NumeroUe");
        Map(m => m.IntituleUe).Name("IntituleUe");
        Map(m => m.Note).Name("Note").Optional();
    }
}
