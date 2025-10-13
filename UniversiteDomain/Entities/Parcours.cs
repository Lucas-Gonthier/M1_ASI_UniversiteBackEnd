namespace UniversiteDomain.Entities;

public class Parcours
{
    public long ParcoursId { get; set; }
    public string NomParcours { get; set; } = String.Empty;

    public int AnneeFormation { get; set; } = 1;

    // OneToMany : un parcours contient plusieurs étudiants
    // Remarque : pour éviter quelques NullPointerException disgracieux, j'ai choisi de créer une liste d'incrits vide quand aucun étudiant n'est inscrit dans un parcours plutôt que de l'initialiser à null
    public List<Etudiant>? Inscrits { get; set; } = new();

    public List<Ue> UEsEnseignees { get; set; } = [];

    public override string ToString()
    {
        return $"ID {ParcoursId} : {NomParcours} - Année {AnneeFormation}";
    }
}