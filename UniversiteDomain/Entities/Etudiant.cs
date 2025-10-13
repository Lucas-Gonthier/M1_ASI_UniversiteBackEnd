namespace UniversiteDomain.Entities;

public class Etudiant
{
    public long EtudiantId { get; set; }
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // ManyToOne : l'étudiant est inscrit dans un parcours
    public Parcours? ParcoursSuivi { get; set; } = null;
    
    // OneToMany : un étudiant peut avoir plusieurs notes
    public List<Note>? Notes { get; set; }
    
    public override string ToString()
    {
        return $"ID {EtudiantId} : {NumEtud} - {Nom} {Prenom} inscrit en "+ParcoursSuivi;
    }
}