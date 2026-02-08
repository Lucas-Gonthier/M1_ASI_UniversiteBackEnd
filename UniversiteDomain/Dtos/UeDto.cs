using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class UeDto
{
    public long Id { get; init; }
    public string NumeroUe { get; init; } = string.Empty;
    public string Intitule { get; init; } = string.Empty;

    public static UeDto ToDto(Ue ue)
    {
        return new UeDto
        {
            Id = ue.UeId,
            NumeroUe = ue.NumeroUe,
            Intitule = ue.Intitule
        };
    }

    public Ue ToEntity()
    {
        return new Ue { UeId = Id, NumeroUe = NumeroUe, Intitule = Intitule };
    }
}