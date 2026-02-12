using CsvHelper.Configuration;

namespace UniversiteDomain.Dtos;

public class NoteCsvLigneDto
{
    public string NumEtud { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string Prenom { get; init; } = string.Empty;
    public string NumeroUe { get; init; } = string.Empty;
    public string IntituleUe { get; init; } = string.Empty;
    public float? Note { get; init; }
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