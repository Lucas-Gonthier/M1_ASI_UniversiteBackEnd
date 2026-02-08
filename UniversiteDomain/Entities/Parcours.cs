namespace UniversiteDomain.Entities;

public class Parcours
{
    public long ParcoursId { get; init; }
    public string NomParcours { get; init; } = string.Empty;

    public int AnneeFormation { get; init; } = 1;

    public List<Etudiant>? Inscrits { get; init; } = [];

    public List<Ue> UEsEnseignees { get; init; } = [];

    public override string ToString() => $"ID {ParcoursId} : {NomParcours} - Année {AnneeFormation}";
}