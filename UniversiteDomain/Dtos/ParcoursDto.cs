using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class ParcoursDto
{
    public long Id { get; init; }
    public string NomParcours { get; init; } = string.Empty;
    public int AnneeFormation { get; init; }

    public static ParcoursDto ToDto(Parcours parcours)
    {
        return new ParcoursDto
        {
            Id = parcours.ParcoursId,
            NomParcours = parcours.NomParcours,
            AnneeFormation = parcours.AnneeFormation
        };
    }

    public Parcours ToEntity()
    {
        return new Parcours { ParcoursId = Id, NomParcours = NomParcours, AnneeFormation = AnneeFormation };
    }
}