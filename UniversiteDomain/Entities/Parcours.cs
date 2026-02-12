namespace UniversiteDomain.Entities;

public class Parcours
{
    public long ParcoursId { get; set; }
    public string NomParcours { get; set; } = string.Empty;

    public int AnneeFormation { get; set; } = 1;

    public List<Etudiant>? Inscrits { get; init; } = [];

    public List<Ue> UEsEnseignees { get; init; } = [];

    public override string ToString() => $"ID {ParcoursId} : {NomParcours} - Année {AnneeFormation}";
}